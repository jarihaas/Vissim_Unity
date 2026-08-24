using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PTV.Vision.Interfaces;

public partial class VissimInterface : MonoBehaviour {
  private class CarSketchupModelTransformation {
    public string modelName;
    public float scalingFactor;
  }

  /* contains the model data for vehicles and pedestrians */
  private void Initialize_Car_Model_Transformations() {
    carSketchupModelTransformations = new Dictionary<string, CarSketchupModelTransformation> {

      //-------------------------------------------------
      //  mapping for vehicles
      //-------------------------------------------------

      //  for these models we actually have SKP data
      { "XYZ", new CarSketchupModelTransformation { modelName = "Car - Anonymous", scalingFactor = 1.0F }},

      { "Car - Volkswagen Golf (2007)", new CarSketchupModelTransformation { modelName = "Car - AV1", scalingFactor = 1.0F }},

      { "Car - Audi A4 (2005) Hatchback", new CarSketchupModelTransformation { modelName = "Car - Audi A4 (2005) Hatchback", scalingFactor = 1.0F }},
      { "Car - Mercedes CLK500 (2006)", new CarSketchupModelTransformation { modelName = "Car - Mercedes CLK500 (2006)", scalingFactor = 1.0F }},
      { "Car - Peugeot 607 (2006)", new CarSketchupModelTransformation { modelName = "Car - Peugeot 607 (2006)", scalingFactor = 1.0F }},
      { "Car - Volkswagen Beetle (2005)", new CarSketchupModelTransformation { modelName = "Car - Volkswagen Beetle (2005)", scalingFactor = 1.0F }},
      { "Car - Porsche Cayman (2008)", new CarSketchupModelTransformation { modelName = "Car - Porsche Cayman (2008)", scalingFactor = 1.0F }},
      { "Car - Toyota Yaris (2006)", new CarSketchupModelTransformation { modelName = "Car - Toyota Yaris (2006)", scalingFactor = 1.0F }},
      { "HGV - EU 04 Tractor", new CarSketchupModelTransformation { modelName = "HGV - EU 9m", scalingFactor = 1.0F }},
      { "Bus - C2 Standard 2-doors", new CarSketchupModelTransformation { modelName = "Bus - C2 Standard 2-doors", scalingFactor = 1.0F }},
      { "Tram - GT8-2S front", new CarSketchupModelTransformation { modelName = "Tram 6ngt01-03-01 narrow90%", scalingFactor = 1.0F }},
      { "Tram 01 RNV - Front", new CarSketchupModelTransformation { modelName = "Tram 6ngt01-03-01 narrow90%", scalingFactor = 1.0F }},

      //  for these we don't -> map them on something else
      { "Bus - EU Bendy front", new CarSketchupModelTransformation { modelName = "Car - Anonymous", scalingFactor = 1.0F }},
      { "Bus - EU Bendy rear", new CarSketchupModelTransformation { modelName = "Car - Anonymous", scalingFactor = 1.0F }},

      { "Tram - GT8-2S joint", new CarSketchupModelTransformation { modelName = "Car - Anonymous", scalingFactor = 1.0F }},
      { "Tram - GT8-2S mid", new CarSketchupModelTransformation { modelName = "Car - Anonymous", scalingFactor = 1.0F }},
      { "Tram - GT8-2S rear", new CarSketchupModelTransformation { modelName = "Car - Anonymous", scalingFactor = 1.0F }},

      { "Tram 01 RNV - Mid", new CarSketchupModelTransformation { modelName = "Car - Anonymous", scalingFactor = 1.0F }},
      { "Tram 01 RNV - Joint", new CarSketchupModelTransformation { modelName = "Car - Anonymous", scalingFactor = 1.0F }},
      { "Tram 01 RNV - Rear", new CarSketchupModelTransformation { modelName = "Car - Anonymous", scalingFactor = 1.0F }},

      { "Bike - Cycle Man 02", new CarSketchupModelTransformation { modelName = "Bike - Cycle Man 02", scalingFactor = 1.0F }},
      { "Bike - Cycle Woman", new CarSketchupModelTransformation { modelName = "Bike - Cycle Man 02", scalingFactor = 1.0F }}
    };
  }

  // adjust our local vehicle states according to the data retrieved from Vissim
  private void Update_Local_Vehicle_Data(VISSIM_Veh_Data[] VissimVehData) {
    foreach (VISSIM_Veh_Data vehData in VissimVehData) {
      if (vehData.CreateID == 42)
      {
        exchangeData.DriverVehData[0].VehicleID = vehData.VehicleID;
        continue;
      }
      int ID = vehData.VehicleID;
      if (!exchangeData.VehicleData.ContainsKey(ID)) {
        exchangeData.VehicleData[ID] = new VISSIMVehicleData();
        exchangeData.VehicleData[ID].VehicleType = vehData.VehicleType;
        exchangeData.VehicleData[ID].ModelFileName = vehData.ModelFileName;
        exchangeData.VehicleData[ID].color = vehData.color;
      }
      exchangeData.VehicleData[ID].PositionX = vehData.Position_X;
      exchangeData.VehicleData[ID].PositionY = vehData.Position_Y;
      exchangeData.VehicleData[ID].PositionZ = vehData.Position_Z;
      exchangeData.VehicleData[ID].OrientHeading = vehData.Orient_Heading;
      exchangeData.VehicleData[ID].OrientPitch = vehData.Orient_Pitch;
      exchangeData.VehicleData[ID].Speed = vehData.Speed / 3.6;
      exchangeData.VehicleData[ID].IsInVissim = true;
    }
  }

