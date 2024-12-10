using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControll : MonoBehaviour
{
    public float maxSpeed = 200f;  // Velocidad máxima
    public float pitchSpeed = 5f;  // Velocidad de rotación alrededor del eje X (pitch)
    public float rollSpeed = 5f;  // Velocidad de rotación alrededor del eje Y (alabeo)
    public float yawSpeed = 5f;   // Velocidad de rotación alrededor del eje Z (guiñada)
    public float thrust = 50f;     // Potencia del motor (aceleración)

    public Camera mainCamera;      // Referencia a la cámara principal

    private float rollInput;
    private float pitchInput;
    private float yawInput;
    private float throttleInput;

    private Vector3 velocity; // La velocidad del avión en el espacio

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero; // Inicializar la velocidad en cero
    }

    // Update is called once per frame
    void Update()
    {
        // Leer las entradas del teclado para controlar la rotación y la aceleración
        rollInput = 0f;
        pitchInput = 0f;
        yawInput = 0f;
        throttleInput = 0f;

        // Controles de rotación (alabeo, pitch, guiñada)
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
        if (Input.GetKey(KeyCode.Q)) // Girar a la izquierda (guiñada)
        {
            yawInput = -1f;
        }
        if (Input.GetKey(KeyCode.E)) // Girar a la derecha (guiñada)
        {
            yawInput = 1f;
        }

        // Controlar la aceleración
        if (Input.GetKey(KeyCode.UpArrow)) // Acelerar (throttle hacia adelante)
        {
            throttleInput = 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow)) // Desacelerar (throttle hacia atrás)
        {
            throttleInput = -1f;
        }

        // Aplicar las fuerzas de movimiento y rotación
        MoveAircraft();
    }

    void MoveAircraft()
    {
        // Aceleración
        float currentThrottle = throttleInput * thrust;
        velocity += transform.forward * currentThrottle * Time.deltaTime;  // Movimiento hacia adelante

        // Controlar velocidad máxima
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;  // Limitar la velocidad máxima
        }

        // Aplicar rotaciones
        float roll = rollInput * rollSpeed * Time.deltaTime;  // Alabeo
        float pitch = pitchInput * pitchSpeed * Time.deltaTime; // Pitch
        float yaw = yawInput * yawSpeed * Time.deltaTime;     // Guiñada

        // Rotar el avión
        transform.Rotate(pitch, yaw, roll); // Aplicar rotación utilizando el método Rotate del Transform

        // Mover el avión en la dirección del forward de la cámara
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0f; // Evitar que el avión se mueva hacia arriba o abajo en función de la inclinación de la cámara
        cameraForward.Normalize();  // Normalizar para que la velocidad sea consistente

        // Mover el avión en dirección al "forward" de la cámara
        transform.position += cameraForward * velocity.magnitude * Time.deltaTime;
    }
}
