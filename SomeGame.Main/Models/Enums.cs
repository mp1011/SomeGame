﻿using System;

namespace SomeGame.Main.Models
{

    public enum InterfaceType : byte
    {
        None,
        PlayerStatus,
        TitleCard
    }

    public enum Direction : byte
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

    public enum Angle : byte
    {
        Right=0,
        Up=64,
        Left=128,
        Down=192
    }

    public enum InputButton : byte
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

    public enum ButtonState : byte
    {
        None=0,
        Down=1,
        Changed=2
    }

    public enum Flip : byte
    {
        None = 0,
        FlipH = 1,
        H =1,
        FlipV = 2,
        V=2,
        Both = 3,
    }

    public enum SpriteFlags: byte
    {
        None=0,
        FlipH=1,
        FlipV=2,
        Visible=4,
        Priority=8
    }

    public enum ActorFlags : byte
    {
        None=0,
        FlipH=1,
        FlipV=2,
        Both=3,
        Enabled=4,
        Visible=16,
        IsAnimationFinished=32,
        HasBeenActivated=64,
        Destroying=128
    }

    public enum TileFlags : byte
    {
        None=0,
        FlipH=1,
        FlipV=2,
        FlipHV=3,
        Solid=4,
        Collectible=8
    }

    public enum PaletteIndex : byte
    {
        P1,
        P2,
        P3,
        P4
    }

    public enum LayerIndex : byte
    {
        BG,
        FG,
        Interface
    }

    public enum SpriteIndex : byte
    {
        Sprite1,
        Sprite2,
        Sprite3,
        Sprite4,
        Sprite5,
        Sprite6,
        Sprite7,
        Sprite8,
        LastIndex=Sprite8
    }

    public enum AnimationKey : byte
    {
        Idle,
        Moving,
        Jumping,
        Attacking,
        Hurt
    }

    public enum Orientation : byte
    {
        Vertical,
        Horizontal
    }

    public enum TileChoiceMode : byte
    {
        Free,
        SemiStrict,
        Strict
    }

    public enum UIButtonState : byte
    {
        None,
        MouseOver,
        Pressed
    }

    public enum LevelEditorMode : byte
    {
        Free,
        Auto,
        Copy,
        Move,
        Relate,
        SetSolid,
        Objects,   
        Collectibles
    }

    public enum StandardEnemyState : byte
    {
        Idle,
        Moving,
        Attacking
    }

    [Flags]
    public enum ActorType : byte
    {
        Player=1,
        Enemy=2,
        Character=4,
        Bullet=8,
        Item=16,
        Decoration=32,
        Gizmo=64
    }

    public enum AnimationState : byte
    {
        None,
        Playing,
        Finished
    }

    public enum ActorId : byte
    {
        Player,
        PlayerBullet,
        Skeleton,
        SkeletonBone,
        Coin,
        Gem,
        Apple,
        Meat,
        Skull,
        DeadSkeletonBone,
        Key,
        MovingPlatform,
        Bat,
        Ghost,
        GhostBullet
    }

    public enum CollectibleId : byte
    {
        Coin = ActorId.Coin,
        Gem = ActorId.Gem,
        Apple = ActorId.Apple,
        Meat = ActorId.Meat,
        Key = ActorId.Key
    }

    public enum DestroyedState : byte
    {
        None,
        Destroying,
        Destroyed
    }
}
