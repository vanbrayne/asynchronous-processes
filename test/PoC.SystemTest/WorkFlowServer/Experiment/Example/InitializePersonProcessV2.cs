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
            // TODO: Define parameters
            var action = ActionStep("Get person", "A4D6F17F-ED40-4318-A08B-482302E53063")
                .Synchronous();
            action.Parameters.Add(1, "Person");
            var existingPerson = await action.ExecuteAsync(GetPersonActionAsync, cancellationToken, _initialPerson);

            // 2. Condition: Person exists?
            var condition = ConditionStep("Person exists?", "C5A628AC-5BAD-4DF9-BA46-B878C06D47CE");
            condition.Parameters.Add(1, "Person");
            var exists = await condition.EvaluateAsync(PersonExistsAsync, cancellationToken, _initialPerson);
            if (exists) return existingPerson;

            // 3. Get official data
            action = ActionStep("Get official data", "BA96BC20-83F3-4042-BA1C-B1068DE0AD8D", TimeSpan.FromHours(1))
                .Idempotent();
            action.Parameters.Add(1, "Person");
            var officialPersonData = await action.ExecuteAsync(GetOfficialDataAsync, cancellationToken, _initialPerson);

            // 4. Loop to get valid person data
            var loop = LoopStep("Loop to get valid person data", "7FB1FEEE-11DC-44A4-BFBF-C1EEA0D34F74"); 
            var correctedPerson = await loop.ExecuteAsync(AskUserToCorrectTheirInformationAsync, cancellationToken, officialPersonData);


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
            var inPerson = (Person)stepInstance.Arguments["Person"];
            var person = await ProcessDefinition.CustomerInformationMgmt.Person.GetByPersonalNumberAsync(_initialPerson.PersonalNumber, cancellationToken);
            return person;
        }

        private Task<bool> PersonExistsAsync(ProcessStepInstance<Person> stepInstance, CancellationToken cancellationToken)
        {
            var inPerson = (Person)stepInstance.Arguments["Person"];
            return Task.FromResult(inPerson != null);
        }

        private async Task<Person> GetOfficialDataAsync(ProcessStepInstance<Person> stepInstance, CancellationToken cancellationToken)
        {
            var inPerson = (Person)stepInstance.Arguments["Person"];
            try
            {
                var personDataTemplate = await ProcessDefinition.CustomerInformationMgmt.Person.GetOfficialInformation(inPerson, cancellationToken);
                return personDataTemplate ?? inPerson;
            }
            // TODO: Take care of time outs
            catch (FulcrumServiceContractException e)
            {
                // Report to someone that knows 
                throw;
            }
            catch (FulcrumUnauthorizedException e)
            {
                // Report to process developers or process owner
                throw;
            }
        }

        private async Task<Person> AskUserToCorrectTheirInformationAsync(ProcessStepInstance<Person> loopStepInstance, CancellationToken cancellationToken)
        {
            var inPerson = (Person)Arguments["Person"];
            bool isValid;
            var count = 0;
            do
            {
                await loopStepInstance.IterationAsync(++count);
                var action = ActionStep(loopStepInstance,  "Ask user to correct data and fill in missing data.", "96D25A66-15A5-4E1F-81BC-2438491A7401");
                person =
                    await _restClient.PostAndReturnCreatedObjectAsync<Person>("CustomerCommunication/GetPersonData",
                        personDataTemplate, cancellationToken: cancellationToken);

                process.CreateStep(loop, 2, "Validate customer input");
                isValid = await _restClient.PostAsync<bool, Person>($"CustomerInformationMgmt/Validate", person,
                    cancellationToken: cancellationToken);
            } while (!isValid);

            return person;
        }
    }
}