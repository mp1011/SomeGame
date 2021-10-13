using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SomeGame.Main.Services
{
    class DataReader : IDisposable
    {
        private readonly BinaryReader _reader;
        private readonly Dictionary<Type, MethodInfo> _readMethods;
        private readonly Dictionary<Type, MethodInfo> _customReadMethods;

        public DataReader(Stream stream)
        {
            _reader = new BinaryReader(stream);
            _readMethods = typeof(BinaryReader)
                .GetMethods()
                .Where(p => p.Name.StartsWith("Read") 
                            && p.GetParameters().Length == 0
                            && p.Name != "Read"
                            && !p.Name.StartsWith("Read7"))
                .ToDictionary(k => k.ReturnType, v => v);

            _customReadMethods = typeof(DataReader)
                .GetMethods()
                .Where(p => p.Name.StartsWith("Read") && p.GetParameters().Length == 0)
                .ToDictionary(k => k.ReturnType, v => v);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public Grid<T> ReadGrid<T>()
        {
            var width = _reader.ReadInt32();
            var height = _reader.ReadInt32();
            var grid = new Grid<T>(width, height, (x, y) => Read<T>());
            return grid;          
        }

        public Tile ReadTile()
        {
            return new Tile(_reader.ReadInt32(), (TileFlags)_reader.ReadByte());
        }

        public EditorTileSet ReadEditorTileset()
        {
            var tileset = new EditorTileSet();

            var editorTiles = ReadEnumerable<EditorTile>()
                .ToArray();

            foreach(var inputTile in editorTiles)
            {
                var editorTile = tileset.GetOrAddTile(inputTile.Tile);
                foreach (var theme in inputTile.Themes)
                    editorTile.AddTheme(theme);

                foreach (Direction direction in Enum.GetValues<Direction>())
                {
                    if (direction == Direction.None)
                        continue;

                    var inputMatches = inputTile.Matches[direction]
                        .Select(t => tileset.GetOrAddTile(t.Tile))
                        .ToArray();

                    editorTile.Matches[direction].AddRange(inputMatches);
                }

            }

            return tileset;
        }

        public EditorTile ReadEditorTile()
        {
            string[] themes = ReadEnumerable<string>();
            var tile = ReadTile();

            var editorTile = new EditorTile(tile);
            foreach (var theme in themes)
                editorTile.AddTheme(theme);

            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                if (direction == Direction.None)
                    continue;

                var matches = ReadEnumerable<Tile>()
                                .Select(t => new EditorTile(t))
                                .ToArray();

                editorTile.Matches[direction].AddRange(matches);
            }

            return editorTile;
        }

        public SpriteFrame ReadSpriteFrame()
        {
            return new SpriteFrame(ReadTile(), ReadTile(), ReadTile(), ReadTile());
        }

        public Dictionary<AnimationKey, Animation> ReadAnimations()
        {
            var keys = ReadEnumerable<byte>()
                .Select(b => (AnimationKey)b)
                .ToArray();

            var animations = new Dictionary<AnimationKey, Animation>();
            foreach (var key in keys)
                animations[key] = ReadAnimation();

            return animations;
        }

        public Animation ReadAnimation() => new Animation(ReadEnumerable<AnimationFrame>());

        public AnimationFrame ReadAnimationFrame() => new AnimationFrame(_reader.ReadByte(), _reader.ReadByte());

        public T ReadEnum<T>() where T:Enum
        {
            var value = _reader.ReadByte();
            return (T)Enum.ToObject(typeof(T), value);
        }

        public T[] ReadEnumerable<T>()
        {
            var count = _reader.ReadInt32();
            var ret = new T[count];
            for (int i = 0; i < count; i++)
                ret[i] = Read<T>();
            return ret;
        }

        private T Read<T>()
        {
            var customMethod = _customReadMethods.GetValueOrDefault(typeof(T));
            if (customMethod != null)
                return (T)customMethod.Invoke(this, new object[] {});
            else
                return (T)_readMethods[typeof(T)].Invoke(_reader, new object[] {});
        }
    }
}
