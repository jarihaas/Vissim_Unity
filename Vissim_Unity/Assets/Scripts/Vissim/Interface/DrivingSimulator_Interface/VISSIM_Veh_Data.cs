using System.Runtime.InteropServices;

namespace PTV.Vision.Interfaces
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  public struct VISSIM_Veh_Data
  {
    public int VehicleID;
    public int VehicleType;                         /* vehicle type number from VISSIM */
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string ModelFileName;                    /* *.v3d */
    public int color;                               /* RGB */
    public double Position_X;                       /* in m */
    public double Position_Y;                       /* in m */
    public double Position_Z;                       /* in m */
    public double Orient_Heading;                   /* in radians */
    public double Orient_Pitch;                     /* in radians */
    public double Speed;                            /* in m/s */
    public int LeadingVehicleID;                    /* relevant vehicle in front */
    public int TrailingVehicleID;                   /* next vehicle back on the same lane */
    public int LinkID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string LinkName;                         /* empty if not set in VISSIM */
    public double LinkCoordinate;                   /* in m */
    public int LaneIndex;                           /* 0 = rightmost */
    public TurningIndicatorType TurningIndicator;   /* 1 = left, 0 = none, -1 = right */
    public int PreviousIndex;                       /* for interpolation: index in the array in the previous VISSIM time step, -1 = new in the network */
    public int NumUDAs;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.R8)]
    public double[] UdaDoubles;
    public int CreateID;            /* unique ID as passed from the simulator for the new vehicle, else zero */
    public byte ControlledByVissim; /* 1 = true, 0 = false (i.e. vehicle controlled by the Driving Simulator) */
  };
}
