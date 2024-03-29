﻿namespace SomeGame.Main.Models
{
    public record SpriteFrame(Tile TopLeft, Tile TopRight, Tile BottomLeft, Tile BottomRight);
    public record AnimationFrame(byte SpriteFrameIndex, byte Duration);
    public record Animation(params AnimationFrame[] Frames)
    {
        public Animation(byte SpriteFrameIndex, byte Duration) : this(new AnimationFrame(SpriteFrameIndex, Duration)) { }

    }
}
