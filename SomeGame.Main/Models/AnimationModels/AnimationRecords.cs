namespace SomeGame.Main.Models.AnimationModels
{
    public record SpriteFrame(Tile TopLeft, Tile TopRight, Tile BottomLeft, Tile BottomRight);
    public record AnimationFrame(byte SpriteFrameIndex, byte Duration);
    public record Animation(params byte[] FrameIndices);    
}
