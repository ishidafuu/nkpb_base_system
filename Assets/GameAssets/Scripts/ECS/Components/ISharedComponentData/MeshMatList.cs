using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    [Serializable]
    public struct MeshMatList : IEquatable<MeshMatList>, ISharedComponentData
    {

        public Dictionary<string, Material> m_materialDict;
        public Dictionary<string, Mesh> m_meshDict;
        int m_colorPropertyId;

        public MeshMatList(string path, string shader)
        {
            m_meshDict = new Dictionary<string, Mesh>();
            m_materialDict = new Dictionary<string, Material>();
            UnityEngine.Object[] list = Resources.LoadAll(path, typeof(Sprite));
            // listがnullまたは空ならエラーで返す
            if (list == null || list.Length == 0)
            {
                Debug.LogWarning(path);
            }

            // マテリアル用シェーダー
            var matShader = Shader.Find(shader);
            m_colorPropertyId = Shader.PropertyToID("_Color");

            if (matShader == null)
            {
                Debug.LogWarning(shader);
            }

            // listを回してDictionaryに格納
            for (var i = 0; i < list.Length; ++i)
            {
                // Debug.LogWarning(list[i].name);
                var sprite = list[i] as Sprite;
                var mesh = MeshUtil.GenerateQuadMesh(sprite);
                var material = new Material(matShader);
                material.mainTexture = sprite.texture;
                m_meshDict.Add(list[i].name, mesh);
                m_materialDict.Add(list[i].name, material);

            }
        }

        public Material SetColor(string imageName, Color color)
        {
            Material mat = m_materialDict[imageName];
            mat.SetColor(m_colorPropertyId, color);
            return mat;
        }

        public bool Equals(MeshMatList obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }

}

