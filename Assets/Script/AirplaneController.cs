using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public Transform[] propellers; // Hélices del avión
    public float thrustPower = 500f;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float thrustInput = Input.GetAxis("Vertical");
        Vector3 thrustForce = transform.forward * thrustInput * thrustPower;
        rb.AddForce(thrustForce);

        // Rotar hélices
        foreach (Transform propeller in propellers)
        {
            propeller.Rotate(Vector3.forward * thrustInput * 1000f * Time.deltaTime);
        }
    }
}
