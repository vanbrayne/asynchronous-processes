using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Core.Assert;
using Nexus.Link.Libraries.Core.Misc;
using PoC.LinkLibraries.LibraryCode.MethodSupport;

namespace PoC.LinkLibraries.LibraryCode
{
    public abstract class ProcessInstance<T>
    {
        protected MethodHandler MethodHandler { get; }

        public static ProcessInstance<T> CreateInstance(ProcessVersion<T> processVersion, string instanceName, object[] arguments)
        {
            throw new NotImplementedException($"You should create your own factory method with the same signature in your class.");
        }

        public ProcessVersion<T> ProcessVersion { get; }
        public string Title { get; }

        public ProcessDefinition<T> ProcessDefinition => ProcessVersion.ProcessDefinition;

        protected ProcessInstance(ProcessVersion<T> processVersion, string instanceTitle, params object[] arguments)
        {
            ProcessVersion = processVersion;
            Title = instanceTitle;
            MethodHandler = processVersion.MethodHandler.NewInstance(instanceTitle, arguments);
        }

        public abstract Task<T> ExecuteAsync(CancellationToken cancellationToken);

        protected ProcessStepInstance<T> ActionStep(string stepTitle, string stepId, TimeSpan? expiresAt = null)
        {
            return Step(null, stepTitle, ProcessStepTypeEnum.Action, stepId, expiresAt);
        }

        protected ProcessStepInstance<T> ConditionStep(ProcessStepInstance<T> parentStep, string stepTitle, string stepId, TimeSpan? expiresAt = null)
        {
            return Step(parentStep, stepTitle, ProcessStepTypeEnum.Condition, stepId, expiresAt);
        }

        protected ProcessStepInstance<T> LoopStep(string stepTitle, string stepId, TimeSpan? expiresAt = null)
        {
            return Step(null, stepTitle, ProcessStepTypeEnum.Loop, stepId, expiresAt);
        }

        protected ProcessStepInstance<T> ActionStep(ProcessStepInstance<T> parentStep, string stepTitle, string stepId, TimeSpan? expiresAt = null)
        {
            return Step(parentStep, stepTitle, ProcessStepTypeEnum.Action, stepId, expiresAt);
        }

        protected ProcessStepInstance<T> ConditionStep(string stepTitle, string stepId, TimeSpan? expiresAt = null)
        {
            return Step(null, stepTitle, ProcessStepTypeEnum.Condition, stepId, expiresAt);
        }

        protected ProcessStepInstance<T> LoopStep(ProcessStepInstance<T> parentStep, string stepTitle, string stepId, TimeSpan? expiresAt = null)
        {
            return Step(parentStep, stepTitle, ProcessStepTypeEnum.Loop, stepId, expiresAt);
        }

        protected TParameter GetArgument<TParameter>(string name)
        {
            return MethodHandler.GetArgument<TParameter>(name);
        }

        private ProcessStepInstance<T> Step(ProcessStepInstance<T> parentStep, string stepName, ProcessStepTypeEnum stepTypeEnum, string stepId, TimeSpan? expiresAt)
        {
            // TODO: Get or create DB Step
            var step = new ProcessStep<T>(ProcessVersion, stepTypeEnum)
            {
                Id = stepId,
                Title = stepName,
                ExpiresAt = expiresAt
            };
            var stepInstance = new ProcessStepInstance<T>(this, step); // TODO: Add step as input
            // TODO: Create a step instance in DB, with start time, etc
            return stepInstance;
        }
    }
}