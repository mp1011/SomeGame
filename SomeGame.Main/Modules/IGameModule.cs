using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;
using SomeGame.Main.Models.AnimationModels;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    interface IGameModule
    {
        void Initialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void OnWindowSizeChanged(Viewport viewport);
        Rectangle Screen { get; }
    }

    abstract class GameModuleBase : IGameModule
    {
        private ulong _frameNumber;
        private readonly GameSystem _system;
        private readonly RenderService _renderService;
        private SpriteAnimator _spriteAnimator;

        private double _mouseScale = 1.0;
        protected InputModel Input { get; private set; }

        public Rectangle Screen => _system.Screen;

        public GameModuleBase()
        {
            _system = new GameSystem();
            _renderService = new RenderService(_system);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var sprite in _system.GetBackSprites())
                _renderService.DrawSprite(spriteBatch, sprite);

            _renderService.DrawLayer(spriteBatch, _system.GetLayer(LayerIndex.BG));
            _renderService.DrawLayer(spriteBatch, _system.GetLayer(LayerIndex.FG));
            _renderService.DrawLayer(spriteBatch, _system.GetLayer(LayerIndex.Interface));


            foreach (var sprite in _system.GetFrontSprites())
                _renderService.DrawSprite(spriteBatch, sprite);
        }

        public void Initialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            var vramImages = LoadVramImages(resourceLoader, _system);

            _system.SetPalettes(CreatePalette(vramImages, PaletteIndex.P1),
                                CreatePalette(vramImages, PaletteIndex.P2),
                                CreatePalette(vramImages, PaletteIndex.P3),
                                CreatePalette(vramImages, PaletteIndex.P4));

            _system.SetVram(graphicsDevice, vramImages);

            InitializeLayer(_system, LayerIndex.BG, _system.GetLayer(LayerIndex.BG));
            InitializeLayer(_system, LayerIndex.FG, _system.GetLayer(LayerIndex.FG));
            InitializeLayer(_system, LayerIndex.Interface, _system.GetLayer(LayerIndex.Interface));

            _system.Input.Initialize(_system.Screen);

            _spriteAnimator = InitializeAnimations();
            InitializeSprites(_system, _spriteAnimator);

            AfterInitialize(resourceLoader, graphicsDevice);
        }

        protected virtual void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {

        }

        protected virtual SpriteAnimator InitializeAnimations()
        {
            return new SpriteAnimator(new SpriteFrame[] { }, new AnimationFrame[] { }, new Animation[] { });
        }

        protected virtual void InitializeSprites(GameSystem system, SpriteAnimator spriteAnimator) { }

        protected abstract void InitializeLayer(GameSystem system, LayerIndex index, Layer layer);

        protected abstract Palette CreatePalette(IndexedTilesetImage[] tilesetImages, PaletteIndex index);

        protected abstract IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader, GameSystem system);

        public void Update(GameTime gameTime)
        {
            Input = _system.Input.ReadInput(_mouseScale);
            _spriteAnimator.Update(_system);
            Update(gameTime, _system);
        }

        protected abstract void Update(GameTime gameTime, GameSystem gameSystem);

        public void OnWindowSizeChanged(Viewport viewport)
        {
            _mouseScale = _system.Screen.Width / (double)viewport.Width;
        }
    }
}
