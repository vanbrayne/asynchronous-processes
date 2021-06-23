using System.Collections.Generic;

namespace PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode
{
    public class ProcessInstance<T>
    {
        private readonly ProcessVersion<T> _processVersion;

        public ProcessInstance(ProcessVersion<T> processVersion, string instanceTitle, params object[] arguments)
        {
            _processVersion = processVersion;
            // TODO: Set the arguments: Parameters.SetArguments(arguments);
        }

        public Dictionary<string, object> Arguments { get; } = new Dictionary<string, object>();

        public ProcessStepInstance<T> Step(string stepName, ProcessStepTypeEnum stepTypeEnum, string stepId)
        {
            // TODO: Get or create DB Step
            var step = new ProcessStep<T>(_processVersion);
            var stepInstance = new ProcessStepInstance<T>(this, step); // TODO: Add step as input
            // TODO: Create a step instance in DB, with start time, etc
            return stepInstance;
        }
    }
}