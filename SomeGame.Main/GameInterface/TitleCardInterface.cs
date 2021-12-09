using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System;

namespace SomeGame.Main.GameInterface
{
    class TitleCardInterface : IGameInterface
    {
        private readonly GameSystem _gameSystem;
        private readonly Font _font;
        private readonly string _text;
        private int _timer;

        public TitleCardInterface(GameSystem gameSystem, SceneContentKey nextScene)
        {
            _gameSystem = gameSystem;
            _font = new Font(_gameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
            _text = GetLevelText(nextScene);

            int textX = ((_gameSystem.Screen.Width / _gameSystem.TileSize) - _text.Length) / 2;
            _font.WriteToLayer(_text, _gameSystem.GetLayer(LayerIndex.Interface), new Point(textX, 10));
        }

        private string GetLevelText(SceneContentKey nextScene)
        {
            switch (nextScene)
            {
                case SceneContentKey.Test3:
                    return "STAGE 1-1";
                default:
                    return "UNDEFINED";
            }
        }

        public void Update()
        {
            if (++_timer == 18)
            {
                _timer = 0;
                var layer = _gameSystem.GetLayer(LayerIndex.Interface);
                if (layer.Palette == PaletteIndex.P1)
                    layer.Palette = PaletteIndex.P3;
                else
                    layer.Palette = PaletteIndex.P1;
            }
        }
    }
}
