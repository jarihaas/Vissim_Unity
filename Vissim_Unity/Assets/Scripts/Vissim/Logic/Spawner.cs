using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Vissim.Logic
{
    class Spawner : MonoBehaviour {
        public static GameObject Spawn(string prefab)
        {
            return (GameObject)Instantiate(Resources.Load(prefab));
        }
    }
}
