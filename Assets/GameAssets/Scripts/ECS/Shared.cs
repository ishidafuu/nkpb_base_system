﻿using System.Collections.Generic;
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
		// SharedComponentData
		public static MeshMatList charaMeshMat;
		public static AniScriptSheet aniScriptSheet;
		public static AniBasePos aniBasePos;
		public static MeshMatList bgFrameMeshMat;
		// public static MeshMatList meterMeshMat;

		static readonly string ShaderBg = "Sprites/DefaultSprite";

		// SharedComponentDataの読み込み
		public static void ReadySharedComponentData()
		{
			bgFrameMeshMat = new MeshMatList("yyhs/bg/bg", ShaderBg);
			// meterMeshMat = new MeshMatList("yyhs/bg/meter", ShaderBg);

			// // スクリプタブルオブジェクトの読み込み
			// aniScriptSheet = new AniScriptSheet();
			// if (Resources.FindObjectsOfTypeAll<AniScriptSheetObject>().Length == 0)
			// 	Debug.LogError("aniScriptSheet 0");
			// aniScriptSheet.scripts = (Resources.FindObjectsOfTypeAll<AniScriptSheetObject>().First()as AniScriptSheetObject).scripts;

			// aniBasePos = new AniBasePos();
			// if (Resources.FindObjectsOfTypeAll<AniBasePosObject>().Length == 0)
			// 	Debug.LogError("aniBasePos 0");
			// aniBasePos = (Resources.FindObjectsOfTypeAll<AniBasePosObject>().First()as AniBasePosObject).aniBasePos;
		}

	}
}
