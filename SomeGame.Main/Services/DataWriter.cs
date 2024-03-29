﻿using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
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

        public void Write(SongData song)
        {
            WriteEnumerable(song.Stems);
            WriteEnumerable(song.SongSequence);
        }

        public void Write(SongStem stem)
        {
            _writer.Write((byte)stem.Group);
            _writer.Write(stem.Number);
            _writer.Write(stem.Volume);
        }

        public void Write(SongSection section)
        {
            _writer.Write(section.Channel1Index);
            _writer.Write(section.Channel2Index);
        }

        public void Write(TilesetContentKey key) => _writer.Write((byte)key);
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
            WriteEnumerable(tileSet.Blocks);
        }

        public void WriteEnumerable<T>(IEnumerable<T> list)
        {
            _writer.Write(list.Count());
            foreach (var item in list)
                Write(item);
        }

        public void Write(Dictionary<AnimationKey, Animation> animations)
        {
            var keys = animations.Keys.ToArray();

            WriteEnumerable(keys.Cast<byte>());

            foreach(var key in keys)
                Write(animations[key]);
        }

        public void Write(Animation animation) => WriteEnumerable(animation.Frames);
        
        public void Write(AnimationFrame animationFrame)
        {
            _writer.Write(animationFrame.SpriteFrameIndex);
            _writer.Write(animationFrame.Duration);
        }

        public void Write(EditorBlock editorBlock)
        {
            _writer.Write(editorBlock.Theme);
            Write(editorBlock.Grid);
        }

        public void Write(SpriteFrame spriteFrame)
        {
            Write(spriteFrame.TopLeft);
            Write(spriteFrame.TopRight);
            Write(spriteFrame.BottomLeft);
            Write(spriteFrame.BottomRight);
        }

        public void Write(SceneInfo scene)
        {
            Write(scene.BgMap);
            Write(scene.FgMap);
            _writer.Write((byte)scene.InterfaceType);
            _writer.Write((byte)scene.Song);
            Write(scene.Bounds);
            _writer.Write(scene.BackgroundColor);
            WriteEnumerable(scene.VramImagesP1);
            WriteEnumerable(scene.VramImagesP2);
            WriteEnumerable(scene.VramImagesP3);
            WriteEnumerable(scene.VramImagesP4);
            WriteEnumerable(scene.Sounds);
            WriteEnumerable(scene.Actors);
            WriteEnumerable(scene.CollectiblePlacements);
            Write(scene.Transitions);
        }

        public void Write(SceneTransitions sceneTransitions)
        {
            _writer.Write((byte)sceneTransitions.Left);
            _writer.Write((byte)sceneTransitions.Right);
            _writer.Write((byte)sceneTransitions.Up);
            _writer.Write((byte)sceneTransitions.Down);
            _writer.Write((byte)sceneTransitions.Door1);
            _writer.Write((byte)sceneTransitions.Door2);
        }

        public void Write(ActorStart actorStart)
        {
            _writer.Write((byte)actorStart.ActorId);
            Write(actorStart.Position);
            _writer.Write((byte)actorStart.Palette);
        }

        public void Write(CollectiblePlacement collectiblePlacement)
        {
            _writer.Write((byte)collectiblePlacement.Id);
            _writer.Write(collectiblePlacement.Position.X);
            _writer.Write(collectiblePlacement.Position.Y);
        }

        public void Write(PixelPoint pixelPoint)
        {
            Write(pixelPoint.X);
            Write(pixelPoint.Y);
        }

        public void Write(PixelValue pixelValue)
        {
            _writer.Write(pixelValue.Pixel);
            _writer.Write(pixelValue.SubPixel);
        }

        public void Write(Rectangle rectangle)
        {
            _writer.Write(rectangle.X);
            _writer.Write(rectangle.Y);
            _writer.Write(rectangle.Width);
            _writer.Write(rectangle.Height);
        }

        public void Write(LayerInfo layerInfo)
        {
            _writer.Write((byte)layerInfo.Key);
            _writer.Write((byte)layerInfo.Palette);
            _writer.Write((byte)layerInfo.ScrollFactor);
        }

        public void Write(SoundInfo soundInfo)
        {
            _writer.Write((byte)soundInfo.Key);
            _writer.Write(soundInfo.MaxOccurences);
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
