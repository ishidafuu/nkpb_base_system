using Unity.Entities;
namespace NKPB
{
    public struct CharaLook : IComponentData
    {
        public int isLeft;
        public int isBack;
        public int faceNo;
    }
}
