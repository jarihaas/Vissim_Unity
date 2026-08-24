using PTV.Vision.Interfaces;

public class VISSIMSignalData {
  public int ControllerID;
  public int SignalGroupID;
  /*
     1 = Red, 2 = Red+Amber, 3 = Green, 4 = Amber, 5 = Off (black), 6 = Undefined,
     7 = Flashing Amber, 8 = Flashing Red, 9 = Flashing Green, 10 = Alternating Red/Green,
     11 = Green+Amber
   */
  public SignalStateType SignalState;
}
