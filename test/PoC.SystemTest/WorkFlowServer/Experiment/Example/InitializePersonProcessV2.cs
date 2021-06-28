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

        public new CreatePersonProcess ProcessDefinition => (CreatePersonProcess)base.ProcessDefinition;

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
            var action = ActionStep("Get person", "A4D6F17F-ED40-4318-A08B-482302E53063");
            action.Parameters.Add(1, "Person");
            var existingPerson = await action.ExecuteAsync(GetPersonActionAsync, cancellationToken, _initialPerson);

            // 2. Condition: Person exists?
            var condition = ConditionStep("Person exists?", "C5A628AC-5BAD-4DF9-BA46-B878C06D47CE");
            condition.Parameters.Add(1, "Person");
            var exists = await condition.EvaluateAsync(PersonExistsAsync, cancellationToken, _initialPerson);
            if (exists) return existingPerson;

            // 3. Get official data (from e.g. Klarna)
            action = ActionStep("Get official data", "BA96BC20-83F3-4042-BA1C-B1068DE0AD8D", TimeSpan.FromHours(1))
                .Idempotent();
            action.Parameters.Add(1, "Person");
            var officialPersonData = await action.ExecuteAsync(GetOfficialDataAsync, cancellationToken, _initialPerson);

            // 4. do-while loop to get valid person data
            var loop = LoopStep("Loop to get valid person data", "7FB1FEEE-11DC-44A4-BFBF-C1EEA0D34F74");
            var correctedPerson = await loop.ExecuteAsync(AskUserToCorrectTheirInformationAsync, cancellationToken, officialPersonData);

            loop = LoopStep("Loop to inform players", "4E083535-7FF1-4DB9-A605-2C22607298A1");
            await loop.ExecuteAsync(InformPlayers, cancellationToken, correctedPerson);


            //// Parallel loop
            //loop = process.CreateLoopStep(2, "Loop to get and validate person data");
            //var taskList = new List<Task<Person>>();
            //foreach (var p in persons)
            //{
            //    loop.Increment();
            //    process.CreateStep(loop, 1, "Verify existence.");
            //    var task = _restClient.GetAsync<Person>($"Persons/{person.Id}", null, cancellationToken);
            //    taskList.Add(task);
            //}

            return _personSoFar;
        }

        private async Task InformPlayers(ProcessStepInstance<Person> loopStepInstance, CancellationToken cancellationToken)
        {
            var inPerson = (Person)loopStepInstance.Arguments["Person"];
            foreach (var player in inPerson.FavoriteFootballPlayers)
            {
                var action = ActionStep(loopStepInstance, $"Send mail to player",
                    "2CBE2645-436B-4386-9A15-EAF6BFD82D9F");
                await action.ExecuteAsync(SendMailToPlayerAsync, cancellationToken, inPerson, player);
            }
        }

        private Task SendMailToPlayerAsync(ProcessStepInstance<Person> loopStepInstance, CancellationToken cancellationToken)
        {
            var inPerson = (Person)loopStepInstance.Arguments["Person"];
            var player = (Person)loopStepInstance.Arguments["Player"];

            return ProcessDefinition.CommunicationMgmt.Email.SendEmailAsync(player.EmailAddress, "You have a new fan!",
                $"{inPerson.Name} has you as one of their favorite players!", cancellationToken);
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
                var personDataTemplate = await ProcessDefinition.CustomerInformationMgmt.Person.GetOfficialInformationAsync(inPerson, cancellationToken);
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
            Person personDetailsFromUser;
            do
            {
                await loopStepInstance.IterationAsync(++count);

                // 4.i.1 Request data from the user
                var action = ActionStep(loopStepInstance, "Ask user to correct data and fill in missing data.", "96D25A66-15A5-4E1F-81BC-2438491A7401");
                personDetailsFromUser = await action.ExecuteAsync(GetDataFromUserAsync, cancellationToken, inPerson);

                var condition = ConditionStep(loopStepInstance, "Verify user input.",
                    "0CA6B036-F448-4EC3-A757-36565CCF8534");
                isValid = await condition.EvaluateAsync(VerifyUserInput, cancellationToken, personDetailsFromUser);
            } while (!isValid);

            return personDetailsFromUser;
        }

        private async Task<bool> VerifyUserInput(ProcessStepInstance<Person> stepInstance, CancellationToken cancellationToken)
        {
            var inPerson = (Person)stepInstance.Arguments["Person"];
            var ok = await ProcessDefinition.CustomerInformationMgmt.Person.ValidateAsync(inPerson, cancellationToken);
            return ok;
        }

        private async Task<Person> GetDataFromUserAsync(ProcessStepInstance<Person> stepInstance, CancellationToken cancellationToken)
        {
            var inPerson = (Person)stepInstance.Arguments["Person"];
            var person =
                await ProcessDefinition.CustomerInformationMgmt.Person.AskUserToFillInDetailsAsync(inPerson, cancellationToken);
            return person;
        }
    }
}