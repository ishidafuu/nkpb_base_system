using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace NKPB
{
	[ExecuteInEditMode]
	public class AniScriptManager : SingletonMonoBehaviour<AniScriptManager>
		{
			public AniScriptSheetObject aniScriptSheet;
			public AniBasePosObject aniBasePos;

			public void LoadObject()
			{
				// オートセット
				aniScriptSheet = Resources.FindObjectsOfTypeAll<AniScriptSheetObject>().First()as AniScriptSheetObject;
				aniBasePos = Resources.FindObjectsOfTypeAll<AniBasePosObject>().First()as AniBasePosObject;
			}

		}

	[CustomEditor(typeof(AniScriptManager))] // 拡張するクラスを指定
	public class AniScriptManagerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// 元のInspector部分を表示
			base.OnInspectorGUI();

			// ボタンを表示
			if (GUILayout.Button("LoadObject"))
			{
				(target as AniScriptManager).LoadObject();
			}
		}

	}
}
