using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;
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
        private readonly GameSystem _system;
        private readonly RenderService _renderService;
        private SpriteAnimator _spriteAnimator;
        private ActorManager _actorManager;
        private Scene _currentScene;

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
            _actorManager = new ActorManager(_spriteAnimator);
            InitializeActors(_system, _spriteAnimator, _actorManager);

            _currentScene = InitializeScene(_system);

            AfterInitialize(resourceLoader, graphicsDevice);
        }

        protected virtual Scene InitializeScene(GameSystem gameSystem)
        {
            return new Scene(new Rectangle(0,0,_system.LayerPixelWidth, _system.LayerPixelHeight), _system);
        }

        protected virtual void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {

        }

        protected virtual SpriteAnimator InitializeAnimations()
        {
            return new SpriteAnimator(new SpriteFrame[] { }, new AnimationFrame[] { }, new Animation[] { });
        }

        protected virtual void InitializeActors(GameSystem system, SpriteAnimator spriteAnimator, ActorManager actorManager) { }

        protected abstract void InitializeLayer(GameSystem system, LayerIndex index, Layer layer);

        protected abstract Palette CreatePalette(IndexedTilesetImage[] tilesetImages, PaletteIndex index);

        protected abstract IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader, GameSystem system);

        public void Update(GameTime gameTime)
        {
            Input = _system.Input.ReadInput(_mouseScale);
            _spriteAnimator.Update(_system);
            _currentScene.Update(_system);
            _actorManager.Update(_system, _currentScene);
            Update(_system, _currentScene);
        }

        protected abstract void Update(GameSystem gameSystem, Scene currentScene);

        public void OnWindowSizeChanged(Viewport viewport)
        {
            _mouseScale = _system.Screen.Width / (double)viewport.Width;
        }
    }
}
