using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;

namespace SomeGame.Main.Services
{
    class ResourceLoader
    {
        private readonly ContentManager _contentManager;

        public ResourceLoader(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public Texture2D LoadTexture(ImageContentKey key)
        {
            return _contentManager.Load<Texture2D>(@$"Images\{key}");
        }

        public Texture2D LoadTexture(TilesetContentKey key)
        {
            return _contentManager.Load<Texture2D>(@$"Tilesets\{key}");
        }
    }
}
