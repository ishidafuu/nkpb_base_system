using System;
using System.Collections.Generic;
using UnityEngine;
namespace NKPB
{
    [Flags]
    public enum FlagMotion
    {
        None = 0x0000,
        Move = 0x1,
        Slip = 0x2,
        Accel = 0x4,
        DashStart = 0x8,
        Dash = 0x10,
        Brake = 0x20,
        JumpStart = 0x40,
        Jump = 0x80,
        Fall = 0x100,
        Hover = 0x200,
        Crouch = 0x400,
        Stick = 0x800,
        Dizzy = 0x1000,
        Groggy = 0x2000,
        Stand = 0x4000,
        Lie = 0x8000,
    }

    public static partial class FlagMotionExtend
    {
        //motionFlags.HasFlagはバーストできないので != 0で代用
        public static bool IsFlag(this FlagMotion param, FlagMotion flag) => (param & flag) != 0;
        public static void AddFlag(this ref FlagMotion param, FlagMotion flag) => param |= flag;
        public static void SubFlag(this ref FlagMotion param, FlagMotion flag) => param &= ~flag;
    }

    [Flags]
    public enum FlagInputCheck
    {
        None = 0x0000,
        Jump = 0x0001,
        Dash = 0x0002,
        Walk = 0x0004,
        Slip = 0x0008,
        Idle = 0x0010,
    };

    public static partial class FlagInputCheckExtend
    {
        public static bool IsFlag(this FlagInputCheck param, FlagInputCheck flag) => (param & flag) != 0;
        public static void AddFlag(this ref FlagInputCheck param, FlagInputCheck flag) => param |= flag;
        public static void SubFlag(this ref FlagInputCheck param, FlagInputCheck flag) => param &= ~flag;
    }

    [Flags]
    public enum FlagMove
    {
        None = 0x0000,
        Walk = 0x0001,
        Dash = 0x0002,
        Friction = 0x0004,
        Stop = 0x0008,
        Air = 0x0010,
    };

    public static partial class FlagMoveExtend
    {
        public static bool IsFlag(this FlagMove param, FlagMove flag) => (param & flag) != 0;
        public static void AddFlag(this ref FlagMove param, FlagMove flag) => param |= flag;
        public static void SubFlag(this ref FlagMove param, FlagMove flag) => param &= ~flag;
    }


    [Flags]
    public enum FlagMapCheck
    {
        None = 0x0000,
        Land = 0x0001,
        Fall = 0x0002,
        Stick = 0x0004,
        Crash = 0x0008,
        // Slip = 0x0008,
        // Idle = 0x0010,
    };

    public static partial class FlagMapCheckExtend
    {
        public static bool IsFlag(this FlagMapCheck param, FlagMapCheck flag) => (param & flag) != 0;
        public static void AddFlag(this ref FlagMapCheck param, FlagMapCheck flag) => param |= flag;
        public static void SubFlag(this ref FlagMapCheck param, FlagMapCheck flag) => param &= ~flag;
    }
}
