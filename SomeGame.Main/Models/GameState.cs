namespace SomeGame.Main.Models
{
    internal class PlayerState
    {
        public BoundedRamByte Health { get; } 
        public RamByte Lives { get; } 
        public RamInt Score { get; }

        private RamByte _hasKey; // todo, condense into bit

        public bool HasKey
        {
            get => _hasKey == 1;
            set
            {
                if (value)
                    _hasKey.Set(1);
                else
                    _hasKey.Set(0);
            }
        }

        public PlayerState(GameSystem gameSystem)
        {
            gameSystem.RAM.AddLabel("Begin Player State");
            Health = gameSystem.RAM.DeclareBoundedByte(1,1);
            Lives = gameSystem.RAM.DeclareByte();
            Score = gameSystem.RAM.DeclareInt();
            _hasKey = gameSystem.RAM.DeclareByte();
            gameSystem.RAM.AddLabel("End Player State");
        }
    }
}
