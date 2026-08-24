public class VISSIMPedestrianData {
  /* pedestrian type number from VISSIM */
  public int PedestrianType;

  /* *.v3d */
  public string ModelFileName;

  /* data about a pedestrian in m */
  public double Length;
  public double Width;
  public double Height;
  public double PositionX;
  public double PositionY;
  public double PositionZ;
  public double DistanceSinceBirth;

  /* in radians */
  public double OrientHeading;

  /* in m/s */
  public double Speed;

  public bool IsWalking;
  public bool IsInVissim;
};
