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
