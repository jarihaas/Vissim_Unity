using UnityEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using PTV.Vision.Interfaces;
using Vissim.Interface;
using Vissim.Logic;
using Vissim.Logic.Autonomy;
using Vissim.Signal;

public partial class VissimInterface : MonoBehaviour {
  // publics
  /* version * 10 -> e.g. 2600 for PTV Vissim 2026 */
  public int VissimVersion = 2600;
  public String InpxFilename = "Smallville.inpx";
  public int VehicleIDForTrackingCam = 1189;
  public float groundZOffset = 0.0f;
  public float lineWidth = 0.1f;
  public float laneHeight = 0.1f;
  public bool fillSegmentGaps = false;

  // Should normally be true -> unity app and Vissim+communication run in parallel
  // If false, both run synchrounously. This might only be useful when not actually driving around in the
  // scene, but for capturing videos/photos by attaching the cam to a Vissim vehicle
  public bool doInParallel = true;

  public Inpx_Importer importer { get; private set; }
  private DrivingSimulatorInterface drivingSimulatorInterface;
  private Dictionary<int, GameObject> currentCars;
  private Dictionary<int, GameObject> currentPedestrians;

  private Dictionary<long, Group> currentSignalControllers;
  private Transform myself;

  private float myCarPosX;
  private float myCarPosZ;
  private Vector3 startingPos;
  private Vector3 startingOrientation;

  private bool trackingCamIsAttached = false;

  // this worker is for the communication with VISSIM
  private BackgroundWorker vissimCommunicator;
  public ExchangeData exchangeData;
  private Pool manPool;
  private long simStepInTicks;
  private long lastUpdate;

  // contains the model data for vehicles and pedestrians
  private Dictionary<string, CarSketchupModelTransformation> carSketchupModelTransformations;

  void Start() {
    // initializing the 3D pedestrians models
    Debug.Log("Initializing pool for 3D pedestrians...");
    manPool = GetComponent<Pool>();

    // initializing the 3D vehicle models
    Debug.Log("Initializing data fro 3D car models...");
    Initialize_Car_Model_Transformations();

    Debug.Log("Importing network from Vissim...");

    //  fetch "Player" (-> our own car)
    myself = GetComponent<Transform>();

    // Get the current working directory
    string currentFolder = System.IO.Directory.GetCurrentDirectory();
    string filename = currentFolder + "\\data\\" + InpxFilename;

    // imports data from the inpx file...
    importer = new Inpx_Importer(filename, groundZOffset, lineWidth, laneHeight, fillSegmentGaps);
    importer.Import();
    // ...and retrieves an initial position and rotation of my car
    startingPos = importer.Starting_Pos;
    startingOrientation = importer.Starting_Orientation;

    Debug.Log("Creating simulator vehicle...");
    ResetMyPosition();

    // the dictionaries of the local "agents" for Vissim vehicles and pedestrians
    currentCars = new Dictionary<int, GameObject>();
    currentPedestrians = new Dictionary<int, GameObject>();

    // communicator vissim <-> unity
    Debug.Log("Starting Vissim and communication threads...");
    InitVissimCommunicator();

    simStepInTicks = importer.Sim_Steps_In_Ticks;
    lastUpdate = DateTime.Now.Ticks;

    // signal controller with all groups and heads
    currentSignalControllers = importer.Signal_Controllers;
  }

  public void ResetMyPosition()
  {
    myself.localPosition = startingPos;
    myCarPosX = startingPos.x;
    myCarPosZ = startingPos.z;
    myself.rotation = Quaternion.LookRotation(startingOrientation);
  }

  public void ExitSimulator()
  {
    Application.Quit();
  }

  void OnApplicationQuit() {
    ExitVissimCommunicator();
  }
}
