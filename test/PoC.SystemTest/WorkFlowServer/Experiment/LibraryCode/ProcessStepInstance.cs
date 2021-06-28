using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Core.Assert;
using PoC.AM.Abstract.Exceptions;

namespace PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode
{
    public delegate Task<TMethodReturnType> StepActionMethod<TProcessReturnType, TMethodReturnType>(
        ProcessStepInstance<TProcessReturnType> stepInstance,
        CancellationToken cancellationToken);
    public delegate Task StepActionMethod<TProcessReturnType>(
        ProcessStepInstance<TProcessReturnType> stepInstance,
        CancellationToken cancellationToken);

    public class ProcessStepInstance<TProcessReturnType>
    {
        private readonly ProcessInstance<TProcessReturnType> _instance;
        private readonly ProcessStep<TProcessReturnType> _processStep;

        public ProcessStepInstance(ProcessInstance<TProcessReturnType> instance, ProcessStep<TProcessReturnType> processStep)
        {
            _instance = instance;
            _processStep = processStep;
        }
        public Dictionary<int, string> Parameters { get;  } = new Dictionary<int, string>();
        public Dictionary<string, object> Arguments { get; } = new Dictionary<string, object>();

        public ProcessStepInstance<TProcessReturnType> Synchronous()
        {
            throw new NotImplementedException();
        }

        public ProcessStepInstance<TProcessReturnType> Idempotent()
        {
            throw new NotImplementedException();
        }

        public Task<TMethodReturnType> ExecuteAsync<TMethodReturnType>(StepActionMethod<TProcessReturnType, TMethodReturnType> method, CancellationToken cancellationToken, params object[] arguments)
        {
            InternalContract.Require(_processStep.StepType == ProcessStepTypeEnum.Action || _processStep.StepType == ProcessStepTypeEnum.Loop,
                $"The step {_processStep} was declared as {_processStep.StepType}, so you can't call {nameof(ExecuteAsync)}.");

            return InternalExecuteAsync(method, cancellationToken, arguments);
        }

        public Task ExecuteAsync(StepActionMethod<TProcessReturnType> method, CancellationToken cancellationToken, params object[] arguments)
        {
            InternalContract.Require(_processStep.StepType == ProcessStepTypeEnum.Action || _processStep.StepType == ProcessStepTypeEnum.Loop,
                $"The step {_processStep} was declared as {_processStep.StepType}, so you can't call {nameof(ExecuteAsync)}.");

            return InternalExecuteAsync(method, cancellationToken, arguments);
        }

        public Task<bool> EvaluateAsync(StepActionMethod<TProcessReturnType, bool> method, CancellationToken cancellationToken, params object[] arguments)
        {
            InternalContract.RequireAreEqual(ProcessStepTypeEnum.Condition, _processStep.StepType, null, 
                $"The step {_processStep} was declared as {_processStep.StepType}, so you can't call {nameof(EvaluateAsync)}.");

            return InternalExecuteAsync(method, cancellationToken, arguments);
        }

        private async Task<TMethodReturnType> InternalExecuteAsync<TMethodReturnType>(StepActionMethod<TProcessReturnType, TMethodReturnType> method, CancellationToken cancellationToken, params object[] arguments)
        {
            // TODO: Create/update LatestRequest in DB
            // TODO: Create/update Arguments in DB
            try
            {
                var result = await method(this, cancellationToken);
                // TODO: Update the DB StepInstance with FinishedAt
                // TODO: Create/update LatestResponse in DB
                return result;
            }
            catch (PostponeException e)
            {
                throw;
            }
            catch (Exception e)
            {
                // TODO: Smart error handling
                throw;
            }
        }

        private async Task InternalExecuteAsync(StepActionMethod<TProcessReturnType> method, CancellationToken cancellationToken, params object[] arguments)
        {
            // TODO: Create/update LatestRequest in DB
            // TODO: Create/update Arguments in DB
            try
            {
                await method(this, cancellationToken);
                // TODO: Update the DB StepInstance with FinishedAt
                // TODO: Create/update LatestResponse in DB
            }
            catch (PostponeException e)
            {
                throw;
            }
            catch (Exception e)
            {
                // TODO: Smart error handling
                throw;
            }
        }

        public Task IterationAsync(int i)
        {
            InternalContract.RequireAreEqual(ProcessStepTypeEnum.Loop, _processStep.StepType, null, 
                $"The step {_processStep} was declared as {_processStep.StepType}, so you can't call {nameof(EvaluateAsync)}.");
            throw new NotImplementedException();
        }
    }
}