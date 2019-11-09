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

    public enum EnumMotion
    {
        Idle = 0,
        Walk,
        Dash,
        Slip,
        Jump,
        Fall,
        Land,
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
}
