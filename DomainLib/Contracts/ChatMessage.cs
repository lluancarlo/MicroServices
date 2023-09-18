namespace DomainLib.Contracts
{
    public class ChatMessage
    {
        public Guid SessionId { get; set; }
        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public string CustomerName { get; set; }
        public bool Active { get; set; }
    }
}