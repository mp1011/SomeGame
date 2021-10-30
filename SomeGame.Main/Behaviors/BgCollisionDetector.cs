using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class BgCollisionDetector : BlockCollisionDetector, ICollisionDetector
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

        public CollisionInfo DetectCollisions(Actor actor, GameRectangleWithSubpixels frameStartPosition)
        {
            var collisionInfo = new CollisionInfo();
            var fg = _gameSystem.GetLayer(LayerIndex.FG);

            var layerPosition = _scroller.WorldPositionToLayerPosition(actor.WorldPosition, LayerIndex.FG);

            var topLeftTile = fg.TilePointFromLayerPixelPoint(layerPosition.TopLeft).Offset(-2, -2);
            var bottomRightTile = fg.TilePointFromLayerPixelPoint(layerPosition.BottomRight).Offset(2, 2);

            fg.TileMap.ForEach(topLeftTile, bottomRightTile, (x,y,t) =>
            {
                if(t.IsSolid)
                {
                    var tileAbove = fg.TileMap.GetTile(x, y - 1);
                    var tileBelow = fg.TileMap.GetTile(x, y + 1);
                    var tileLeft = fg.TileMap.GetTile(x - 1, y);
                    var tileRight = fg.TileMap.GetTile(x + 1, y);

                    var tileBounds = fg.GetTileLayerPosition(x, y);
                    tileBounds = _scroller.LayerPositionToWorldPosition(tileBounds, LayerIndex.FG);

                    if (tileBounds.Intersects(actor.WorldPosition))
                        collisionInfo += HandleCollision(actor, tileBounds, frameStartPosition, tileAbove, tileBelow, tileLeft, tileRight);

                    if (!tileAbove.IsSolid)
                        collisionInfo += CheckTouchingGround(actor, tileBounds);

                    if (collisionInfo.IsOnGround && (!tileLeft.IsSolid || !tileRight.IsSolid))
                        collisionInfo += CheckOnLedge(actor, tileBounds, tileLeft.IsSolid, tileRight.IsSolid);
                }
                else if(_collectiblesService != null && t.IsCollectible)
                {
                    var tileBounds = fg.GetTileLayerPosition(x, y);
                    tileBounds = _scroller.LayerPositionToWorldPosition(tileBounds, LayerIndex.FG);
                    if (tileBounds.Intersects(actor.WorldPosition))
                        collisionInfo += _collectiblesService.HandleCollectibleCollision(x, y, fg);
                }
            });

            return collisionInfo += HandleMovingBlockCollisions(actor, frameStartPosition);
        }

        private CollisionInfo HandleMovingBlockCollisions(Actor actor, GameRectangleWithSubpixels frameStartPosition)
        {
            CollisionInfo collisionInfo = new CollisionInfo();

            foreach(var block in _actorManager.GetActors(ActorType.Gizmo))
            {
                if (actor.WorldPosition.IntersectsWith(block.WorldPosition))
                    collisionInfo += HandleCollision(actor, block.WorldPosition, frameStartPosition);

                collisionInfo += CheckTouchingGround(actor, block.WorldPosition);

                if (collisionInfo.IsOnGround)
                    collisionInfo += CheckOnLedge(actor, block.WorldPosition, false, false);
            }

            return collisionInfo;
        }
    }
}
