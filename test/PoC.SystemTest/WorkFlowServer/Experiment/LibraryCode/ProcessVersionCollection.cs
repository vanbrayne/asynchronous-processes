using System.Collections.Generic;

namespace PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode
{
    public class ProcessVersionCollection<T>
    {
        public Dictionary<int, ProcessVersion<T>> Versions = new Dictionary<int, ProcessVersion<T>>();
        public ProcessVersion<T> Add(int majorVersion, int minorVersion, ProcessMethod<T> method)
        {
            var processVersion = new ProcessVersion<T>
            {
                MajorVersion = majorVersion,
                MinorVersion = minorVersion,
                Method = method
            };
            Versions.Add(majorVersion, processVersion);
            return processVersion;
        }

        public ProcessVersion<T> GetVersion(in int majorVersionNumber)
        {
            // TODO: Validate majorVersionNumber, e.g. contains
            return Versions[majorVersionNumber];
        }
    }
}