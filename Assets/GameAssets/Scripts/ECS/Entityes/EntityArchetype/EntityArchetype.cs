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
            // typeof(PadScan), // 入力（CharaEntityFactoryで必要なキャラのみつける）
            typeof(ToukiMeter), // 闘気メーター
            // typeof(BgScroll), // 背景スクロール

            // typeof(CharaMove), // 座標
            // typeof(CharaMuki), // 向き
            // typeof(CharaDash), // ダッシュ
            // typeof(CharaLook), // 向き
            // typeof(CharaMotion), // モーション
        };
    }
}
