using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace NKPB
{
    [System.Serializable]
    public class MapPathNode : IAStarNode
    {
        public enum enConnectType
        {
            Walk,
            Jump,
            Warp,
        }

        public int nodeNo;
        public MapNodePlate plate;
        public Dictionary<int, MapPathNode> connectedNode = new Dictionary<int, MapPathNode>();
        public Dictionary<int, enConnectType> connectType = new Dictionary<int, enConnectType>();
        public int walkableNodeNum;
        public int jumpableNodeNum;
        public MapPathNode(int nodeNo, MapNodePlate plate)
        {
            this.plate = plate;
            this.nodeNo = nodeNo;

        }

        public void AddConnectedNode(MapPathNode node, enConnectType connect)
        {
            connectedNode[node.nodeNo] = node;
            connectType[node.nodeNo] = connect;
            walkableNodeNum = connectType.Values
                .Where(v => (v == enConnectType.Walk))
                .Count();
            jumpableNodeNum = connectType.Values
                .Where(v => (v == enConnectType.Jump))
                .Count();
        }

        //崖ノード
        public bool IsCliff()
        {
            int walkableNum = connectType.Values
                .Where(v => (v == enConnectType.Walk))
                .Count();

            //画面端の場合は崖判断のエッジ数を減らす
            int edgeNum = 8;
            switch (plate.mapEnd)
            {
                case enMapEnd.None:
                    break;
                case enMapEnd.Back:
                case enMapEnd.Right:
                case enMapEnd.Front:
                case enMapEnd.Left:
                    edgeNum = 5;
                    break;
                case enMapEnd.BackRight:
                case enMapEnd.FrontRight:
                case enMapEnd.FrontLeft:
                case enMapEnd.BackLeft:
                    edgeNum = 3;
                    break;
            }

            return (walkableNum < edgeNum); //歩きノードではない方向がある
        }

        public bool IsContainsNodeNo(int nodeNo)
        {
            return connectedNode.ContainsKey(nodeNo);
        }

        public IEnumerable<IAStarNode> GetConnectedNodes()
        {
            MapPathNode[] values = new MapPathNode[connectedNode.Values.Count];
            connectedNode.Values.CopyTo(values, 0);
            return values;
        }

        public float CalculateMoveCost(IAStarNode node)
        {
            //接続タイプによってはNGを出す
            //MapPathNode node2 = node as MapPathNode;

            return 1;

        }

        ////接続できるノードかチェック
        //public static bool IsConnect(MapNodePlate node, MapNodePlate targetNode)
        //{
        //	//同一プレート
        //	if ((node.stX == targetNode.stX)
        //		&& (node.stZ == targetNode.stZ)) return false;

        //	//接続とみなす距離
        //	const float FARX = 2;
        //	const float FARZ = 2;
        //	const float FARY = 2;

        //	//左すぎ
        //	if ((targetNode.EndX() + FARX) < node.stX) return false;
        //	//右すぎ
        //	if ((targetNode.stX - FARX) > node.EndX()) return false;

        //	//手前すぎ
        //	if ((targetNode.EndZ() + FARZ) < node.stZ) return false;
        //	//奥すぎ
        //	if ((targetNode.stZ - FARZ) > node.EndZ()) return false;

        //	//高すぎ
        //	if ((targetNode.stY - FARY) > node.stY) return false;

        //	return true;
        //}

        ////
        //public static MapPathConnection GetConnection(MapNodePlate node, MapNodePlate targetNode, int targetNodeNo)
        //{
        //	//float cost = 0;
        //	int distX = 0;
        //	int distY = 0;
        //	int distZ = 0;

        //	//X
        //	if ((node.EndX() == targetNode.stX) || (node.stX == targetNode.EndX()))//隣接
        //	{
        //		distX = 1;//隣接を距離１にする
        //	}
        //	else if ((node.EndX() >= targetNode.stX) && (node.stX <= targetNode.EndX()))
        //	{
        //		//Z側で接続している場合はX側は重なるのでX距離は０
        //		distX = 0;
        //	}
        //	else if (node.EndX() < targetNode.stX)//タゲが右
        //	{
        //		distX = (targetNode.stX - node.EndX()) + 1;
        //	}
        //	else if (node.stX > targetNode.EndX())//タゲが左
        //	{
        //		distX = (targetNode.EndX() - node.stX) - 1;
        //	}

        //	//Z
        //	if ((node.EndZ() == targetNode.stZ) || (node.stZ == targetNode.EndZ()))//隣接
        //	{
        //		distZ = 1;
        //	}
        //	else if ((node.EndZ() >= targetNode.stZ) && (node.stZ <= targetNode.EndZ()))
        //	{
        //		distZ = 0;
        //	}
        //	else if (node.EndZ() < targetNode.stZ)//タゲが奥
        //	{
        //		distZ = (targetNode.stZ - node.EndZ()) + 1;
        //	}
        //	else if (node.stZ > targetNode.EndZ())//タゲが手前
        //	{
        //		distZ = (targetNode.EndZ() - node.stZ) - 1;
        //	}

        //	//Y
        //	distY = targetNode.stY - node.stY;

        //	//移動コスト
        //	float cost = Mathf.Max(Mathf.Abs(distX), Mathf.Abs(distZ));

        //	return new MapPathConnection(targetNodeNo, cost, distX, distY, distZ);
        //}

    }
}