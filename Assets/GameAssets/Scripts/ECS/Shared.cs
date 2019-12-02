namespace NKPB
{
    public static class Shared
    {
        public static SingleMeshMat m_mapMeshMat;
        public static MeshMatList m_charaMeshMat;
        public static CharaCellList m_charaCellList;
        public static CharaMotionList m_charaMotionList;
        public static MapTipList m_mapTipList;

        public static void ReadySharedComponentData()
        {
            m_charaMeshMat = new MeshMatList(PathSettings.CharaSprite, PathSettings.DefaultShader);
            m_mapMeshMat = new SingleMeshMat();
            m_mapMeshMat.Load(PathSettings.MapSprite, PathSettings.DefaultShader);

            m_charaCellList = new CharaCellList();
            m_charaCellList.Init();

            m_charaMotionList = new CharaMotionList();
            m_charaMotionList.Init();

            m_mapTipList = new MapTipList();
            m_mapTipList.Init();
        }

    }
}
