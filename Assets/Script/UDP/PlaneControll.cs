using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControll : MonoBehaviour
{
    public float maxSpeed = 200f;  // Velocidad m�xima
    public float pitchSpeed = 5f;  // Velocidad de rotaci�n alrededor del eje X (pitch)
    public float rollSpeed = 5f;  // Velocidad de rotaci�n alrededor del eje Y (alabeo)
    public float yawSpeed = 5f;   // Velocidad de rotaci�n alrededor del eje Z (gui�ada)
    public float thrust = 50f;     // Potencia del motor (aceleraci�n)

    public Camera mainCamera;      // Referencia a la c�mara principal

    private float rollInput;
    private float pitchInput;
    private float yawInput;
    private float throttleInput;

    private Vector3 velocity; // La velocidad del avi�n en el espacio

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero; // Inicializar la velocidad en cero
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
        if (Input.GetKey(KeyCode.A)) // Girar a la izquierda (alabeo)
        {
            rollInput = -1f;
        }
        if (Input.GetKey(KeyCode.D)) // Girar a la derecha (alabeo)
        {
            rollInput = 1f;
        }
        if (Input.GetKey(KeyCode.W)) // Elevar (pitch hacia arriba)
        {
            pitchInput = 1f;
        }
        if (Input.GetKey(KeyCode.S)) // Bajar (pitch hacia abajo)
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
        velocity += transform.forward * currentThrottle * Time.deltaTime;  // Movimiento hacia adelante

        // Controlar velocidad m�xima
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;  // Limitar la velocidad m�xima
        }

        // Aplicar rotaciones
        float roll = rollInput * rollSpeed * Time.deltaTime;  // Alabeo
        float pitch = pitchInput * pitchSpeed * Time.deltaTime; // Pitch
        float yaw = yawInput * yawSpeed * Time.deltaTime;     // Gui�ada

        // Rotar el avi�n
        transform.Rotate(pitch, yaw, roll); // Aplicar rotaci�n utilizando el m�todo Rotate del Transform

        // Mover el avi�n en la direcci�n del forward de la c�mara
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0f; // Evitar que el avi�n se mueva hacia arriba o abajo en funci�n de la inclinaci�n de la c�mara
        cameraForward.Normalize();  // Normalizar para que la velocidad sea consistente

        // Mover el avi�n en direcci�n al "forward" de la c�mara
        transform.position += cameraForward * velocity.magnitude * Time.deltaTime;
    }
}
