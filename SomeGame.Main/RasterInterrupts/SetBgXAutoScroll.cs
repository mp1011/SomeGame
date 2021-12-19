using SomeGame.Main.Models;

namespace SomeGame.Main.RasterInterrupts
{
    class SetBgXAutoScroll : IRasterInterrupt
    {
        private readonly GameSystem _gameSystem;
        private readonly int _autoScroll;
        private readonly RamInt _originalPosition;

        public int VerticalLine { get; }

        public SetBgXAutoScroll(GameSystem gameSystem, int autoScroll, int verticalLine, RamInt originalPosition)
        {
            _originalPosition = originalPosition;
            _gameSystem = gameSystem;
            _autoScroll = autoScroll;
            VerticalLine = verticalLine;
        }

        public void Handle(int frameNumber)
        {
            var bg = _gameSystem.GetLayer(LayerIndex.BG);
            if (_autoScroll == 0)
                bg.ScrollX = bg.ScrollX.Set(_originalPosition);
            else
            {
                _originalPosition.Set(bg.ScrollX);
                bg.ScrollX  = bg.ScrollX.Set((_autoScroll * frameNumber)/10);
            }            
        }
    }
}
