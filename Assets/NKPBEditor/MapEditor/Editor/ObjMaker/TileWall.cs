using System.Collections;
using UnityEngine;

namespace NKPB
{
    public class TileWall
    {
        public EnumShapeType shapeNo;
        public int baseZ;
        public int stX;
        public int stY;
        public int palNo;
        public TileWall(EnumShapeType shapeNo, int palNo, int baseZ, int stX, int stY)
        {
            this.shapeNo = shapeNo;
            this.palNo = palNo;
            this.baseZ = baseZ;
            this.stX = stX;
            this.stY = stY;
        }
    }
}