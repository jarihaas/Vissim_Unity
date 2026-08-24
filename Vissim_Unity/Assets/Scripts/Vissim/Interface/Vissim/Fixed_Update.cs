using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using Vissim.Logic.Autonomy;

public partial class VissimInterface : MonoBehaviour {
  void FixedUpdate() {
    if (exchangeData == null || exchangeData.DriverVehData == null || myself == null)
      return;

    //  set current speed of our own car
    Set_Current_Speed();

    // set text of speedometer
    Update_Speedometer();

    if (!doInParallel) {
      DoVissimCommunication();
    }
    if (Monitor.TryEnter(exchangeData, 1)) {
      try {
        if (exchangeData.dirty) {
          exchangeData.dirty = false;
          long ticksSinceLastUpdate = DateTime.Now.Ticks - lastUpdate;
          float factor = 0.0f;
          if (doInParallel) {
            factor = ((float)ticksSinceLastUpdate / (float)simStepInTicks) - 1f;
          }
          lastUpdate = DateTime.Now.Ticks;

          //==================================
          //  send our vehicle state to Vissim
          //==================================
          Send_My_Vehicle_State();

          Update_Local_Vehicles(factor);
          Remove_Removed_Vehicles();

          Update_Local_Pedestrians(factor);
          Remove_Removed_Pedestrians();

          Update_Signal_States();
        }
      } catch (Exception e) {
        string error = "Exception in Update: " + e.Message + " " + e.StackTrace;
        Debug.Log(error);
      } finally {
        Monitor.Exit(exchangeData);
      }
    }
  }

  private void Set_Current_Speed() {
    Vector2 myRecentMovement = new Vector2(myCarPosX - myself.localPosition.x, myCarPosZ - myself.localPosition.z);
    exchangeData.DriverVehData[0].Speed = Math.Abs(myRecentMovement.magnitude / Time.deltaTime);

    myCarPosX = myself.localPosition.x;
    myCarPosZ = myself.localPosition.z;
  }

  private void Update_Speedometer() {
    string speedString = Math.Round(exchangeData.DriverVehData[0].Speed * 3.6f) + " km/h";
    GameObject speedDisplay = GameObject.Find("Speed");
    if (speedDisplay != null) {
      Text speedText = speedDisplay.GetComponent<Text>();
      if (speedText != null)
        speedText.text = speedString;
    }
    GameObject cockpitSpeedDisplay = GameObject.Find("CockpitSpeed");
    if (cockpitSpeedDisplay != null) {
      TextMesh cockpitSpeedText = cockpitSpeedDisplay.GetComponent<TextMesh>();
      if (cockpitSpeedText != null)
        cockpitSpeedText.text = speedString;
    }
  }

  private void Send_My_Vehicle_State() {
    //  set current orientation (Careful: Vissim expects radians)
    Vector3 myOrientation = myself.rotation.eulerAngles;
    exchangeData.DriverVehData[0].Orient_Heading = -1.0f * Mathf.Deg2Rad * (myOrientation.y - 90.0f);  //  eastbound = 0, northbound = pi/2
    exchangeData.DriverVehData[0].Orient_Pitch = Mathf.Deg2Rad * myOrientation.z;

    //  set current position (Careful: Vissim expects the center of our leading edge)
    float vehLength = 4.47f;  //  Known from Unity editor
    float displaceToLeadingCenterX = 0.5f * vehLength * Mathf.Sin(Mathf.Deg2Rad * myOrientation.x);
    float displaceToLeadingCenterZ = 0.5f * vehLength * Mathf.Sin(Mathf.Deg2Rad * myOrientation.z);
    exchangeData.DriverVehData[0].Position_X = myself.localPosition.x + displaceToLeadingCenterX;
    exchangeData.DriverVehData[0].Position_Y = myself.localPosition.z + displaceToLeadingCenterZ;
    exchangeData.DriverVehData[0].Position_Z = myself.localPosition.y;
    Auto_Driving_Toggle autonomousDrivingData = myself.GetComponent<Auto_Driving_Toggle>();
    if (autonomousDrivingData != null) {
      exchangeData.DriverVehData[0].ControlledByVissim = autonomousDrivingData.Controlled_By_Vissim;
      if (autonomousDrivingData.Controlled_By_Vissim)
      {
        exchangeData.DriverVehData[0].RoutingDecisionNo = autonomousDrivingData.Routing_Decision_No;
        exchangeData.DriverVehData[0].RouteNo = autonomousDrivingData.Route_No;
      }
    }

    //=====================================
    //  send our pedestrian state to Vissim
    //=====================================
    //  you might want to have simulator pedestrians in addition or instead
    //exchangeData.DriverPedData[0].Position_X = 0;
    //exchangeData.DriverPedData[0].Position_Y = 0;
    //exchangeData.DriverPedData[0].Position_Z = 0;
    //exchangeData.DriverPedData[0].Speed = 0;
    //exchangeData.DriverPedData[0].Orient_Heading = 0;
  }

  // rotates the target game object
  private void rotateObject(GameObject gameObject, float orientHeading, float orientPitch) {
    float pitch = Mathf.Rad2Deg * orientPitch;
    float heading = -1.0f * Mathf.Rad2Deg * orientHeading;
    gameObject.transform.rotation = Quaternion.Euler(0, heading, pitch);
  }
}
