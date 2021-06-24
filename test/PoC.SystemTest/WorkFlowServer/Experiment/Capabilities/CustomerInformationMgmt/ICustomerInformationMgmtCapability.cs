namespace PoC.SystemTest.WorkFlowServer.Experiment.Capabilities.CustomerInformationMgmt
{
    public interface ICustomerInformationMgmtCapability
    {
        IPersonService Person { get; set; }
    }
}