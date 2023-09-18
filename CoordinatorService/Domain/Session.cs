using CoordinatorService.Domain.Enums;

namespace CoordinatorService.Domain
{
    public class Session
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public bool Active { get; set; }
        public int PollCount { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}