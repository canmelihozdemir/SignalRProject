namespace SignalRTestProject.API.Models
{
    public class Room
    {
        public Room()
        {
            Players = new List<Player>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerConnectionId { get; set; }

        public virtual ICollection<Player> Players { get; set; }

    }
}
