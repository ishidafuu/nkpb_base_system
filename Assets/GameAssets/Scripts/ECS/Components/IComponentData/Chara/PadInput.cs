using Unity.Entities;
using UnityEngine;
namespace NKPB
{
    /// <summary>
    /// キー入力
    /// </summary>
    public struct PadScan : IComponentData
    {
        // 十字のバッファ
        Vector2 axis;
        // 十字
        public Button crossUp { get; private set; }
        public Button crossDown { get; private set; }
        public Button crossLeft { get; private set; }
        public Button crossRight { get; private set; }
        // ボタン
        public Button buttonA { get; private set; }
        public Button buttonB { get; private set; }
        public Button buttonX { get; private set; }
        public Button buttonY { get; private set; }
        public void SetCross(Vector2 _axis, float _time)
        {
            // 直前キーから変更無ければ処理しない
            if (_axis != axis)
            {
                var isUp = (_axis.y > +0.1f);
                var isDown = (_axis.y < -0.1f);
                var isRight = (_axis.x > +0.1f);
                var isLeft = (_axis.x < -0.1f);

                axis = _axis;
                crossUp.SetCrossData(isUp, Time.time);
                crossDown.SetCrossData(isDown, Time.time);
                crossRight.SetCrossData(isRight, Time.time);
                crossLeft.SetCrossData(isLeft, Time.time);
                // Debug.Log(axis);
            }
        }

        public EnumCrossType GetPressCross()
        {
            if (crossUp.isPress)
                return EnumCrossType.Up;

            if (crossDown.isPress)
                return EnumCrossType.Down;

            if (crossLeft.isPress)
                return EnumCrossType.Left;

            if (crossRight.isPress)
                return EnumCrossType.Right;

            return EnumCrossType.None;
        }

        public EnumButtonType GetPressButton()
        {
            if (buttonA.isPress)
                return EnumButtonType.A;

            if (buttonB.isPress)
                return EnumButtonType.B;

            if (buttonX.isPress)
                return EnumButtonType.X;

            if (buttonY.isPress)
                return EnumButtonType.Y;

            return EnumButtonType.None;
        }

        /// <summary>
        /// どれか十字が押されてる
        /// </summary>
        /// <returns></returns>
        public bool IsAnyCrossPress()
        {
            return (crossUp.isPress || crossDown.isPress || crossLeft.isPress || crossRight.isPress);
        }
        /// <summary>
        /// ジャンプ入力
        /// </summary>
        /// <returns></returns>
        public bool IsJumpPush()
        {
            return ((buttonA.isPress && buttonB.isPush)
                || (buttonA.isPush && buttonB.isPress));
        }

    }

    public struct Button
    {
        // 連打受付時間
        const float DOUBLE_TIME = 0.4f;
        // 押した瞬間
        public boolean isPush { get; private set; }
        // 押してる
        public boolean isPress { get; private set; }
        // 離した瞬間
        public boolean isPop { get; private set; }
        // 連打
        public boolean isDouble { get; private set; }
        // ダッシュ用直前押した瞬間時間
        public float lastPushTime { get; private set; }

        public void SetButtonData(bool _isPush, bool _isPress, bool _isPop, float _time)
        {
            isPush = _isPush;
            isPress = _isPress;
            isPop = _isPop;
            isDouble = (_isPush && ((_time - lastPushTime) < DOUBLE_TIME));
            if (_isPush)
                lastPushTime = _time;

        }

        public void SetCrossData(bool _isPress, float _time)
        {
            isPush = (!isPress && _isPress);
            isPress = (_isPress);
            isPop = (isPress && !_isPress);
            isDouble = (isPush && ((_time - lastPushTime) < DOUBLE_TIME));
            if (isPush)
                lastPushTime = _time;

            // if (isPress)
            // 	Debug.Log("isPress");
        }
    }
}
