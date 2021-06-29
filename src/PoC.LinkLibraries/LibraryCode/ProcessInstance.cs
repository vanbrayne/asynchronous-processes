using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Core.Assert;
using Nexus.Link.Libraries.Core.Misc;

namespace PoC.LinkLibraries.LibraryCode
{
    public abstract class ProcessInstance<T>
    {
        private readonly Dictionary<string, MethodArgument> _arguments = new Dictionary<string, MethodArgument>();

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
            var position = 0;
            foreach (var value in arguments)
            {
                position++;
                var parameter = ProcessVersion.GetParameter(position);
                SetArgument(parameter, value);
            }
        }

        public void SetArgument(MethodParameter parameter, object value)
        {
            if (value == null)
            {
                InternalContract.Require(parameter.IsNullable, 
                    $"The parameter {parameter} does not accept the value null.");
            }
            else
            {
                InternalContract.Require(parameter.Type.IsInstanceOfType(value), 
                    $"Expected {nameof(value)} to be an instance of type {parameter.Type.FullName}, but was of type {value.GetType().FullName}.");
            }
            
            var argument = new MethodArgument(parameter, value);
            _arguments.Add(parameter.Name, argument);
        }

        public object GetArgument(string parameterName)
        {
            if (!_arguments.TryGetValue(parameterName, out var argument))
            {
                var argumentParameters = string.Join(", ", _arguments.Values.Select(a => a.Parameter.Name));
                InternalContract.Fail($"The process {ProcessVersion} has a parameter named {parameterName}, but this instance {Title} had no argument for that parameter. Found these: {argumentParameters}");
                return default;
            }

            FulcrumAssert.IsTrue(argument.Parameter.Type.IsInstanceOfType(argument.Value), CodeLocation.AsString());

            return argument.Value;
        }

        public TArgument GetArgument<TArgument>(string parameterName)
        {
            return (TArgument) GetArgument(parameterName);
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