namespace PoC.SystemTest.WorkFlowServer.Experiment.Capabilities.CommunicationMgmtCapability
{
    public interface ICommunicationMgmtCapability
    {
        IEmailService Email { get; set; }
    }
}