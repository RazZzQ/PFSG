using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorController : MonoBehaviour
{
    public WheelCollider[] wheels; // Array de WheelColliders (motores)
    public float motorTorque = 200f;

    void FixedUpdate()
    {
        float motorInput = Input.GetAxis("Vertical");

        foreach (WheelCollider wheel in wheels)
        {
            wheel.motorTorque = motorInput * motorTorque;
        }
    }
}
