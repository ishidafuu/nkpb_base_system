using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;

[System.Serializable]
public class MapPathConnection
{
	public int nodeNo;
	public float cost;
	public int distX;
	public int distY;
	public int distZ;
	public int connectType;

	public MapPathConnection(int nodeNo, float cost, int distX, int distY, int distZ)
	{
		this.nodeNo = nodeNo;
		this.cost = cost;
		this.distX = distX;
		this.distY = distY;
		this.distZ = distZ;
	}
}



