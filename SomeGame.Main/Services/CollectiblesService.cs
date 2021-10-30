using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using System.Collections.Generic;

namespace SomeGame.Main.Services
{
    class CollectiblesService
    {
        private readonly GameSystem _gameSystem;
        private readonly Layer _layer;
        private List<MapCollectible> _collectibles = new List<MapCollectible>();
        private ActorPool _coins;
        private ActorPool _gems;
        private ActorPool _apples;
        private ActorPool _meat;
        private ActorPool _keys;

        public CollectiblesService(GameSystem gameSystem, Layer collectiblesLayer)
        {
            _gameSystem = gameSystem;
            _layer = collectiblesLayer;
        }

        public void Reset()
        {
            _collectibles = new List<MapCollectible>();
        }

        public void CreateCollectedItemActors(ActorFactory actorFactory)
        {
            _coins = actorFactory.CreatePool(ActorId.Coin, 3);
            _gems = actorFactory.CreatePool(ActorId.Gem, 2);
            _apples = actorFactory.CreatePool(ActorId.Apple, 2);
            _meat = actorFactory.CreatePool(ActorId.Meat, 2);
            _keys = actorFactory.CreatePool(ActorId.Key, 2);
        }

        public void AddCollectible(CollectibleId collectibleId, Point position, Point? position2 = null)
        {
            _collectibles.Add(CreateCollectible(collectibleId, position, position2));
        }

        private MapCollectible CreateCollectible(CollectibleId collectibleId, Point position, Point? position2=null)
        {
            switch(collectibleId)
            {
                case CollectibleId.Coin:
                    return new MapCoin(position, position2 ?? position, _gameSystem, _coins)
                        .StampOntoMap(_layer);
                case CollectibleId.Gem:
                    return new MapGem(position, _gameSystem, _gems)
                        .StampOntoMap(_layer);
                case CollectibleId.Apple:
                    return new MapApple(position, _gameSystem, _apples)
                        .StampOntoMap(_layer);
                case CollectibleId.Meat:
                    return new MapMeat(position, _gameSystem, _meat)
                        .StampOntoMap(_layer);
                case CollectibleId.Key:
                    return new MapKey(position, _gameSystem, _keys)
                        .StampOntoMap(_layer);
                default:
                    throw new System.Exception("Invalid collectible type");
            }
        }

        public CollisionInfo HandleCollectibleCollision(int tileX, int tileY, Layer layer)
        {
            foreach(var collectible in _collectibles)
            {
                var collisionInfo = collectible.CheckCollision(tileX, tileY,layer);
                if (collisionInfo != null)
                    return collisionInfo;
            }

            return null;
        }
    }
}
