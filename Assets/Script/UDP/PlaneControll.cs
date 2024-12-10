using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControll : MonoBehaviour
{
    public Rigidbody rb;  // El Rigidbody del avi�n, usado para aplicar fuerzas f�sicas
    public float maxSpeed = 200f;  // Velocidad m�xima
    public float pitchSpeed = 5f;  // Velocidad de rotaci�n alrededor del eje X (pitch)
    public float rollSpeed = 5f;  // Velocidad de rotaci�n alrededor del eje Y (alabeo)
    public float yawSpeed = 5f;   // Velocidad de rotaci�n alrededor del eje Z (gui�ada)
    public float thrust = 50f;     // Potencia del motor (aceleraci�n)

    private float rollInput;
    private float pitchInput;
    private float yawInput;
    private float throttleInput;

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>(); // Aseg�rate de que se asigna el Rigidbody
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Leer las entradas del teclado para controlar la rotaci�n y la aceleraci�n
        rollInput = 0f;
        pitchInput = 0f;
        yawInput = 0f;
        throttleInput = 0f;

        // Controles de rotaci�n (alabeo, pitch, gui�ada)
        if (Input.GetKey(KeyCode.W)) // Girar a la izquierda (alabeo)
        {
            rollInput = -1f;
        }
        if (Input.GetKey(KeyCode.S)) // Girar a la derecha (alabeo)
        {
            rollInput = 1f;
        }
        if (Input.GetKey(KeyCode.A)) // Elevar (pitch hacia arriba)
        {
            pitchInput = 1f;
        }
        if (Input.GetKey(KeyCode.D)) // Bajar (pitch hacia abajo)
        {
            pitchInput = -1f;
        }
        if (Input.GetKey(KeyCode.Q)) // Girar a la izquierda (gui�ada)
        {
            yawInput = -1f;
        }
        if (Input.GetKey(KeyCode.E)) // Girar a la derecha (gui�ada)
        {
            yawInput = 1f;
        }

        // Controlar la aceleraci�n
        if (Input.GetKey(KeyCode.UpArrow)) // Acelerar (throttle hacia adelante)
        {
            throttleInput = 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow)) // Desacelerar (throttle hacia atr�s)
        {
            throttleInput = -1f;
        }

        // Aplicar las fuerzas de movimiento y rotaci�n
        MoveAircraft();
    }

    void MoveAircraft()
    {
        // Aceleraci�n
        float currentThrottle = throttleInput * thrust;
        Vector3 forwardMovement = transform.forward * currentThrottle * Time.deltaTime;  // Movimiento hacia adelante
        rb.AddForce(forwardMovement, ForceMode.Force);  // Aplicar la fuerza hacia adelante

        // Controlar velocidad m�xima
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;  // Limitar la velocidad m�xima
        }

        // Aplicar rotaciones
        float roll = rollInput * rollSpeed * Time.deltaTime;  // Alabeo
        float pitch = pitchInput * pitchSpeed * Time.deltaTime; // Pitch
        float yaw = yawInput * yawSpeed * Time.deltaTime;     // Gui�ada

        // Rotar el avi�n
        rb.AddTorque(transform.forward * pitch, ForceMode.Force);  // Pitch (X)
        rb.AddTorque(transform.right * roll, ForceMode.Force);     // Alabeo (Y)
        rb.AddTorque(transform.up * yaw, ForceMode.Force);        // Gui�ada (Z)
    }
}
