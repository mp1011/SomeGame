using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class BgCollisionDetector : ICollisionDetector
    {
        private readonly GameSystem _gameSystem;
        private readonly CollectiblesService _collectiblesService;

        public BgCollisionDetector(GameSystem gameSystem, CollectiblesService collectiblesService=null)
        {
            _gameSystem = gameSystem;
            _collectiblesService = collectiblesService;
        }

        public CollisionInfo DetectCollisions(Actor actor, GameRectangleWithSubpixels frameStartPosition)
        {
            var collisionInfo = new CollisionInfo();
            var fg = _gameSystem.GetLayer(LayerIndex.FG);
            var topLeftTile = fg.TilePointFromWorldPixelPoint(actor.WorldPosition.TopLeft).Offset(-2, -2);
            var bottomRightTile = fg.TilePointFromWorldPixelPoint(actor.WorldPosition.BottomRight).Offset(2, 2);

            fg.TileMap.ForEach(topLeftTile, bottomRightTile, (x,y,t) =>
            {
                if(t.IsSolid)
                {
                    var tileAbove = fg.TileMap.GetTile(x, y - 1);
                    var tileBelow = fg.TileMap.GetTile(x, y + 1);
                    var tileLeft = fg.TileMap.GetTile(x - 1, y);
                    var tileRight = fg.TileMap.GetTile(x + 1, y);

                    var tileBounds = fg.GetTileWorldPosition(x, y);
                    if (tileBounds.Intersects(actor.WorldPosition))                    
                        collisionInfo += HandleCollision(actor, tileBounds, frameStartPosition, tileAbove, tileBelow, tileLeft, tileRight);

                    if(!tileAbove.IsSolid)
                        collisionInfo += CheckTouchingGround(actor, tileBounds);

                    if (collisionInfo.IsOnGround && (!tileLeft.IsSolid || !tileRight.IsSolid))
                        collisionInfo += CheckOnLedge(actor, tileBounds, tileLeft.IsSolid, tileRight.IsSolid);
                }
                else if(_collectiblesService != null && t.IsCollectible)
                {
                    var tileBounds = fg.GetTileWorldPosition(x, y);
                    if (tileBounds.Intersects(actor.WorldPosition))
                        collisionInfo += _collectiblesService.HandleCollectibleCollision(x, y);
                }
            });

            return collisionInfo;
        }

        private CollisionInfo CheckOnLedge(Actor actor, Rectangle tile, bool leftTileSolid, bool rightTileSolid)
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

        private CollisionInfo HandleCollision(Actor actor, Rectangle collidingTile, Rectangle frameStartPosition,
            Tile tileAbove, Tile tileBelow, Tile tileLeft, Tile tileRight)
        {

            var displacement = actor.WorldPosition.TopLeft - frameStartPosition.TopLeft();
            var xCorrection = new PixelValue(0, -displacement.X);
            var yCorrection = new PixelValue(0, -displacement.Y);

            if (tileAbove.IsSolid && yCorrection.SubPixel < 0)
                yCorrection = new PixelValue(0, 0);
            if (tileBelow.IsSolid && yCorrection.SubPixel > 0)
                yCorrection = new PixelValue(0, 0);
            if (tileLeft.IsSolid && xCorrection.SubPixel < 0)
                xCorrection = new PixelValue(0, 0);
            if (tileRight.IsSolid && xCorrection.SubPixel > 0)
                xCorrection = new PixelValue(0, 0);

            var xAdjustedPosition = actor.WorldPosition.Copy();
            var yAdjustedPosition = actor.WorldPosition.Copy();
            var xyAdjustedPosition = actor.WorldPosition.Copy();

            if (xCorrection.SubPixel != 0 || yCorrection.SubPixel != 0)
            {
                while(true)
                {
                    if(!collidingTile.Intersects(xAdjustedPosition))
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

        private CollisionInfo CheckTouchingGround(Actor actor, Rectangle collidingTile)
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
