﻿using System;
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
        public Dictionary<int, string> Parameters { get;  } = new Dictionary<int, string>();
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public Type Type { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{ProcessDefinition} {MajorVersion}.{MinorVersion}";
    }
}