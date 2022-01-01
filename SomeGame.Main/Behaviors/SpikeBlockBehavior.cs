using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class SpikeBlockBehavior : GizmoBlockBehavior
    {
        private readonly Actor[] _spikes;

        public SpikeBlockBehavior(GameSystem gameSystem, Scroller scroller, AudioService audioService, Actor[] spikes) 
            : base(gameSystem, scroller, audioService)
        {
            _spikes = spikes;
            foreach (var spike in _spikes)
                spike.Visible = false;
        }

        protected override void OnBlockCreated(Actor actor)
        {
            _spikes[0].WorldPosition.Y.Set(actor.WorldPosition.Top() - _gameSystem.TileSize*2);
            _spikes[0].WorldPosition.X.Set(actor.WorldPosition.Left());

            _spikes[1].WorldPosition.Y.Set(actor.WorldPosition.Top());
            _spikes[1].WorldPosition.X.Set(actor.WorldPosition.Left() + _gameSystem.TileSize * 2);

            _spikes[2].WorldPosition.Y.Set(actor.WorldPosition.Top() + _gameSystem.TileSize);
            _spikes[2].WorldPosition.X.Set(actor.WorldPosition.Left());
            _spikes[2].Flip = Flip.FlipV;

            _spikes[3].WorldPosition.Y.Set(actor.WorldPosition.Top());
            _spikes[3].WorldPosition.X.Set(actor.WorldPosition.Left() - _gameSystem.TileSize);
            _spikes[3].Flip = Flip.FlipH;


            _timer.Set(0);
            SetTiles(actor, show: true);

            DrawSpikeTiles(_spikes[0], false);
            DrawSpikeTiles(_spikes[1], false);
            DrawSpikeTiles(_spikes[2], false);
            DrawSpikeTiles(_spikes[3], false);
        }

        protected override void DoUpdate()
        {
            _timer.Inc();

            UpdateSpike(_spikes[0]);
            UpdateSpike(_spikes[1]);
            UpdateSpike(_spikes[2]);
            UpdateSpike(_spikes[3]);
            SetTiles(Actor, show: true);
        }

        private void UpdateSpike(Actor spike)
        {
            if (_timer == 0)
                DrawSpikeTiles(spike, false);

            if (_timer == 64)
                spike.CurrentAnimation = AnimationKey.Appearing;

            if (_timer >= 64 && _timer < 192)
            {
                if (_timer > 64 && spike.IsAnimationFinished)
                    spike.CurrentAnimation = AnimationKey.Idle;

                DrawSpikeTiles(spike, true);
            }

            if (_timer == 192)
            {
                spike.CurrentAnimation = AnimationKey.Disappearing;
                DrawSpikeTiles(spike, true);
            }

            if (_timer > 192 && spike.CurrentAnimation == AnimationKey.Disappearing)
            {
                if (spike.IsAnimationFinished)
                {
                    DrawSpikeTiles(spike, false);
                    spike.CurrentAnimation = AnimationKey.Idle;
                }
                else
                    DrawSpikeTiles(spike, true);
            }
        }

        private void DrawSpikeTiles(Actor spike, bool show)
        {
            SetTiles(spike, show ? TileFlags.Harmful : TileFlags.None);
        }


    }
}
