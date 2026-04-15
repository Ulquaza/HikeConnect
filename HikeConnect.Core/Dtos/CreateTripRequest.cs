namespace HikeConnect.Core.Dtos
{
    public class CreateTripRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
    }
}
