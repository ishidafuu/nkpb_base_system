using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    /// <summary>
    /// キャラの座標移動量
    /// </summary>
    public struct CharaMove : IComponentData
    {
        public Vector3Int position;
        public Vector3Int delta;

        // XZ方向減速
        public void Friction(int _brakeDelta)
        {
            if (delta.x > 0)
            {
                delta.x = Mathf.Min(0, delta.x - _brakeDelta);
            }
            else if (delta.x < 0)
            {
                delta.x = Mathf.Max(0, delta.x + _brakeDelta);
            }

            if (delta.z > 0)
            {
                delta.z = Mathf.Min(0, delta.z - _brakeDelta);
            }
            else if (delta.x < 0)
            {
                delta.z = Mathf.Max(0, delta.z + _brakeDelta);
            }
        }

        public Vector3Int DeltaToVector3Int(int _delta, EnumMoveMuki _moveMuki)
        {
            Vector3 res = Vector3.zero;
            const float NANAME45 = 0.7f;
            const float NANAME30 = 0.5f;
            const float NANAME30L = 0.87f;
            switch (_moveMuki)
            {
                case EnumMoveMuki.Left:
                    res = new Vector3(-1, 0, 0);
                    break;
                case EnumMoveMuki.LeftLeftDown:
                    res = new Vector3(-NANAME30L, 0, -NANAME30);
                    break;
                case EnumMoveMuki.LeftDown:
                    res = new Vector3(-NANAME45, 0, -NANAME45);
                    break;
                case EnumMoveMuki.LeftLeftUp:
                    res = new Vector3(-NANAME30L, 0, +NANAME30);
                    break;
                case EnumMoveMuki.LeftUp:
                    res = new Vector3(-NANAME45, 0, +NANAME45);
                    break;
                case EnumMoveMuki.Right:
                    res = new Vector3(1, 0, 0);
                    break;
                case EnumMoveMuki.RightRightDown:
                    res = new Vector3(+NANAME30L, 0, -NANAME30);
                    break;
                case EnumMoveMuki.RightDown:
                    res = new Vector3(+NANAME45, 0, -NANAME45);
                    break;
                case EnumMoveMuki.RightRightUp:
                    res = new Vector3(+NANAME30L, 0, +NANAME30);
                    break;
                case EnumMoveMuki.RightUp:
                    res = new Vector3(+NANAME45, 0, +NANAME45);
                    break;
                case EnumMoveMuki.Up:
                    res = new Vector3(0, 0, 1);
                    break;
                case EnumMoveMuki.Down:
                    res = new Vector3(0, 0, -1);
                    break;
            }
            res *= _delta;
            return new Vector3Int((int)res.x, 0, (int)res.z);
        }

        public void SetDelta(int _delta, EnumMoveMuki _moveMuki)
        {
            delta = DeltaToVector3Int(_delta, _moveMuki);
            Debug.Log(delta.x);
        }

        //XZ方向停止
        public void Stop()
        {
            delta.x = 0;
            delta.z = 0;
        }

        public void Move()
        {
            position += delta;
        }

    }
}
