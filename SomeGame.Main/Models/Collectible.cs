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
        protected readonly TileMap _layer;

        public Point TileLocation { get; }

        protected abstract int Width { get; }
        protected abstract int Height { get; }

        public GameRectangle Position { get; }

        public bool Collected { get; private set; }

        public MapCollectible(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool, TileMap layer)
        {
            TileLocation = tileLocation;
            _tileOffset = gameSystem.GetTileOffset(TilesetContentKey.Items) - gameSystem.GetLayer(LayerIndex.FG).TileOffset;
            _gameSystem = gameSystem;
            _collectedItemPool = collectedItemPool;
            _layer = layer;

            Position = new GameRectangle(tileLocation.X * gameSystem.TileSize, tileLocation.Y * gameSystem.TileSize,
                                         Width * _gameSystem.TileSize, Height * gameSystem.TileSize);
        }

        public abstract MapCollectible StampOntoMap();

        public Actor CreateActiveObject()
        {
            var collected = _collectedItemPool.ActivateNext();
            if (collected == null)
                return null;

            collected.WorldPosition.X = Position.X;
            collected.WorldPosition.Y = Position.Y;
            return collected;
        }

        public void EraseFromMap(Point topLeftTile, Layer layer)
        {
            Collected = true;
            EraseFromMap(_layer, Point.Zero);
            EraseFromMap(layer.TileMap, topLeftTile);
        }

        private  void EraseFromMap(TileMap layer, Point topLeftTile)
        {
            var layerLocation = TileLocation - topLeftTile;
            var tile = layerLocation;

            while (tile.Y < layerLocation.Y+Height)
            {
                while (tile.X < layerLocation.X+Width)
                {
                    layer.SetTile(tile.X, tile.Y, new Tile(-1, TileFlags.None));
                    tile = tile.Offset(1, 0);
                }
                tile = new Point(layerLocation.X, tile.Y + 1);
            }
        }
    }

    class MapCoin : MapCollectible
    {
        protected override int Width => 1;
        protected override int Height => 1;
        public MapCoin(Point position, GameSystem gameSystem, ActorPool actorPool, TileMap layer) 
            : base(position, gameSystem, actorPool,layer)
        {
        }

        public override MapCollectible StampOntoMap()
        {
            var tile = TileLocation;
            _layer.SetTile(tile.X, tile.Y, new Tile(_tileOffset + 2, TileFlags.Collectible));
            return this;
        }
    }

    class MapGem : MapCollectible
    {
        protected override int Width => 2;
        protected override int Height => 2;

        public MapGem(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool, TileMap layer) 
            : base(tileLocation, gameSystem, collectedItemPool, layer)
        {
        }

        public override MapCollectible StampOntoMap()
        {
            _layer.SetTile(TileLocation.X, TileLocation.Y, new Tile(_tileOffset, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X+1, TileLocation.Y, new Tile(_tileOffset+1, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X, TileLocation.Y+1, new Tile(_tileOffset+3, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X+1, TileLocation.Y+1, new Tile(_tileOffset+4, TileFlags.Collectible));
            return this;
        }
    }

    class MapApple : MapCollectible
    {
        protected override int Width => 2;
        protected override int Height => 2;
        public MapApple(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool, TileMap layer) 
            : base(tileLocation, gameSystem, collectedItemPool,layer)
        {
        }

        public override MapCollectible StampOntoMap()
        {
            _layer.SetTile(TileLocation.X, TileLocation.Y, new Tile(_tileOffset+5, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X + 1, TileLocation.Y, new Tile(_tileOffset+6, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X, TileLocation.Y + 1, new Tile(_tileOffset+9, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X + 1, TileLocation.Y + 1, new Tile(_tileOffset+10, TileFlags.Collectible));
            return this;
        }
    }

    class MapMeat : MapCollectible
    {
        protected override int Width => 2;
        protected override int Height => 2;
        public MapMeat(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool, TileMap layer)
            : base(tileLocation, gameSystem,collectedItemPool,layer)
        {
        }

        public override MapCollectible StampOntoMap()
        {
            _layer.SetTile(TileLocation.X, TileLocation.Y, new Tile(_tileOffset+7, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X + 1, TileLocation.Y, new Tile(_tileOffset+8, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X, TileLocation.Y + 1, new Tile(_tileOffset+11, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X + 1, TileLocation.Y + 1, new Tile(_tileOffset+12, TileFlags.Collectible));
            return this;
        }
    }

    class MapKey : MapCollectible
    {
        protected override int Width => 2;
        protected override int Height => 1;
        public MapKey(Point tileLocation, GameSystem gameSystem, ActorPool collectedItemPool, TileMap layer) 
            : base(tileLocation, gameSystem, collectedItemPool, layer)
        {
        }

        public override MapCollectible StampOntoMap()
        {
            _layer.SetTile(TileLocation.X, TileLocation.Y, new Tile(_tileOffset+13, TileFlags.Collectible));
            _layer.SetTile(TileLocation.X+1, TileLocation.Y, new Tile(_tileOffset+14, TileFlags.Collectible));
            return this;
        }
    }
}
