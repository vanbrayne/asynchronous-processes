using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Core.Error.Logic;
using PoC.SystemTest.WorkFlowServer.Experiment.Capabilities.CustomerInformationMgmt;
using PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode;

namespace PoC.SystemTest.WorkFlowServer.Experiment.Example
{
    public class InitializePersonProcessV2 : ProcessInstance<Person>
    {
        private Person _initialPerson;
        private Person _personSoFar;

        public new CreatePersonProcess ProcessDefinition => (CreatePersonProcess) base.ProcessDefinition;

        private InitializePersonProcessV2(ProcessVersion<Person> processVersion, string instanceName, object[] arguments)
            : base(processVersion, instanceName, arguments)
        {
        }

        public new static InitializePersonProcessV2 CreateInstance(ProcessVersion<Person> processVersion, string instanceName, object[] arguments)
        {
            return new InitializePersonProcessV2(processVersion, instanceName, arguments);
        }

        public override async Task<Person> ExecuteAsync(CancellationToken cancellationToken)
        {
            // TODO: Type check the argument vs. the cast type
            _initialPerson = (Person)Arguments["Person"];

            // 1. Action: Get person
            var step = ActionStep("Get person", "A4D6F17F-ED40-4318-A08B-482302E53063")
                .Synchronous();
            step.Parameters.Add(1, "personalNumber"); // Not mandatory
            _personSoFar = await step.ExecuteAsync(GetPersonActionAsync, cancellationToken, _initialPerson);

            // 2. Condition: Person exists?
            step = ConditionStep("Person exists?", "C5A628AC-5BAD-4DF9-BA46-B878C06D47CE");
            var exists = await step.EvaluateAsync(PersonExistsAsync, cancellationToken);
            if (exists) return _personSoFar;

            // 3. Get official data
            step = ActionStep("Get official data", "BA96BC20-83F3-4042-BA1C-B1068DE0AD8D", TimeSpan.FromHours(1))
                .Idempotent();
            var officialPersonData = await step.ExecuteAsync(GetOfficialDataAsync, cancellationToken, _initialPerson);

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

            return _personSoFar;
        }

        private async Task<Person> GetPersonActionAsync(ProcessStepInstance<Person> stepInstance, CancellationToken cancellationToken)
        {
            var inPerson = (Person)Arguments["Person"];
            var person = await ProcessDefinition.CustomerInformationMgmt.Person.GetByPersonalNumberAsync(_initialPerson.PersonalNumber, cancellationToken);
            return person;
        }

        private Task<bool> PersonExistsAsync(ProcessStepInstance<Person> stepInstance, CancellationToken cancellationToken)
        {
            return Task.FromResult(_personSoFar != null);
        }

        private async Task<Person> GetOfficialDataAsync(ProcessStepInstance<Person> stepInstance, CancellationToken cancellationToken)
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
                // Report to someone that knows 
            }
            catch (FulcrumUnauthorizedException e)
            {
                // Report to process developers or process owner
            }

        }
    }
}