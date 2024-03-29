﻿using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace RamConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new RamWindow();
            SomeGame.Main.Program.StartGame(window);   
        }
    }

    class RamWindow : IRamViewer
    {
        public IReadOnlyDictionary<int,string> Labels { get; set; }

        public RamWindow()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        [SupportedOSPlatform("windows")]
        public void MemoryChanged(int address, byte value)
        {
            var labelsBefore = Labels
                .Where(p => p.Key < address)
                .ToArray();

            var thisLabel = Labels.Where(p => p.Key == address).ToArray();

            int labelOffset = labelsBefore
                .Select(p => p.Value.Length)
                .Sum();

            var index = labelOffset + (address * 2);
         
            int width = Math.Min(32 * 2, Console.BufferWidth);

            var row = index / width;
            var col = index % width;

            Console.SetCursorPosition(col, row);

            if (thisLabel.Length == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(thisLabel[0].Value);
            }

            if (address % 2 == 0)
                Console.ForegroundColor = ConsoleColor.Blue;
            else
                Console.ForegroundColor = ConsoleColor.DarkGreen;

            
           Console.Write(value.ToString("X2"));
        }

        public void Initialize(RAM ram)
        {
        }

        public void BeforeFrame()
        {
        }
    }
}
