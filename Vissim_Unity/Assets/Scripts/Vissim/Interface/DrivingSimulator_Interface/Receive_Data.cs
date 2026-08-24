using System;
using System.Runtime.InteropServices;

namespace PTV.Vision.Interfaces
{
  public partial class DrivingSimulatorInterface
  {
    //=============
    // dll imports
    //=============

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void VISSIM_GetTrafficVehicles(out int Num_Vehicles, out IntPtr VehicleData);

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void VISSIM_GetTrafficPedestrians(out int Num_Pedestrians, out IntPtr PedestrianData);

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void VISSIM_GetVehicleLists(out int NumNewVehicles, out IntPtr NewVehicleIds, out IntPtr NewVehType
      , out int NumMovedVehicles, out IntPtr MovedVehicleIds
      , out int NumDeletedVehicles, out IntPtr DeletedVehicleIds);

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void VISSIM_GetPedestrianLists(out int NumNewPedestrians, out IntPtr NewPedestrianIds, out IntPtr NewPedType
      , out int NumMovedPedestrians, out IntPtr MovedPedestrianIds
      , out int NumDeletedPedestrians, out IntPtr DeletedPedestrianIds);

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    private static extern void VISSIM_GetSignalStates(out int NumSignals, out IntPtr SignalStateData);

    //==========================
    // receive data from VISSIM
    //==========================

    public void GetVehicleLists(out int[] newIds, out int[] newVehTypes, out int[] movedIds, out int[] deletedIds)
    {
      int numnew = 0;
      IntPtr newids = IntPtr.Zero;
      IntPtr newvehtypes = IntPtr.Zero;
      int nummoved = 0;
      int numdeleted = 0;
      IntPtr deletedids = IntPtr.Zero;
      IntPtr movedids = IntPtr.Zero;

      VISSIM_GetVehicleLists(out numnew, out newids, out newvehtypes, out nummoved, out movedids, out numdeleted, out deletedids);

      newIds = UnsafeArray.ToArray(newids, numnew);
      newVehTypes = UnsafeArray.ToArray(newvehtypes, numnew);
      movedIds = UnsafeArray.ToArray(movedids, nummoved);
      deletedIds = UnsafeArray.ToArray(deletedids, numdeleted);
    }

    public void GetPedestrianLists(out int[] newPedestrianIds, out int[] newPedestrianTypes, out int[] movedPedestrianIds, out int[] deletedPedestrianIds)
    {
      int numNewPedestriansRaw = 0;
      IntPtr newPedestrianIdsRaw = IntPtr.Zero;
      IntPtr newPedestriantypesRaw = IntPtr.Zero;
      int numMovedPedestriansRaw = 0;
      int numDeletedPedestriansRaw = 0;
      IntPtr deletedPedestrianIdsRaw = IntPtr.Zero;
      IntPtr movedPedestrianIdsRaw = IntPtr.Zero;

      VISSIM_GetPedestrianLists(out numNewPedestriansRaw, out newPedestrianIdsRaw, out newPedestriantypesRaw, out numMovedPedestriansRaw, out movedPedestrianIdsRaw, out numDeletedPedestriansRaw, out deletedPedestrianIdsRaw);

      newPedestrianIds = UnsafeArray.ToArray(newPedestrianIdsRaw, numNewPedestriansRaw);
      newPedestrianTypes = UnsafeArray.ToArray(newPedestriantypesRaw, numNewPedestriansRaw);
      movedPedestrianIds = UnsafeArray.ToArray(movedPedestrianIdsRaw, numMovedPedestriansRaw);
      deletedPedestrianIds = UnsafeArray.ToArray(deletedPedestrianIdsRaw, numDeletedPedestriansRaw);
    }

    public void GetVehicleAndPedestrianLists(out int[] newVehicleIds, out int[] newVehicleTypes, out int[] movedVehicleIds, out int[] deletedVehicleIds, out int[] newPedestrianIds, out int[] newPedestrianTypes, out int[] movedPedestrianIds, out int[] deletedPedestrianIds)
    {
      GetVehicleLists(out newVehicleIds, out newVehicleTypes, out movedVehicleIds, out deletedVehicleIds);
      GetPedestrianLists(out newPedestrianIds, out newPedestrianTypes, out movedPedestrianIds, out deletedPedestrianIds);
    }

    public void GetTrafficeVehicles(out VISSIM_Veh_Data[] retVehicleData)
    {
      int vehDataCount = 0;
      IntPtr vehicleData = IntPtr.Zero;
      VISSIM_GetTrafficVehicles(out vehDataCount, out vehicleData);
      retVehicleData = UnsafeArray.ToArray<VISSIM_Veh_Data>(vehicleData, vehDataCount);
    }

    public void GetTrafficePedestrians(out VISSIM_Ped_Data[] retPedestrianData)
    {
      int pedestrianDataCount = 0;
      IntPtr pedestrianData = IntPtr.Zero;
      VISSIM_GetTrafficPedestrians(out pedestrianDataCount, out pedestrianData);
      retPedestrianData = UnsafeArray.ToArray<VISSIM_Ped_Data>(pedestrianData, pedestrianDataCount);
    }

    public void GetTrafficeVehiclesAndPedestrians(out VISSIM_Veh_Data[] retVehicleData, out VISSIM_Ped_Data[] retPedestrianData)
    {
      int vehicleDataCount = 0;
      IntPtr vehicleData = IntPtr.Zero;
      VISSIM_GetTrafficVehicles(out vehicleDataCount, out vehicleData);
      retVehicleData = UnsafeArray.ToArray<VISSIM_Veh_Data>(vehicleData, vehicleDataCount);

      int pedestrianDataCount = 0;
      IntPtr pedestrianData = IntPtr.Zero;
      VISSIM_GetTrafficPedestrians(out pedestrianDataCount, out pedestrianData);
      retPedestrianData = UnsafeArray.ToArray<VISSIM_Ped_Data>(pedestrianData, pedestrianDataCount);
    }

    public void GetSignalStates(out VISSIM_Sig_Data[] retSigData)
    {
      int signalDataCount = 0;
      IntPtr sigData = IntPtr.Zero;
      VISSIM_GetSignalStates(out signalDataCount, out sigData);
      retSigData = UnsafeArray.ToArray<VISSIM_Sig_Data>(sigData, signalDataCount);
    }
  }
}
