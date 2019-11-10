using System.Collections;
using UnityEngine;

namespace NKPB
{
    [System.Serializable]
    public class MapNodePlate
    {

        enum enAngle
        {
            Same,
            Back,
            BackRight,
            Right,
            FrontRight,
            Front,
            FrontLeft,
            Left,
            BackLeft,
        }

        public EnumShapeType shapeType;

        public int stY;
        public int stX;
        public int stZ;
        public enMapEnd mapEnd;
        //public int sameW;
        //public int sameD;
        public MapNodePlate(EnumShapeType shapeType, int stY, int stX, int stZ, int mapW, int mapD) //, int sameW, int sameD)
        {
            this.shapeType = shapeType;
            this.stY = stY;
            this.stX = stX;
            this.stZ = stZ;
            this.mapEnd = enMapEnd.None;
            if (stX == 0)
            {
                if (stZ == 0) mapEnd |= enMapEnd.FrontLeft;
                else if (stZ == (mapD - 1)) mapEnd |= enMapEnd.BackLeft;
                else mapEnd |= enMapEnd.Left;
            }
            else if (stX == (mapW - 1))
            {
                if (stZ == 0) mapEnd |= enMapEnd.FrontRight;
                else if (stZ == (mapD - 1)) mapEnd |= enMapEnd.BackRight;
                else mapEnd |= enMapEnd.Right;
            }
            else
            {
                if (stZ == 0) mapEnd |= enMapEnd.Front;
                else if (stZ == (mapD - 1)) mapEnd |= enMapEnd.Back;
                else mapEnd |= enMapEnd.None;
            }
            //this.sameW = sameW;
            //this.sameD = sameD;
        }

        //public int EndX()
        //{
        //	return stX + sameW;
        //}

        //public int EndZ()
        //{
        //	return stZ + sameD;
        //}

        override public string ToString()
        {
            return "tipType:" + shapeType + " "
                + "X:" + stX + " "
                + "Y:" + stY + " "
                + "Z:" + stZ + " "
            //+ "W:" + sameW + " "
            //+ "D:" + sameD
            ;
        }

        public bool IsSideBySide(MapNodePlate target)
        {
            //距離が隣ではない
            if (Mathf.Abs(this.stX - target.stX) > 1) return false;
            if (Mathf.Abs(this.stZ - target.stZ) > 1) return false;

            //高さチェック
            bool isWalkable = IsWalkable(target);

            return isWalkable;
        }

        public bool IsJumpable(MapNodePlate target)
        {
            //距離が隣ではない
            int distX = Mathf.Abs(this.stX - target.stX);
            int distZ = Mathf.Abs(this.stZ - target.stZ);
            const int MAXJUMPDISTXZ = 3;
            const int MAXJUMPDISTXZ_SQ = MAXJUMPDISTXZ * MAXJUMPDISTXZ;

            //ジャンプで届く距離
            if (((distX * distX) + (distZ * distZ)) > MAXJUMPDISTXZ_SQ) return false;

            //高すぎチェック
            const int MAXDISTH = 3;
            if (this.stY < (target.stY - MAXDISTH)) return false;

            return true;
        }

        //位置関係から角度
        enAngle GetAngle(MapNodePlate target)
        {
            enAngle res = enAngle.Same;

            if (this.stX < target.stX)
            {
                if (this.stZ < target.stZ) res = enAngle.BackRight;
                else if (this.stZ > target.stZ) res = enAngle.FrontRight;
                else res = enAngle.Right;
            }
            else if (this.stX > target.stX)
            {
                if (this.stZ < target.stZ) res = enAngle.BackLeft;
                else if (this.stZ > target.stZ) res = enAngle.FrontLeft;
                else res = enAngle.Left;
            }
            else
            {
                if (this.stZ < target.stZ) res = enAngle.Back;
                else if (this.stZ > target.stZ) res = enAngle.Front;
                else res = enAngle.Same;
            }
            return res;
        }

        //逆方向
        enAngle RevAngle(enAngle angle)
        {
            enAngle res = angle;
            switch (angle)
            {
                case enAngle.Back:
                    res = enAngle.Front;
                    break;
                case enAngle.BackRight:
                    res = enAngle.FrontLeft;
                    break;
                case enAngle.Right:
                    res = enAngle.Left;
                    break;
                case enAngle.FrontRight:
                    res = enAngle.BackLeft;
                    break;
                case enAngle.Front:
                    res = enAngle.Back;
                    break;
                case enAngle.FrontLeft:
                    res = enAngle.BackRight;
                    break;
                case enAngle.Left:
                    res = enAngle.Right;
                    break;
                case enAngle.BackLeft:
                    res = enAngle.FrontRight;
                    break;
            }
            return res;
        }

