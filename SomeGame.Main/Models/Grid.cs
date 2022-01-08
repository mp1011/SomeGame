using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SomeGame.Main.Models
{
    public interface IGrid<T>
    {
        int Width { get; }
        int Height { get; }
        public int Size { get; }
        T this[int i] { get; }
        T this[int x, int y] { get;set; }
        T[] ToArray();
        IEnumerable<T> Where(Func<T, bool> predicate);
        bool All(Func<int, int, T, bool> predicate);
        void ForEach(Action<int, int, T> action);
        void ForEach(int xStart, int xEnd, int yStart, int yEnd, Action<int, int, T> action);

        MemoryGrid<K> Map<K>(Func<int, int, T, K> transform);

        MemoryGrid<K> Map<K>(Func<T, K> transform);
    }

    public abstract class BaseGrid<T> : IGrid<T>
    {
        public abstract int Width { get; }
        public abstract int Height { get; }

        public int Size => Width * Height;

        public abstract T this[int x, int y] { get; set; }

        public T this[int i]
        {
            get
            {
                int row = i / Width;
                int col = i % Width;
                return this[col, row];
            }
        }

        public bool All(Func<int,int,T,bool> predicate)
        {
            var result = true;
            ForEach((x, y, t) =>
            {
                if (!predicate(x, y, t))
                    result = false;
            });
            return result;
        }
        public void ForEach(Action<int, int, T> action)
        {
            ForEach(0, Width, 0, Height, action);
        }

        public void ForEach(int xStart, int xEnd, int yStart, int yEnd, Action<int,int,T> action)
        {
            xStart = xStart.Clamp(Width - 1);
            yStart = yStart.Clamp(Height - 1);
            xEnd = xEnd.Clamp(Width);
            yEnd = yEnd.Clamp(Height);

            for (int y = yStart; y < yEnd; y++)
                for (int x = xStart; x < xEnd; x++)                
                    action(x, y, this[x, y]);
        }
        public void SetEach(Func<int, int, T> createTile)
        {
            SetEach(0, Width, 0, Height, createTile);
        }

        public void SetEach(int xStart, int xEnd, int yStart, int yEnd, Func<int, int, T> createTile)
        {
            for (int y = yStart; y < yEnd; y++)
                for (int x = xStart; x < xEnd; x++)
                    this[x, y] = createTile(x,y);
        }

        public T[] ToArray()
        {
            var array = new T[Width * Height];
            int i = 0;
            ForEach((x, y, v) => array[i++] = v);
            return array;
        }

        public IEnumerable<T> Where(Func<T,bool> predicate)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    if (predicate(this[x, y]))
                        yield return this[x, y];
                }
        }

        public MemoryGrid<T> CreateMirror(Flip flip)
        {
            var thisGrid = this;
            return Map((x, y, v) =>
            {
                int srcX = x, srcY = y;
                if ((flip & Flip.H) != 0)
                    srcX = (thisGrid.Width - x) - 1;
                if ((flip & Flip.V) != 0)
                    srcY = (thisGrid.Height - y) - 1;

                return thisGrid[srcX, srcY];
            });
        }

        public MemoryGrid<T> Extract(Point upperLeft, Point bottomRight)
        {
            var thisGrid = this;
            return new MemoryGrid<T>(
                            width: (bottomRight.X - upperLeft.X) + 1,
                            height: (bottomRight.Y - upperLeft.Y) + 1,
                            (x, y) => thisGrid[x + upperLeft.X, y + upperLeft.Y]);
        }


        public T GetNeighborOrDefault(int x, int y, Direction direction)
        {
            var neighbor = new Point(x, y).Offset(direction.ToPoint());
            return GetElementOrDefault(neighbor);
        }

        public T GetElementOrDefault(Point p)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= Width || p.Y >= Height)
                return default(T);
            else
                return this[p.X, p.Y];
        }

        public override int GetHashCode()
        {
            int hash = 0;
            ForEach((x, y, v) =>
            {
                hash = (hash * v.GetHashCode()) % 77777777;
            });

            return hash;
        }

        public override bool Equals(object obj)
        {
            if(obj is IGrid<T> otherGrid)
            {
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        if (!this[x, y].Equals(otherGrid[x, y]))
                            return false;

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            int lastY = 0;
            ForEach((x, y, v) =>
            {
                if (y > lastY)
                {
                    lastY = y;
                    sb.AppendLine();
                }
                sb.Append(v.ToString());
                
            });

            return sb.ToString();
        }

        public MemoryGrid<K> Map<K>(Func<int, int, T, K> transform)
        {
            var newGrid = new K[Width, Height];
            ForEach((x, y, v) => newGrid[x, y] = transform(x, y, v));
            return new MemoryGrid<K>(newGrid);
        }

        public MemoryGrid<K> Map<K>(Func<T, K> transform)
        {
            return Map((x, y, i) => transform(i));
        }
    }

    public class MemoryGrid<T> :BaseGrid<T>
    {
        private readonly T[,] _grid;

        public override int Width => _grid.GetLength(0);
        public override int Height => _grid.GetLength(1);

        public MemoryGrid(int width, int height, Func<int, int, T> generator = null)
        {
            _grid = new T[width, height];
            var t = this;

            if (width == 0 || height == 0)
                return;

            if (generator == null)
                generator = (x, y) => default(T);

            ForEach((x, y, v) => t._grid[x, y] = generator(x, y));
        }

        public MemoryGrid(T[,] grid)
        {
            if (grid == null)
                throw new NullReferenceException();
            _grid = grid;
        }

        public override T this[int x, int y]
        {
            get
            {
                if (x < 0)
                    x = 0;
                if (x >= Width)
                    x = Width - 1;
                if (y < 0)
                    y = 0;
                if (y >= Height)
                    y = Height - 1;

                return _grid[x, y];
            }
            set => _grid[x.AsRotatingInt(Width), y.AsRotatingInt(Height)] = value;
        }

        public MemoryGrid<MemoryGrid<T>> Split(int segmentSize)
        {
            var thisGrid = this;
            return new MemoryGrid<MemoryGrid<T>>(Width / segmentSize, Height / segmentSize,
                (newX, newY) =>
                {
                    var cell = new T[segmentSize, segmentSize];
                    thisGrid.ForEach(xStart: newX * segmentSize,
                            xEnd: (newX + 1) * segmentSize,
                            yStart: newY * segmentSize,
                            yEnd: (newY + 1) * segmentSize,
                            action: (x, y, t) =>
                            {
                                cell[x - (newX * segmentSize), y - (newY * segmentSize)] = thisGrid[x, y];
                            });
                    return new MemoryGrid<T>(cell);
                });
        }

    }

    public abstract class RamGrid<T> : BaseGrid<T>
    {
        private int _address;
        private RAM _ram;

        public override int Width { get; }

        public override int Height { get; }

        public override T this[int x, int y] 
        {
            get
            {
                int index = (y * Width) + x;
                return ReadValue(_ram, _address, index);
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        protected abstract T ReadValue(RAM ram, int gridAddress, int index);

        internal RamGrid(GameSystem gameSystem, int width, int height, Func<T> declareMemory)
        {
            Width = width;
            Height = height;
            _ram = gameSystem.RAM;
            _address = gameSystem.RAM.WritePointer;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    declareMemory();
        }
    }


    class NibbleGrid : RamGrid<RamNibble>
    {
        internal NibbleGrid(GameSystem gameSystem, int width, int height, Func<RamNibble> declareMemory) : base(gameSystem, width, height, declareMemory)
        {
        }

        protected override RamNibble ReadValue(RAM ram, int gridAddress, int index)
        {
            var realAddress = gridAddress + (index / 2);
            if ((index % 2) == 0)
                return ram.ReadLowNibble(realAddress);
            else
                return ram.ReadHighNibble(realAddress);
        }
    }

    class ByteGrid : RamGrid<RamByte>
    {
        public ByteGrid(GameSystem gameSystem, int width, int height, Func<RamByte> declareMemory) : base(gameSystem, width, height, declareMemory)
        {
        }

        protected override RamByte ReadValue(RAM ram, int gridAddress, int index)
        {
            int address = gridAddress + index;
            return ram.ReadByte(address);
        }
    }

    class TileGrid : RamGrid<RamTile>
    {
        public TileGrid(GameSystem gameSystem, int width, int height, Func<RamTile> declareMemory) : base(gameSystem, width, height, declareMemory)
        {
        }

        public TileGrid(GameSystem gameSystem, IGrid<Tile> source) : base(gameSystem, source.Width,source.Height, 
            ()=> gameSystem.RAM.DeclareTile(255, TileFlags.None))
        {
            ForEach((x, y, t) => t.Set(source[x, y]));
        }

        protected override RamTile ReadValue(RAM ram, int gridAddress, int index)
        {
            int address = gridAddress + (index * 2);
            return ram.ReadTile(address);
        }
    }
}
