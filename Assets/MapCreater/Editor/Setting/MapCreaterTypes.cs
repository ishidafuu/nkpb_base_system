using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enRotate
{
    Front = 0,
    Right = 1,
    // r180,
    Left = 3,
}

public enum enExpand
{
    None = 0,
    Vertical = 1,
    Horizontal = 2,
}

public enum enFace
{
    fLeft,
    fRight,
    fTop,
    fBottom,
    fRear,
}

public enum enMapEnd
{
    None = 0x000,
    Back = 0x001,
    BackRight = 0x002,
    Right = 0x004,
    FrontRight = 0x008,
    Front = 0x010,
    FrontLeft = 0x020,
    Left = 0x040,
    BackLeft = 0x080,
}

public enum enShapeType
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
