using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Scenes;

namespace SomeGame.Main.Models
{
    record SceneUpdateResult(Scene LoadScene=null);

    record TilesetWithPalette(TilesetContentKey TileSet, PaletteIndex Palette);
    record ActorStart(ActorId ActorId, PixelPoint Position);
    record CollectiblePlacement(CollectibleId Id, Point Position, Point? Position2=null);
    record LayerInfo(LevelContentKey Key, byte ScrollFactor);
    record SoundInfo(SoundContentKey Key, int MaxOccurences);
    record SceneInfo(
        LayerInfo BgMap,
        LayerInfo FgMap,
        InterfaceType InterfaceType,
        Rectangle Bounds,
        PaletteKeys PaletteKeys, 
        TilesetWithPalette[] VramImages,
        SoundInfo[] Sounds,
        ActorStart[] Actors,
        CollectiblePlacement[] CollectiblePlacements);
}
