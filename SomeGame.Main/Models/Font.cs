using System.Linq;

namespace SomeGame.Main.Models
{
    class Font
    {
        private readonly TileSet _tileSet;
        private readonly string _charset;

        public Font(TileSet tileSet, string charset)
        {
            _tileSet = tileSet;
            _charset = charset;
        }

        public Tile[] FromString(string str)
        {
            return str.Select(chr => GetTile(chr))
                      .ToArray();
        }

        private Tile GetTile(char c)
        {
            var index = _charset.IndexOf(c);
            if (index < 0)
                return new Tile(-1, TileFlags.None);
            else
                return new Tile(index, TileFlags.None);
        }
    }
}
