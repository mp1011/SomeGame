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

        public GameRectangle Camera { get; private set; }

        public Scroller(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
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
            Camera = new BoundedGameRectangle(bounds.X, bounds.Y, _gameSystem.Screen.Width, _gameSystem.Screen.Height,
                    maxX: bounds.Width - _gameSystem.Screen.Width,
                    maxY: bounds.Height - _gameSystem.Screen.Height);

            _bgLayer = new ScrollingLayer(_gameSystem, bg, _gameSystem.GetLayer(LayerIndex.BG));
            _fgLayer = new ScrollingLayer(_gameSystem, fg, _gameSystem.GetLayer(LayerIndex.FG));
        }

        public void Initialize()
        {
            _bgLayer.Initialize();
            _fgLayer.Initialize();
        }

        public void Update()
        {
            _bgLayer.ScrollLayer(Camera);
            _fgLayer.ScrollLayer(Camera);
        }

        public void ScrollActor(Actor actor, Sprite sprite)
        {
            var actorScreenX = sprite.ScrollX.Set(actor.WorldPosition.X - Camera.X);
            var actorScreenY = sprite.ScrollX.Set(actor.WorldPosition.Y - Camera.Y);

            sprite.ScrollX = actorScreenX - actor.LocalHitbox.X;
            sprite.ScrollY = actorScreenY - actor.LocalHitbox.Y;
        }

        

    }
}
