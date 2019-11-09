using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

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
            var mesh = GenerateQuadMesh(sprite);
            var material = new Material(matShader);
            material.mainTexture = sprite.texture;
            m_meshDict.Add(list[i].name, mesh);
            m_materialDict.Add(list[i].name, material);

        }
    }

    Mesh GenerateQuadMesh(Sprite sprite)
    {
        List<Vector3> _vertices = new List<Vector3>();
        for (int i = 0; i < sprite.uv.Length; i++)
        {
            _vertices.Add(new Vector3(sprite.vertices[i].x, 0, sprite.vertices[i].y));
        }

        int[] triangles = {
                sprite.triangles[0],
                sprite.triangles[1],
                sprite.triangles[2],
                sprite.triangles[3],
                sprite.triangles[4],
                sprite.triangles[5]
            };

        return new Mesh
        {
            vertices = _vertices.ToArray(),
            uv = sprite.uv,
            triangles = triangles
        };
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
