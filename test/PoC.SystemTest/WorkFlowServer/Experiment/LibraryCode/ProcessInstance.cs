﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode
{
    public abstract class ProcessInstance<T>
    {
        private readonly ProcessVersion<T> _processVersion;

        protected ProcessInstance(ProcessVersion<T> processVersion, string instanceTitle, params object[] arguments)
        {
            _processVersion = processVersion;
            // TODO: Set the arguments: Parameters.SetArguments(arguments);
        }

        public Dictionary<string, object> Arguments { get; } = new Dictionary<string, object>();

        public abstract Task<T> ExecuteAsync(CancellationToken cancellationToken);

        protected ProcessStepInstance<T> ActionStep(string stepTitle, string stepId)
        {
            return Step(stepTitle, ProcessStepTypeEnum.Action, stepId);
        }

        public ProcessStepInstance<T> Step(string stepName, ProcessStepTypeEnum stepTypeEnum, string stepId)
        {
            // TODO: Get or create DB Step
            var step = new ProcessStep<T>(_processVersion, stepTypeEnum)
            {
                Id = stepId,
                Title = stepName,
            };
            var stepInstance = new ProcessStepInstance<T>(this, step); // TODO: Add step as input
            // TODO: Create a step instance in DB, with start time, etc
            return stepInstance;
        }

        protected ProcessStepInstance<T> ConditionStep(string stepTitle, string stepId)
        {
            return Step(stepTitle, ProcessStepTypeEnum.Condition, stepId);
        }
    }
}