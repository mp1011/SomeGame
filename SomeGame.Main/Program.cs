using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Modules;
using System;
using System.Collections.Generic;

namespace SomeGame.Main
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameEngine(CreateModule))
                game.Run();
        }

        private static IGameModule CreateModule(ContentManager cm, GraphicsDevice gd)
        {
         //    return new SceneDefinitionModule();
          //return new LevelEditorModule(SceneContentKey.Test3, LayerIndex.FG, cm, gd);

           // return new MusicEditorModule(cm, gd);

          //  return new SpriteEditorModule(TilesetContentKey.Bullet2, cm, gd);
          //   return new AnimationDefinitionModule();
            return new SceneModule(SceneContentKey.Test3, cm, gd);
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
