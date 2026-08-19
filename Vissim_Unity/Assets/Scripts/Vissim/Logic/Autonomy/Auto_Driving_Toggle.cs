using UnityEngine;

namespace Vissim.Logic.Autonomy
{
    public class Auto_Driving_Toggle : MonoBehaviour
    {
        public bool Controlled_By_Vissim
        { get; set; }

        public int Routing_Decision_No
        { get;set; }

        public int Route_No
        { get; set; }

        void Start()
        { Controlled_By_Vissim = false; }

        void Update()
        {
            if (Controlled_By_Vissim)
                if (Input.GetKeyDown(KeyCode.F8))
                    Controlled_By_Vissim = false;
        }
    }
}