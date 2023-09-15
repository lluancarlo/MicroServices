using ChatCoordinatorService.Domain.Enums;

namespace ChatCoordinatorService.Domain
{
    public class Agent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public StatusEnum Status { get; set; }
        public SeniorityEnum Seniority { get; set; }
        public ShiftEnum Shift { get; set; }
    }
}