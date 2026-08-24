using System.Runtime.InteropServices;

namespace PTV.Vision.Interfaces
{
  [StructLayout(LayoutKind.Sequential)]
  public struct Simulator_Veh_Data
  {
    public int VehicleID;           /* vehicle number in Vissim, irrelevant for new vehicles */
    public int VehicleType;         /* vehicle type number in Vissim */
    public double Position_X;       /* in m */
    public double Position_Y;       /* in m */
    public double Position_Z;       /* in m */
    public double Orient_Heading;   /* in radians */
    public double Orient_Pitch;     /* in radians */
    public double Speed;            /* in m/s */
    [MarshalAs(UnmanagedType.U1)]
    public bool Create;             /* is this a new vehicle to be placed in the network? */
    public int CreateID;            /* unique ID for the new vehicle to be returned in VISSIM_Veh_Data */
    [MarshalAs(UnmanagedType.U1)]
    public bool Delete;             /* is this vehicle to be removed from the network? */
    [MarshalAs(UnmanagedType.U1)]
    public bool ControlledByVissim; /* is this vehicle to be controlled by Vissim (after this time step)? */
    public int RoutingDecisionNo;   /* used once if ControlledByVissim is changed from false to true */
    public int RouteNo;             /* used once if ControlledByVissim is changed from false to true */
  };
}
