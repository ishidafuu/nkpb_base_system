using System;

[Serializable]
public struct AniDepth
{
	public float dAnt;
	public float dHead;
	public float dThorax;
	public float dGaster;
	public float dLeftArm;
	public float dRightArm;
	public float dLeftLeg;
	public float dRightLeg;

	public void SetData(float[] data)
	{
		dAnt = data[0];
		dHead = data[1];
		dThorax = data[2];
		dGaster = data[3];
		dLeftArm = data[4];
		dRightArm = data[5];
		dLeftLeg = data[6];
		dRightLeg = data[7];
	}
	public float GetData(int index)
	{
		float res = 0;
		switch (index)
		{
			case 0:
				res = dAnt;
				break;
			case 1:
				res = dHead;
				break;
			case 2:
				res = dThorax;
				break;
			case 3:
				res = dGaster;
				break;
			case 4:
				res = dLeftArm;
				break;
			case 5:
				res = dRightArm;
				break;
			case 6:
				res = dLeftLeg;
				break;
			case 7:
				res = dRightLeg;
				break;
		}
		return res;
	}
}