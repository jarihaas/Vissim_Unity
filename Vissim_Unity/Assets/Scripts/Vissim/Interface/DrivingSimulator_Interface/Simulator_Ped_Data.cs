using System.Runtime.InteropServices;

namespace PTV.Vision.Interfaces
{
  [StructLayout(LayoutKind.Sequential)]
  public struct Simulator_Ped_Data
  {
    public double Position_X;         /* in m */
    public double Position_Y;         /* in m */
    public double Position_Z;         /* in m */
    public double Orient_Heading;     /* in radians */
    public double DistanceSinceBirth; /* in m */
    public double Speed;              /* in m/s */
  };
}
