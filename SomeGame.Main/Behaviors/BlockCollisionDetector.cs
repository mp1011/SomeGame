using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    abstract class BlockCollisionDetector
    {
        protected CollisionInfo CheckOnLedge(Actor actor, Rectangle tile, bool leftTileSolid, bool rightTileSolid)
        {
            if (actor.MotionVector.X > 0
                && !rightTileSolid
                && tile.Bottom > actor.WorldPosition.Bottom
                && actor.WorldPosition.Right > tile.Center.X)
            {
                return new CollisionInfo(IsFacingLedge: true);
            }
            else if (actor.MotionVector.X < 0
               && !leftTileSolid
               && tile.Bottom > actor.WorldPosition.Bottom
               && actor.WorldPosition.Left < tile.Center.X)
            {
                return new CollisionInfo(IsFacingLedge: true);
            }
            else
                return new CollisionInfo();
        }
        protected CollisionInfo HandleCollision(Actor actor, Rectangle collidingTile, Rectangle frameStartPosition,
           Tile tileAbove, Tile tileBelow, Tile tileLeft, Tile tileRight)
        {
            return HandleCollision(actor, collidingTile, frameStartPosition, tileAbove.IsSolid,
                tileBelow.IsSolid, tileLeft.IsSolid, tileRight.IsSolid);
        }

        protected CollisionInfo HandleCollision(Actor actor, Rectangle collidingTile, Rectangle frameStartPosition,
            bool aboveSolid=false, bool belowSolid = false, bool leftSolid = false, bool rightSolid = false)
        {

            var displacement = actor.WorldPosition.TopLeft - frameStartPosition.TopLeft();
            var xCorrection = new PixelValue(0, -displacement.X);
            var yCorrection = new PixelValue(0, -displacement.Y);

            if (aboveSolid && yCorrection.SubPixel < 0)
                yCorrection = new PixelValue(0, 0);
            if (belowSolid && yCorrection.SubPixel > 0)
                yCorrection = new PixelValue(0, 0);
            if (leftSolid && xCorrection.SubPixel < 0)
                xCorrection = new PixelValue(0, 0);
            if (rightSolid && xCorrection.SubPixel > 0)
                xCorrection = new PixelValue(0, 0);

            var xAdjustedPosition = actor.WorldPosition.Copy();
            var yAdjustedPosition = actor.WorldPosition.Copy();
            var xyAdjustedPosition = actor.WorldPosition.Copy();

            if (xCorrection.SubPixel != 0 || yCorrection.SubPixel != 0)
            {
                while (true)
                {
                    if (!collidingTile.Intersects(xAdjustedPosition))
                    {
                        actor.WorldPosition = xAdjustedPosition;
                        yCorrection = new PixelValue(0, 0);
                        break;
                    }
                    else if (!collidingTile.Intersects(yAdjustedPosition))
                    {
                        actor.WorldPosition = yAdjustedPosition;
                        xCorrection = new PixelValue(0, 0);
                        break;
                    }
                    else if (!collidingTile.Intersects(xyAdjustedPosition))
                    {
                        actor.WorldPosition = xyAdjustedPosition;
                        break;
                    }

                    xyAdjustedPosition.XPixel += xCorrection;
                    xyAdjustedPosition.YPixel += yCorrection;
                    xAdjustedPosition.XPixel += xCorrection;
                    yAdjustedPosition.YPixel += yCorrection;
                }
            }

            return new CollisionInfo(XCorrection: xCorrection, YCorrection: yCorrection);
        }

        protected CollisionInfo CheckTouchingGround(Actor actor, Rectangle collidingTile)
        {
            if (actor.MotionVector.Y >= 0
                && actor.WorldPosition.Bottom == collidingTile.Top
                && actor.WorldPosition.Right >= collidingTile.Left
                && actor.WorldPosition.Left <= collidingTile.Right)
            {
                return new CollisionInfo(IsOnGround: true);
            }
            else
                return new CollisionInfo();
        }
    }
}
