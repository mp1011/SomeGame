using SomeGame.Main.Models;

namespace SomeGame.Main.RasterInterrupts
{
    class ChangeBackgroundColor : IRasterInterrupt
    {
        private readonly GameSystem _gameSystem;
        private readonly byte _colorIndex;
        public int VerticalLine { get; }

        public ChangeBackgroundColor(GameSystem gameSystem, byte color, int verticalLine)
        {
            _gameSystem = gameSystem;
            _colorIndex = color;
            VerticalLine = verticalLine;
        }

        public void Handle()
        {
            _gameSystem.BackgroundColorIndex.Set(_colorIndex);
        }
    }
}
