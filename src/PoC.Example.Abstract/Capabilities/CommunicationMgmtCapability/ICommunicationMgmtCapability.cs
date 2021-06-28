namespace PoC.Example.Abstract.Capabilities.CommunicationMgmtCapability
{
    public interface ICommunicationMgmtCapability
    {
        IEmailService Email { get; set; }
    }
}