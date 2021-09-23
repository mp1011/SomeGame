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

        public void Save(TileMap tileMap)
        {
            Save(tileMap.GetGrid(), GetPath(LevelContentKey.TestLevel));
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

    }
}
