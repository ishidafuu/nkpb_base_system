using Unity.Entities;
namespace NKPB
{
    /// <summary>
    /// 画像の向きと表情（システム的な向きではない）
    /// </summary>
    public struct CharaLook : IComponentData
    {
        public int isLeft;
        public int isBack;
        public int faceNo;
    }
}
