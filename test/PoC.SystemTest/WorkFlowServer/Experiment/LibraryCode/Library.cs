using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Core.Application;
using PoC.AM.Abstract.Exceptions;

namespace PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode
{
    public enum StepTypeEnum
    {
        Action, Condition, Loop
    }

    public class NexusLinkProcessAttribute : Attribute
    {
        public string ProcessName { get; }
        public string ProcessId { get; }
        public int Version { get; }

        public NexusLinkProcessAttribute(string processName, string processId, int version)
        {
            ProcessName = processName;
            ProcessId = processId;
            Version = version;
        }
    }
    public abstract class NexusLinkProcess<T> : IDisposable
    {
        protected List<string> Parameters { get; } = new List<string>();
        protected VersionCollection<T> Versions { get;  } = new VersionCollection<T>();
        public string ProcessName { get; }
        public string ProcessId { get; }
        public NexusLinkProcess(string processName, string processId)
        {
            ProcessName = processName;
            ProcessId = processId;
            FulcrumApplication.Context.AsyncContext.CurrentProcessId = processId;
            throw new NotImplementedException();
        }
        
        public async Task<T> ExecuteAsync(int majorVersionNumber, string instanceTitle, CancellationToken cancellationToken, params object[] arguments)
        {
            var instance = CreateInstance(majorVersionNumber, instanceTitle, arguments);
            // TODO: Verify arguments with Parameters
            // TODO: Set the arguments: Parameters.SetArguments(arguments);
            var version = Versions.GetVersion(majorVersionNumber);
            try
            {
                var result = await version.Method(instance, cancellationToken);
                // TODO: What to do, now that the instance has completed, e.g. DB ProcessInstance.FinishedAt = DateTimeOffset.NowUtc.
                return result;
            }
            catch (PostponeException e)
            {
                // TODO: Postpone
            }
            catch (Exception e)
            {
                // TODO: Fatal error
            }
        }

        private ProcessInstance<T> CreateInstance(in int version, string instanceTitle, object[] arguments)
        {
            // TODO: Set start time
            // TODO: Set correlation id
            // TODO: Set arguments

            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class ProcessInstance<T>
    {
        private readonly ProcessVersion<T> _processVersion;

        public ProcessInstance(ProcessVersion<T> processVersion)
        {
            _processVersion = processVersion;
        }

        public Dictionary<string, object> Arguments { get; } = new Dictionary<string, object>();

        public ProcessStepInstance<T> Step(string stepName, StepTypeEnum stepTypeEnum, string stepId)
        {
            // TODO: Get or create DB Step
            var step = new ProcessStep<T>(_processVersion);
            var stepInstance = new ProcessStepInstance<T>(this, step); // TODO: Add step as input
            // TODO: Create a step instance in DB, with start time, etc
            return stepInstance
        }
    }

    public class ProcessStep<T>
    {
        private readonly ProcessVersion<T> _processVersion;

        public ProcessStep(ProcessVersion<T> processVersion)
        {
            _processVersion = processVersion;
        }
    }

    public delegate Task<T> ProcessMethod<T>(ProcessInstance<T> processInstance, CancellationToken cancellationToken);

    public class VersionCollection<T>
    {
        public Dictionary<int, ProcessVersion<T>> versions = new Dictionary<int, ProcessVersion<T>>();
        public ProcessVersion<T> Add(int majorVersion, int minorVersion, ProcessMethod<T> method)
        {
            var processVersion = new ProcessVersion<T>
            {
                MajorVersion = majorVersion,
                MinorVersion = minorVersion,
                Method = method
            };
            versions.Add(majorVersion, processVersion);
            return processVersion;
        }

        public ProcessVersion<T> GetVersion(in int majorVersionNumber)
        {
            // TODO: Validate majorVersionNumber, e.g. contains
            return versions[majorVersionNumber];
        }
    }

    public class ProcessVersion<T>
    {
        public Dictionary<int, string> Parameters { get;  } = new Dictionary<int, string>();
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public ProcessMethod<T> Method { get; set; }
    }

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

        public void Increment()
        {
            throw new NotImplementedException();
        }

        public async Task<TMethodReturnType> ExecuteAsync<TMethodReturnType>(StepActionMethod<TProcessReturnType, TMethodReturnType> method, CancellationToken cancellationToken, params object[] arguments)
        {
            // TODO: Create/update LatestRequest in DB
            // TODO: Create/update Arguments in DB
            try
            {
                var result = await method(this, cancellationToken);
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

        private ProcessStepInstance CreateInstance(ProcessStepInstance<TProcessReturnType> processStepInstance)
        {
            throw new NotImplementedException();
        }
    }

    public class ProcessStepInstance
    {
    }

    public class Person
    {
    }
}