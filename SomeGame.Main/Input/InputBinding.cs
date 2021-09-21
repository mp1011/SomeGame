using SomeGame.Main.Models;
using System.Collections.Generic;

namespace SomeGame.Main.Input
{


    class InputBinding<TIn>
    {
        private Dictionary<InputButton, TIn> _mapping = new Dictionary<InputButton, TIn>();

        public TIn this[InputButton input] => _mapping.GetValueOrDefault(input);

        public void SetBinding(TIn input, InputButton mappedButton)
        {
            _mapping[mappedButton] = input;
        }
    }
}
