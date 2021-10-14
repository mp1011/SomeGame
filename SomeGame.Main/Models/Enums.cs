﻿namespace SomeGame.Main.Models
{

    public enum Direction
    {
        None,
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    }

    public enum InputButton
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Start,
        A,
        B
    }

    public enum ButtonState
    {
        None=0,
        Down=1,
        Changed=2
    }

    public enum Flip
    {
        None=0,
        H=1,
        V=2,
        Both=3
    }

    public enum TileFlags
    {
        None=0,
        FlipH=1,
        FlipV=2,
        FlipHV=3,
        Solid=4
    }

    public enum PaletteIndex
    {
        P1,
        P2,
        P3,
        P4
    }

    public enum LayerIndex
    {
        BG,
        FG,
        Interface
    }

    public enum SpriteIndex
    {
        Sprite1,
        Sprite2,
        Sprite3,
        Sprite4,
        Sprite5,
        Sprite6,
        Sprite7,
        Sprite8
    }

    public enum SpritePriority
    {
        Back,
        Front
    }

    public enum AnimationKey : byte
    {
        Idle,
        Moving,
        Jumping,
        Attacking,
        Hurt
    }

    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public enum TileChoiceMode
    {
        Free,
        SemiStrict,
        Strict
    }

    public enum UIButtonState
    {
        None,
        MouseOver,
        Pressed
    }

    public enum LevelEditorMode
    {
        Free,
        Auto,
        Copy,
        Move
    }

    public enum StandardEnemyState
    {
        Idle,
        Moving,
        Attacking
    }

    public enum ActorType
    {
        Player=1,
        Enemy=2,
        Character=4,
        Bullet=8
    }

    public enum AnimationState
    {
        None,
        Playing,
        Finished
    }

    public enum ActorId
    {
        Player,
        PlayerBullet,
        Skeleton,
        SkeletonBone
    }

    public enum DestroyedState
    {
        None,
        Destroying,
        Destroyed
    }
}
