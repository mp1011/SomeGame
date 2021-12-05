using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using System.Linq;

namespace SomeGame.Main.Services
{
    public static class TextureConverter
    {
        public static Color[] ToColorData(this Texture2D texture)
        {
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
            return pixels;
        }

        public static IndexedImage ToIndexedImage(this Grid<Color> pixels, Palette palette)
        {
            return new IndexedImage(pixels.Map(palette.GetIndex), palette);
        }

        public static IndexedImage ToIndexedImage(this Texture2D texture)
        {
            var pixels = ToColorData(texture);
            var palette = new Palette(pixels);
            return ToIndexedImage(pixels.ToGrid(texture.Width,texture.Height), palette);
        }

        public static IndexedTilesetImage ToIndexedTilesetImage(this Texture2D texture)
        {
            var pixels = ToColorData(texture);
            var palette = new Palette(pixels);
            var key = texture.Name.Split('\\')
                                  .Last()
                                  .ParseEnum<TilesetContentKey>();

            var grid = pixels.ToGrid(texture.Width, texture.Height)
                             .Map(palette.GetIndex);

            return new IndexedTilesetImage(key, grid, palette);
        }

        public static IndexedTilesetImage ToIndexedTilesetImage(this Texture2D texture, Palette palette)
        {
            var pixels = ToColorData(texture);
            var key = texture.Name.Split('\\')
                                  .Last()
                                  .ParseEnum<TilesetContentKey>();

            var grid = pixels.ToGrid(texture.Width, texture.Height)
                             .Map(palette.GetIndex);

            return new IndexedTilesetImage(key, grid, palette);
        }


        public static IndexedImage ToIndexedImage(this Texture2D texture, Palette palette)
        {
            var pixels = ToColorData(texture);
            return ToIndexedImage(pixels.ToGrid(texture.Width, texture.Height), palette);
        }

        public static Texture2D ToTexture2D(this IndexedImage indexedImage, GraphicsDevice graphicsDevice)
        {
            var texture = new Texture2D(graphicsDevice, indexedImage.Image.Width, indexedImage.Image.Height);
            var colors = indexedImage.Image
                                        .Map(ix => indexedImage.Palette[ix])
                                        .ToArray();
            texture.SetData(colors);

            return texture;
        }

        public static Texture2D ToTexture2D(this Palette palette, GraphicsDevice graphicsDevice)
        {
            var texture = new Texture2D(graphicsDevice, palette.Count(),1);
            texture.SetData(palette.ToArray());
            return texture;
        }
    }
}
