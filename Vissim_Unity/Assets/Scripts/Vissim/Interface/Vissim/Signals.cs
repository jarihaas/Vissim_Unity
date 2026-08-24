using UnityEngine;
using System.Linq;
using PTV.Vision.Interfaces;
using Vissim.Signal;

public partial class VissimInterface : MonoBehaviour {
  //=================================================
  //  update all signal states
  //=================================================
  private void Update_Signal_States() {
    foreach (VISSIM_Sig_Data signal in exchangeData.SignalData) {
      var targetController = currentSignalControllers.FirstOrDefault(x => x.Key == signal.ControllerID);
      if (targetController.Value == null)
        continue;
      foreach (var targetSignals in targetController.Value.Signal_Heads) {
        foreach (var signalHead in targetSignals.Value.Where(x => x.Value.SG == signal.SignalGroupID).Select(x => x.Value)) {
          if (signalHead.Obj != null)
            Set_Signal_Head_Colour(signalHead, signal.SignalState);
        }
      }
    }
  }

  private void Set_Signal_Head_Colour(Head signalHead, SignalStateType currentState) {
    signalHead.Colour = currentState == SignalStateType.SignalStateRed ? Color.red :
                             currentState == SignalStateType.SignalStateGreen ? Color.green :
                             currentState == SignalStateType.SignalStateAmber ? Color.yellow :
                             currentState == SignalStateType.SignalStateOff ? Color.black :
                             currentState == SignalStateType.SignalStateFlashingRed ? signalHead.Colour == Color.red ? Color.black : Color.red :
                             currentState == SignalStateType.SignalStateFlashingAmber ? signalHead.Colour == Color.yellow ? Color.black : Color.yellow :
                             currentState == SignalStateType.SignalStateFlashingGreen ? signalHead.Colour == Color.green ? Color.black : Color.green :
                             currentState == SignalStateType.SignalStateAlternatingRedGreen ? signalHead.Colour == Color.red ? Color.green : Color.red :
                             currentState == SignalStateType.SignalStateRedAmber ? Color.yellow :
                             currentState == SignalStateType.SignalStateGreenAmber ? new Color(1f, 166f / 255f, 0f, 1f) : Color.gray;

    signalHead.UpdateState();
  }
}
