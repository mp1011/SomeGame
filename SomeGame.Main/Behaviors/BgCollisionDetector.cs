using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

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

            var topLeftTile = actor.WorldPosition.TopLeft
                                        .Divide(_gameSystem.TileSize)
                                        .Offset(-2, -2);

            var bottomRightTile = actor.WorldPosition.BottomRight
                                        .Divide(_gameSystem.TileSize)
                                        .Offset(2, 2);

            var xCorrection = new PixelValue(0, 0);
            _tileMap.ForEach(topLeftTile, bottomRightTile, (x,y,t) =>
            {
                if (t.IsSolid)
                {
                    var tileBounds = new GameRectangleWithSubpixels(x * _gameSystem.TileSize, y * _gameSystem.TileSize,
                        _gameSystem.TileSize, _gameSystem.TileSize);

                    var tileLeft = _tileMap.GetTile(x - 1, y);
                    var tileRight = _tileMap.GetTile(x + 1, y);

                    var xTemp = new PixelValue(0, 0);
                    if (tileBounds.IntersectsWith(actor.WorldPosition))
                    {
                        if (actor.MotionVector.X > 0 && !tileLeft.IsSolid)
                        {
                            var minXCorrection = -actor.MotionVector.X.NearestPixel(1);

                            xTemp = tileBounds.LeftPixel - actor.WorldPosition.RightPixel;
                            if (xTemp > 0)
                                xTemp = 0;
                            if (xTemp < minXCorrection)
                                xTemp = minXCorrection;
                            if (xTemp < xCorrection)
                                xCorrection = xTemp;
                        }
                        else if (actor.MotionVector.X < 0 && !tileRight.IsSolid)
                        {
                            var maxXCorrection = -actor.MotionVector.X.NearestPixel(-1);
                            xTemp = tileBounds.RightPixel - actor.WorldPosition.LeftPixel;
                            if (xTemp < 0)
                                xTemp = 0;
                            if (xTemp > maxXCorrection)
                                xTemp = maxXCorrection;
                            if (xTemp > xCorrection)
                                xCorrection = xTemp;
                        }
                    }
                }
            });

            if (xCorrection != 0)
            {
                actor.WorldPosition.XPixel += xCorrection;
                collisionInfo += new CollisionInfo(XCorrection: xCorrection);
            }

            var yCorrection = new PixelValue(0, 0);
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
        
                    var yTemp = new PixelValue(0, 0);

                    if (tileBounds.IntersectsWith(actor.WorldPosition))
                    {
                        if (actor.MotionVector.Y > 0 && !tileAbove.IsSolid)
                        {
                            yTemp = tileBounds.TopPixel - actor.WorldPosition.BottomPixel;

                            if (yTemp > 0)
                                yTemp = 0;
                            if (yTemp < yCorrection)
                            {
                                yCorrection = yTemp;
                                collisionInfo += CheckTouchingGround(actor, tileBounds, yCorrection);
                            }
                        }
                        else if (actor.MotionVector.Y < 0 && !tileBelow.IsSolid)
                        {
                            yTemp = tileBounds.BottomPixel - actor.WorldPosition.TopPixel;
                            if (yTemp < 0)
                                yTemp = 0;
                            if (yTemp > yCorrection)
                                yCorrection = yTemp;
                        }
                    }

                    if(!tileAbove.IsSolid)
                        collisionInfo += CheckTouchingGround(actor, tileBounds, yCorrection);

                    if (collisionInfo.IsOnGround && (!tileLeft.IsSolid || !tileRight.IsSolid))
                        collisionInfo += CheckOnLedge(actor, tileBounds, tileLeft.IsSolid, tileRight.IsSolid);
                }
            });

            if (yCorrection != 0)
            {
                actor.WorldPosition.YPixel += yCorrection;
                collisionInfo += new CollisionInfo(YCorrection: yCorrection);
            }

            return collisionInfo += HandleMovingBlockCollisions(actor);
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

        private CollisionInfo CheckTouchingGround(Actor actor, Rectangle collidingTile, PixelValue yCorrection)
        {
            var correctedBottom = actor.WorldPosition.BottomPixel + yCorrection;

            if (actor.MotionVector.Y >= 0
                && correctedBottom == collidingTile.Top
                && actor.WorldPosition.Right >= collidingTile.Left
                && actor.WorldPosition.Left <= collidingTile.Right)
            {
                return new CollisionInfo(IsOnGround: true);
            }
            else
                return new CollisionInfo();
        }

        private CollisionInfo HandleMovingBlockCollisions(Actor actor)
        {
            CollisionInfo collisionInfo = new CollisionInfo();

            PixelValue xCorrection = new PixelValue(0, 0);
            PixelValue yCorrection = new PixelValue(0, 0);

            //foreach (var block in _actorManager.GetActors(ActorType.Gizmo))
            //{
            //    if (!actor.WorldPosition.IntersectsWith(block.WorldPosition))
            //        continue;

            //    var relativeXMotion = actor.MotionVector.X - block.MotionVector.X;

            //    var xTemp = new PixelValue(0, 0);
         
            //    if (relativeXMotion > 0)
            //    {
            //        var minXCorrection = -actor.MotionVector.X.NearestPixel(1);

            //        xTemp = block.WorldPosition.LeftPixel - actor.WorldPosition.RightPixel;
            //        if (xTemp > 0)
            //            xTemp = 0;
            //        if (xTemp < minXCorrection)
            //            xTemp = minXCorrection;
            //        if (xTemp < xCorrection)
            //            xCorrection = xTemp;
            //    }
            //    else if (relativeXMotion < 0)
            //    {
            //        var maxXCorrection = -actor.MotionVector.X.NearestPixel(-1);
            //        xTemp = block.WorldPosition.RightPixel - actor.WorldPosition.LeftPixel;
            //        if (xTemp < 0)
            //            xTemp = 0;
            //        if (xTemp > maxXCorrection)
            //            xTemp = maxXCorrection;
            //        if (xTemp > xCorrection)
            //            xCorrection = xTemp;
            //    }
            //}

            //if (xCorrection != 0)
            //{
            //    actor.WorldPosition.XPixel += xCorrection;
            //    collisionInfo += new CollisionInfo(XCorrection: xCorrection);
            //}

            foreach (var block in _actorManager.GetActors(ActorType.Gizmo))
            {
                if (actor.WorldPosition.IntersectsWith(block.WorldPosition))
                {
                    var yTemp = new PixelValue(0, 0);
                    var relativeYMotion = actor.MotionVector.Y - block.MotionVector.Y;
                   
                    if (relativeYMotion > 0)
                    {
                        yTemp = block.WorldPosition.TopPixel - actor.WorldPosition.BottomPixel;

                        if (yTemp > 0)
                            yTemp = 0;
                        if (yTemp < yCorrection)
                        {
                            yCorrection = yTemp;
                            collisionInfo += CheckTouchingGround(actor, block.WorldPosition, yCorrection);
                        }
                    }
                    //else if (relativeYMotion < 0)
                    //{
                    //    yTemp = block.WorldPosition.BottomPixel - actor.WorldPosition.TopPixel;
                    //    if (yTemp < 0)
                    //        yTemp = 0;
                    //    if (yTemp > yCorrection)
                    //        yCorrection = yTemp;
                    //}                    
                }
                collisionInfo += CheckTouchingGround(actor, block.WorldPosition, yCorrection);

                if(collisionInfo.IsOnGround)
                {
                    actor.WorldPosition.XPixel += block.MotionVector.X;
                    actor.WorldPosition.YPixel += block.MotionVector.Y;
                }

                if (collisionInfo.IsOnGround)
                    collisionInfo += CheckOnLedge(actor, block.WorldPosition, false, false);
            }

            if (yCorrection != 0)
            {
                actor.WorldPosition.YPixel += yCorrection;
                collisionInfo += new CollisionInfo(YCorrection: yCorrection);
            }

            return collisionInfo;
        }
    }
}
