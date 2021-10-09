using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class BgCollisionDetector : ICollisionDetector
    {
        private readonly GameSystem _gameSystem;

        public BgCollisionDetector(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
        }

        public CollisionInfo DetectCollisions(Actor actor, GameRectangleWithSubpixels frameStartPosition)
        {
            var fg = _gameSystem.GetLayer(LayerIndex.FG);

            var posX = actor.WorldPosition.XPixel;
            var posY = actor.WorldPosition.YPixel;

            var topLeftTile = fg.TilePointFromWorldPixelPoint(actor.WorldPosition.TopLeft).Offset(-2, -2);
            var bottomRightTile = fg.TilePointFromWorldPixelPoint(actor.WorldPosition.BottomRight).Offset(2, 2);

            var collisionInfo = new CollisionInfo();
            actor.WorldPosition.XPixel = frameStartPosition.XPixel;
            fg.TileMap.ForEach(topLeftTile, bottomRightTile, (x,y,t) =>
            {
                if(t.IsSolid)
                {
                    var tileBounds = fg.GetTileWorldPosition(x, y);
                    if (tileBounds.IntersectsOrTouches(actor.WorldPosition))
                        collisionInfo += HandleVerticalCollision(actor, tileBounds, frameStartPosition);
                }
            });

            actor.WorldPosition.XPixel = posX;
            fg.TileMap.ForEach(topLeftTile, bottomRightTile, (x, y, t) =>
            {
                if (t.IsSolid)
                {
                    var tileBounds = fg.GetTileWorldPosition(x, y);
                    if (tileBounds.Intersects(actor.WorldPosition))
                        collisionInfo += HandleHorizontalCollision(actor, tileBounds, frameStartPosition);
                }
            });

            return collisionInfo;
        }

        private CollisionInfo HandleVerticalCollision(Actor actor, Rectangle collidingTile, Rectangle frameStartPosition)
        {
            int correctY = 0;
            var displacement = actor.WorldPosition.TopLeft - frameStartPosition.TopLeft();

            if (displacement.Y > 0)
                correctY = -1;
            else if (displacement.Y < 0)
                correctY = 1;

            if (correctY == 0)
                return new CollisionInfo(IsOnGround: actor.WorldPosition.Bottom >= collidingTile.Top);

            while (collidingTile.Intersects(actor.WorldPosition))            
                actor.WorldPosition.Y += correctY;

            if (correctY < 0)
                actor.MotionVector = new PixelPoint(actor.MotionVector.X, 0);

            return new CollisionInfo(IsOnGround: actor.WorldPosition.Bottom >= collidingTile.Top);
        }

        private CollisionInfo HandleHorizontalCollision(Actor actor, Rectangle collidingTile, Rectangle frameStartPosition)
        {
            int correctX = 0;
            var displacement = actor.WorldPosition.TopLeft - frameStartPosition.TopLeft();

            if (displacement.X > 0)
                correctX = -1;
            else if (displacement.X < 0)
                correctX = 1;

            if (correctX == 0)
                return new CollisionInfo();

            while (collidingTile.Intersects(actor.WorldPosition))
                actor.WorldPosition.X += correctX;

            return new CollisionInfo();
        }
    }
}
