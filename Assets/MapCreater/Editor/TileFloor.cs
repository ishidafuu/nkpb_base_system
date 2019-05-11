using UnityEngine;
using System.Collections;

public class TileFloor
{
	public int shapeNo;
	public int baseY;
	public int stX;
	public int stZ;
	public int palNo;
	public TileFloor(int shapeNo, int palNo, int baseY,  int stX, int stZ)
	{
		this.shapeNo = shapeNo;
		this.palNo = palNo;
		this.baseY = baseY;
		this.stX = stX;
		this.stZ = stZ;
	}
}