using System.Linq;

namespace SomeGame.Main.Models
{
    class Font
    {
        private readonly string _charset;
        private readonly int _tileOffset = 0;

        public Font(int tileOffset, string charset)
        {
            _tileOffset = tileOffset;
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
                return new Tile(_tileOffset + index, TileFlags.None);
        }
    }
}
