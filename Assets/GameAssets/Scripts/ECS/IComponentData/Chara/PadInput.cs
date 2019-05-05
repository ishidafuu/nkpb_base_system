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
        public Button crossUp;
        public Button crossDown;
        public Button crossLeft;
        public Button crossRight;
        // ボタン
        public Button buttonA;
        public Button buttonB;
        public Button buttonX;
        public Button buttonY;
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
        public boolean isPush;
        // 押してる
        public boolean isPress;
        // 離した瞬間
        public boolean isPop;
        // 連打
        public boolean isDouble;
        // ダッシュ用直前押した瞬間時間
        public float lastPushTime;

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
