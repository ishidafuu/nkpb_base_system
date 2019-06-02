using Unity.Entities;
using Unity.Transforms;
namespace NKPB
{
    public static class ComponentTypes
    {
        /// <summary>
        /// キャラ
        /// </summary>
        /// <value></value>
        public static ComponentType[] CharaComponentType = {
            typeof(CharaTag), // キャラタグ
            typeof(CharaId), // ID
            typeof(Position), // 座標
            typeof(CharaMove), // 座標移動
            typeof(CharaMuki), // 向き
            typeof(CharaDash), // ダッシュ
            typeof(CharaLook), // 向き
            typeof(CharaMotion), // モーション
            // typeof(PadScan), // 入力（ここでは付けずCharaEntityFactoryで必要なキャラのみつける）
        };
    }
}
