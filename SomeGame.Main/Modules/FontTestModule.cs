using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.Linq;

namespace SomeGame.Main.Modules
{
    class FontTestModule : EditorModule
    {
        public FontTestModule(GameStartup startup) 
            : base(startup)
        {
        }

        protected override void Update()
        {
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.FG)
            {
                var font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
                font.WriteToLayer("HALLO WORLD", layer, new Point(0, 0));
            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            using var font = resourceLoader.LoadTexture(TilesetContentKey.Font);
            return new IndexedTilesetImage[] { font.ToIndexedTilesetImage() };
        }
    }
}
