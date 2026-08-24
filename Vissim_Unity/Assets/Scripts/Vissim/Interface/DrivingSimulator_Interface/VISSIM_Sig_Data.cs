using System.Runtime.InteropServices;

namespace PTV.Vision.Interfaces
{
  [StructLayout(LayoutKind.Sequential)]
  public struct VISSIM_Sig_Data
  {
    public int ControllerID;
    public int SignalGroupID;
    public SignalStateType SignalState;   /* 1 = Red, 2 = Red+Amber, 3 = Green, 4 = Amber, 5 = Off (black),
                                             6 = Undefined, 7 = Flashing Amber, 8 = Flashing Red, 9 = Flashing Green,
                                             10 = Alternating Red/Green, 11 = Green+Amber */
  };
}
