using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using System.Collections.Generic;

namespace SomeGame.Main.Services
{
    class ResourceLoader
    {
        private readonly ContentManager _contentManager;

        public ResourceLoader(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public IEnumerable<Texture2D> LoadBlocks(string theme)
        {
            int index = 1;
            bool done = false;

            while(!done)
            {
                Texture2D texture = null;
                try
                {
                    texture =_contentManager.Load<Texture2D>(@$"Blocks\{theme}_{index}");
                    index++;
                }
                catch
                {
                    done=true;
                }

                if (texture != null)
                    yield return texture;
            }
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
