using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Extensions
{
    public static class GridExtensions
    {
        public static MemoryGrid<T> Combine<T>(this IEnumerable<MemoryGrid<T>> sections, int horizontalSections)
        {
            int sectionWidth = sections.First().Width;
            int sectionHeight = sections.First().Height;

            int width = sectionWidth * horizontalSections;
            int rows = (int)(Math.Ceiling((sections.Count() * (double)sectionWidth) / width));
            int height = sectionHeight * rows;

            var newGrid = new T[width, height];
            int startX = 0, startY = 0;

            foreach(var section in sections)
            {
                if (section.Width != sectionWidth || section.Height != sectionHeight)
                    throw new Exception("All sections must be the same size to be combined");

                section.ForEach((x, y, v) =>
                    newGrid[startX + x, startY + y] = v);

                startX += sectionWidth;
                if(startX >= width)
                {
                    startX = 0;
                    startY += sectionHeight;
                }
            }

            return new MemoryGrid<T>(newGrid);
        }

        private static Exception Exception()
        {
            throw new NotImplementedException();
        }
    }
}
