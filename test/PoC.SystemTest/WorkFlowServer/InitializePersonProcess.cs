using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Core.Application;
using Nexus.Link.Libraries.Core.Error.Logic;
using Nexus.Link.Libraries.Crud.Interfaces;
using PoC.LinkLibraries;


namespace PoC.SystemTest.WorkFlowServer
{
    public class PersonService : IPersonService
    {
        /// <inheritdoc />
        public async Task<Person> ReadAsync(string id, CancellationToken token = new CancellationToken())
        {
            return new Person();;
        }

        /// <inheritdoc />
        public async Task<string> CreateAsync(Person item, CancellationToken token = new CancellationToken())
        {
            var process = new CreatePersonProcess();
            process.Execute(item);
        }

        /// <inheritdoc />
        public async Task<Person> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<Person> InitializePerson(string personalNumber, string emailAddress, CancellationToken cancellationToken = default)
        {
            var process = new InitializePersonProcess(null);
            return await process.ExecuteAsync(1, personalNumber, emailAddress);
        }
    }

    public class InitializePersonProcess : NexusLinkProcess<Person>
    {
        private ICustomerInformationMgmtCapability _customerInformationMgmt;

        public InitializePersonProcess(ICustomerInformationMgmtCapability customerInformationMgmt)
            : base("Initialize person", "81B696D3-E27A-4CA8-9DC1-659B78DFE474")
        {
            _customerInformationMgmt = customerInformationMgmt;
            // TODO: How does this affect the data model?
            //var version = Versions.Add(1, 2, Version1Async); 
            //version.Parameters.Add("personalNumber");
            var version = Versions.Add(2, 1, Version2Async);
            version.Parameters.Add(1, "personalNumber");
            version.Parameters.Add(2, "emailAddress");
        }

        private async Task<Person> Version2Async(CancellationToken cancellationToken, params object[] arguments)
        {
            var personalNumber = Arguments["personalNumber"];
            var emailAddress = Arguments["emailAddress"];


            using var instance = _process.GetOrCreateInstance(1);
            // Create the process
            using var process = new NexusLinkProcess();

            // 1. Action: Get person
            var step = process.Step(1, "Get person").Synchronous();
            var person = await step.ExecuteAsync(GetPersonActionAsync, personalNumber);

            // 2. Condition: Person exists?
            step = process.Step(2, "Person exists?");
            var exists = step.Evaluate(person != null);
            if (exists)
            {
                // Terminate
                return process.Terminate(person);
            }
            else
            {

                // 3. Get official data
                var step = process.Step(3, "Get official data", TimeSpan.FromHours(1))
                    .Idempotent();
                var officialPersonData = await step.ExecuteAsync(GetOfficialDataAsync, personalNumber, emailAddress);



                // 4. Loop to get valid person data
                step = process.AsyncStep(4, "Loop to get valid person data");
                await step.Loop(GetValidDataFromPerson, officialPersonData)
                bool isValid;
                do
                {
                    loop.Increment();
                    process.CreateStep(loop, 1, "Ask user to fill in missing data.");
                    person =
                        await _restClient.PostAndReturnCreatedObjectAsync<Person>("CustomerCommunication/GetPersonData",
                            personDataTemplate, cancellationToken: cancellationToken);

                    process.CreateStep(loop, 2, "Validate customer input");
                    isValid = await _restClient.PostAsync<bool, Person>($"CustomerInformationMgmt/Validate", person,
                        cancellationToken: cancellationToken);
                } while (!isValid);


                // Sequential loop
                loop = process.CreateLoopStep(2, "Loop to get and validate person data");
                foreach (var p in persons)
                {
                    loop.Increment();
                    process.CreateStep(loop, 1, "Verify existence.");
                    var outPerson = await _restClient.GetAsync<Person>($"Persons/{p.Id}", null, cancellationToken);
                }


                // Parallel loop
                loop = process.CreateLoopStep(2, "Loop to get and validate person data");
                var taskList = new List<Task<Person>>();
                foreach (var person in persons)
                {
                    loop.Increment();
                    process.CreateStep(loop, 1, "Verify existence.");
                    var task = _restClient.GetAsync<Person>($"Persons/{person.Id}", null, cancellationToken);
                    taskList.Add(task);
                }

            }
        }

