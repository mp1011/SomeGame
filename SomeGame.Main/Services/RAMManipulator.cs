using Microsoft.Xna.Framework.Input;
using SomeGame.Main.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace SomeGame.Main.Services
{
    class RAMManipulator : IRamViewer
    {
        private RAM _ram;
        private bool _isKeyDown;

        public int Cursor { get; set; }
        public IReadOnlyDictionary<int, string> Labels { get; set; }

        public void IncByte() => Debug.WriteLine($"Address {Cursor}: {++_ram[Cursor]}");

        public void DecByte() => Debug.WriteLine($"Address {Cursor}: {--_ram[Cursor]}");

        public void MemoryChanged(int address, byte value)
        {
        }

        public void Initialize(RAM ram)
        {
            _ram = ram;
        }


        public void BeforeFrame()
        {
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Subtract))
            {
                if(!_isKeyDown)
                {
                    _isKeyDown = true;
                    DecByte();
                }
            }
            else if (keyState.IsKeyDown(Keys.Add))
            {
                if (!_isKeyDown)
                {
                    _isKeyDown = true;
                    IncByte();
                }
            }
            else
                _isKeyDown = false;
        }
    }
}
