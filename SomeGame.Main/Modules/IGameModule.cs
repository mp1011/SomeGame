using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Scenes;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    interface IGameModule
    {
        void Initialize();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void OnWindowSizeChanged(Viewport viewport);
        Rectangle Screen { get; }

        Color BackgroundColor { get; }
    }

    abstract class GameModuleBase : IGameModule
    {
        protected GraphicsDevice GraphicsDevice { get; }
        protected ResourceLoader ResourceLoader { get; }
        protected DataSerializer DataSerializer { get; }
        protected GameSystem GameSystem { get; }
        protected IRenderService RenderService { get; }
        protected InputManager InputManager { get; } 
        protected InputModel Input => InputManager.Input;

        public Color BackgroundColor => GameSystem.BackgroundColor;

        public Rectangle Screen => GameSystem.Screen;

        public GameModuleBase(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            GameSystem = new GameSystem();
            RenderService= new RasterBasedRenderService(GameSystem);
            InputManager = new InputManager(GameSystem);
            DataSerializer = new DataSerializer();
            ResourceLoader = new ResourceLoader(contentManager);
            GraphicsDevice = graphicsDevice;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderService.DrawFrame(spriteBatch);
        }

        public abstract void Initialize();
       // {
            //GameSystem.Input.Initialize(GameSystem.Screen);
            //CollectiblesService = new CollectiblesService(GameSystem, GameSystem.GetLayer(LayerIndex.FG));

          
            //var actorFactory = new ActorFactory(ActorManager, GameSystem, _dataSerializer, InputManager, SceneManager,
            //    null, AudioService, CollectiblesService);

            //var sceneLoader = new SceneLoader(resourceLoader, graphicsDevice, _dataSerializer, actorFactory, GameSystem);
            //SceneManager = new SceneManager(GameSystem, sceneLoader);

            //AfterInitialize(resourceLoader, graphicsDevice);

            //SceneManager.QueueNextScene(GetInitialScene());
            //SceneManager.Update();

            //todo 
            //GameSystem.SetPalettes(
            //   resourceLoader.LoadTexture(PaletteKeys.P1).ToIndexedTilesetImage().Palette,
            //   resourceLoader.LoadTexture(PaletteKeys.P2).ToIndexedTilesetImage().Palette,
            //   resourceLoader.LoadTexture(PaletteKeys.P3).ToIndexedTilesetImage().Palette,
            //   resourceLoader.LoadTexture(PaletteKeys.P4).ToIndexedTilesetImage().Palette);

            //var vramImages = LoadVramImages(resourceLoader);
           
            //GameSystem.SetVram(graphicsDevice, vramImages);

            //InitializeLayer(LayerIndex.BG, GameSystem.GetLayer(LayerIndex.BG));
            //InitializeLayer(LayerIndex.FG, GameSystem.GetLayer(LayerIndex.FG));
            //InitializeLayer(LayerIndex.Interface, GameSystem.GetLayer(LayerIndex.Interface));

            //
            //
            //
            //InitializeActors();

            //SceneManager.QueueNextScene(GetInitialScene());

            //LoadSounds(resourceLoader);

            //AfterInitialize(resourceLoader, graphicsDevice);
       // }


        public void Update(GameTime gameTime)
        {
            InputManager.Update();
            //SceneManager.Update();
           // ActorManager.Update(SceneManager.CurrentScene);
            Update();
        }

        protected abstract void Update();

        public void OnWindowSizeChanged(Viewport viewport)
        {
            InputManager.AdjustMouseScale(GameSystem, viewport);
        }
    }
}
