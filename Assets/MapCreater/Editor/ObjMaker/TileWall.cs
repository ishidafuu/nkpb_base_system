using System.Collections;
using UnityEngine;

public class TileWall
{
    public enShapeType shapeNo;
    public int baseZ;
    public int stX;
    public int stY;
    public int palNo;
    public TileWall(enShapeType shapeNo, int palNo, int baseZ, int stX, int stY)
    {
        this.shapeNo = shapeNo;
        this.palNo = palNo;
        this.baseZ = baseZ;
        this.stX = stX;
        this.stY = stY;
    }
}
