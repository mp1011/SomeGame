using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Models
{
    abstract class Pool<T>
        where T:class
    {
        private readonly T[] _items;
        private RotatingInt _index;

        protected abstract bool IsActive(T item);

        protected abstract void Activate(T item);

        public Pool(IEnumerable<T> items)
        {
            _items = items.ToArray();
            _index = new RotatingInt(_items.Length-1, _items.Length);

        }

        public T ActivateNext()
        {
            _index += 1;

            int originalIndex = _index;
            while(IsActive(_items[_index]))
            {
                _index += 1;
                if (_index == originalIndex)
                    return null;
            }

            Activate(_items[_index]);
            return _items[_index];
        }
    }


    class ActorPool : Pool<Actor>
    {
        public ActorPool(IEnumerable<Actor> items) : base(items)
        {
        }

        protected override void Activate(Actor item) => item.Create();
        
        protected override bool IsActive(Actor item) => item.Enabled;
    }

    class SoundEffectPool : Pool<SoundEffectInstance>
    {
        public SoundEffectPool(SoundEffect soundEffect, int count) 
            :base(Enumerable.Range(0,count).Select(x=> soundEffect.CreateInstance()))
        {
        }

        protected override void Activate(SoundEffectInstance item) => item.Play();

        protected override bool IsActive(SoundEffectInstance item) => item.State == SoundState.Playing;
    }
}
