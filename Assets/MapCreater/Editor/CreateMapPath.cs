using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CreateMapPath : Editor
{

    static public void CreateMapPathObject(int mapId, List<MapNodePlate> pathList)
    {
        //パスリストからすべてのノードを作成
        List<MapPathNode> nodeList = new List<MapPathNode>();
        foreach (var item in pathList.Select((v, i) => new { v, i }))
        {
            MapPathNode node = new MapPathNode(item.i, item.v);
            nodeList.Add(node);
        }

        //隣接ノードチェック
        foreach (var item in nodeList)
        {
            foreach (var target in nodeList)
            {
                //自分自身以外
                if (item.nodeNo == target.nodeNo)continue;
                //接続できるノードかチェック
                if (!item.plate.IsSideBySide(target.plate))continue;

                //Debug.Log("MapPathNode.enConnectType.Walk");
                //歩きで接続
                item.AddConnectedNode(target, MapPathNode.enConnectType.Walk);
            }
        }

        //ジャンプノードチェック
        foreach (var item in nodeList)
        {
            //崖ノードかチェック
            if (!item.IsCliff())continue;

            foreach (var target in nodeList)
            {
                //自分自身以外
                if (item.nodeNo == target.nodeNo)continue;

                //既に登録済みノード
                if (item.IsContainsNodeNo(target.nodeNo))continue;

                //地続きのノードもジャンプ可能に入ってしまっているので、
                //それを排除する

                //ジャンプできるノードかチェック
                if (!item.plate.IsJumpable(target.plate))continue;

                //ジャンプで接続
                item.AddConnectedNode(target, MapPathNode.enConnectType.Jump);
            }
        }

        //スクリプタブルオブジェクトにすべて追加
        MapPathNodes pathNodes = CreateInstance<MapPathNodes>();
        pathNodes.mapId = mapId;
        pathNodes.nodes = nodeList.ToArray();
        //foreach (var item in nodeList) pathNodes.nodes.Add(item);
        string filepath = "Assets/Resources/MapPath_" + mapId.ToString("d3") + ".asset";
        AssetDatabase.CreateAsset(pathNodes, filepath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"CreateAsset:{filepath}");
    }

}
