namespace SignalRTestProject.API.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MessageContent { get; set; }
        public string DateTime { get; set; }

        public int TeamId { get; set; }
        public virtual Room Team { get; set; }

    }
}
