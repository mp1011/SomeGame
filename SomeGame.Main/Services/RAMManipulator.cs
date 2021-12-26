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

        private void DecBytes()
        {
            for (Cursor = 2; Cursor < 20; Cursor++)
                DecByte();
        }

        private void IncBytes()
        {
            for (Cursor = 2; Cursor < 20; Cursor++)
                IncByte();
        }

        public void BeforeFrame()
        {
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Subtract))
            {
                if(!_isKeyDown)
                {
                    _isKeyDown = true;
                    DecBytes();
                }
            }
            else if (keyState.IsKeyDown(Keys.Add))
            {
                if (!_isKeyDown)
                {
                    _isKeyDown = true;
                    IncBytes();
                }
            }
            else
                _isKeyDown = false;
        }
    }
}
