using Microsoft.Xna.Framework.Audio;
using SomeGame.Main.Content;

namespace SomeGame.Main.Models
{
    record SongStem(char Group, byte Number, byte Volume);

    record SongData(MusicContentKey Key, SongStem[] Stems, SongSection[] SongSequence);

    record SongSection(byte Channel1Index, byte Channel2Index);

    class MusicChannel
    {
        private SoundEffectInstance _currentStem;

        public int CurrentLoopCount { get; private set; }

        public bool IsPlaying => _currentStem != null && _currentStem.State == SoundState.Playing;
        public bool IsFinished => !IsPlaying;

        public void Play(SoundEffectInstance stem)
        {
            if (_currentStem != null)
                _currentStem.Stop();

            _currentStem = stem;
            if(_currentStem != null)
                _currentStem.Play();
        }

        public void Repeat() => _currentStem?.Play();
    }
}
