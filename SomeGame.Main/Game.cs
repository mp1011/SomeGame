using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;
using SomeGame.Main.Modules;
using System;

namespace SomeGame.Main
{
    class GameEngine : Game
    {
        private Texture2D _debugTexture;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D _renderTarget;

        private readonly Func<GameStartup, IGameModule> _createModule;
        private IGameModule _currentModule;
        private IRamViewer _ramViewer;

        public IGameModule CurrentModule => _currentModule; 

        public GameEngine(Func<GameStartup, IGameModule> createModule, IRamViewer ramViewer)
        {
            _ramViewer = ramViewer;
            _createModule = createModule;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(15);
            new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            _currentModule.OnWindowSizeChanged(GraphicsDevice.Viewport);
        }

        protected override void Initialize()
        {
            _currentModule = _createModule(new GameStartup(Content, GraphicsDevice, _ramViewer));
            _currentModule.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);           
            _currentModule.OnWindowSizeChanged(GraphicsDevice.Viewport);
            _renderTarget = new RenderTarget2D(GraphicsDevice, _currentModule.Screen.Width, _currentModule.Screen.Height);

            _debugTexture = new Texture2D(GraphicsDevice, 1, 1);
            _debugTexture.SetData(new Color[] { new Color(255,255,255,100) });
        }

        protected override void Update(GameTime gameTime)
        {
            _currentModule.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(_currentModule.BackgroundColor); 

            _spriteBatch.Begin();
            _currentModule.Draw(_spriteBatch);

           // DebugService.DrawHitboxes(_spriteBatch, _debugTexture);

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            var aspectRatio = (double)_renderTarget.Height / _renderTarget.Width;
            // look into this->  spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, m);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, Window.ClientBounds.Width, (int)(Window.ClientBounds.Width*aspectRatio)), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
