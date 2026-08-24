using System;
using System.Runtime.InteropServices;

namespace PTV.Vision.Interfaces
{
  public partial class DrivingSimulatorInterface
  {
    //=============
    // dll imports
    //=============

    /* The proxy copies the arrays into its shared memory during the call,
       so the marshaler-managed buffers are safe (and freed) after return */

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool VISSIM_SetDriverVehicles(int Num_Vehicles, [In] Simulator_Veh_Data[] VehicleData);

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool VISSIM_SetDriverPedestrians(int Num_Pedestrians, [In] Simulator_Ped_Data[] PedestrianData);

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool VISSIM_SetDriverVehiclesAndPedestrians(int Num_Vehicles, [In] Simulator_Veh_Data[] VehicleData,
      int Num_Pedestrians, [In] Simulator_Ped_Data[] PedestrianData);

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool VISSIM_SetDetection(int DetectorID, int ControllerID);

    //========================================
    // send data from the simulator to VISSIM
    //========================================

    public void SetDriverVehicles(Simulator_Veh_Data[] vehData)
    {
      int count = vehData == null ? 0 : vehData.Length;
      if (!VISSIM_SetDriverVehicles(count, vehData))
        throw new Exception("VISSIM_SetDriverVehicles failed: " + GetLastError());
    }

    public void SetDriverPedestrians(Simulator_Ped_Data[] pedData)
    {
      int count = pedData == null ? 0 : pedData.Length;
      if (!VISSIM_SetDriverPedestrians(count, pedData))
        throw new Exception("VISSIM_SetDriverPedestrians failed: " + GetLastError());
    }

    public void SetDriverVehiclesAndPedestrians(Simulator_Veh_Data[] vehData, Simulator_Ped_Data[] pedData)
    {
      int vehCount = vehData == null ? 0 : vehData.Length;
      int pedCount = pedData == null ? 0 : pedData.Length;
      if (!VISSIM_SetDriverVehiclesAndPedestrians(vehCount, vehData, pedCount, pedData))
        throw new Exception("VISSIM_SetDriverVehiclesAndPedestrians failed: " + GetLastError());
    }

    public void SetDetection(int DetectorID, int ControllerID)
    {
      VISSIM_SetDetection(DetectorID, ControllerID);
    }
  }
}
