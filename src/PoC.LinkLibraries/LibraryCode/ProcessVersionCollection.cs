using System.Collections.Generic;
using System.Linq;

namespace PoC.LinkLibraries.LibraryCode
{
    public class ProcessVersionCollection<T>
    {
        public ProcessDefinition<T> ProcessDefinition { get; }

        public ProcessVersionCollection(ProcessDefinition<T> processDefinition)
        {
            ProcessDefinition = processDefinition;
        }
        public Dictionary<int, ProcessVersion<T>> Versions = new Dictionary<int, ProcessVersion<T>>();
        public ProcessVersion<T> Add<TInstanceType>(int majorVersion, int minorVersion) where TInstanceType : ProcessInstance<T>
        {
            var processVersion = new ProcessVersion<T>(ProcessDefinition)
            {
                MajorVersion = majorVersion,
                MinorVersion = minorVersion,
                Type = typeof(TInstanceType)
            };
            Versions.Add(majorVersion, processVersion);
            return processVersion;
        }

        public ProcessVersion<T> GetVersion(in int majorVersionNumber)
        {
            // TODO: Validate majorVersionNumber, e.g. contains
            return Versions[majorVersionNumber];
        }

        public ProcessVersion<T> GetLatestVersion()
        {
            var latestMajorVersion =  Versions.Values.Max(v => v.MajorVersion);
            return GetVersion(latestMajorVersion);
        }
    }
}