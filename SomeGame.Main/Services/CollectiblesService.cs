using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using System.Collections.Generic;

namespace SomeGame.Main.Services
{
    class CollectiblesService
    {
        private readonly GameSystem _gameSystem;
        private readonly Scroller _scroller;
        private List<MapCollectible> _collectibles = new List<MapCollectible>();
        private ActorPool _coins;
        private ActorPool _gems;
        private ActorPool _apples;
        private ActorPool _meat;
        private ActorPool _keys;

        public CollectiblesService(GameSystem gameSystem, Scroller scroller)
        {
            _scroller = scroller;
            _gameSystem = gameSystem;
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

        public void AddCollectible(CollectibleId collectibleId, Point position, TileMap collectiblesLayer)
        {
            _collectibles.Add(CreateCollectible(collectibleId, position, collectiblesLayer));
        }

        private MapCollectible CreateCollectible(CollectibleId collectibleId, Point position, TileMap collectiblesLayer)
        {
            switch(collectibleId)
            {
                case CollectibleId.Coin:
                    return new MapCoin(position, _gameSystem, _coins, collectiblesLayer)
                        .StampOntoMap();
                case CollectibleId.Gem:
                    return new MapGem(position, _gameSystem, _gems, collectiblesLayer)
                        .StampOntoMap();
                case CollectibleId.Apple:
                    return new MapApple(position, _gameSystem, _apples, collectiblesLayer)
                        .StampOntoMap();
                case CollectibleId.Meat:
                    return new MapMeat(position, _gameSystem, _meat, collectiblesLayer)
                        .StampOntoMap();
                case CollectibleId.Key:
                    return new MapKey(position, _gameSystem, _keys, collectiblesLayer)
                        .StampOntoMap();
                default:
                    throw new System.Exception("Invalid collectible type");
            }
        }

        public CollisionInfo HandleCollectibleCollision(IGameRectangle actorPosition)
        {
            var layer = _gameSystem.GetLayer(LayerIndex.FG);

            foreach(var collectible in _collectibles)
            {
                if (collectible.Collected)
                    continue;

                if (!actorPosition.IntersectsWithRec(collectible.Position))
                    continue;

                collectible.EraseFromMap(_scroller.GetTopLeftTile(LayerIndex.FG), layer);
                var actor = collectible.CreateActiveObject();
                if (actor != null)
                    return new CollisionInfo(Actor: actor);
            }

            return null;
        }
    }
}
