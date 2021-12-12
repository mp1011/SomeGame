using SomeGame.Main.Models;
using System;
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
        }

        public void MemoryChanged(int address, byte value)
        {
            var index = address * 3;

            var row = index / Console.BufferWidth;
            var col = index % Console.BufferWidth;

            Console.SetCursorPosition(col, row);
            Console.Write(value.ToString("X2"));
            Console.Write(" ");
        }
    }
}
