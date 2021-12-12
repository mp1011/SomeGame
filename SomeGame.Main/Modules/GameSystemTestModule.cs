using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    class GameSystemTestModule : EditorModule
    {
        public GameSystemTestModule(GameStartup startup) 
            : base(startup)
        {
        }

        protected override void Update()
        {
            var layer = GameSystem.GetLayer(LayerIndex.FG);

            if (Input.Left.IsDown())
                layer.ScrollX -= 2;
            if (Input.Right.IsDown())
                layer.ScrollX += 2;
            if (Input.Up.IsDown())
                layer.ScrollY -= 2;
            if (Input.Down.IsDown())
                layer.ScrollY += 2;
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
            if (index == LayerIndex.BG)
            {
                var font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
                font.WriteToLayer("HELLO WORLD", layer, new Point(3, 4));
            }

            if (index == LayerIndex.FG)
            {
                layer.TileMap.SetTile(0, 0, new Tile(16, TileFlags.None));

                layer.TileMap.SetTile(40, 0, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(40, 1, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 0, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 1, new Tile(17, TileFlags.None));

                layer.TileMap.SetTile(0, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(1, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(2, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(0, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(1, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(2, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(0, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(1, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(2, 22, new Tile(17, TileFlags.None));


                layer.TileMap.SetTile(40, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(42, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(43, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(40, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(42, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(43, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(40, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(42, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(43, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(40, 23, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 23, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(42, 23, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(43, 23, new Tile(17, TileFlags.None));


            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            using var image = resourceLoader.LoadTexture(TilesetContentKey.Tiles);
            using var font = resourceLoader.LoadTexture(TilesetContentKey.Font);

            return new IndexedTilesetImage[] { image.ToIndexedTilesetImage(), font.ToIndexedTilesetImage() };
        }
    }
}
