using System;

namespace PoC.LinkLibraries.LibraryCode
{
    public enum ProcessStepTypeEnum
    {
        Action, Condition, Loop
    }
    public class ProcessStep<T>
    {
        public ProcessStepTypeEnum StepType { get; }
        public ProcessVersion<T> ProcessVersion { get; }
        public string Id { get; set; }
        public string Title { get; set; }
        public TimeSpan? ExpiresAt { get; set; }

        public ProcessStep(ProcessVersion<T> processVersion, ProcessStepTypeEnum stepType)
        {
            StepType = stepType;
            ProcessVersion = processVersion;
        }

        /// <inheritdoc />
        public override string ToString() => $"{ProcessVersion}: {StepType} {Title} ({Id})";
    }
}