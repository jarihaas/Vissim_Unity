public class VISSIMVehicleData {
  /* vehicle type number from VISSIM */
  public int VehicleType;

  /* *.v3d */
  public string ModelFileName;

  /* RGB */
  public int color;

  /* in m */
  public double PositionX;
  public double PositionY;
  public double PositionZ;

  /* in radians */
  public double OrientHeading;
  public double OrientPitch;

  /* in m/s */
  public double Speed;

  public bool IsInVissim;
};
