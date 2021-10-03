using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SomeGame.Main.Models
{
    public struct Grid<T>
    {
        private readonly T[,] _grid;

        public Grid(int width, int height, Func<int, int, T> generator=null)
        {
            _grid = new T[width, height];
            var t = this;

            if(generator == null)
                generator = (x,y) => default(T);

            ForEach((x, y, v) => t._grid[x, y] = generator(x, y));
        }

        public Grid(T[,] grid)
        {
            if (grid == null)
                throw new NullReferenceException();
            _grid = grid;
        }

        public int Width => _grid.GetLength(0);
        public int Height => _grid.GetLength(1);

        public int Size => Width * Height;

        public T this[int x, int y]
        {
            get => _grid[x, y];
            set => _grid[x, y] = value;
        }

        public T this[int i]
        {
            get
            {
                int row = i / Width;
                int col = i % Width;
                return _grid[col, row];
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
                    action(x, y, _grid[x, y]);
        }
        public void SetEach(Func<int, int, T> createTile)
        {
            SetEach(0, Width, 0, Height, createTile);
        }

        public void SetEach(int xStart, int xEnd, int yStart, int yEnd, Func<int, int, T> createTile)
        {
            for (int y = yStart; y < yEnd; y++)
                for (int x = xStart; x < xEnd; x++)
                    _grid[x, y] = createTile(x,y);
        }

        public Grid<K> Map<K>(Func<int,int,T,K> transform)
        {
            var newGrid = new K[Width, Height];
            ForEach((x, y, v) => newGrid[x, y] = transform(x, y, v));
            return new Grid<K>(newGrid);
        }

        public Grid<K> Map<K>(Func<T,K> transform)
        {
            return Map((x, y, i) => transform(i));
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
                    if (predicate(_grid[x, y]))
                        yield return _grid[x, y];
                }
        }

        public Grid<Grid<T>> Split(int segmentSize)
        {
            var thisGrid = this;
            return new Grid<Grid<T>>(Width / segmentSize, Height / segmentSize,
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
                    return new Grid<T>(cell);
                });
        }


        public Grid<T> CreateMirror(Flip flip)
        {
            var thisGrid = this;
            return Map((x, y, v) =>
            {
                int srcX = x, srcY = y;
                if ((flip & Flip.H) != 0)
                    srcX = (thisGrid.Width - x)-1;
                if ((flip & Flip.V) != 0)
                    srcY = (thisGrid.Height - y)-1;

                return thisGrid[srcX, srcY];
            });
        }

        public Grid<T> Extract(Point upperLeft, Point bottomRight)
        {
            var thisGrid = this;
            return new Grid<T>(
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
            if(obj is Grid<T> otherGrid)
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
    }
}
