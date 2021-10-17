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

        public Color this[byte b] => _colors[new RotatingInt((int)b, _colors.Length)];

        public Palette(IEnumerable<Color> colors)
        {

            _colors = new Color[] { new Color(0, 0, 0, 0) }
                        .Union(colors.Where(p => p.A == 255))
                        .ToArray();
        }

        public byte GetIndex(Color color)
        {
            return (byte)Array.IndexOf(_colors, color);
        }

        public byte? GetIndexOrDefault(Color color)
        {
            var index = Array.IndexOf(_colors, color);
            if (index == -1)
                return null;
            else
                return (byte)index;
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
        
        public Dictionary<byte, byte> CreateMap(Palette other)
        {
            var map = new Dictionary<byte, byte>();
            for(byte index = 0; index < _colors.Length;index++)
            {
                var mapped = other.GetIndexOrDefault(_colors[index]);
                if (mapped == null)
                    return null;

                map[index] = mapped.Value;
            }

            return map;
        }
    }
}
