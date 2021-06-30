using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PoC.LinkLibraries.LibraryCode.MethodSupport;

namespace PoC.LinkLibraries.LibraryCode
{
    public delegate Task<T> ProcessMethod<T>(ProcessInstance<T> processInstance, CancellationToken cancellationToken);
    public class ProcessVersion<T>
    {
        private string Title => $"{ProcessDefinition} {MajorVersion}.{MinorVersion}";
        public ProcessDefinition<T> ProcessDefinition { get; }

        public ProcessVersion(ProcessDefinition<T> processDefinition)
        {
            ProcessDefinition = processDefinition;
            MethodHandler = new MethodHandler(Title);
        }

        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public Type Type { get; set; }
        public MethodHandler MethodHandler { get; set; }

        /// <inheritdoc />
        public override string ToString() => Title;

        public void AddParameter<TParameter>(string name)
        {
            MethodHandler.AddParameter<TParameter>(name);
        }
    }
}