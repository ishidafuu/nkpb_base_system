using Unity.Entities;
namespace NKPB
{
    public struct CharaLook : IComponentData
    {
        public boolean m_isLeft;
        public int m_faceNo;
    }
}
