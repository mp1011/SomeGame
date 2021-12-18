using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    abstract class EditorModule : GameModuleBase
    {
        protected EditorModule(GameStartup startup)
            : base(startup) { }

        protected abstract void InitializeLayer(LayerIndex index, Layer layer);

        protected TilesetContentKey[] VramImagesP1 { get; private set; } = new TilesetContentKey[] { };
        protected TilesetContentKey[] VramImagesP2 { get; private set; } = new TilesetContentKey[] { };
        protected TilesetContentKey[] VramImagesP3 { get; private set; } = new TilesetContentKey[] { };
        protected TilesetContentKey[] VramImagesP4 { get; private set; } = new TilesetContentKey[] { };

        protected void SetVram(TilesetContentKey[] p1, TilesetContentKey[] p2, TilesetContentKey[] p3, TilesetContentKey[] p4)
        {
            VramImagesP1 = p1;
            VramImagesP2 = p2;
            VramImagesP3 = p3;
            VramImagesP4 = p4;
        }

        protected virtual void AfterInitialize()
        {
        }

        protected override void OnInitialize()
        {
            GameSystem.Input.Initialize(GameSystem.Screen);
            GameSystem.SetVram(VramImagesP1, VramImagesP2, VramImagesP3, VramImagesP4);

            InitializeLayer(LayerIndex.BG, GameSystem.GetLayer(LayerIndex.BG));
            InitializeLayer(LayerIndex.FG, GameSystem.GetLayer(LayerIndex.FG));
            InitializeLayer(LayerIndex.Interface, GameSystem.GetLayer(LayerIndex.Interface));

            AfterInitialize();
        }
    }
}
