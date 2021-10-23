using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Scenes
{
    class Scene
    {
        public SceneContentKey Key { get; }
        public SceneInfo SceneInfo { get; }
        public Rectangle Bounds { get; }
      
        public Rectangle LeftEdge { get; }
        public Rectangle RightEdge { get; }
        public Rectangle TopEdge { get; }
        public Rectangle BottomEdge { get; }

        public Scene(SceneContentKey key, SceneInfo sceneInfo, GameSystem gameSystem)
        {
            Key = key;
            SceneInfo = sceneInfo;
            Bounds = sceneInfo.Bounds;

            LeftEdge = Bounds.LeftSection(10);
            RightEdge = Bounds.RightSection(10);
            TopEdge = Bounds.TopSection(10);
            BottomEdge = Bounds.BottomSection(10);
        }

        public Rectangle GetEdge(Direction d)
        {
            switch(d)
            {
                case Direction.Left: return LeftEdge;
                case Direction.Right: return RightEdge;
                case Direction.Up: return TopEdge;
                case Direction.Down: return BottomEdge;
                default: return Rectangle.Empty;
            }
        }
    }

}
