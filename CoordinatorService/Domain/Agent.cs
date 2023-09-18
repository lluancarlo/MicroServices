using CoordinatorService.Domain.Enums;

namespace CoordinatorService.Domain
{
    public class Agent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public StatusEnum Status { get; set; }
        public SeniorityEnum Seniority { get; set; }
        public ShiftEnum Shift { get; set; }
        public int Capacity { get; set; }
        public int ActiveChats { get; set; }
    }
}