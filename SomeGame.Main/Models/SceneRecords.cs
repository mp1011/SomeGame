using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Scenes;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Models
{
    record TilesetWithPalette(TilesetContentKey TileSet, PaletteIndex Palette);
    record ActorStart(ActorId ActorId, PixelPoint Position);
    record CollectiblePlacement(CollectibleId Id, Point Position, Point? Position2=null);
    record LayerInfo(LevelContentKey Key, PaletteIndex Palette, byte ScrollFactor);
    record SoundInfo(SoundContentKey Key, byte MaxOccurences);
    record SceneTransitions(
        SceneContentKey Left = SceneContentKey.None, 
        SceneContentKey Right = SceneContentKey.None, 
        SceneContentKey Up = SceneContentKey.None,
        SceneContentKey Down = SceneContentKey.None, 
        SceneContentKey Door1 = SceneContentKey.None, 
        SceneContentKey Door2 = SceneContentKey.None);


    record TransitionInfo(SceneContentKey NextScene, Rectangle ExitSide, Point PlayerExitPosition, Direction Direction)
    {
        public TransitionInfo() : this(SceneContentKey.None, Rectangle.Empty, Point.Zero, Direction.None) { }
    }

    record SceneInfo(
        LayerInfo BgMap,
        LayerInfo FgMap,
        InterfaceType InterfaceType,
        Rectangle Bounds,
        PaletteKeys PaletteKeys, 
        byte BackgroundColor,
        TilesetWithPalette[] VramImages,
        SoundInfo[] Sounds,
        ActorStart[] Actors,
        CollectiblePlacement[] CollectiblePlacements,
        SceneTransitions Transitions)
    {

        public SceneInfo SetActors(IEnumerable<ActorStart> actors)
        {
            return new SceneInfo(BgMap, FgMap, InterfaceType, Bounds, PaletteKeys, BackgroundColor, VramImages, Sounds, actors.ToArray(),
                CollectiblePlacements, Transitions);
        }
    }
}
