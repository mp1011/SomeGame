using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    abstract class ActorFinder
    {
        private readonly ActorManager _actorManager;
        private Actor _foundActor;

        public ActorFinder(ActorManager actorManager)
        {
            _actorManager = actorManager;
        }

        public Actor FindActor(bool enabledOnly=true)
        {
            if (_foundActor != null && (_foundActor.Enabled || !enabledOnly))
                return _foundActor;

            foreach(var actor in _actorManager.GetActors(enabledOnly: enabledOnly))
            {
                if(IsMatch(actor))
                {
                    _foundActor = actor;
                    return _foundActor;
                }
            }

            return null;
        }

        protected abstract bool IsMatch(Actor actor);
    }


    class PlayerFinder : ActorFinder
    {
        public PlayerFinder(ActorManager actorManager) : base(actorManager)
        {
        }

        protected override bool IsMatch(Actor actor)
        {
            return actor.ActorType == (ActorType.Player | ActorType.Character);
        }
    }

    class LockFinder : ActorFinder
    {
        private RamGameRectangle _blockPosition;

        public LockFinder(ActorManager actorManager, RamGameRectangle blockPosition) : base(actorManager)
        {
            _blockPosition = blockPosition;
        }

        protected override bool IsMatch(Actor actor)
        {
            if (actor.ActorId != ActorId.Lock)
                return false;

            //todo, handle multiple key blocks
            return true;
        }
    }
}
