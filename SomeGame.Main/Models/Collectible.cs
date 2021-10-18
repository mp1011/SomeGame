using Microsoft.Xna.Framework;
using SomeGame.Main.Content;

namespace SomeGame.Main.Models
{
    interface ICollectible
    {
        Point TileLocation { get; }
    }

    abstract class MapCollectible : ICollectible
    {
        protected readonly int _tileOffset;
        public Point TileLocation { get; }

        public MapCollectible(Point tileLocation, GameSystem gameSystem)
        {
            TileLocation = tileLocation;
            _tileOffset = gameSystem.GetTileOffset(TilesetContentKey.Items);
        }

        public abstract void StampOntoMap(Layer layer);
    }

    class MapCoin : MapCollectible
    {
        public MapCoin(Point tileLocation, GameSystem gameSystem) : base(tileLocation, gameSystem)
        {
        }

        public override void StampOntoMap(Layer layer)
        {
            layer.TileMap.SetTile(TileLocation.X, TileLocation.Y, new Tile(_tileOffset + 2, TileFlags.Collectible));
        }
    }
}
