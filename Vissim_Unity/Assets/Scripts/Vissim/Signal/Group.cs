using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Vissim.Signal
{
  public class Group
  {
    public Dictionary<long, Dictionary<long, Head>> Signal_Heads
    { get; set; }
  }
}