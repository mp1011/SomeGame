using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    class TextureCreatorModule : IGameModule
    {
        private readonly ResourceLoader _resourceLoader;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly TileSetService _tileSetService = new TileSetService();
        private readonly DataSerializer _dataSerializer = new DataSerializer();

        public TextureCreatorModule(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _resourceLoader = new ResourceLoader(contentManager);
            _graphicsDevice = graphicsDevice;
        }


        public Color BackgroundColor => Color.Black;
        public Rectangle Screen => new Rectangle(0, 0, 100, 100);

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void Initialize()
        {
            CreateTileset("MountainSky", TilesetContentKey.Mountains);
        }

        private void CreateTileset(string sourceImage, TilesetContentKey key)
        {
            var img = _dataSerializer.LoadImageFromDisk(_graphicsDevice, "MountainSky")
              .ToIndexedImage();

            var tileset = _tileSetService.CreateTilesetFromImage(img)
                                         .ToTexture2D(_graphicsDevice);

            _dataSerializer.SaveTilesetImage(key.ToString(), tileset);
        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public bool Update(GameTime gameTime)
        {
            return false;
        }
    }
}
