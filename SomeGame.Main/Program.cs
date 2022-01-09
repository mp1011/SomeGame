using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Modules;
using SomeGame.Main.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SomeGame.Main
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            StartGame(new RAMManipulator());
        }

        public static void StartGame(IRamViewer ramViewer=null)
        {
          // RunGame(CreateTextureCreatorModule, ramViewer);
         //   RunGame(startup=> CreateThemeDefinerModule(startup, TilesetContentKey.Tiles1), ramViewer);
         //   RunGame(CreatePaletteEditorModule, ramViewer);
              RunGame(startup=> CreateSpriteEditorModule(startup, TilesetContentKey.Gizmos), ramViewer);
              RunGame(CreateAnimationDefinitionModule, ramViewer);
              RunGame(CreateSceneDefinitionModule, ramViewer);
          //  RunGame(startup=> CreateEditorModule(startup, SceneContentKey.Level1, LayerIndex.FG), ramViewer);
            RunGame(startup => CreateSceneModule(startup, SceneContentKey.Level1TitleCard), ramViewer);

        }

        private static void RunGame(Func<GameStartup, IGameModule> createModule, IRamViewer ramViewer)
        {
            using (var game = new GameEngine(createModule, ramViewer))
            {
                game.Run();
            }
        }

        private static IGameModule CreateSceneDefinitionModule(GameStartup gameStartup) => 
            new SceneDefinitionModule(gameStartup);

        private static IGameModule CreateSceneModule(GameStartup gameStartup, SceneContentKey scene) =>
            new SceneModule(scene, gameStartup);
        private static IGameModule CreateEditorModule(GameStartup gameStartup, SceneContentKey level, LayerIndex layer) =>
           new LevelEditorModule(level,layer, gameStartup);

        private static IGameModule CreateSpriteEditorModule(GameStartup gameStartup, TilesetContentKey tileset) =>
            new SpriteEditorModule(tileset, gameStartup);

        private static IGameModule CreateAnimationDefinitionModule(GameStartup gameStartup) =>
             new AnimationDefinitionModule();

        private static IGameModule CreatePaletteEditorModule(GameStartup gameStartup) =>
            new PaletteCreatorModule(gameStartup);

        private static IGameModule CreateTextureCreatorModule(GameStartup gameStartup) =>
            new TextureCreatorModule(gameStartup.ContentManager, gameStartup.GraphicsDevice);

        private static IGameModule CreateThemeDefinerModule(GameStartup gameStartup, TilesetContentKey contentKey) =>
            new ThemeDefinerModule(contentKey, gameStartup);


        //private static IEnumerable<IGameModule> CreateModules(GameStartup startup)
        //{
        // return new SceneDefinitionModule(startup);
        // return new LevelEditorModule(SceneContentKey.Level1, LayerIndex.FG, startup);

        // return new MusicEditorModule(cm, gd);

        //  return new SpriteEditorModule(TilesetContentKey.Bullet2, cm, gd);
        //   return new AnimationDefinitionModule();
        // return new SceneModule(SceneContentKey.Level1TitleCard, startup);
        // return new TextureCreatorModule(cm, gd, ImageContentKey.Mountains);
        // ImageContentKey.Bullet3, ImageContentKey.Clouds, ImageContentKey.Ghost, ImageContentKey.Mountains);


        //  return new PaletteCreatorModule(startup);
        // var module = new GameSystemTestModule();
        //var module = new FontTestModule();
        //   var module = new LevelEditorModule(LevelContentKey.TestLevelBG);
        //  var module = new SceneModule();
        // var module = new TileNeighborModule(new TileSetService());
        //  }
    }
}
