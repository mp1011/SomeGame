using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Scenes;
using System;

namespace SomeGame.Main.Services
{
    class Scroller
    {
        private readonly GameSystem _gameSystem;
        private ScrollingLayer _bgLayer;
        private ScrollingLayer _fgLayer;

        public BoundedGameRectangle Camera { get; }

        public Scroller(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
            Camera = gameSystem.RAM.DeclareBoundedRectangle(0, 0, _gameSystem.Screen.Width, _gameSystem.Screen.Height, 0, 0);
            SetTileMaps(new TileMap(LevelContentKey.None,2,2),new TileMap(LevelContentKey.None, 2, 2));
        }

        public GameRectangleWithSubpixels WorldPositionToLayerPosition(GameRectangleWithSubpixels worldPosition, LayerIndex layerIndex)
        {
            switch (layerIndex)
            {
                case LayerIndex.BG: return _bgLayer.WorldPositionToLayerPosition(worldPosition);
                case LayerIndex.FG: return _fgLayer.WorldPositionToLayerPosition(worldPosition);
                default: throw new Exception("Scrollong of Interface Layer not supported");
            }
        }

        public GameRectangleWithSubpixels LayerPositionToWorldPosition(GameRectangleWithSubpixels layerPosition, LayerIndex layerIndex)
        {
            switch (layerIndex)
            {
                case LayerIndex.BG: return _bgLayer.LayerPositionToWorldPosition(layerPosition);
                case LayerIndex.FG: return _fgLayer.LayerPositionToWorldPosition(layerPosition);
                default: throw new Exception("Scrollong of Interface Layer not supported");
            }
        }

        public Point GetTopLeftTile(LayerIndex layerIndex)
        {
            switch(layerIndex)
            {
                case LayerIndex.BG: return _bgLayer.TopLeftTile;
                case LayerIndex.FG: return _fgLayer.TopLeftTile;
                default: throw new Exception("Scrolling of Interface Layer not supported");
            }
        }

        public TileMap GetTilemap(LayerIndex layerIndex)
        {
            switch(layerIndex)
            {
                case LayerIndex.BG: return _bgLayer.TileMap;
                case LayerIndex.FG: return _fgLayer.TileMap;
                default: throw new ArgumentException();
            }
        }

        public void SetTileMaps(TileMap bg, TileMap fg)
        {
            var maxWidth = Math.Max(bg.TilesX, fg.TilesX) * _gameSystem.TileSize;
            var maxHeight = Math.Max(bg.TilesY, fg.TilesY) * _gameSystem.TileSize;

            var bounds = new Rectangle(0, 0, maxWidth, maxHeight);
            Camera.X = bounds.X;
            Camera.Y = bounds.Y;
            Camera.MaxX = bounds.Width - _gameSystem.Screen.Width;
            Camera.MaxY = bounds.Height - _gameSystem.Screen.Height;

            _bgLayer = new ScrollingLayer(_gameSystem, bg, _gameSystem.GetLayer(LayerIndex.BG));
            _fgLayer = new ScrollingLayer(_gameSystem, fg, _gameSystem.GetLayer(LayerIndex.FG));
        }

        public void Initialize()
        {
            _bgLayer.Initialize();
            _fgLayer.Initialize();
            Camera.X = 0;
            Camera.Y = 0;
        }

        public void Update()
        {
            _bgLayer.ScrollLayer(Camera);
            _fgLayer.ScrollLayer(Camera);
        }

        public void ScrollActor(Actor actor, Sprite sprite)
        {
            if(!_fgLayer.IsInBounds(actor.WorldPosition))
            {
                actor.Visible = false;
                return;
            }

            actor.Visible = true;

            var actorScreenX = sprite.ScrollX.Set(actor.WorldPosition.X.Pixel - Camera.X);
            var actorScreenY = sprite.ScrollY.Set(actor.WorldPosition.Y.Pixel - Camera.Y);

            sprite.ScrollX = actorScreenX - actor.LocalHitbox.X;
            sprite.ScrollY = actorScreenY - actor.LocalHitbox.Y;
        }

        

    }
}
