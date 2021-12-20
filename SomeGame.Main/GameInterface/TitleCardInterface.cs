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
        private RamByte _timer;

        public TitleCardInterface(GameSystem gameSystem, SceneContentKey thisScene)
        {
            _gameSystem = gameSystem;
            _font = new Font(_gameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
            _text = GetLevelText(thisScene);

            var layer = _gameSystem.GetLayer(LayerIndex.Interface);
            layer.TileMap.SetEach((x, y) => new Tile(-1, TileFlags.None));

            int textX = ((_gameSystem.Screen.Width / _gameSystem.TileSize) - _text.Length) / 2;
            _font.WriteToLayer(_text, layer, new Point(textX, 10));

            _timer = gameSystem.RAM.DeclareByte();
        }

        private string GetLevelText(SceneContentKey thisScene)
        {
            switch (thisScene)
            {
                case SceneContentKey.Level1TitleCard:
                    return "STAGE 1-1";
                case SceneContentKey.GameOver:
                    return "GAME OVER";
                default:
                    return "UNDEFINED";
            }
        }

        public void Update()
        {
            if (++_timer == 18)
            {
                _timer.Set(0);
                var layer = _gameSystem.GetLayer(LayerIndex.Interface);
                if (layer.Palette == PaletteIndex.P1)
                    layer.Palette = PaletteIndex.P2;
                else
                    layer.Palette = PaletteIndex.P1;
            }
        }
    }
}
