namespace PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode
{
    public enum ProcessStepTypeEnum
    {
        Action, Condition, Loop
    }
    public class ProcessStep<T>
    {
        private readonly ProcessVersion<T> _processVersion;

        public ProcessStep(ProcessVersion<T> processVersion)
        {
            _processVersion = processVersion;
        }
    }
}