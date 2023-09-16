namespace CoordinatorService.Domain
{
    public class Chat
    {
        public Guid SessionId { get; set; }
        public Guid AgentId { get; set; }
        public DateTime StartAt { get; set; }
    }
}