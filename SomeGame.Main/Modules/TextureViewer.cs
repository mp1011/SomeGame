using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    class TextureViewer : IGameModule
    {
        private ImageContentKey _imageKey;
        private Texture2D _texture;
        private readonly ResourceLoader _resourceLoader;

        public TextureViewer(ImageContentKey imageKey, ContentManager contentManager)
        {
            _resourceLoader = new ResourceLoader(contentManager);
            _imageKey = imageKey;
        }

        public Rectangle Screen => new Rectangle(0,0,320,240);

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
        }

        public void Initialize()
        {
            _texture = _resourceLoader.LoadTexture(_imageKey);
        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
