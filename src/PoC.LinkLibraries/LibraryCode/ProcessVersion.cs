using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PoC.LinkLibraries.LibraryCode
{
    public delegate Task<T> ProcessMethod<T>(ProcessInstance<T> processInstance, CancellationToken cancellationToken);
    public class ProcessVersion<T>
    {
        public ProcessDefinition<T> ProcessDefinition { get; }

        public ProcessVersion(ProcessDefinition<T> processDefinition)
        {
            ProcessDefinition = processDefinition;
        }

        private readonly Dictionary<int, MethodParameter> _parameters = new Dictionary<int, MethodParameter>();

        public void AddParameter<TParameter>(string name)
        {
            var position = _parameters.Count + 1;
            _parameters.Add(position, new MethodParameter(name, typeof(TParameter)));
        }

        public MethodParameter GetParameter(int position)
        {
            return _parameters[position];
        }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public Type Type { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{ProcessDefinition} {MajorVersion}.{MinorVersion}";
    }
}