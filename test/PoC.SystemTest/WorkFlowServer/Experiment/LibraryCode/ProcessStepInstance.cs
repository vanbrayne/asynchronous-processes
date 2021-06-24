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

        public async Task<TMethodReturnType> ExecuteAsync<TMethodReturnType>(StepActionMethod<TProcessReturnType, TMethodReturnType> method, CancellationToken cancellationToken, params object[] arguments)
        {
            InternalContract.RequireAreEqual(ProcessStepTypeEnum.Action, _processStep.StepType, null, 
                $"The step {_processStep}");
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

        public async Task<bool> EvaluateAsync(StepActionMethod<TProcessReturnType, bool> method, CancellationToken cancellationToken, params object[] arguments)
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
    }
}