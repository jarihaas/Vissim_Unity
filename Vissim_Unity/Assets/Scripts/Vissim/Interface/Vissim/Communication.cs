using UnityEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using PTV.Vision.Interfaces;

public partial class VissimInterface : MonoBehaviour {
  private void InitVissimCommunicator() {
    exchangeData = new ExchangeData();

    exchangeData.dirty = false;

    exchangeData.VehicleData = new Dictionary<int, VISSIMVehicleData>();

    exchangeData.DriverVehData = new Simulator_Veh_Data[1];
    exchangeData.DriverVehData[0].VehicleID = 42;
    exchangeData.DriverVehData[0].VehicleType = 100;
    exchangeData.DriverVehData[0].Create = true;
    exchangeData.DriverVehData[0].CreateID = 42;
    exchangeData.DriverVehData[0].Delete = false;

    exchangeData.newVehIdsList = new List<int>();
    exchangeData.movedVehIdsList = new List<int>();
    exchangeData.deletedVehIdsList = new List<int>();

    exchangeData.PedestrianData = new Dictionary<int, VISSIMPedestrianData>();
    //  you might want to have simulator pedestrians in addition or instead
    //exchangeData.DriverPedData = new Simulator_Ped_Data[1];
    exchangeData.newPedIdsList = new List<int>();
    exchangeData.movedPedIdsList = new List<int>();
    exchangeData.deletedPedIdsList = new List<int>();

    exchangeData.SignalData = new List<VISSIM_Sig_Data>();

    if (doInParallel) {
      Debug.Log("Starting Vissim in parallel mode...");
      vissimCommunicator = new BackgroundWorker();
      vissimCommunicator.DoWork += VissimCommunicationThreadFunc;
      vissimCommunicator.RunWorkerCompleted += WorkerCompleted;
      vissimCommunicator.WorkerSupportsCancellation = true;
      vissimCommunicator.RunWorkerAsync();
    } else {
      StartVissim();
    }
  }

  private void StartVissim() {
    // Get the current working directory
    string currentFolder = System.IO.Directory.GetCurrentDirectory();
    string filename = currentFolder + "\\data\\" + InpxFilename;
    try
    {
      string msg = "Starting Vissim " + VissimVersion.ToString() + " and the simulation for " + filename + "...";
      Debug.Log(msg);
      drivingSimulatorInterface = new DrivingSimulatorInterface(VissimVersion  //  VissimVersion
        , filename  //  filename of inpx file (including path)
        , 10  //  SimulatorFrequency
        , 1000  //  VisibilityRadius
        , 1  //  MaxNumSimulatorVehicles
        , 0  //  MaxNumSimulatorPedestrians
        , 0  //  MaxNumSimulatorDetectors
        , 1000  //  MaxNumVissimVehicles
        , 1000  //  MaxNumVissimPedestrians
        , 25);  //  MaxNumVissimSignalGroups
    }
    catch (Exception e)
    {
      Debug.LogError(e.Message);
    }
  }

  private void DoVissimCommunication() {
    if (drivingSimulatorInterface == null)
      return;

    if (Monitor.TryEnter(exchangeData, 10)) {
      try {
        exchangeData.dirty = true;

        //  send state of our vehicles and pedestrians to Vissim
        Simulator_Veh_Data[] DriverVehData = exchangeData.DriverVehData;
        drivingSimulatorInterface.SetDriverVehicles(DriverVehData);
        exchangeData.DriverVehData[0].Create = false;

        //  you might want to have simulator pedestrians in addition or instead
        //Simulator_Ped_Data[] DriverPedData = exchangeData.DriverPedData;
        //drivingSimulatorInterface.SetDriverVehiclesAndPedestrians(DriverVehData, DriverPedData);

        //  retrieve data for Vissims vehicles and pedestrians
        VISSIM_Veh_Data[] VissimVehData;
        VISSIM_Ped_Data[] VissimPedData;
        drivingSimulatorInterface.GetTrafficeVehiclesAndPedestrians(out VissimVehData, out VissimPedData);

        //  retrieve data for Vissims signal states
        VISSIM_Sig_Data[] VissimSignalData;
        drivingSimulatorInterface.GetSignalStates(out VissimSignalData);

        Update_Local_Vehicle_Data(VissimVehData);
        Remove_Left_Vehicles_From_Data();

        Update_Local_Pedestrian_Data(VissimPedData);
        Remove_Left_Pedestrians_From_Data();

        // adjust our local signal states accordingly
        exchangeData.SignalData = new List<VISSIM_Sig_Data>();
        for (int i = 0; i < VissimSignalData.Length; ++i) {
          exchangeData.SignalData.Add(VissimSignalData[i]);
        }
      } finally {
        Monitor.Exit(exchangeData);
      }
    }
  }

  // method to control the data exchange timing
  private void VissimCommunicationThreadFunc(object sender, DoWorkEventArgs e) {
    StartVissim();
    long nextUpdate = DateTime.Now.Ticks + simStepInTicks;

    while (!vissimCommunicator.CancellationPending) {
      if (nextUpdate <= DateTime.Now.Ticks) {
        nextUpdate += simStepInTicks;
        DoVissimCommunication();
      }
    }
  }

  private void ExitVissimCommunicator() {
    if (vissimCommunicator != null && vissimCommunicator.IsBusy)
      vissimCommunicator.CancelAsync();
  }

  private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
    if (e.Error != null)
      Debug.LogError("Exception in the Vissim communication thread: " + e.Error.Message);

    if (drivingSimulatorInterface != null) {
      drivingSimulatorInterface.Disconnect();
      drivingSimulatorInterface = null;
    }
    exchangeData = null;
  }
}
