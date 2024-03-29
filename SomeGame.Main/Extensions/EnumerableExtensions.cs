﻿using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeGame.Main.Extensions
{
    public static class EnumerableExtensions
    { 
        public static Grid<T> ToGrid<T>(this IEnumerable<T> list, int width)
        {
            int height = list.Count() / width;
            return list.ToGrid(width, height);
        }

        public static Grid<T> ToGrid<T>(this IEnumerable<T> list, int width, int height)
        {
            var grid = new T[width, height];
            int x = 0, y = 0;

            foreach(var item in list)
            {
                grid[x, y] = item;
                x++;
                if(x == width)
                {
                    x = 0;
                    y++;
                }
            }

            while (y < height)
            {
                while (x < width)
                {
                    grid[x, y] = default;
                    x++;
                }
                y++;
            }

            return new Grid<T>(grid);
        }

        internal static T GetItemAtRotatingIndex<T>(this IEnumerable<T> list, int rotatingIndex)
        {
            return list.ElementAt(rotatingIndex.AsRotatingInt(list.Count()));
        }

        internal static RotatingInt GetIndexAfter<T>(this IEnumerable<T> list, T findItem)
        {
            int index = 0;
            foreach(var item in list)
            {
                if (item.Equals(findItem))
                    return new RotatingInt(index+1, list.Count());
                index++;
            }

            return new RotatingInt(0, list.Count());
        }
    }
}
