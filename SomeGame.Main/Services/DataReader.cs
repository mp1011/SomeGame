using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
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

        public EditorTileSet ReadEditorTileset(TilesetContentKey key)
        {
            var tileset = new EditorTileSet(key);
            tileset.Blocks.AddRange(ReadEnumerable<EditorBlock>());
            return tileset;
        }

        public EditorBlock ReadEditorTile()
        {
            return new EditorBlock(_reader.ReadString(),ReadGrid<Tile>());
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

        public SceneInfo ReadScene()
        {
            return new SceneInfo(
                BgMap: ReadLayerInfo(),
                FgMap: ReadLayerInfo(),
                InterfaceType: ReadEnum<InterfaceType>(),
                Bounds: ReadRectangle(),
                PaletteKeys: ReadPaletteKeys(),
                BackgroundColor: _reader.ReadByte(),
                VramImages: ReadEnumerable<TilesetWithPalette>(),
                Sounds: ReadEnumerable<SoundInfo>(),
                Actors: ReadEnumerable<ActorStart>(),
                CollectiblePlacements: ReadEnumerable<CollectiblePlacement>(),
                Transitions: ReadSceneTransitions());
        }

        public SceneTransitions ReadSceneTransitions() => new SceneTransitions(
            ReadEnum<SceneContentKey>(),
            ReadEnum<SceneContentKey>(),
            ReadEnum<SceneContentKey>(),
            ReadEnum<SceneContentKey>(),
            ReadEnum<SceneContentKey>(),
            ReadEnum<SceneContentKey>());

        public LayerInfo ReadLayerInfo() => new LayerInfo(ReadEnum<LevelContentKey>(), ReadEnum<PaletteIndex>(), _reader.ReadByte());

        public Rectangle ReadRectangle() => new Rectangle(
            _reader.ReadInt32(), 
            _reader.ReadInt32(), 
            _reader.ReadInt32(), 
            _reader.ReadInt32());

        public PaletteKeys ReadPaletteKeys() => new PaletteKeys(
            ReadEnum<ImageContentKey>(),
            ReadEnum<ImageContentKey>(),
            ReadEnum<ImageContentKey>(),
            ReadEnum<ImageContentKey>());


        public TilesetWithPalette ReadTilesetWithPalette() =>
            new TilesetWithPalette(ReadEnum<TilesetContentKey>(), ReadEnum<PaletteIndex>());

        public SoundInfo ReadSoundInfo() =>
            new SoundInfo(ReadEnum<SoundContentKey>(), _reader.ReadByte());

        public ActorStart ReadActorStart() =>
            new ActorStart(ReadEnum<ActorId>(), ReadPixelPoint());

        public PixelPoint ReadPixelPoint() => new PixelPoint(ReadPixelValue(), ReadPixelValue());

        public PixelValue ReadPixelValue() => new PixelValue(_reader.ReadInt32(), _reader.ReadInt32());

        public CollectiblePlacement ReadCollectiblePlacement()
        {
            var id = ReadEnum<CollectibleId>();
            int x = _reader.ReadInt32();
            int y = _reader.ReadInt32();

            return new CollectiblePlacement(id, new Point(x, y));            
        }

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
