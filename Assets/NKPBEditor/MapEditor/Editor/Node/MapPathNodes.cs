using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace NKPB
{
    [System.Serializable]
    public class MapPathNodes : ScriptableObject
    {
        //public List<MapPathNode> nodes = new List<MapPathNode>();
        public MapPathNode[] nodes;
        public int mapId;
    }

    //#if UNITY_EDITOR
    //[CanEditMultipleObjects]
    //[CustomEditor(typeof(MapPathNodes))]
    //public class MapPathNodesEditor : Editor
    //{
    //	//SerializedProperty nodes;
    //	MapPathNodes obj;
    //	void OnEnable()
    //	{
    //		//Character コンポーネントを取得
    //		obj = (MapPathNodes)target;
    //	}

    //	override public void OnInspectorGUI()
    //	{
    //		DrawDefaultInspector();
    //		if (target == null) return;
    //		serializedObject.Update();
    //		//FindSerializedProperties();
    //		DrawInspector();
    //		//serializedObject.ApplyModifiedProperties();
    //	}

    //	void FindSerializedProperties()
    //	{

    //	}

    //	void DrawInspector()
    //	{
    //		foreach (var item in obj.nodes)
    //		{
    //			EditorGUILayout.LabelField(item.plate.ToString());
    //		}
    //	}

    //}
    //#endif
}