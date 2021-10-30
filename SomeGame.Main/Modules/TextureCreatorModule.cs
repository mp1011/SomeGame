using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
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
        private readonly ResourceLoader _resourceLoader;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly ImageContentKey _src;
        private readonly TilesetContentKey _dest;
        private readonly TileSetService _tileSetService = new TileSetService();
        private readonly DataSerializer _dataSerializer = new DataSerializer();

        public TextureCreatorModule(ImageContentKey src, TilesetContentKey dest, 
            ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _resourceLoader = new ResourceLoader(contentManager);
            _graphicsDevice = graphicsDevice;
            _src = src;
            _dest = dest;
        }

        public Rectangle Screen => new Rectangle(0, 0, 100, 100);

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void Initialize()
        {
            var img = _resourceLoader.LoadTexture(_src)
                                    .ToIndexedImage();

            var tileset = _tileSetService.CreateTilesetFromImage(img)
                                         .ToTexture2D(_graphicsDevice);

            _dataSerializer.SaveTilesetImage(_dest, tileset);
        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
