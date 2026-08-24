using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PTV.Vision.Interfaces;

public partial class VissimInterface : MonoBehaviour {
  // adjust our local pedestrian states according to the data retrieved from Vissim
  private void Update_Local_Pedestrian_Data(VISSIM_Ped_Data[] VissimPedData) {
    foreach (VISSIM_Ped_Data pedData in VissimPedData) {
      int ID = pedData.PedestrianID;
      if (!exchangeData.PedestrianData.ContainsKey(ID)) {
        exchangeData.PedestrianData[ID] = new VISSIMPedestrianData();
        exchangeData.PedestrianData[ID].PedestrianType = pedData.PedestrianType;
        exchangeData.PedestrianData[ID].ModelFileName = pedData.ModelFileName;
        exchangeData.PedestrianData[ID].Length = pedData.Length;
        exchangeData.PedestrianData[ID].Width = pedData.Width;
        exchangeData.PedestrianData[ID].Height = pedData.Height;
      }
      exchangeData.PedestrianData[ID].PositionX = pedData.Position_X;
      exchangeData.PedestrianData[ID].PositionY = pedData.Position_Y;
      exchangeData.PedestrianData[ID].PositionZ = pedData.Position_Z;
      exchangeData.PedestrianData[ID].OrientHeading = pedData.Orient_Heading;
      exchangeData.PedestrianData[ID].DistanceSinceBirth = pedData.DistanceSinceBirth;
      exchangeData.PedestrianData[ID].Speed = pedData.Speed;
      exchangeData.PedestrianData[ID].IsWalking =
           pedData.MotionState != Pedestrian_Motion_State_Type.Motion_State_Type_WaitingForPTVehicle
        && pedData.MotionState != Pedestrian_Motion_State_Type.Motion_State_Type_StandingOnEscalator
        && pedData.MotionState != Pedestrian_Motion_State_Type.Motion_State_Type_StandingOnMovingWalkway
        && pedData.MotionState != Pedestrian_Motion_State_Type.Motion_State_Type_WaitingAtQueueHead
        && pedData.MotionState != Pedestrian_Motion_State_Type.Motion_State_Type_WaitingForElevator
        && pedData.MotionState != Pedestrian_Motion_State_Type.Motion_State_Type_RidingElevator
        && pedData.MotionState != Pedestrian_Motion_State_Type.Motion_State_Type_Waiting;
      exchangeData.PedestrianData[ID].IsInVissim = true;
    }
  }

  private void Remove_Left_Pedestrians_From_Data() {
    foreach (var pd in exchangeData.PedestrianData.Where(pd => !pd.Value.IsInVissim).ToList()) {
      exchangeData.PedestrianData.Remove(pd.Key);
    }
  }

  //==============================================
  //  update our own copies of Vissims pedestrians
  //  and add newly created pedestrians
  //==============================================
  private void Update_Local_Pedestrians(float factor) {
    foreach (KeyValuePair<int, VISSIMPedestrianData> pedData in exchangeData.PedestrianData) {
      pedData.Value.IsInVissim = false;
      if (currentPedestrians.ContainsKey(pedData.Key)) { // moved ped
        GameObject thePedestrian = currentPedestrians[pedData.Key];
        Vector3 newPos = new Vector3((float)exchangeData.PedestrianData[pedData.Key].PositionX, (float)exchangeData.PedestrianData[pedData.Key].PositionZ, (float)exchangeData.PedestrianData[pedData.Key].PositionY);
        thePedestrian.transform.localPosition = newPos + (newPos - thePedestrian.transform.localPosition) * factor;
        rotateObject(thePedestrian, (float)exchangeData.PedestrianData[pedData.Key].OrientHeading - (Mathf.Deg2Rad * 90.0f), 0);
        Rigidbody pedRb = thePedestrian.GetComponent<Rigidbody>();
        if (pedRb != null)
          pedRb.velocity = thePedestrian.transform.TransformDirection(Vector3.right * (float)pedData.Value.Speed);
        Animator pedAnimator = thePedestrian.GetComponent<Animator>();
        if (pedAnimator != null)
          pedAnimator.SetFloat("Speed", (float) exchangeData.PedestrianData[pedData.Key].Speed);
      }
      else { // new ped
        //  spawn pedestrians that were newly inserted by Vissim
        GameObject thePedestrian = manPool.GetPoolItem();
        thePedestrian.transform.localPosition = new Vector3((float)exchangeData.PedestrianData[pedData.Key].PositionX, (float)exchangeData.PedestrianData[pedData.Key].PositionZ, (float)exchangeData.PedestrianData[pedData.Key].PositionY);
        rotateObject(thePedestrian, (float)exchangeData.PedestrianData[pedData.Key].OrientHeading - (Mathf.Deg2Rad * -90.0f), 0);
        currentPedestrians.Add(pedData.Key, thePedestrian);

        Rigidbody newPedRb = thePedestrian.GetComponent<Rigidbody>();
        if (newPedRb != null)
          newPedRb.velocity = thePedestrian.transform.TransformDirection(Vector3.forward * (float)pedData.Value.Speed);
      }
    }
  }

  //=========================================================
  //  delete pedestrians which do not exist in Vissim anymore
  //=========================================================
  private void Remove_Removed_Pedestrians() {
    foreach (KeyValuePair<int, GameObject> pedData in currentPedestrians.Where(p => !exchangeData.PedestrianData.ContainsKey(p.Key)).ToList()) {
      GameObject deadPedestrian = currentPedestrians[pedData.Key];
      manPool.Return_Pool_Item(deadPedestrian);
      currentPedestrians.Remove(pedData.Key);
    }
  }
}


