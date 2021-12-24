using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.RasterInterrupts;
using SomeGame.Main.Services;

namespace SomeGame.Main.GameInterface
{
    class PlayerStatusInterface : IGameInterface
    {
        private readonly PlayerStateManager _playerStateManager;
        private readonly GameSystem _gameSystem;
        private readonly Font _font;
        private const int _sectionsPerHeart = 4;

        public PlayerStatusInterface(PlayerStateManager playerStateManager, GameSystem gameSystem, 
            RasterBasedRenderService renderService, Scroller scroller)
        {
            _gameSystem = gameSystem;
            _playerStateManager = playerStateManager;
            _font = new Font(_gameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");

            var interfaceLayer = _gameSystem.GetLayer(LayerIndex.Interface);
            interfaceLayer.TileMap.SetEach((x, y) => new Tile(-1, TileFlags.None));

            interfaceLayer.Palette = PaletteIndex.P4;
            interfaceLayer.TileOffset = _gameSystem.GetTileOffset(TilesetContentKey.Hud);

            interfaceLayer.TileMap.SetTile(0, 0, new Tile(6, TileFlags.FlipH));
            interfaceLayer.TileMap.SetTile(0, 1, new Tile(7, TileFlags.FlipH));
            interfaceLayer.TileMap.SetTile(0, 2, new Tile(6, TileFlags.FlipHV));

            interfaceLayer.TileMap.SetEach(1, 39, 0, 1, (x, y) => new Tile(5, TileFlags.None));
            interfaceLayer.TileMap.SetEach(1, 39, 1, 2, (x, y) => new Tile(8, TileFlags.None));
            interfaceLayer.TileMap.SetEach(1, 39, 2, 3, (x, y) => new Tile(5, TileFlags.FlipV));

            interfaceLayer.TileMap.SetTile(39, 0, new Tile(6, TileFlags.None));
            interfaceLayer.TileMap.SetTile(39, 1, new Tile(7, TileFlags.None));
            interfaceLayer.TileMap.SetTile(39, 2, new Tile(6, TileFlags.FlipV));

            _font.WriteToLayer("SCORE", interfaceLayer, new Point(1, 1));

            _font.WriteToLayer("LIVES", interfaceLayer, new Point(20, 1));

            var bgColor = _gameSystem.BackgroundColorIndex;
            renderService.AddInterrupt(new ChangeBackgroundColor(_gameSystem, 3, 0));
            renderService.AddInterrupt(new ChangeBackgroundColor(_gameSystem, bgColor, 24));

            var bgXScroll = _gameSystem.RAM.DeclareInt();
            renderService.AddInterrupt(new SetBgXAutoScroll(_gameSystem, -5, 25, bgXScroll));
            renderService.AddInterrupt(new SetBgXAutoScroll(_gameSystem, 0, 96, bgXScroll));
        }

        public void Update()
        {
            var playerState = _playerStateManager.CurrentState;

            var interfaceLayer = _gameSystem.GetLayer(LayerIndex.Interface);
            _font.WriteToLayer(playerState.Score.ToString("00000000"), interfaceLayer, new Point(7, 1));

            _font.WriteToLayer(playerState.Lives.ToString(), interfaceLayer, new Point(26, 1));

            int heartX = 39 - (playerState.Health.Max / _sectionsPerHeart);
            interfaceLayer.TileMap.SetEach(heartX, heartX + playerState.Health.Max / _sectionsPerHeart, 1, 2,
                (x, y) =>
                {
                    int heartIndex = x - heartX;
                    int fullHealthValue = (heartIndex + 1) * _sectionsPerHeart;
                    int emptyHealthValue = heartIndex * _sectionsPerHeart;

                    if (playerState.Health <= emptyHealthValue)
                        return new Tile(0, TileFlags.None);
                    else if (playerState.Health >= fullHealthValue)
                        return new Tile(1, TileFlags.None);
                    else
                    {
                        int partialValue = playerState.Health - emptyHealthValue;
                        return new Tile(5 - partialValue, TileFlags.None);
                    }
                });

            if (_gameSystem.Paused)
            {
                string label = "PAUSED";
                int textX = ((_gameSystem.Screen.Width / _gameSystem.TileSize) - label.Length) / 2;
                _font.WriteToLayer(label, interfaceLayer, new Point(textX, 10));
            }
            else
            {
                //hack
                string label = "      ";
                int textX = ((_gameSystem.Screen.Width / _gameSystem.TileSize) - label.Length) / 2;
                _font.WriteToLayer(label, interfaceLayer, new Point(textX, 10));
            }
        }
        
    }
}
