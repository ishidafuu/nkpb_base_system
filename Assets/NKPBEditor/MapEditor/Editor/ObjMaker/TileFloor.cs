using System.Collections;
using UnityEngine;

namespace NKPB
{
    public class TileFloor
    {
        public EnumShapeType shapeNo;
        public int baseY;
        public int stX;
        public int stZ;
        public int palNo;
        public TileFloor(EnumShapeType shapeNo, int palNo, int baseY, int stX, int stZ)
        {
            this.shapeNo = shapeNo;
            this.palNo = palNo;
            this.baseY = baseY;
            this.stX = stX;
            this.stZ = stZ;
        }
    }
}