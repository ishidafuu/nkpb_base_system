using UnityEngine;
using System.Collections;

public class TileWall
{
	public int shapeNo;
	public int baseZ;
	public int stX;
	public int stY;
	public int palNo;
	public TileWall(int shapeNo, int palNo, int baseZ,  int stX, int stY)
	{
		this.shapeNo = shapeNo;
		this.palNo = palNo;
		this.baseZ = baseZ;
		this.stX = stX;
		this.stY = stY;
	}
}