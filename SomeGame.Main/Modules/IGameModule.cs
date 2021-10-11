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
        protected GameSystem GameSystem { get; }
        protected RenderService RenderService { get; }
        protected SpriteAnimator SpriteAnimator { get; private set; }
        protected ActorManager ActorManager { get; private set; }
        protected SceneManager SceneManager { get; }
        protected InputManager InputManager { get; } 
        protected InputModel Input => InputManager.Input;

        public Rectangle Screen => GameSystem.Screen;

        public GameModuleBase()
        {
            GameSystem = new GameSystem();
            RenderService= new RenderService(GameSystem);
            SceneManager = new SceneManager();
            InputManager = new InputManager(GameSystem);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var sprite in GameSystem.GetBackSprites())
                RenderService.DrawSprite(spriteBatch, sprite);

            RenderService.DrawLayer(spriteBatch, GameSystem.GetLayer(LayerIndex.BG));
            RenderService.DrawLayer(spriteBatch, GameSystem.GetLayer(LayerIndex.FG));

            foreach (var sprite in GameSystem.GetFrontSprites())
                RenderService.DrawSprite(spriteBatch, sprite);

            RenderService.DrawLayer(spriteBatch, GameSystem.GetLayer(LayerIndex.Interface));

        }

        public void Initialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            var vramImages = LoadVramImages(resourceLoader);

            GameSystem.SetPalettes(CreatePalette(vramImages, PaletteIndex.P1),
                                CreatePalette(vramImages, PaletteIndex.P2),
                                CreatePalette(vramImages, PaletteIndex.P3),
                                CreatePalette(vramImages, PaletteIndex.P4));

            GameSystem.SetVram(graphicsDevice, vramImages);

            InitializeLayer(LayerIndex.BG, GameSystem.GetLayer(LayerIndex.BG));
            InitializeLayer(LayerIndex.FG, GameSystem.GetLayer(LayerIndex.FG));
            InitializeLayer(LayerIndex.Interface, GameSystem.GetLayer(LayerIndex.Interface));

            GameSystem.Input.Initialize(GameSystem.Screen);

            SpriteAnimator = InitializeAnimations();
            ActorManager = new ActorManager(GameSystem, SpriteAnimator, SceneManager);
            InitializeActors();

            SceneManager.SetScene(InitializeScene());

            AfterInitialize(resourceLoader, graphicsDevice);
        }

        protected virtual Scene InitializeScene()
        {
            return new Scene(new Rectangle(0,0,GameSystem.LayerPixelWidth, GameSystem.LayerPixelHeight), GameSystem);
        }

        protected virtual void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
        }

        protected virtual SpriteAnimator InitializeAnimations()
        {
            return new SpriteAnimator(GameSystem, new SpriteFrame[] { }, new AnimationFrame[] { }, new Animation[] { });
        }

        protected virtual void InitializeActors() { }

        protected abstract void InitializeLayer(LayerIndex index, Layer layer);

        protected abstract Palette CreatePalette(IndexedTilesetImage[] tilesetImages, PaletteIndex index);

        protected abstract IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader);

        public void Update(GameTime gameTime)
        {
            InputManager.Update();
            SceneManager.Update();
            ActorManager.Update();
            Update();
        }

        protected abstract void Update();

        public void OnWindowSizeChanged(Viewport viewport)
        {
            InputManager.AdjustMouseScale(GameSystem, viewport);
        }
    }
}
