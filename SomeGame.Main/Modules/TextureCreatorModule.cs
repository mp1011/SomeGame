using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeGame.Main.Modules
{
    class TextureCreatorModule : IGameModule
    {
        private readonly TileSetService _tileSetService = new TileSetService();

        public Rectangle Screen => new Rectangle(0, 0, 100, 100);

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void Initialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            var img = resourceLoader.LoadTexture(Content.ImageContentKey.Hero)
                                    .ToIndexedImage();

            var tileset = _tileSetService.CreateTilesetFromImage(img)
                                         .ToTexture2D(graphicsDevice);

            using var stream = File.OpenWrite("characters.png");
            tileset.SaveAsPng(stream, tileset.Width,tileset.Height);
        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
