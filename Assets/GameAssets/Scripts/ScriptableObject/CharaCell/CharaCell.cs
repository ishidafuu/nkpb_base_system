using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace NKPB
{
    [System.Serializable]
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

        public static string ReplaceHyphen(string name)
        {
            return name.Replace("-", "_");
        }

        public static string OutputCSVLineHeader()
        {
            Type t = typeof(CharaCell);
            //フィールドの情報
            FieldInfo[] fields = t.GetFields();
            string res = "";
            bool isFirst = true;
            foreach (FieldInfo field in fields)
            {
                if (!isFirst)res += ",";
                res += field.Name;
                isFirst = false;
            }

            return res;
        }
        public static string OutputCSVLineType()
        {
            Type t = typeof(CharaCell);
            //フィールドの情報
            FieldInfo[] fields = t.GetFields();
            string res = "";
            bool isFirst = true;

            foreach (FieldInfo field in fields)
            {
                if (!isFirst)res += ",";
                Type ftype = field.FieldType;
                if (field.Name == "ID")
                {
                    res += "TYPE";
                }
                else if (ftype == typeof(string))
                {
                    res += "string";
                }
                else if (ftype == typeof(int))
                {
                    res += "int";
                }
                isFirst = false;
            }

            return res;
        }

        public string OutputCSVLine()
        {
            Type t = typeof(CharaCell);
            //フィールドの情報
            FieldInfo[] fields = t.GetFields();
            string res = "";
            bool isFirst = true;

            foreach (FieldInfo field in fields)
            {
                if (!isFirst)res += ",";
                res += field.GetValue(this).ToString();
                isFirst = false;
            }

            return res;
        }

        public void InputCSVLine(string[] spritLine)
        {
            // 読み込んだ一行をカンマ毎に分けて配列に格納する

            Type t = typeof(CharaCell);
            //フィールドの情報
            FieldInfo[] fields = t.GetFields();
            string res = "";
            int index = 0;
            //Debug.Log(spritLine);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(string))
                {
                    field.SetValue(this, (spritLine[index]));
                }
                else
                {
                    field.SetValue(this, int.Parse(spritLine[index]));
                }

                //Debug.Log(field.Name + spritLine[index].ToString());

                index++;
            }

        }
    }

}
