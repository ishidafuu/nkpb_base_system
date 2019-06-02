using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace NKPB
{
    [Serializable]
    public class CharaCell
    {
        public string ID;
        public int faceNo;
        public int faceX;
        public int faceY;
        public int faceZ;
        public int faceAngle;
        public int ballX;
        public int ballY;
        public int ballZ;
        public int partsNo;
        public int partsX;
        public int partsY;

        public CharaCell ShallowCopy()
        {
            return (CharaCell)this.MemberwiseClone();
        }

        public static string ReplaceHyphen(string name)
        {
            return name.Replace("-", "_");
        }
    }

}
