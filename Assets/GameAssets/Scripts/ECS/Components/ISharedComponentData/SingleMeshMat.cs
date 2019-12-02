using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    [Serializable]
    public struct SingleMeshMat : IEquatable<SingleMeshMat>, ISharedComponentData
    {
        public Material m_material;
        public Mesh m_mesh;
        int m_colorPropertyId;

        public int m_sizeX;
        public int m_sizeY;
        public int m_sizeXHalf;
        public int m_sizeYHalf;

        public void Load(string path, string shader)
        {
            m_material = null;
            m_mesh = null;
            UnityEngine.Object obj = Resources.Load(path, typeof(Sprite));

            // マテリアル用シェーダー
            var matShader = Shader.Find(shader);
            m_colorPropertyId = Shader.PropertyToID("_Color");

            if (matShader == null)
            {
                Debug.LogWarning(shader);
            }

            m_material = new Material(matShader);
            var sprite = obj as Sprite;
            m_mesh = MeshUtil.GenerateQuadMesh(sprite);
            m_material.mainTexture = sprite.texture;

            m_sizeX = (int)m_mesh.bounds.size.x;
            m_sizeXHalf = m_sizeX / 2;
            m_sizeY = (int)m_mesh.bounds.size.z;
            m_sizeYHalf = m_sizeY / 2;
        }


        public Material SetColor(string imageName, Color color)
        {
            m_material.SetColor(m_colorPropertyId, color);
            return m_material;
        }

        public bool Equals(SingleMeshMat obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }

}

