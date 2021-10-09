﻿using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SomeGame.Main.Services
{
    class DataSerializer
    {
        private readonly DirectoryInfo _contentFolder;

        public DataSerializer()
        {
            var dir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;

#if DEBUG
            dir = dir.Parent;
#endif
            while (_contentFolder == null)
            {
                _contentFolder = dir.GetDirectories()
                                    .FirstOrDefault(p => p.Name == "Content");
                dir = dir.Parent;
            }
        }

        private string GetPath(LevelContentKey levelContentKey)
        {
            return $"{_contentFolder.FullName}\\Levels\\{levelContentKey}.bin";
        }
        private string GetPath(TilesetContentKey tilesetContentKey)
        {
            return $"{_contentFolder.FullName}\\Tilesets\\{tilesetContentKey}.bin";
        }

        private string GetImagePath(TilesetContentKey tilesetContentKey)
        {
            return $"{_contentFolder.FullName}\\Tilesets\\{tilesetContentKey}.png";
        }

        private string GetSpriteFramePath(TilesetContentKey tilesetContentKey)
        {
            return $"{_contentFolder.FullName}\\Tilesets\\{tilesetContentKey}_SpriteFrames.bin";
        }

        public void SaveTilesetImage(TilesetContentKey tilesetContentKey, Texture2D image)
        {
            var path = GetImagePath(tilesetContentKey);
            if (File.Exists(path))
                File.Delete(path);

            using var stream = File.OpenWrite(path);
            image.SaveAsPng(stream, image.Width, image.Height);
        }

        public void Save(TileMap tileMap)
        {
            Save(tileMap.GetGrid(), GetPath(LevelContentKey.TestLevel));
        }

        public void Save(TilesetContentKey tilesetContentKey, SpriteFrame[] spriteFrames)
        {
            Save(spriteFrames, GetSpriteFramePath(tilesetContentKey));
        }

        public void Save(EditorTileSet editorTileSet)
        {
            Save(editorTileSet, GetPath(TilesetContentKey.Tiles));
        }

        private void Save(EditorTileSet editorTileSet, string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            using var fileStream = File.OpenWrite(path);
            using var writer = new DataWriter(fileStream);
            writer.Write(editorTileSet);
        }

        private void Save(SpriteFrame[] spriteFrames, string path)
        {
            using var fileStream = File.OpenWrite(path);
            using var writer = new DataWriter(fileStream);
            writer.WriteEnumerable(spriteFrames);
        }

        private void Save<T>(Grid<T> grid, string path)
        {
            using var fileStream = File.OpenWrite(path);
            using var writer = new DataWriter(fileStream);
            writer.Write(grid);
        }

        public TileMap Load(LevelContentKey key)
        {
            var grid = LoadGrid<Tile>(GetPath(key));
            return new TileMap(grid);
        }

        private Grid<T> LoadGrid<T>(string path)
        {
            using var fileStream = File.OpenRead(path);
            using var reader = new DataReader(fileStream);
            return reader.ReadGrid<T>();
        }

        public EditorTileSet Load(TilesetContentKey tilesetContentKey)
        {
            var path = GetPath(tilesetContentKey);
            using var fileStream = File.OpenRead(path);
            using var reader = new DataReader(fileStream);
            return reader.ReadEditorTileset();
        }

        public SpriteFrame[] LoadSpriteFrames(TilesetContentKey tilesetContentKey)
        {
            var path = GetSpriteFramePath(tilesetContentKey);
            if (!File.Exists(path))
                return new SpriteFrame[] { };
            using var fileStream = File.OpenRead(path);
            using var reader = new DataReader(fileStream);
            return reader.ReadEnumerable<SpriteFrame>();
        }

    }
}
