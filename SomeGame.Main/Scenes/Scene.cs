using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Scenes
{
    class Scene
    {
        public SceneInfo SceneInfo { get; }
        public Rectangle Bounds { get; }
      
        public Scene(SceneInfo sceneInfo, GameSystem gameSystem)
        {
            SceneInfo = sceneInfo;
            Bounds = sceneInfo.Bounds;
        }
    }

}
