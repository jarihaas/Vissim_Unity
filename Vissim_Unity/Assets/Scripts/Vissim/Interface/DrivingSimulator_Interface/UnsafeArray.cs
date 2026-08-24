using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PTV.Vision.Interfaces
{
  //=========================================================
  // convenience class to map structs onto raw memory blocks
  //=========================================================
  internal static class UnsafeArray
  {
    public static IEnumerable<T> Enumerable<T>(IntPtr pointer, int length) where T : struct
    {
      int sizeInBytes = Marshal.SizeOf(typeof(T));
      IntPtr p = pointer;
      for (int i = 0; i < length; i++)
      {
        yield return (T)Marshal.PtrToStructure(p, typeof(T));
        p = new IntPtr(p.ToInt64() + sizeInBytes);
      }
    }

    public static T[] ToArray<T>(IntPtr pointer, int length) where T : struct
    {
      return Enumerable<T>(pointer, length).ToArray();
    }

    public static int[] ToArray(IntPtr pointer, int length)
    {
      var array = new int[length];
      Marshal.Copy(pointer, array, 0, length);
      return array;
    }
  }
}
