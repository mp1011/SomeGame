using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Modules;
using System;
using System.Threading.Tasks;

namespace SomeGame.Main
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            StartGame();
        }

        public static void StartGame(IRamViewer ramViewer=null)
        {
            using (var game = new GameEngine(CreateModule, ramViewer))
            {
                game.Run();
            }
        }

        private static IGameModule CreateModule(GameStartup startup)
        {
         //  return new SceneDefinitionModule();
         //return new LevelEditorModule(SceneContentKey.Test3, LayerIndex.FG, cm, gd);

           // return new MusicEditorModule(cm, gd);

          //  return new SpriteEditorModule(TilesetContentKey.Bullet2, cm, gd);
          //   return new AnimationDefinitionModule();
          return new SceneModule(SceneContentKey.Level1TitleCard, startup);
           // return new TextureCreatorModule(cm, gd, ImageContentKey.Mountains);
            // ImageContentKey.Bullet3, ImageContentKey.Clouds, ImageContentKey.Ghost, ImageContentKey.Mountains);
         //  return new ThemeDefinerModule(ImageContentKey.Tiles1, TilesetContentKey.Tiles1, cm, gd);


           //   return new PaletteCreatorModule(cm,gd);
            // var module = new GameSystemTestModule();
            //var module = new FontTestModule();
            //   var module = new LevelEditorModule(LevelContentKey.TestLevelBG);
            //  var module = new SceneModule();
            // var module = new TileNeighborModule(new TileSetService());
        }
    }
}
