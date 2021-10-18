using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Services
{
    class CollectiblesService
    {
        private readonly GameSystem _gameSystem;

        public CollectiblesService(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
        }

        public void AddCoins(Point upperLeft, Point lowerRight, Layer layer)
        {
            var tile = upperLeft;

            while (tile.Y <= lowerRight.Y)
            {
                while (tile.X <= lowerRight.X)
                {
                    var coin = new MapCoin(tile, _gameSystem);
                    coin.StampOntoMap(layer);

                    tile = tile.Offset(1, 0);
                }
                tile = new Point(upperLeft.X, tile.Y + 1);
            }
        }
    }
}
