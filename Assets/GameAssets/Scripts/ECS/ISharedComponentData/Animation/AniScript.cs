using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct AniScript : ISharedComponentData
{
    public string id;
    public List<AniFrame> frames;
    public void Setup(string id, List<AniFrame> AniFrames)
    {
        this.id = id;
        this.frames = AniFrames;
    }
    public static AniScript CreateAniScript(string id, List<AniFrame> AniFrames)
    {
        AniScript res = new AniScript();
        res.Setup(id, AniFrames);
        return res;
    }
}