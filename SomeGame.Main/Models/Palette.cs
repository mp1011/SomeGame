using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Models
{
    public class Palette : IEnumerable<Color>
    {
        private readonly Color[] _colors;

        public Color this[byte b] => _colors[(int)b];

        public Palette(IEnumerable<Color> colors)
        {
            _colors = colors
                        .Distinct()
                        .ToArray();
        }

        public byte GetIndex(Color color)
        {
            return (byte)Array.IndexOf(_colors, color);
        }

        public IEnumerator<Color> GetEnumerator()
        {
            return ((IEnumerable<Color>)_colors).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _colors.GetEnumerator();
        }

        public Palette CreateTransformed(Func<Color,Color> transform)
        {
            return new Palette(this.Select(transform));
        }
    }
}
