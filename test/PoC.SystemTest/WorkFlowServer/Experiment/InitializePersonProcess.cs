using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Core.Error.Logic;
using PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode;

namespace PoC.SystemTest.WorkFlowServer.Experiment
{
    public class InitializePersonProcess : NexusLinkProcess<Person>
    {
        private readonly ICustomerInformationMgmtCapability _customerInformationMgmt;

        public InitializePersonProcess(ICustomerInformationMgmtCapability customerInformationMgmt)
            : base("Initialize person", "81B696D3-E27A-4CA8-9DC1-659B78DFE474")
        {
            _customerInformationMgmt = customerInformationMgmt;
            // TODO: How does the major/minor version affect the data model?
            //var version = Versions.Add(1, 2, Version1Async); 
            //version.Parameters.Add("personalNumber");
            var version = Versions.Add(2, 1, Version2Async);
            // TODO: Make Parameters a class and change Add to "Register".
            // TODO: NO need for numbers, they are sequential
            version.Parameters.Add(1, "personalNumber");
            version.Parameters.Add(2, "emailAddress");
        }

        private async Task<Person> Version2Async(ProcessInstance<Person> processInstance, CancellationToken cancellationToken)
        {
            // TODO: Type check the argument vs. the cast type
            var personalNumber = (string) processInstance.Arguments["personalNumber"];
            var emailAddress = (string)processInstance.Arguments["emailAddress"];
            
            // 1. Action: Get person
            var step = processInstance.Step("Get person", StepTypeEnum.Action, "A4D6F17F-ED40-4318-A08B-482302E53063").Synchronous();
            step.Parameters.Add(1, "personalNumber"); // Not mandatory
            var person = await step.ExecuteAsync(GetPersonActionAsync, cancellationToken, personalNumber);

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
                step = process.Step(3, "Get official data", TimeSpan.FromHours(1))
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
                foreach (var p in persons)
                {
                    loop.Increment();
                    process.CreateStep(loop, 1, "Verify existence.");
                    var task = _restClient.GetAsync<Person>($"Persons/{person.Id}", null, cancellationToken);
                    taskList.Add(task);
                }

            }
        }


        private async Task<Person> GetPersonActionAsync(ProcessStepInstance<Person> stepInstance, CancellationToken cancellationToken)
        {
            var personalNumber = (string) stepInstance.Arguments["personalNumber"];
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
}