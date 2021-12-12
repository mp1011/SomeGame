namespace SomeGame.Main.Models
{
    internal class PlayerState
    {
        public BoundedRamByte Health { get; } 
        public RamByte Lives { get; } 
        public RamInt Score { get; }

        public PlayerState(GameSystem gameSystem)
        {
            Health = gameSystem.RAM.DeclareBoundedByte(1,1);
            Lives = gameSystem.RAM.DeclareByte();
            Score = gameSystem.RAM.DeclareInt();
        }
    }
}
