using SomeGame.Main.Models;
using System;
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
        public RamWindow()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        [SupportedOSPlatform("windows")]
        public void MemoryChanged(int address, byte value)
        {
            var index = address * 2;

            int width = Math.Min(32 * 2, Console.BufferWidth);

            var row = index / width;
            var col = index % width;

            //Task.Run(() =>
            //{
            //    int f = 500 + (int)((500) * ((double)value / 255.0));
            //    Console.Beep(f, 100);
            //});

            Console.SetCursorPosition(col, row);

            if (address % 2 == 0)
                Console.ForegroundColor = ConsoleColor.Blue;
            else
                Console.ForegroundColor = ConsoleColor.DarkGreen;

            
            Console.Write(value.ToString("X2"));
        }
    }
}
