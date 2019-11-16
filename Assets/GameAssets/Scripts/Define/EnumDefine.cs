using System;
using System.Collections.Generic;
using UnityEngine;
namespace NKPB
{
    public enum EnumCrossType
    {
        None = 0,
        Up,
        Down,
        Left,
        Right,
    }

    public enum EnumButtonType
    {
        None = 0,
        A,
        B,
        X,
        Y,
    }

    public enum EnumToukiMaterState
    {
        Active = 0,
        Inactive,
        Decide,
    }

    public enum EnumInputCross
    {
        Up,
        Down,
        Left,
        Right,
        _END,
    }

    public enum EnumInputButton
    {
        A,
        B,
        Jump,
        _END,
    }

    public enum EnumMuki
    {
        Left = -1,
        None = 0,
        Right = 1,
    }

    public enum EnumMotionType
    {
        Idle = 0,
        Jump,
        Dash,
        Walk,
        Punch,
        Land,
        Slip,
        Fall,
        Damage,
        Fly,
        Down,
        Dead,
        Action,
    }

    public enum EnumMoveMuki
    {
        None,
        Left,
        LeftLeftUp,
        LeftUp,
        LeftLeftDown,
        LeftDown,
        Up,
        Right,
        RightRightUp,
        RightUp,
        RightRightDown,
        RightDown,
        Down,
    }

    public enum EnumShapeType
    {
        Empty,
        Box,
        LUpSlope,
        RUpSlope,
        LUpSlope2H,
        LUpSlope2L,
        RUpSlope2L,
        RUpSlope2H,
        SlashWall,
        BSlashWall,
    }
    public static partial class EnumShapeTypeExtend
    {
        public static bool IsEmpty(this EnumShapeType value) => value == EnumShapeType.Empty;
        public static bool IsBox(this EnumShapeType value) => value == EnumShapeType.Box;
        public static bool IsSlashWall(this EnumShapeType value) => (value == EnumShapeType.SlashWall) || (value == EnumShapeType.BSlashWall);

        public static bool IsSlope(this EnumShapeType value)
        {
            bool res = false;
            switch (value)
            {
                case EnumShapeType.LUpSlope:
                case EnumShapeType.RUpSlope:
                case EnumShapeType.LUpSlope2L:
                case EnumShapeType.LUpSlope2H:
                case EnumShapeType.RUpSlope2L:
                case EnumShapeType.RUpSlope2H:
                    res = true;
                    break;
            }
            return res;
        }
    }
}
