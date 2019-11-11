using Unity.Entities;
using UnityEngine;

namespace NKPB
{
    public struct CharaFlag : IComponentData
    {
        public boolean m_mukiFlag;
        public FlagInputCheck m_inputCheckFlag;
        public FlagMove m_moveFlag;
        public FlagMotion m_motionFlag;
    }
}
