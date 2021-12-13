namespace SomeGame.Main.Models
{
    internal class PlayerState
    {
        public BoundedRamByte Health { get; } 
        public RamByte Lives { get; } 
        public RamInt Score { get; }

        public PlayerState(GameSystem gameSystem)
        {
            gameSystem.RAM.AddLabel("Begin Player State");
            Health = gameSystem.RAM.DeclareBoundedByte(1,1);
            Lives = gameSystem.RAM.DeclareByte();
            Score = gameSystem.RAM.DeclareInt();
            gameSystem.RAM.AddLabel("End Player State");
        }
    }
}
