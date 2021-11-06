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
        private readonly ActorManager _actorManager;
        private readonly Scroller _scroller;
        public BgCollisionDetector(GameSystem gameSystem, Scroller scroller, 
            ActorManager actorManager,
            CollectiblesService collectiblesService=null)
        {
            _scroller = scroller;
            _actorManager = actorManager;
            _gameSystem = gameSystem;
            _collectiblesService = collectiblesService;
        }

        public CollisionInfo DetectCollisions(Actor actor)
        {
            var collisionInfo = new CollisionInfo();
            var fg = _gameSystem.GetLayer(LayerIndex.FG);

            var layerPosition = _scroller.WorldPositionToLayerPosition(actor.WorldPosition, LayerIndex.FG);

            var topLeftTile = fg.TilePointFromLayerPixelPoint(layerPosition.TopLeft).Offset(-2, -2);
            var bottomRightTile = fg.TilePointFromLayerPixelPoint(layerPosition.BottomRight).Offset(2, 2);

            var xCorrection = new PixelValue(0, 0);
            fg.TileMap.ForEach(topLeftTile, bottomRightTile, (x,y,t) =>
            {
                if(t.IsSolid)
                {                   
                    var tileBounds = fg.GetTileLayerPosition(x, y);
                    tileBounds = _scroller.LayerPositionToWorldPosition(tileBounds, LayerIndex.FG);

                    var tileLeft = fg.TileMap.GetTile(x - 1, y);
                    var tileRight = fg.TileMap.GetTile(x + 1, y);

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
            fg.TileMap.ForEach(topLeftTile, bottomRightTile, (x, y, t) =>
            {
                if (t.IsSolid)
                {
                    var tileAbove = fg.TileMap.GetTile(x, y - 1);
                    var tileBelow = fg.TileMap.GetTile(x, y + 1);
                    var tileLeft = fg.TileMap.GetTile(x - 1, y);
                    var tileRight = fg.TileMap.GetTile(x + 1, y);

                    var tileBounds = fg.GetTileLayerPosition(x, y);
                    tileBounds = _scroller.LayerPositionToWorldPosition(tileBounds, LayerIndex.FG);

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
                else if (_collectiblesService != null && t.IsCollectible)
                {
                    var tileBounds = fg.GetTileLayerPosition(x, y);
                    tileBounds = _scroller.LayerPositionToWorldPosition(tileBounds, LayerIndex.FG);
                    if (tileBounds.IntersectsWith(actor.WorldPosition))
                        collisionInfo += _collectiblesService.HandleCollectibleCollision(x, y, fg);
                }
            });

            if (yCorrection != 0)
            {
                actor.WorldPosition.YPixel += yCorrection;
                collisionInfo += new CollisionInfo(YCorrection: yCorrection);
            }

            DebugService.CheckBadPosition(actor);

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

            //foreach(var block in _actorManager.GetActors(ActorType.Gizmo))
            //{
            //    if (actor.WorldPosition.IntersectsWith(block.WorldPosition))
            //        collisionInfo += HandleXCollision(actor, block.WorldPosition);
            //}

            //foreach (var block in _actorManager.GetActors(ActorType.Gizmo))
            //{
            //    if (actor.WorldPosition.IntersectsWith(block.WorldPosition))
            //        collisionInfo += HandleYCollision(actor, block.WorldPosition);

            //    collisionInfo += CheckTouchingGround(actor, block.WorldPosition);

            //    if (collisionInfo.IsOnGround)
            //        collisionInfo += CheckOnLedge(actor, block.WorldPosition, false, false);
            //}

            return collisionInfo;
        }
    }
}
