namespace SomeGame.Main.Models
{
    internal struct PlayerState
    {
        public BoundedInt Health { get; set; } 
        public byte Lives { get; set; } 
        public int Score { get; set; }
    }
}
