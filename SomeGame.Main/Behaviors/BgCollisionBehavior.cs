using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class BgCollisionBehavior : Behavior
    {
        private readonly GameSystem _gameSystem;

        public BgCollisionBehavior(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition)
        {
            var fg = _gameSystem.GetLayer(LayerIndex.FG);

            var posX = actor.WorldPosition.X;
            var posY = actor.WorldPosition.Y;

            var topLeftTile = fg.TilePointFromWorldPixelPoint(actor.WorldPosition.TopLeft).Offset(-1, -1);
            var bottomRightTile = fg.TilePointFromWorldPixelPoint(actor.WorldPosition.BottomRight).Offset(1, 1);

            actor.WorldPosition.X = frameStartPosition.X;
            fg.TileMap.ForEach(topLeftTile, bottomRightTile, (x,y,t) =>
            {
                if(t.IsSolid)
                {
                    var tileBounds = fg.GetTileWorldPosition(x, y);
                    if (tileBounds.Intersects(actor.WorldPosition))
                        HandleVerticalCollision(actor, tileBounds, frameStartPosition);
                }
            });

            actor.WorldPosition.X = posX;
            fg.TileMap.ForEach(topLeftTile, bottomRightTile, (x, y, t) =>
            {
                if (t.IsSolid)
                {
                    var tileBounds = fg.GetTileWorldPosition(x, y);
                    if (tileBounds.Intersects(actor.WorldPosition))
                        HandleHorizontalCollision(actor, tileBounds, frameStartPosition);
                }

            });
        }

        private void HandleVerticalCollision(Actor actor, Rectangle collidingTile, Rectangle frameStartPosition)
        {
            int correctY = 0;
            var displacement = actor.WorldPosition.TopLeft - frameStartPosition.TopLeft();

            if (displacement.Y > 0)
                correctY = -1;
            else if (displacement.Y < 0)
                correctY = 1;

            if (correctY == 0)
                return;

            while (collidingTile.Intersects(actor.WorldPosition))            
                actor.WorldPosition.Y += correctY;
        }

        private void HandleHorizontalCollision(Actor actor, Rectangle collidingTile, Rectangle frameStartPosition)
        {
            int correctX = 0;
            var displacement = actor.WorldPosition.TopLeft - frameStartPosition.TopLeft();

            if (displacement.X > 0)
                correctX = -1;
            else if (displacement.X < 0)
                correctX = 1;

            if (correctX == 0)
                return;

            while (collidingTile.Intersects(actor.WorldPosition))
                actor.WorldPosition.X += correctX;
        }
    }
}
