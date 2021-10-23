using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System.Collections.Generic;

namespace SomeGame.Main.Services
{
    class AudioService
    {
        private readonly ResourceLoader _resourceLoader;
        private Dictionary<SoundContentKey, SoundEffectPool> _sounds = new Dictionary<SoundContentKey, SoundEffectPool>();

        public AudioService(ResourceLoader resourceLoader)
        {
            _resourceLoader = resourceLoader;
        }

        public void LoadSound(SoundContentKey sound, int maxOccurences)
        {
            var sfx = _resourceLoader.LoadSound(sound);
            _sounds[sound] = new SoundEffectPool(sfx, maxOccurences);
        }

        public void UnloadSounds()
        {
            _sounds = new Dictionary<SoundContentKey, SoundEffectPool>();
        }

        public void Play(SoundContentKey id)
        {
            var sound = _sounds[id];
            sound.ActivateNext();
        }

    }
}
