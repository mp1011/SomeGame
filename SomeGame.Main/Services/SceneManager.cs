using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Scenes;

namespace SomeGame.Main.Services
{
    class SceneManager
    {
        private TransitionInfo _transitionInfo = new TransitionInfo();
        public Scene CurrentScene { get; private set; }

 
        public void RestartCurrentScene() => QueueNextScene(CurrentScene.Key.GetTitleCardFromScene());

        public void QueueNextScene(SceneContentKey sceneContentKey)
        {
            _transitionInfo = new TransitionInfo(sceneContentKey, Rectangle.Empty, Point.Zero, Direction.None);
        }
       
        public SceneLoadResult Update(SceneLoader sceneLoader)
        {
            if (_transitionInfo.NextScene != SceneContentKey.None)
            {
                CurrentScene = sceneLoader.LoadScene(_transitionInfo);
                sceneLoader.InitializeScene(CurrentScene.SceneInfo, _transitionInfo);
                _transitionInfo = new TransitionInfo();

                return new SceneLoadResult(NewScene: true, 
                    GameInterface: sceneLoader.CreateInterfaceLayer(CurrentScene),
                    Controller: sceneLoader.CreateSceneController(CurrentScene.SceneInfo));
            }

            return new SceneLoadResult();
        }

        public bool CheckLevelTransitions(Actor actor)
        {
            var transitions = CurrentScene.SceneInfo.Transitions;

            return CheckEdgeTransition(actor, transitions.Left, CurrentScene.LeftEdge, Direction.Left)
                || CheckEdgeTransition(actor, transitions.Right, CurrentScene.RightEdge, Direction.Right)
                || CheckEdgeTransition(actor, transitions.Up, CurrentScene.TopEdge, Direction.Up)
                || CheckEdgeTransition(actor, transitions.Door1, CurrentScene.BottomEdge, Direction.Down);
        }

        private bool CheckEdgeTransition(Actor actor, SceneContentKey nextScene, Rectangle exitSide, Direction direction)
        {
            if (nextScene == SceneContentKey.None)
                return false;

            if(actor.WorldPosition.IntersectsWith(exitSide))
            {
                _transitionInfo = new TransitionInfo(nextScene, exitSide, actor.WorldPosition.Center, direction);
                return true;
            }

            return false;
        }
    }
}
