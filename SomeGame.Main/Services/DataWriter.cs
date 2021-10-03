using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SomeGame.Main.Services
{
    class DataWriter : IDisposable
    {
        private readonly BinaryWriter _writer;
        private readonly Dictionary<Type, MethodInfo> _writeMethods;
        private readonly Dictionary<Type, MethodInfo> _customWriteMethods;

        public DataWriter(Stream stream)
        {
            _writer = new BinaryWriter(stream);
            _writeMethods = typeof(BinaryWriter)
                .GetMethods()
                .Where(p => p.Name == nameof(_writer.Write) && p.GetParameters().Length == 1)
                .ToDictionary(k => k.GetParameters()[0].ParameterType, v => v);

            _customWriteMethods = typeof(DataWriter)
                .GetMethods()
                .Where(p => p.Name == nameof(_writer.Write) && p.GetParameters().Length == 1)
                .ToDictionary(k => k.GetParameters()[0].ParameterType, v => v);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void Write<T>(Grid<T> grid)
        {
            _writer.Write(grid.Width);
            _writer.Write(grid.Height);
            grid.ForEach((x, y, v) => Write(v));
        }

        public void Write(Tile tile)
        {
            _writer.Write(tile.Index);
            _writer.Write((byte)tile.Flags);
        }

        public void Write(EditorTileSet tileSet)
        {
            WriteEnumerable(tileSet.Tiles);
        }

        public void WriteEnumerable<T>(IEnumerable<T> list)
        {
            _writer.Write(list.Count());
            foreach (var item in list)
                Write(item);
        }

        public void Write(EditorTile editorTile)
        {
            WriteEnumerable(editorTile.Themes);
            Write(editorTile.Tile);

            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                if (direction == Direction.None)
                    continue;

                var matches = editorTile.Matches[direction]
                                        .Select(p => p.Tile)
                                        .ToArray();

                WriteEnumerable(matches);
            }
        }

        private void Write(object o)
        {
            var customMethod = _customWriteMethods.GetValueOrDefault(o.GetType());
            if (customMethod != null)
                customMethod.Invoke(this, new object[] { o });
            else 
                _writeMethods[o.GetType()].Invoke(_writer, new object[] { o });            
        }
    }
}
