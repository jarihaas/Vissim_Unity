using UnityEngine;
using System.Collections;

namespace Vissim.Logic
{
    public class MCS_Animator_Controller : MonoBehaviour
    {
        private Animator Anim;
        public float Speed;
        void Start ()
        {
            Anim = GetComponent<Animator>();
            Speed = 0.0f;
        }
    }
}
