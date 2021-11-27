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

        public Actor FindActor()
        {
            if (_foundActor != null && _foundActor.Enabled)
                return _foundActor;

            foreach(var actor in _actorManager.GetActors())
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
}
