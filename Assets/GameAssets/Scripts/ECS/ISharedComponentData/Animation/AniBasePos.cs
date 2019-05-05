using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct AniBasePos : ISharedComponentData
{
    public Vector2Int ANT_BASE;
    public Vector2Int HEAD_BASE;
    public Vector2Int BODY_BASE;
    public Vector2Int L_ARM_BASE;
    public Vector2Int R_ARM_BASE;
    public Vector2Int L_HAND_BASE;
    public Vector2Int R_HAND_BASE;
    public Vector2Int L_LEG_BASE;
    public Vector2Int R_LEG_BASE;
    public Vector2Int L_FOOT_BASE;
    public Vector2Int R_FOOT_BASE;

    public AniDepth FRONTDEPTH;
    public AniDepth BACKDEPTH;
}