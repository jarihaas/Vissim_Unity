namespace PTV.Vision.Interfaces
{
  /* Note: value 9 is named WaitingAtQueueHead since Vissim 2024
     (previously Motion_State_Type_ServicedAtQueueHead) */
  public enum Pedestrian_Motion_State_Type
  {
    Motion_State_Type_ApproachingPTVehicle = 1,
    Motion_State_Type_AlightingFromPTVehicle = 2,
    Motion_State_Type_WaitingForPTVehicle = 3,
    Motion_State_Type_WalkingUpOnEscalator = 4,
    Motion_State_Type_WalkingDownOnEscalator = 5,
    Motion_State_Type_StandingOnEscalator = 6,
    Motion_State_Type_WalkingOnMovingWalkway = 7,
    Motion_State_Type_StandingOnMovingWalkway = 8,
    Motion_State_Type_WaitingAtQueueHead = 9,
    Motion_State_Type_WaitingInQueue = 10,
    Motion_State_Type_WalkingUpstairs = 11,
    Motion_State_Type_WalkingDownstairs = 12,
    Motion_State_Type_ApproachingElevator = 13,
    Motion_State_Type_AlightingFromElevator = 14,
    Motion_State_Type_WaitingForElevator = 15,
    Motion_State_Type_RidingElevator = 16,
    Motion_State_Type_Waiting = 17,
    Motion_State_Type_WalkingOnLevel = 18,
    Motion_State_Type_End = 19
  };
}
