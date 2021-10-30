using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;

namespace SomeGame.Main.Models
{
    interface ICollectible
    {
        Point TileLocation { get; }
    }

    abstract class MapCollectible : ICollectible
    {
        private readonly ActorPool _collectedItemPool;
        private readonly GameSystem _gameSystem;
        protected readonly int _tileOffset;
        public Point TileLocation { get; }

        protected abstract int Width { get; }
        protected abstract int Height { get; }

        public MapCollectible(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool)
        {
            TileLocation = tileLocation;
            _tileOffset = gameSystem.GetTileOffset(TilesetContentKey.Items);
            _gameSystem = gameSystem;
            _collectedItemPool = collectedItemPool;
        }

        public abstract MapCollectible StampOntoMap(Layer layer);

        public CollisionInfo CheckCollision(int tileX, int tileY, Layer layer)
        {
            if(tileX >= TileLocation.X 
                && tileY >= TileLocation.Y 
                && tileX <= TileLocation.X + Width
                && tileY <= TileLocation.Y + Height)
            {
                EraseFromMap(tileX, tileY, layer);

                var collected = _collectedItemPool.ActivateNext();
                if (collected == null)
                    return new CollisionInfo();

                collected.WorldPosition.X = tileX * _gameSystem.TileSize;
                collected.WorldPosition.Y = tileY * _gameSystem.TileSize;
                return new CollisionInfo(collected);
            }

            return null;
        }

        protected virtual void EraseFromMap(int tileX, int tileY, Layer layer)
        {
            var tile = TileLocation;

            while (tile.Y < TileLocation.Y+Height)
            {
                while (tile.X <= TileLocation.X+Width)
                {
                    layer.TileMap.SetTile(tile.X, tile.Y, new Tile(-1, TileFlags.None));
                    tile = tile.Offset(1, 0);
                }
                tile = new Point(TileLocation.X, tile.Y + 1);
            }
        }
    }



    class MapCoin : MapCollectible
    {
        private Point _bottomRight;
        protected override int Width => _bottomRight.X- TileLocation.X;
        protected override int Height => _bottomRight.Y - TileLocation.Y;
        public MapCoin(Point topLeft, Point bottomRight, GameSystem gameSystem, ActorPool actorPool) 
            : base(topLeft, gameSystem, actorPool)
        {
            _bottomRight = bottomRight;
        }

        public override MapCollectible StampOntoMap(Layer layer)
        {
            var tile = TileLocation;

            while (tile.Y <= _bottomRight.Y)
            {
                while (tile.X <= _bottomRight.X)
                {
                    layer.TileMap.SetTile(tile.X, tile.Y, new Tile(_tileOffset + 2, TileFlags.Collectible));
                    tile = tile.Offset(1, 0);
                }
                tile = new Point(TileLocation.X, tile.Y + 1);
            }

            return this;
        }

        protected override void EraseFromMap(int tileX, int tileY, Layer layer)
        {
            layer.TileMap.SetTile(tileX, tileY, new Tile(-1, TileFlags.None));
        }
    }

    class MapGem : MapCollectible
    {
        protected override int Width => 2;
        protected override int Height => 2;

        public MapGem(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool) 
            : base(tileLocation, gameSystem, collectedItemPool)
        {
        }

        public override MapCollectible StampOntoMap(Layer layer)
        {
            layer.TileMap.SetTile(TileLocation.X, TileLocation.Y, new Tile(_tileOffset, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X+1, TileLocation.Y, new Tile(_tileOffset+1, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X, TileLocation.Y+1, new Tile(_tileOffset+3, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X+1, TileLocation.Y+1, new Tile(_tileOffset+4, TileFlags.Collectible));
            return this;
        }
    }

    class MapApple : MapCollectible
    {
        protected override int Width => 2;
        protected override int Height => 2;
        public MapApple(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool) 
            : base(tileLocation, gameSystem, collectedItemPool)
        {
        }

        public override MapCollectible StampOntoMap(Layer layer)
        {
            layer.TileMap.SetTile(TileLocation.X, TileLocation.Y, new Tile(_tileOffset+5, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X + 1, TileLocation.Y, new Tile(_tileOffset+6, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X, TileLocation.Y + 1, new Tile(_tileOffset+9, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X + 1, TileLocation.Y + 1, new Tile(_tileOffset+10, TileFlags.Collectible));
            return this;
        }
    }

    class MapMeat : MapCollectible
    {
        protected override int Width => 2;
        protected override int Height => 2;
        public MapMeat(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool)
            : base(tileLocation, gameSystem,collectedItemPool)
        {
        }

        public override MapCollectible StampOntoMap(Layer layer)
        {
            layer.TileMap.SetTile(TileLocation.X, TileLocation.Y, new Tile(_tileOffset+7, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X + 1, TileLocation.Y, new Tile(_tileOffset+8, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X, TileLocation.Y + 1, new Tile(_tileOffset+11, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X + 1, TileLocation.Y + 1, new Tile(_tileOffset+12, TileFlags.Collectible));
            return this;
        }
    }

    class MapKey : MapCollectible
    {
        protected override int Width => 2;
        protected override int Height => 1;
        public MapKey(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool) 
            : base(tileLocation, gameSystem, collectedItemPool)
        {
        }

        public override MapCollectible StampOntoMap(Layer layer)
        {
            layer.TileMap.SetTile(TileLocation.X, TileLocation.Y, new Tile(_tileOffset+13, TileFlags.Collectible));
            layer.TileMap.SetTile(TileLocation.X+1, TileLocation.Y, new Tile(_tileOffset+14, TileFlags.Collectible));
            return this;
        }
    }
}
