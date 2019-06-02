	using System.Collections.Generic;
	using System;
	using Unity.Entities;
	using UnityEngine;

	[Serializable]
	public struct MeshMatList : ISharedComponentData
	{
	    public Material material;
	    public Dictionary<string, Mesh> meshes;
	    public Dictionary<string, Sprite> sprites;

	    public MeshMatList(string path, string shader)
	    {
	        meshes = new Dictionary<string, Mesh>();
	        sprites = new Dictionary<string, Sprite>();
	        UnityEngine.Object[] spriteList = Resources.LoadAll(path, typeof(Sprite));

	        // listがnullまたは空ならエラーで返す
	        if (spriteList == null || spriteList.Length == 0)
	        {
	            Debug.LogWarning(path);
	        }
	        // マテリアル用シェーダー
	        var matShader = Shader.Find(shader);
	        material = new Material(matShader);

	        if (matShader == null)
	        {
	            Debug.LogWarning(shader);
	        }

	        // listを回してDictionaryに格納
	        for (var i = 0; i < spriteList.Length; ++i)
	        {
	            // Debug.Log(spriteList[i].name);
	            var sprite = spriteList[i] as Sprite;
	            sprites.Add(spriteList[i].name, sprite);

	            var mesh = GenerateQuadMesh(sprite);
	            Debug.Log(spriteList[i].name);
	            meshes.Add(spriteList[i].name, mesh);
	            if (i == 0)
	            {
	                material.mainTexture = sprite.texture;
	            }
	        }
	    }

	    Mesh GenerateQuadMesh(Sprite sprite)
	    {
	        Vector3[] _vertices = {
	            new Vector3(sprite.vertices[0].x, 0, sprite.vertices[0].y),
	            new Vector3(sprite.vertices[1].x, 0, sprite.vertices[1].y),
	            new Vector3(sprite.vertices[2].x, 0, sprite.vertices[2].y),
	            new Vector3(sprite.vertices[3].x, 0, sprite.vertices[3].y)
	        };

	        // Debug.Log(sprite.name);
	        // foreach (var item in sprite.vertices)
	        // {
	        //     Debug.Log(item);
	        // }
	        // foreach (var item in sprite.uv)
	        // {
	        //     Debug.Log(item);
	        // }

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
	            vertices = _vertices,
	                uv = sprite.uv,
	                triangles = triangles
	        };
	    }
	}
