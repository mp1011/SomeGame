using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System.Collections.Generic;

namespace SomeGame.Main.Services
{
    class AudioService
    {
        private Dictionary<SoundContentKey, SoundEffectPool> _sounds = new Dictionary<SoundContentKey, SoundEffectPool>();

        public void LoadSound(ResourceLoader resourceLoader, SoundContentKey sound, int maxOccurences)
        {
            var sfx = resourceLoader.LoadSound(sound);
            _sounds[sound] = new SoundEffectPool(sfx, maxOccurences);
        }

        public void Play(SoundContentKey id)
        {
            var sound = _sounds[id];
            sound.ActivateNext();
        }

    }
}
