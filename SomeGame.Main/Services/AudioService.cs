using Microsoft.Xna.Framework.Audio;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Services
{
    class AudioService
    {
        private bool _muted = true;

        private readonly ResourceLoader _resourceLoader;
        private Dictionary<SoundContentKey, SoundEffectPool> _sounds = new Dictionary<SoundContentKey, SoundEffectPool>();
        private SoundEffectInstance[] _songStems = new SoundEffectInstance[] { };

        private SongData _currentSong;
        private MusicChannel _songChannel1 = new MusicChannel();
        private MusicChannel _songChannel2 = new MusicChannel();
        private MusicChannel _waitChannel;
        private RotatingInt _songPosition;
        
        public AudioService(ResourceLoader resourceLoader)
        {
            _resourceLoader = resourceLoader;
        }

        public void LoadSound(SoundContentKey sound, int maxOccurences)
        {
            var sfx = _resourceLoader.LoadSound(sound);
            _sounds[sound] = new SoundEffectPool(sfx, maxOccurences);
        }

        public void LoadSong(SongData song)
        {
            _songStems = song.Stems
                .Select(s =>
                {
                    var instance = _resourceLoader.LoadSongStem(song.Key, $"{s.Group}{s.Number}")
                                                  .CreateInstance();
                    instance.Volume = (float)s.Volume / 255f;
                    return instance;
                })
                .ToArray();

            _currentSong = song;
            _songPosition = new RotatingInt(0, song.SongSequence.Length);
        }

        public void StartMusic()
        {
            if (_muted)
                return;

            _songPosition = _songPosition.Set(0);
            PlaySongSection();
        }

        public void StopMusic()
        {
            _songChannel1.Play(null);
            _songChannel2.Play(null);
            _waitChannel = null;
        }

        public void PauseMusic()
        {
            _songChannel1.Pause();
            _songChannel2.Pause();
        }

        public void ResumeMusic()
        {
            _songChannel1.Resume();
            _songChannel2.Resume();
        }

        public void UpdateMusic()
        {
            if (_muted)
                return;

            if (_songChannel1 != null && _songChannel1.IsPaused)
                return;

            if (_songChannel2 != null && _songChannel2.IsPaused)
                return;

            if (_waitChannel == null)
            {
                if (_songChannel1.IsFinished && _songChannel2.IsPlaying)
                    _waitChannel = _songChannel2;
                else if (_songChannel2.IsFinished && _songChannel1.IsPlaying)
                    _waitChannel = _songChannel1;
            }
            else
            {
                if (_waitChannel.IsFinished)
                {
                    _waitChannel = null;
                    _songPosition++;
                    PlaySongSection();
                }
                else
                {
                    if (_songChannel1.IsFinished && !_waitChannel.IsFinished)
                        _songChannel1.Repeat();
                    if (_songChannel2.IsFinished && !_waitChannel.IsFinished)
                        _songChannel2.Repeat();
                }
            }

          
        }

        private void PlaySongSection()
        {
            if (_muted)
                return;

            var currentSection = _currentSong.SongSequence[_songPosition];
            var stem1 = currentSection.Channel1Index == 0 ? null : _songStems[currentSection.Channel1Index - 1];
            var stem2 = currentSection.Channel2Index == 0 ? null : _songStems[currentSection.Channel2Index - 1];

            _songChannel1.Play(stem1);
            _songChannel2.Play(stem2);
        }

        public void UnloadSounds()
        {
            _sounds = new Dictionary<SoundContentKey, SoundEffectPool>();
        }

        public void Play(SoundContentKey id)
        {
            if (_muted)
                return;

            var sound = _sounds[id];
            sound.ActivateNext();
        }

    }
}
