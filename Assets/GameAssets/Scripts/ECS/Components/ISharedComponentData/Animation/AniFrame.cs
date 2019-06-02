using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct AniFrame : ISharedComponentData
{
    public Vector2Int ant;
    public Vector2Int head;
    public Vector2Int body;
    public Vector2Int leftArm;
    public Vector2Int rightArm;
    public Vector2Int leftHand;
    public Vector2Int rightHand;
    public Vector2Int leftLeg;
    public Vector2Int rightLeg;
    public Vector2Int leftFoot;
    public Vector2Int rightFoot;
    public Vector2Int core;
    public Vector2Int waist;

    public int angle;
    public int face;
}