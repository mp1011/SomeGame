using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class BgCollisionDetector : ICollisionDetector
    {
        private readonly GameSystem _gameSystem;
        private readonly ActorManager _actorManager;
        private readonly TileMap _tileMap;

        public BgCollisionDetector(GameSystem gameSystem, TileMap tileMap, ActorManager actorManager)
        {
            _tileMap = tileMap;
            _actorManager = actorManager;
            _gameSystem = gameSystem;
        }

        public CollisionInfo DetectCollisions(Actor actor)
        {
            var collisionInfo = new CollisionInfo();

            var topLeftTile = actor.WorldPosition.TopLeft()
                                        .Divide(_gameSystem.TileSize)
                                        .Offset(-2, -2);

            var bottomRightTile = actor.WorldPosition.BottomRight()
                                        .Divide(_gameSystem.TileSize)
                                        .Offset(2, 2);

            var correction = new CollisionCorrection();
            correction.X = new PixelValue(0, 0);
            correction.Y = new PixelValue(0, 0);

            _tileMap.ForEach(topLeftTile, bottomRightTile, (x,y,t) =>
            {
                if (t.IsSolid)
                {
                    var tileBounds = new GameRectangleWithSubpixels(x * _gameSystem.TileSize, y * _gameSystem.TileSize,
                    _gameSystem.TileSize, _gameSystem.TileSize);

                    var tileLeft = _tileMap.GetTile(x - 1, y);
                    var tileRight = _tileMap.GetTile(x + 1, y);

                    bool checkLeftCollision = actor.MotionVector.X > 0 && !tileLeft.IsSolid;
                    bool checkRightCollision = actor.MotionVector.X < 0 && !tileRight.IsSolid;

                    CheckCollisionCorrectionX(actor, correction, tileBounds, checkLeftCollision, checkRightCollision);
                }
            });

            foreach (var block in _actorManager.GetActors(ActorType.Gizmo))
            {
                if (!block.Visible)
                    continue;

                bool checkLeftCollision = actor.MotionVector.X > 0;
                bool checkRightCollision = actor.MotionVector.X < 0;
                CheckCollisionCorrectionX(actor, correction, block.WorldPosition, checkLeftCollision, checkRightCollision);
            }

            if (correction.X != 0)
            {
                actor.WorldPosition.X.Add(correction.X);
                collisionInfo += new CollisionInfo(XCorrection: correction.X);
            }

            _tileMap.ForEach(topLeftTile, bottomRightTile, (x, y, t) =>
            {
                if (t.IsSolid)
                {
                    var tileAbove = _tileMap.GetTile(x, y - 1);
                    var tileBelow = _tileMap.GetTile(x, y + 1);
                    var tileLeft = _tileMap.GetTile(x - 1, y);
                    var tileRight = _tileMap.GetTile(x + 1, y);

                    var tileBounds = new GameRectangleWithSubpixels(x * _gameSystem.TileSize, y * _gameSystem.TileSize,
                        _gameSystem.TileSize, _gameSystem.TileSize);

                    bool checkAbove = actor.MotionVector.Y >= 0 && !tileAbove.IsSolid;
                    bool checkBelow = actor.MotionVector.Y < 0 && !tileBelow.IsSolid;

                    collisionInfo = CheckCollisionCorrectionY(actor, correction, collisionInfo, tileBounds,
                        tileLeft.IsSolid, tileRight.IsSolid, checkAbove, checkBelow);
                }
            });


            foreach (var block in _actorManager.GetActors(ActorType.Gizmo))
            {
                if (!block.Visible)
                    continue;

                bool checkAbove = actor.MotionVector.Y >= 0;
                bool checkBelow = actor.MotionVector.Y < 0;

                collisionInfo = CheckCollisionCorrectionY(actor, correction, collisionInfo, block.WorldPosition,
                false, false, checkAbove, checkBelow);

                var correctedBottom = actor.WorldPosition.BottomPixel + correction.Y;

                if (collisionInfo.IsOnGround && correctedBottom == block.WorldPosition.TopPixel)
                {
                    correction.CarryMotion = block.MotionVector;
                }
            }

            if (correction.Y != 0)
            {
                actor.WorldPosition.Y.Add(correction.Y);
                collisionInfo += new CollisionInfo(YCorrection: correction.Y);
            }

            if (correction.CarryMotion != null)
            {
                actor.WorldPosition.X.Add(correction.CarryMotion.X);
                actor.WorldPosition.Y.Add(correction.CarryMotion.Y);
            }

            return collisionInfo;
        }

        private void CheckCollisionCorrectionX(Actor actor, CollisionCorrection correction,
            GameRectangleWithSubpixels bounds, bool checkLeftCollision, bool checkRightCollision)
        {

            var xTemp = new PixelValue(0, 0);
            if (bounds.IntersectsWith(actor.WorldPosition))
            {
                var leftDifference = bounds.LeftPixel - actor.WorldPosition.RightPixel;
                var rightDifference = bounds.RightPixel - actor.WorldPosition.LeftPixel;

                if (checkLeftCollision && Math.Abs(leftDifference) <= Math.Abs(rightDifference))
                {                    
                    xTemp = leftDifference;
                    if (xTemp > 0)
                        xTemp = 0;
                    if (xTemp < correction.X)
                        correction.X = xTemp;
                }
                else if(checkRightCollision && Math.Abs(leftDifference) >= Math.Abs(rightDifference))
                {
                    xTemp = rightDifference;
                    if (xTemp < 0)
                        xTemp = 0;
                    if (xTemp > correction.X)
                        correction.X = xTemp;
                }
            }
        }

        private CollisionInfo CheckCollisionCorrectionY(Actor actor, CollisionCorrection correction, CollisionInfo collisionInfo,
            GameRectangleWithSubpixels bounds, bool leftSolid, bool rightSolid, bool checkAbove, bool checkBelow)
        {
           
            var yTemp = new PixelValue(0, 0);

            if (bounds.IntersectsWith(actor.WorldPosition))
            {
                var topDifference = bounds.TopPixel - actor.WorldPosition.BottomPixel;
                var bottomDifference = bounds.BottomPixel - actor.WorldPosition.TopPixel;

                if (checkAbove && (Math.Abs(topDifference) <= Math.Abs(bottomDifference)))
                {
                    yTemp = topDifference;

                    if (yTemp > 0)
                        yTemp = 0;
                    if (yTemp < correction.Y)
                    {
                        correction.Y = yTemp;
                        collisionInfo += CheckTouchingGround(actor, bounds, correction.Y);
                    }
                }
                else if (checkBelow && (Math.Abs(topDifference) >= Math.Abs(bottomDifference)))
                {
                    yTemp = bottomDifference;
                    if (yTemp < 0)
                        yTemp = 0;
                    if (yTemp > correction.Y)
                        correction.Y = yTemp;
                }
            }

            if (checkAbove)
            {
                var groundCollision = CheckTouchingGround(actor, bounds, correction.Y);
                collisionInfo += groundCollision;

                if (groundCollision.IsOnGround && (!leftSolid || !rightSolid))
                    collisionInfo += CheckOnLedge(actor, bounds, leftSolid, rightSolid);
            }

            return collisionInfo;
        }

        private CollisionInfo CheckOnLedge(Actor actor, Rectangle tile, bool leftTileSolid, bool rightTileSolid)
        {
            if (actor.MotionVector.X > 0
                && !rightTileSolid
                && tile.Bottom > actor.WorldPosition.Bottom()
                && actor.WorldPosition.Right() > tile.Center.X)
            {
                return new CollisionInfo(IsFacingLedge: true);
            }
            else if (actor.MotionVector.X < 0
               && !leftTileSolid
               && tile.Bottom > actor.WorldPosition.Bottom()
               && actor.WorldPosition.Left() < tile.Center.X)
            {
                return new CollisionInfo(IsFacingLedge: true);
            }
            else
                return new CollisionInfo();
        }

        private CollisionInfo CheckTouchingGround(Actor actor, Rectangle collidingTile, PixelValue yCorrection)
        {
            var correctedBottom = actor.WorldPosition.BottomPixel + yCorrection;

            if (actor.MotionVector.Y >= 0
                && correctedBottom == collidingTile.Top
                && actor.WorldPosition.Right() >= collidingTile.Left
                && actor.WorldPosition.Left() <= collidingTile.Right)
            {
                return new CollisionInfo(IsOnGround: true);
            }
            else
                return new CollisionInfo();
        }
    }
}