        //同じ高さかチェック
        bool IsWalkable(MapNodePlate target)
        {
            enAngle angle = GetAngle(target);
            enAngle tagAngle = RevAngle(angle);
            float myY = this.stY + GetAngleHeight((EnumShapeType)this.shapeType, angle);
            float tagY = target.stY + GetAngleHeight((EnumShapeType)target.shapeType, tagAngle);

            //Debug.Log("myY"+ myY + "tagY" + tagY);

            return (myY == tagY);
        }

        //チップの向きごとの高さ
        static float GetAngleHeight(EnumShapeType tipType, enAngle angle)
        {
            float res = 0f;
            switch (tipType)
            {
                case EnumShapeType.Box:
                    res = 1f;
                    break;
                case EnumShapeType.LUpSlope:
                    switch (angle)
                    {
                        case enAngle.Back:
                        case enAngle.Front:
                            res = 0.5f;
                            break;
                        case enAngle.BackRight:
                        case enAngle.Right:
                        case enAngle.FrontRight:
                            res = 0f;
                            break;
                        case enAngle.FrontLeft:
                        case enAngle.Left:
                        case enAngle.BackLeft:
                            res = 1f;
                            break;
                    }
                    break;
                case EnumShapeType.RUpSlope:
                    switch (angle)
                    {
                        case enAngle.Back:
                        case enAngle.Front:
                            res = 0.5f;
                            break;
                        case enAngle.BackRight:
                        case enAngle.Right:
                        case enAngle.FrontRight:
                            res = 1f;
                            break;
                        case enAngle.FrontLeft:
                        case enAngle.Left:
                        case enAngle.BackLeft:
                            res = 0f;
                            break;
                    }
                    break;
                case EnumShapeType.LUpSlope2H:
                    switch (angle)
                    {
                        case enAngle.Back:
                        case enAngle.Front:
                            res = 0.75f;
                            break;
                        case enAngle.BackRight:
                        case enAngle.Right:
                        case enAngle.FrontRight:
                            res = 0.5f;
                            break;
                        case enAngle.FrontLeft:
                        case enAngle.Left:
                        case enAngle.BackLeft:
                            res = 1f;
                            break;
                    }
                    break;
                case EnumShapeType.LUpSlope2L:
                    switch (angle)
                    {
                        case enAngle.Back:
                        case enAngle.Front:
                            res = 0.25f;
                            break;
                        case enAngle.BackRight:
                        case enAngle.Right:
                        case enAngle.FrontRight:
                            res = 0f;
                            break;
                        case enAngle.FrontLeft:
                        case enAngle.Left:
                        case enAngle.BackLeft:
                            res = 0.5f;
                            break;
                    }
                    break;
                case EnumShapeType.RUpSlope2L:
                    switch (angle)
                    {
                        case enAngle.Back:
                        case enAngle.Front:
                            res = 0.25f;
                            break;
                        case enAngle.BackRight:
                        case enAngle.Right:
                        case enAngle.FrontRight:
                            res = 0.5f;
                            break;
                        case enAngle.FrontLeft:
                        case enAngle.Left:
                        case enAngle.BackLeft:
                            res = 0f;
                            break;
                    }
                    break;
                case EnumShapeType.RUpSlope2H:
                    switch (angle)
                    {
                        case enAngle.Back:
                        case enAngle.Front:
                            res = 0.75f;
                            break;
                        case enAngle.BackRight:
                        case enAngle.Right:
                        case enAngle.FrontRight:
                            res = 1f;
                            break;
                        case enAngle.FrontLeft:
                        case enAngle.Left:
                        case enAngle.BackLeft:
                            res = 0.5f;
                            break;
                    }
                    break;
                case EnumShapeType.SlashWall:
                    switch (angle)
                    {
                        case enAngle.Back:
                        case enAngle.BackLeft:
                        case enAngle.BackRight:
                        case enAngle.FrontLeft:
                        case enAngle.Left:
                            res = 1f;
                            break;
                        case enAngle.Right:
                        case enAngle.FrontRight:
                        case enAngle.Front:
                            res = 0f;
                            break;
                    }
                    break;
                case EnumShapeType.BSlashWall:
                    switch (angle)
                    {
                        case enAngle.Back:
                        case enAngle.BackLeft:
                        case enAngle.BackRight:
                        case enAngle.Right:
                        case enAngle.FrontRight:
                            res = 1f;
                            break;
                        case enAngle.FrontLeft:
                        case enAngle.Left:
                        case enAngle.Front:
                            res = 0f;
                            break;
                    }
                    break;
            }

            return res;
        }
    }
}