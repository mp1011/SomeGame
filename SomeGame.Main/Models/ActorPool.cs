using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Models
{
    class ActorPool
    {
        private readonly Actor[] _actors;
        private RotatingInt _index;

        public ActorPool(IEnumerable<Actor> actors)
        {
            _actors = actors.ToArray();
            _index = new RotatingInt(0, _actors.Length);
        }

        public Actor ActivateNext()
        {
            int originalIndex = _index;
            while(_actors[_index].Enabled)
            {
                _index += 1;
                if (_index == originalIndex)
                    return null;
            }

            _actors[_index].Create();
            return _actors[_index];
        }
    }
}
