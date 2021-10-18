using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Services
{
    class CollectiblesService
    {
        private readonly GameSystem _gameSystem;
        private readonly Layer _layer;

        private ActorPool _coins;

        public CollectiblesService(GameSystem gameSystem, Layer collectiblesLayer)
        {
            _gameSystem = gameSystem;
            _layer = collectiblesLayer;
        }

        public void CreateCollectedItemActors(ActorFactory actorFactory)
        {
            _coins = actorFactory.CreatePool(ActorId.Coin, 3);
        }

        public void AddCoins(Point upperLeft, Point lowerRight)
        {
            var tile = upperLeft;

            while (tile.Y <= lowerRight.Y)
            {
                while (tile.X <= lowerRight.X)
                {
                    var coin = new MapCoin(tile, _gameSystem);
                    coin.StampOntoMap(_layer);

                    tile = tile.Offset(1, 0);
                }
                tile = new Point(upperLeft.X, tile.Y + 1);
            }
        }

        public CollisionInfo HandleCollectibleCollision(int tileX, int tileY)
        {
            _layer.TileMap.SetTile(tileX, tileY, new Tile(-1, TileFlags.None));

            var coin = _coins.ActivateNext();
            if (coin == null)
                return new CollisionInfo();

            coin.WorldPosition.X = tileX * _gameSystem.TileSize;
            coin.WorldPosition.Y = tileY * _gameSystem.TileSize;
            return new CollisionInfo(coin);
        }
    }
}
