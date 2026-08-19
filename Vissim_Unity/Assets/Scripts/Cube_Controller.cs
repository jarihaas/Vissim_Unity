using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Controller : MonoBehaviour
{
    /* Properties and Predicates */
    public float Mov_Speed;
    void Start()
    {
        /* Set the values of Predicates. */
        Mov_Speed = 5f;
    }

    void Update()
    {
        /* Get input from Keyboard */
        float Axis_X = Input.GetAxisRaw("Horizontal");
        float Axis_Y = Input.GetAxisRaw("Vertical");

        /* Calculate direction relative to the forward vector. */
        Vector3 Mov_Y = transform.forward * Axis_Y;
        Vector3 Mov_X = transform.right * Axis_X;

        /* Move it. */
        Vector3 Mov = new Vector3(Axis_X, 0f, Axis_Y);
        transform.Translate(Mov * Mov_Speed * Time.deltaTime, Space.World);
    }
}
