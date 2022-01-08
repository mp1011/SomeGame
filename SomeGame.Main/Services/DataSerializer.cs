using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System.Collections.Generic;
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
            while (_contentFolder == null || !_contentFolder.Exists)
            {
                _contentFolder = dir.GetDirectories()
                                    .FirstOrDefault(p => p.Name == "Content")
                                    ?? new DirectoryInfo($"{dir.FullName}\\SomeGame.Main\\Content");
                
                dir = dir.Parent;
            }
        }

        private string GetPath(LevelContentKey levelContentKey)
        {
            return $"{_contentFolder.FullName}\\Levels\\{levelContentKey}.bin";
        }

        private string GetPath(MusicContentKey songKey)
        {
            return $"{_contentFolder.FullName}\\Music\\{songKey}.bin";
        }

        private string GetPath(SceneContentKey contentKey)
        {
            return $"{_contentFolder.FullName}\\Levels\\Scene_{contentKey}.bin";
        }

        private string GetPath(TilesetContentKey tilesetContentKey)
        {
            return $"{_contentFolder.FullName}\\Tilesets\\{tilesetContentKey}.bin";
        }

        private string GetImagePath(TilesetContentKey tilesetContentKey)
        {
            return $"{_contentFolder.FullName}\\Tilesets\\{tilesetContentKey}.png";
        }

        private string GetImagePath(ImageContentKey key)
        {
            return $"{_contentFolder.FullName}\\Images\\{key}.png";
        }

        private string GetAnimationPath(ActorId actorId)
        {
            return $"{_contentFolder.FullName}\\Data\\{actorId}_Animations.bin";
        }

        private string GetSpriteFramePath(TilesetContentKey tilesetContentKey)
        {
            return $"{_contentFolder.FullName}\\Tilesets\\{tilesetContentKey}_SpriteFrames.bin";
        }
        public void SaveTilesetImage(string name, Texture2D image)
        {
            var path = $"{_contentFolder.FullName}\\Tilesets\\{name}.png";
            if (File.Exists(path))
                File.Delete(path);

            using var stream = File.OpenWrite(path);
            image.SaveAsPng(stream, image.Width, image.Height);
        }

        public void SaveTilesetImage(TilesetContentKey tilesetContentKey, Texture2D image)
        {
            var path = GetImagePath(tilesetContentKey);
            if (File.Exists(path))
                File.Delete(path);

            using var stream = File.OpenWrite(path);
            image.SaveAsPng(stream, image.Width, image.Height);
        }

        public void SaveImage(ImageContentKey key, Texture2D image)
        {
            var path = GetImagePath(key);
            if (File.Exists(path))
                File.Delete(path);

            using var stream = File.OpenWrite(path);
            image.SaveAsPng(stream, image.Width, image.Height);
        }

        public void SaveAnimations(ActorId actorKey, Dictionary<AnimationKey, Animation> animations)
        {
            var path = GetAnimationPath(actorKey);
            if (File.Exists(path))
                File.Delete(path);

            using var stream = File.OpenWrite(path);
            using var writer = new DataWriter(stream);
            writer.Write(animations);
        }

        public Dictionary<AnimationKey,Animation> LoadAnimations(ActorId actorKey)
        {
            var path = GetAnimationPath(actorKey);
            using var stream = File.OpenRead(path);
            using var reader = new DataReader(stream);
            return reader.ReadAnimations();
        }

        public void Save(TileMap tileMap)
        {
            Save(tileMap.GetGrid(), GetPath(tileMap.LevelKey));
        }

        public void Save(SongData song)
        {
            var path = GetPath(song.Key);
            using var stream = File.OpenWrite(path);
            using var writer = new DataWriter(stream);
            writer.Write(song);
        }

        public SongData LoadSong(MusicContentKey songKey)
        {
            var path = GetPath(songKey);
            using var stream = File.OpenRead(path);
            using var reader = new DataReader(stream);
            return reader.ReadSong(songKey);
        }

        public void Save(SceneContentKey sceneContentKey, SceneInfo scene)
        {
            var path = GetPath(sceneContentKey);
            using var stream = File.OpenWrite(path);
            using var writer = new DataWriter(stream);
            writer.Write(scene);
        }

        public SceneInfo Load(SceneContentKey sceneContentKey)
        {
            var path = GetPath(sceneContentKey);
            if (!File.Exists(path))
                return null;

            using var stream = File.OpenRead(path);
            using var reader = new DataReader(stream);
            return reader.ReadScene();
        }

        public void Save(TilesetContentKey tilesetContentKey, SpriteFrame[] spriteFrames)
        {
            Save(spriteFrames, GetSpriteFramePath(tilesetContentKey));
        }

        public void Save(EditorTileSet editorTileSet)
        {
            Save(editorTileSet, GetPath(editorTileSet.Key));
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

        private void Save<T>(IGrid<T> grid, string path)
        {
            using var fileStream = File.OpenWrite(path);
            using var writer = new DataWriter(fileStream);
            writer.Write(grid);
        }

        public TileMap LoadTileMap(LevelContentKey key, GameSystem gameSystem)
        {
            var grid = LoadGrid<Tile>(GetPath(key));
            return new TileMap(key, new TileGrid(gameSystem, grid));
        }

        private MemoryGrid<T> LoadGrid<T>(string path)
        {
            if (!File.Exists(path))
                return new MemoryGrid<T>(1,1);

            using var fileStream = File.OpenRead(path);
            using var reader = new DataReader(fileStream);
            return reader.ReadGrid<T>();
        }

        public EditorTileSet LoadEditorTileset(TilesetContentKey tilesetContentKey)
        {
            var path = GetPath(tilesetContentKey);
            if (!File.Exists(path))
                return null;

            using var fileStream = File.OpenRead(path);
            using var reader = new DataReader(fileStream);
            return reader.ReadEditorTileset(tilesetContentKey);
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


        public Texture2D LoadImageFromDisk(GraphicsDevice device, string imageName)
        {
            var path = $@"{_contentFolder.FullName}\Images\{imageName}.png";
            return Texture2D.FromFile(device, path);
        }
    }
}