        public Dictionary<string, object> Arguments { get; set; }


        private async Task<Person> GetPersonActionAsync(string personalNumber, CancellationToken cancellationToken)
        {
            var person = await
                _customerInformationMgmt.Person.GetByPersonalNumberAsync(personalNumber, cancellationToken);
            return person;
        }

        private async Task<OfficialPersonData> GetOfficialDataAsync(string personalNumber, string emailAddress)
        {
            try
            {
                var personDataTemplate = await _restClient.GetAsync<Person>(
                    $"Klarna/Persons/{personalNumber}/{emailAdress}", cancellationToken: cancellationToken);
                if (personDataTemplate == null)
                {
                    personDataTemplate = new Person
                    {
                        PersonalNumber = personalNumber,
                        EmailAddress = emailAdress
                    };
                }
            }
            catch (FulcrumTimeOutException e)
            {
                // Escalate 
            }
            catch (FulcrumServiceContractException e)
            {
                // Report to somone that knows 
            }
            catch (FulcrumUnauthorizedException e)
            {
                // Report to process developers or process owner
            }

        }
    }

    public class NexusLinkProcessAttribute : Attribute
    {
        public string ProcessName { get; }
        public string ProcessId { get; }
        public int Version { get; }

        public NexusLinkProcessAttribute(string processName, string processId, int version)
        {
            ProcessName = processName;
            ProcessId = processId;
            Version = version;
        }
    }
    public abstract class NexusLinkProcess<T> : IDisposable
    {
        protected List<string> Parameters { get; } = new List<string>();
        protected VersionCollection<T> Versions { get;  } = new VersionCollection<T>();
        public string ProcessName { get; }
        public string ProcessId { get; }
        public NexusLinkProcess(string processName, string processId)
        {
            ProcessName = processName;
            ProcessId = processId;
            FulcrumApplication.Context.AsyncContext.CurrentProcessId = processId;
            throw new NotImplementedException();
        }
        
        public async Task<T> ExecuteAsync(int version, params object[] arguments)
        {
            // TODO: Verify arguments with Parameters
            Parameters.SetArguments(arguments);
        }

        public NexusLinkProcessStep Step(int stepNumber, string stepName)
        {
            FulcrumApplication.Context.AsyncContext.CurrentStep = stepNumber;
            throw new NotImplementedException();
        }

        public NexusLinkProcessStep CreateStep(int stepNumber, string stepName, TimeSpan? maxTime = null)
        {
            throw new NotImplementedException();
        }

        public NexusLinkProcessStep CreateLoopStep(int stepNumber, string stepName)
        {
            throw new NotImplementedException();
        }

        public void CreateStep(NexusLinkProcessStep loop, int stepNumber, string stepName)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class VersionCollection<T>
    {
        public ProcessVersion Add(int majorVersion, int minorVersion, Func<CancellationToken, object[], Task<T>> method)
        {
            throw new NotImplementedException();
        }
    }

    public class ProcessVersion
    {
        public Dictionary<int, string> Parameters { get;  } = new Dictionary<int, string>();
    }

    public class NexusLinkProcessStep
    {
        public NexusLinkProcessStep AlwaysFresh()
        {
            throw new NotImplementedException();
        }

        public NexusLinkProcessStep Synchronous()
        {
            throw new NotImplementedException();
        }

        public void Increment()
        {
            throw new NotImplementedException();
        }
    }

    public class Person
    {
    }

    public interface ICustomerInformationMgmtCapability
    {
        IPersonService Person { get; set; }
    }

    public interface IPersonService : IRead<Person, string>, ICreate<Person, string>
    {
        Task<Person> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default);

        Task<Person> InitializePerson(string personalNumber, string emailAddress,
            CancellationToken cancellationToken = default);
    }
}