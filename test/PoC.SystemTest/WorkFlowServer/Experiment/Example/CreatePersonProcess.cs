using PoC.SystemTest.WorkFlowServer.Experiment.Capabilities.CustomerInformationMgmt;
using PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode;

namespace PoC.SystemTest.WorkFlowServer.Experiment.Example
{
    public class CreatePersonProcess : ProcessDefinition<Person>
    {
        public ICustomerInformationMgmtCapability CustomerInformationMgmt { get; }

        public CreatePersonProcess(ICustomerInformationMgmtCapability customerInformationMgmt)
        : base("Initialize person", "81B696D3-E27A-4CA8-9DC1-659B78DFE474")
        {
            CustomerInformationMgmt = customerInformationMgmt;
            // TODO: How does the major/minor version affect the data model?
            //var version = Versions.Add(1, 2, Version1Async); 
            //version.Parameters.Add("personalNumber");
            var version = ProcessVersions.Add<InitializePersonProcessV2>(2, 1);
            // TODO: Make Parameters a class and change Add to Register(string parameterName, Type parameterType)
            // TODO: NO need for numbers, they are sequential
            version.Parameters.Add(1, "Person");

        }
    }
}