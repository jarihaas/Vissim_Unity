using System;
using System.Runtime.InteropServices;

namespace PTV.Vision.Interfaces
{
  //==============================================
  // interface for the simulator
  //==============================================
  public partial class DrivingSimulatorInterface : IDisposable
  {
    public bool Connected { get; private set; }

    //=============
    // dll imports
    //=============

    /* Native proxy returns C++ bool (1 byte) and wchar_t* error messages,
       hence the explicit marshaling on every return value */

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool VISSIM_Connect(int versionNo
      , [MarshalAs(UnmanagedType.LPWStr)] string networkFileName
      , int simulatorFrequency
      , double visibilityRadius
      , int maxSimulatorVeh
      , int maxSimulatorPed
      , int maxSimulatorDet
      , int maxVissimVeh
      , int maxVissimPed
      , int maxVissimSigGrp);

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool VISSIM_ConnectToConsole([MarshalAs(UnmanagedType.LPWStr)] string consoleFileName
      , [MarshalAs(UnmanagedType.LPWStr)] string networkFileName
      , int simulatorFrequency
      , double visibilityRadius
      , int maxSimulatorVeh
      , int maxSimulatorPed
      , int maxSimulatorDet
      , int maxVissimVeh
      , int maxVissimPed
      , int maxVissimSigGrp);

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool VISSIM_Disconnect();

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool VISSIM_DataReady();

    [DllImport("DrivingSimulatorProxy", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPWStr)]
    private static extern string VISSIM_GetLastErrorMessage();

    //=======================================
    // connect with normal VISSIM (with GUI)
    //=======================================

    public DrivingSimulatorInterface(int version
      , string networkFileName
      , int simulatorFrequency
      , double visibilityRadius
      , int maxSimulatorVeh
      , int maxSimulatorPed
      , int maxSimulatorDet
      , int maxVissimVeh
      , int maxVissimPed
      , int maxVissimSigGrp)
    {
      if (!Connect(version
          , networkFileName
          , simulatorFrequency
          , visibilityRadius
          , maxSimulatorVeh
          , maxSimulatorPed
          , maxSimulatorDet
          , maxVissimVeh
          , maxVissimPed
          , maxVissimSigGrp))
      {
        throw new Exception("Connection has failed: " + GetLastError());
      }
    }

    public bool Connect(int version
      , string networkFileName
      , int simulatorFrequency
      , double visibilityRadius
      , int maxSimulatorVeh
      , int maxSimulatorPed
      , int maxSimulatorDet
      , int maxVissimVeh
      , int maxVissimPed
      , int maxVissimSigGrp)
    {
      this.Connected = VISSIM_Connect(version
        , networkFileName
        , simulatorFrequency
        , visibilityRadius
        , maxSimulatorVeh
        , maxSimulatorPed
        , maxSimulatorDet
        , maxVissimVeh
        , maxVissimPed
        , maxVissimSigGrp);
      return this.Connected;
    }

    //=============================
    // connect with console VISSIM
    //=============================

    public DrivingSimulatorInterface(string consoleFileName
      , string networkFileName
      , int simulatorFrequency
      , double visibilityRadius
      , int maxSimulatorVeh
      , int maxSimulatorPed
      , int maxSimulatorDet
      , int maxVissimVeh
      , int maxVissimPed
      , int maxVissimSigGrp)
    {
      if (!ConnectConsole(consoleFileName
        , networkFileName
        , simulatorFrequency
        , visibilityRadius
        , maxSimulatorVeh
        , maxSimulatorPed
        , maxSimulatorDet
        , maxVissimVeh
        , maxVissimPed
        , maxVissimSigGrp))
      {
        throw new Exception("Connection has failed: " + GetLastError());
      }
    }

    public bool ConnectConsole(string consoleFileName
      , string networkFileName
      , int simulatorFrequency
      , double visibilityRadius
      , int maxSimulatorVeh
      , int maxSimulatorPed
      , int maxSimulatorDet
      , int maxVissimVeh
      , int maxVissimPed
      , int maxVissimSigGrp)
    {
      this.Connected = VISSIM_ConnectToConsole(consoleFileName
        , networkFileName
        , simulatorFrequency
        , visibilityRadius
        , maxSimulatorVeh
        , maxSimulatorPed
        , maxSimulatorDet
        , maxVissimVeh
        , maxVissimPed
        , maxVissimSigGrp);
      return this.Connected;
    }

    //========================
    // disconnect from VISSIM
    //========================

    public bool Disconnect()
    {
      this.Connected = !VISSIM_Disconnect();
      return this.Connected;
    }

    public bool DataReady()
    {
      return VISSIM_DataReady();
    }

    public string GetLastError()
    {
      return VISSIM_GetLastErrorMessage();
    }

    #region IDisposable Support

    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (this.Connected)
        {
          Disconnect();
        }

        disposedValue = true;
      }
    }

    ~DrivingSimulatorInterface()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion
  }
}
