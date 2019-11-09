using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NKPB
{
    public static class Shared
    {
        public static MeshMatList m_charaMeshMat;

        public static void ReadySharedComponentData()
        {
            m_charaMeshMat = new MeshMatList(PathSettings.CharaSprite, PathSettings.DefaultShader);

        }

    }
}
