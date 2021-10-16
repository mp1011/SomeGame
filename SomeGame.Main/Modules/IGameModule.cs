using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
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
            GameSystem.SetPalettes(
               resourceLoader.LoadTexture(PaletteKeys.P1).ToIndexedTilesetImage().Palette,
               resourceLoader.LoadTexture(PaletteKeys.P2).ToIndexedTilesetImage().Palette,
               resourceLoader.LoadTexture(PaletteKeys.P3).ToIndexedTilesetImage().Palette,
               resourceLoader.LoadTexture(PaletteKeys.P4).ToIndexedTilesetImage().Palette);

            var vramImages = LoadVramImages(resourceLoader);
           
            GameSystem.SetVram(graphicsDevice, vramImages);

            InitializeLayer(LayerIndex.BG, GameSystem.GetLayer(LayerIndex.BG));
            InitializeLayer(LayerIndex.FG, GameSystem.GetLayer(LayerIndex.FG));
            InitializeLayer(LayerIndex.Interface, GameSystem.GetLayer(LayerIndex.Interface));

            GameSystem.Input.Initialize(GameSystem.Screen);

            ActorManager = new ActorManager(GameSystem, SceneManager);
            InitializeActors();

            SceneManager.SetScene(InitializeScene());

            AfterInitialize(resourceLoader, graphicsDevice);
        }

        protected virtual PaletteKeys PaletteKeys => new PaletteKeys(ImageContentKey.Palette1, ImageContentKey.Palette2, ImageContentKey.Palette1Inverse, ImageContentKey.Palette1Inverse);
       
        protected virtual Scene InitializeScene()
        {
            return new Scene(new Rectangle(0,0,GameSystem.LayerPixelWidth, GameSystem.LayerPixelHeight), GameSystem);
        }

        protected virtual void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
        }

        protected virtual void InitializeActors() { }

        protected abstract void InitializeLayer(LayerIndex index, Layer layer);

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