  private void Remove_Left_Vehicles_From_Data() {
    foreach (var vd in exchangeData.VehicleData.Where(vd => !vd.Value.IsInVissim).ToList()) {
      exchangeData.VehicleData.Remove(vd.Key);
    }
  }

  //===========================================
  //  update our own copies of Vissims vehicles
  //  and add newly created vehicles
  //===========================================
  private void Update_Local_Vehicles(float factor) {
    foreach (KeyValuePair<int, VISSIMVehicleData> vehicleData in exchangeData.VehicleData) {
      vehicleData.Value.IsInVissim = false;
      if (currentCars.ContainsKey(vehicleData.Key)) { // moved car
        GameObject theCar = currentCars[vehicleData.Key];
        Vector3 newPos = new Vector3((float)exchangeData.VehicleData[vehicleData.Key].PositionX, (float)exchangeData.VehicleData[vehicleData.Key].PositionZ, (float)exchangeData.VehicleData[vehicleData.Key].PositionY);
        theCar.transform.localPosition = newPos + (newPos - theCar.transform.localPosition) * factor;
        rotateObject(theCar, (float)exchangeData.VehicleData[vehicleData.Key].OrientHeading, (float)exchangeData.VehicleData[vehicleData.Key].OrientPitch);
        Rigidbody rb = theCar.GetComponent<Rigidbody>();
        if (rb != null)
          rb.velocity = theCar.transform.TransformDirection(Vector3.right * (float)vehicleData.Value.Speed);
      } else { // new car
        // spawn vehicles that were newly inserted by Vissim
        GameObject theCar = InstantiateCar(vehicleData.Value);
        if (!trackingCamIsAttached && vehicleData.Key == VehicleIDForTrackingCam)
          Attach_TrackingCam(theCar);
        theCar.transform.localPosition = new Vector3((float)exchangeData.VehicleData[vehicleData.Key].PositionX, (float)exchangeData.VehicleData[vehicleData.Key].PositionZ, (float)exchangeData.VehicleData[vehicleData.Key].PositionY);
        rotateObject(theCar, (float)exchangeData.VehicleData[vehicleData.Key].OrientHeading, (float)exchangeData.VehicleData[vehicleData.Key].OrientPitch);
        currentCars.Add(vehicleData.Key, theCar);
      }
    }
  }

  //======================================================
  //  delete vehicles which do not exist in Vissim anymore
  //======================================================
  private void Remove_Removed_Vehicles() {
    foreach (KeyValuePair<int, GameObject> vehicleData in currentCars.Where(c => !exchangeData.VehicleData.ContainsKey(c.Key)).ToList()) {
      GameObject deadCar = currentCars[vehicleData.Key];
      if (trackingCamIsAttached && vehicleData.Key == VehicleIDForTrackingCam)
        Detach_TrackingCam();
      Destroy(deadCar);
      currentCars.Remove(vehicleData.Key);
    }
  }

  // instatiate a new local "agent" for a Vissim vehicle
  private GameObject InstantiateCar(VISSIMVehicleData vehicleData) {
    string filenameVissim = Path.GetFileNameWithoutExtension(vehicleData.ModelFileName);
    string mn = carSketchupModelTransformations.First().Value.modelName;
    float sf = carSketchupModelTransformations.First().Value.scalingFactor;
    if (carSketchupModelTransformations.ContainsKey(filenameVissim)) {
      mn = carSketchupModelTransformations[filenameVissim].modelName;
      sf = carSketchupModelTransformations[filenameVissim].scalingFactor;
    } else {
      Debug.Log("Using default model for " + filenameVissim);
    }
    GameObject theCar = Instantiate(Resources.Load("Models/" + mn) as GameObject);
    theCar.transform.localScale = new Vector3(sf, sf, sf);
    Rigidbody rb = theCar.AddComponent<Rigidbody>();
    rb.useGravity = false;
    rb.velocity = theCar.transform.TransformDirection(Vector3.right * (float)vehicleData.Speed);

    return theCar;
  }

  // attach the trackingCam to the vissim vehicle with the VehicleIDForTrackingCam when it gets inserted
  private void Attach_TrackingCam(GameObject target) {
    GameObject trackingCam = GameObject.Find("TrackingCam");
    TrackingCamController trackingCamScript = trackingCam != null ? trackingCam.GetComponent<TrackingCamController>() : null;
    if (trackingCamScript != null) {
      trackingCamScript.trackedObject = target.GetComponent<Transform>();
      trackingCamIsAttached = true;
    }
  }

  // detach the trackingCam from the vissim vehicle with the VehicleIDForTrackingCam when it gets deleted
  private void Detach_TrackingCam() {
    GameObject trackingCam = GameObject.Find("TrackingCam");
    TrackingCamController trackingCamScript = trackingCam != null ? trackingCam.GetComponent<TrackingCamController>() : null;
    if (trackingCamScript != null)
      trackingCamScript.trackedObject = null;
    trackingCamIsAttached = false;
  }
}


