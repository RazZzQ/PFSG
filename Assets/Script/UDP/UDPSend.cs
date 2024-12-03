using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class UDPSend : MonoBehaviour
{

    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA();


    private string IP;  // define in init
    public int port;  // define in init
    public Text engineA;
    public Text engineAHex;
    public Slider sliderA;
    public Text engineB;
    public Text engineBHex;
    public Slider sliderB;
    public Text engineC;
    public Text engineCHex;
    public Slider sliderC;

    public Text Data;

    UdpClient client;

    public bool active = false;

    public float SmoothEngine = 0.5f;

    public float A = 0, B = 0, C = 0, longg;

    public Transform vehicle;

    // start from unity3d
    public void Start()
    {
        init();
    }
    public void init()
    {

        // define
        IP = "192.168.15.201";
        port = 7408;

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient(53342);


        // AppControlField
        mUDPDATA.mAppControlField.ConfirmCode = "55aa";
        mUDPDATA.mAppControlField.PassCode = "0000";
        mUDPDATA.mAppControlField.FunctionCode = "1301";
        // AppWhoField
        mUDPDATA.mAppWhoField.AcceptCode = "ffffffff";
        mUDPDATA.mAppWhoField.ReplyCode = "";//"00000001";
                                             // AppDataField
        mUDPDATA.mAppDataField.RelaTime = "00000064";
        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        mUDPDATA.mAppDataField.PortOut = "12345678";

        A = 125;
        B = 125;
        C = 125;

        sliderA.value = A;
        sliderB.value = B;
        sliderC.value = C;

        string HexA = DecToHexMove(A);
        string HexB = DecToHexMove(B);
        string HexC = DecToHexMove(C);

        engineAHex.text = "Engine A: " + HexA;
        engineBHex.text = "Engine B: " + HexB;
        engineCHex.text = "Engine C: " + HexC;

        mUDPDATA.mAppDataField.PlayMotorC = HexC;
        mUDPDATA.mAppDataField.PlayMotorA = HexA;
        mUDPDATA.mAppDataField.PlayMotorB = HexB;


        engineA.text = ((int)sliderA.value).ToString();
        engineB.text = ((int)sliderB.value).ToString();
        engineC.text = ((int)sliderC.value).ToString();

        Data.text = "Data: " + mUDPDATA.GetToString();

        sendString(mUDPDATA.GetToString());

        StartCoroutine(UpMovePlatform(3));
    }
    public void ActiveSend()
    {
        active = true;

    }
    public void ResertPositionEngine()
    {

        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        sendString(mUDPDATA.GetToString());

        mUDPDATA.mAppDataField.RelaTime = "00000064";

    }

    IEnumerator UpMovePlatform(float wait)
    {
        active = false;

        yield return new WaitForSeconds(3f);

        active = true;
    }
    void CalcularAltitud()
    {

        //valueMotor = 125;

        //A = (float)(Mathf.Clamp(valueMotor, 0, 250));
        //B = (float)(Mathf.Clamp(valueMotor, 0, 250));
        //C = (float)(Mathf.Clamp(valueMotor, 0, 250));

    }
    void CalcularRotacion()
    {

        //Quaternion rotY = vehicle.rotation;
        //rotY.x = 0;
        //rotY.z = 0;

        //// rotate left or Right
        //Vector3 Vb1 = vehicle.position + vehicle.rotation * Vector3.right * longg;
        //Vector3 Vb2 = vehicle.position + rotY * Vector3.right * longg;

        //// rotate forward or back
        //Vector3 VbA1 = vehicle.position + vehicle.rotation * Vector3.forward * longg;
        //Vector3 VbA2 = vehicle.position + rotY * Vector3.forward * longg;

        // TIENES QUE SEGUIR PROGRAMANDO ACAAAAAAAAAAAAAA

        // Usaremos las variables A, B, C para determinar el comportamiento de la rotación.
        // A: Eje de pitch (arriba / abajo)
        // B: Eje de yaw (giro horizontal)
        // C: Eje de roll (rotación lateral)

        // Normalizamos A, B, C a un rango más manejable si es necesario.
        float pitchInput = Mathf.Clamp(A - 125, -1f, 1f);  // A = 125 es el valor neutral
        float yawInput = Mathf.Clamp(B - 125, -1f, 1f);    // B = 125 es el valor neutral
        float rollInput = Mathf.Clamp(C - 125, -1f, 1f);   // C = 125 es el valor neutral

        // Aplicamos las entradas de rotación a los ejes correspondientes

        // 1. Pitch: Subir y bajar (eje X)
        // Modificamos la rotación alrededor del eje X del avión
        float pitchRotation = pitchInput * 30f;  // A 30 grados de rotación máxima en pitch
        Quaternion newRotationPitch = Quaternion.Euler(pitchRotation, vehicle.rotation.eulerAngles.y, vehicle.rotation.eulerAngles.z);

        // 2. Yaw: Girar a la izquierda o derecha (eje Y)
        // Modificamos la rotación alrededor del eje Y del avión
        float yawRotation = yawInput * 30f;  // A 30 grados de rotación máxima en yaw
        Quaternion newRotationYaw = Quaternion.Euler(vehicle.rotation.eulerAngles.x, vehicle.rotation.eulerAngles.y + yawRotation, vehicle.rotation.eulerAngles.z);

        // 3. Roll: Girar lateralmente (eje Z)
        // Modificamos la rotación alrededor del eje Z del avión
        float rollRotation = rollInput * 30f;  // A 30 grados de rotación máxima en roll
        Quaternion newRotationRoll = Quaternion.Euler(vehicle.rotation.eulerAngles.x, vehicle.rotation.eulerAngles.y, vehicle.rotation.eulerAngles.z + rollRotation);

        // Aplicamos las rotaciones de pitch, yaw y roll juntas
        vehicle.rotation = newRotationYaw * newRotationPitch * newRotationRoll;

        // También se puede usar la física para una simulación más realista con Rigidbody
        // Si tienes un Rigidbody en el avión, podrías usar lo siguiente:
        // Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        // rb.AddTorque(new Vector3(pitchInput, yawInput, rollInput) * torqueMultiplier);

    }

    void FixedUpdate()
    {
        if (active)
        {

            CalcularRotacion();

            sliderA.value = A;
            sliderB.value = B;
            sliderC.value = C;

            string HexA = DecToHexMove(A);
            string HexB = DecToHexMove(B);
            string HexC = DecToHexMove(C);

            engineAHex.text = "Engine A: " + HexA;
            engineBHex.text = "Engine B: " + HexB;
            engineCHex.text = "Engine C: " + HexC;

            mUDPDATA.mAppDataField.PlayMotorC = HexC;
            mUDPDATA.mAppDataField.PlayMotorA = HexA;
            mUDPDATA.mAppDataField.PlayMotorB = HexB;


            engineA.text = ((int)sliderA.value).ToString();
            engineB.text = ((int)sliderB.value).ToString();
            engineC.text = ((int)sliderC.value).ToString();

            Data.text = "Data: " + mUDPDATA.GetToString();

            sendString(mUDPDATA.GetToString());
        }
    }

    void OnApplicationQuit()
    {

        ResertPositionEngine();



        if (client != null)
            client.Close();
        Application.Quit();
    }

    byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    string DecToHexMove(float num)
    {
        int d = (int)((num / 5f) * 10000f);
        return "000" + d.ToString("X");
    }

    private void sendString(string message)
    {

        try
        {
            // Bytes empfangen.
            if (message != "")
            {

                //byte[] data = StringToByteArray(message);
                print(message);
                // Den message zum Remote-Client senden.
                //client.Send(data, data.Length, remoteEndPoint);

            }


        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    void OnDisable()
    {

        if (client != null)
            client.Close();
    }

    private void OnDrawGizmos()
    {

        // rotate left or Right
        Vector3 FG1 = vehicle.position + Vector3.forward * longg;
        Vector3 FG2 = vehicle.position + vehicle.forward * longg;
        Gizmos.color = Color.black;
        Gizmos.DrawLine(FG1, FG2);
        float d = (FG1 - FG2).magnitude;
        float dMax = 5;
        float dN = d / dMax;
        float Increment = dN * 100;
        Vector3 cross = Vector3.Cross(vehicle.forward, Vector3.forward);
        if (cross.x < 0)
            Increment *= -1;
        float FinalValue = 100 + Increment;
        B = Mathf.Lerp(B, FinalValue, Time.deltaTime * 20f);
        B = Mathf.Clamp(B, 0, 200);

        Debug.Log(B);

        #region WorldSpace
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Vector3.forward * longg, 0.5f);
        Gizmos.DrawLine(Vector3.zero, Vector3.forward * longg);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3.right * longg, 0.5f);
        Gizmos.DrawLine(Vector3.zero, Vector3.right * longg);        
       
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Vector3.up * longg, 0.5f);
        Gizmos.DrawLine(Vector3.zero, Vector3.up * longg);
        #endregion

        #region Axis Vechicle
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(vehicle.position + vehicle.forward * longg, 0.5f);
        Gizmos.DrawLine(vehicle.position, vehicle.position + vehicle.forward * longg);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(vehicle.position + vehicle.right * longg, 0.5f);
        Gizmos.DrawLine(vehicle.position, vehicle.position + vehicle.right * longg);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(vehicle.position + vehicle.up * longg, 0.5f);
        Gizmos.DrawLine(vehicle.position, vehicle.position + vehicle.up * longg);
        #endregion

    }

}
