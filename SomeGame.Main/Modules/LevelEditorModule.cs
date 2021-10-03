using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{

    class LevelEditorModule : TileEditorBaseModule
    {
        protected override void Update()
        {
            var foreground = GameSystem.GetLayer(LayerIndex.FG);
            var background = GameSystem.GetLayer(LayerIndex.BG);

            if (Input.Right.IsDown())
            {
                background.ScrollX -= 2;
                foreground.ScrollX -= 2;
            }
            if (Input.Left.IsDown())
            {
                background.ScrollX += 2;
                foreground.ScrollX += 2;
            }
            if (Input.Down.IsDown())
            {
                background.ScrollY -= 2;
                foreground.ScrollY -= 2;
            }
            if (Input.Up.IsDown())
            {
                background.ScrollY += 2;
                foreground.ScrollY -= 2;
            }

            if (Input.Start.IsPressed())
                SaveMap(background.TileMap);
        }

        private void SaveMap(TileMap t)
        {
            _dataSerializer.Save(t);
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {      
            if(index == LayerIndex.BG)
            {
                var loaded = _dataSerializer.Load(LevelContentKey.TestLevel);
                layer.TileMap.SetEach((x, y) => loaded.GetTile(x, y));
            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            using var image = resourceLoader.LoadTexture(TilesetContentKey.Tiles);
            return new IndexedTilesetImage[] { image.ToIndexedTilesetImage()};
        }
    }
}
