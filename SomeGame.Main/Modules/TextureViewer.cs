using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    class TextureViewer : IGameModule
    {
        private ImageContentKey _imageKey;
        private Texture2D _texture;

        public TextureViewer(ImageContentKey imageKey)
        {
            _imageKey = imageKey;
        }

        public Rectangle Screen => new Rectangle(0,0,320,240);

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
        }

        public void Initialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            _texture = resourceLoader.LoadTexture(_imageKey);
        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
