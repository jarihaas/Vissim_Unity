using System.Collections.Generic;
using PTV.Vision.Interfaces;

public class ExchangeData {
  public bool dirty;

  // collections of data of our own pedestrians and vehicles
  public Simulator_Veh_Data[] DriverVehData;  // currently we use one of these (index 0) ONLY
  //  you might want to have simulator pedestrians in addition or instead
  //public Simulator_Ped_Data[] DriverPedData;

  // collections of data of vissims pedestrians and vehicles
  public Dictionary<int, VISSIMVehicleData> VehicleData;
  public List<int> newVehIdsList;
  public List<int> movedVehIdsList;
  public List<int> deletedVehIdsList;

  public Dictionary<int, VISSIMPedestrianData> PedestrianData;
  public List<int> newPedIdsList;
  public List<int> movedPedIdsList;
  public List<int> deletedPedIdsList;

  // collections of data of vissims signal states
  public List<VISSIM_Sig_Data> SignalData;
}
