using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Editor
{
    class EnumMultiSelect<T> : UIMultiSelect<T>
        where T : struct, Enum
    {
        public EnumMultiSelect(Layer interfaceLayer, Font font, Point location) 
            : base(interfaceLayer, font, Enum.GetValues<T>(), location)
        {
        }
    }

    class UIMultiSelect<T>
    {
        private readonly Font _font;
        private readonly T[] _items;
        private readonly Point _location;

        private UIButton _text;
        private RotatingInt _selectedIndex;

        public UIMultiSelect(Layer interfaceLayer, Font font, IEnumerable<T> items, Point location)
        {
            _font = font;
            _location = location;
            _items = items.ToArray();
            _selectedIndex = new RotatingInt(0, _items.Length);

            _text = RefreshText(interfaceLayer);
        }

        public T SelectedItem => _items[_selectedIndex]; 
        
        public void Refresh(Layer interfaceLayer)
        {
            _text = RefreshText(interfaceLayer);
        }

        private UIButton RefreshText(Layer interfaceLayer)
        {
            var maxStrLen = _items
                                .Select(p => p.ToString().Length)
                                .Max();

            return new UIButton(SelectedItem.ToString().ToUpper().PadRight(maxStrLen), 
                _location, interfaceLayer, _font);
        }

        public void Increment(Layer interfaceLayer)
        {
            _selectedIndex++;
            RefreshText(interfaceLayer);
        }

        public void Decrement(Layer interfaceLayer)
        {
            _selectedIndex--;
            RefreshText(interfaceLayer);
        }

        public bool Update(Layer interfaceLayer, InputModel input)
        {
            if (_text.Update(interfaceLayer, input) == UIButtonState.Pressed)
            {
                _selectedIndex++;
                _text = RefreshText(interfaceLayer);
                return true;
            }

            if (_text.Update(interfaceLayer, input) == UIButtonState.Pressed2)
            {
                _selectedIndex--;
                _text = RefreshText(interfaceLayer);
                return true;
            }

            return false;
        }
    }
}
