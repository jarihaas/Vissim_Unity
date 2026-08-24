using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ResetFPCam : MonoBehaviour
{
  public void ResetCam()
  {
	Vector3 carPos = GameObject.Find("SkyCar").transform.position;
		Vector3 headsetOffset = InputTracking.GetLocalPosition(XRNode.Head);
		Vector3 driverPosition = new Vector3((carPos.x - 0.4f) - headsetOffset.x, (carPos.y + 1.2f) - headsetOffset.y, (carPos.z - 0.19f) - headsetOffset.z);
		gameObject.transform.position = driverPosition;

  }
}
