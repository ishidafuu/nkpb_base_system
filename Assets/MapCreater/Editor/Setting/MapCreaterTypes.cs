using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enRotate
{
    r0,
    r90,
    r180,
    r270,
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
