using System.Runtime.InteropServices;

namespace PTV.Vision.Interfaces
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  public struct VISSIM_Ped_Data
  {
    public int PedestrianID;
    public int PedestrianType;                                             /* pedestrian type number from VISSIM */
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string ModelFileName;                                           /* *.v3d */
    public double Length;                                                  /* in m */
    public double Width;                                                   /* in m */
    public double Height;                                                  /* in m */
    public double Position_X;                                              /* in m */
    public double Position_Y;                                              /* in m */
    public double Position_Z;                                              /* in m */
    public double Orient_Heading;                                          /* in radians */
    public double Orient_Pitch;                                            /* in radians */
    public double DistanceSinceBirth;                                      /* in m */
    public double Speed;                                                   /* in m/s */
    public Pedestrian_Motion_State_Type MotionState;                       /* the current motion state */
    public Pedestrian_Construction_Element_Type ConstructionElementType;   /* the type of the construction element */
    public int ConstructionElementID;                                      /* the construction element the pedestrian is currently on */
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string ConstructionElementName;                                 /* empty if not set in VISSIM */
    public int PreviousIndex;                                              /* for interpolation: index in the array in the previous VISSIM time step, -1 = new in the network */
  };
}
