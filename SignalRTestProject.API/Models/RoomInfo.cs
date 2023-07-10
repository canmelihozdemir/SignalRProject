namespace SignalRTestProject.API.Models
{
    public class RoomInfo
    {
        public string OwnerConnectionId { get; set; }
        public string Name { get; set; }
        public int MemberCount { get; set; }
        public List<string> MemberConnectionId { get; set; } = new List<string>();
    }
}
