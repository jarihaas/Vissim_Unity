using UnityEngine;
using System.Collections;

namespace Vissim.Logic
{
    public class MCSAnimatorController : MonoBehaviour
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
