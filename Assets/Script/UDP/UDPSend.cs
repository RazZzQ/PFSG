using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;
using TMPro;
public class UDPSend : MonoBehaviour
{

    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA();


    private string IP;  // define in init
    public int port;  // define in init
    public TextMeshPro engineA;
    public Text engineAHex;
    public Slider sliderA;
    public TextMeshPro engineB;
    public Text engineBHex;
    public Slider sliderB;
    public TextMeshPro engineC;
    public Text engineCHex;
    public Slider sliderC;

    public Text Data;

    UdpClient client;

    public bool active = false;

    public float SmoothEngine = 0.5f;

    public float A = 0, B = 0, C = 0, longg;

    public Transform vehicle;
    private float servoPitch;
    private float servoRoll;
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

        //A = 125;
        //B = 125;
        //C = 125;

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
        void AplicarLerp(ref float variable, float objetivo, float suavizado)
        {
            variable = Mathf.Lerp(variable, objetivo, Time.deltaTime * suavizado);
        }


        // Calcular la rotación en el eje X
        if (vehicle.eulerAngles.x > 2f && vehicle.eulerAngles.x < 180f)
        {
            AplicarLerp(ref A, 200f, SmoothEngine);
            AplicarLerp(ref B, 0f, SmoothEngine);
            AplicarLerp(ref C, 0f, SmoothEngine);
        }
        else if (vehicle.eulerAngles.x >= 180f && vehicle.eulerAngles.x <= 360f)
        {
            AplicarLerp(ref A, 0f, SmoothEngine);
            AplicarLerp(ref B, 200f, SmoothEngine);
            AplicarLerp(ref C, 200f, SmoothEngine);
        }
        else
        {
            AplicarLerp(ref A, 100f, SmoothEngine);
            AplicarLerp(ref B, 100f, SmoothEngine);
            AplicarLerp(ref C, 100f, SmoothEngine);
        }

        // Calcular la rotación en el eje Z
        if (vehicle.eulerAngles.z > 2f && vehicle.eulerAngles.z < 180f)
        {
            AplicarLerp(ref B, 200f, SmoothEngine);
            AplicarLerp(ref C, 0f, SmoothEngine);
        }
        else if (vehicle.eulerAngles.z >= 180f && vehicle.eulerAngles.z <= 360f)
        {
            AplicarLerp(ref B, 0f, SmoothEngine);
            AplicarLerp(ref C, 200f, SmoothEngine);
        }
        else
        {
            AplicarLerp(ref B, 100f, SmoothEngine);
            AplicarLerp(ref C, 100f, SmoothEngine);
        }
        // Debug values for verification
        Debug.Log($"Servo A: {A}, Servo B: {B}, Servo C: {C}");

        // Convert to hexadecimal (example function: DecToHexMove)
        string hexA = DecToHexMove(A);
        string hexB = DecToHexMove(B);
        string hexC = DecToHexMove(C);

        // Assign to mUDPDATA fields
        mUDPDATA.mAppDataField.PlayMotorA = hexA;
        mUDPDATA.mAppDataField.PlayMotorB = hexB;
        mUDPDATA.mAppDataField.PlayMotorC = hexC;

        // Send updated data
        sendString(mUDPDATA.GetToString());
    }

    void FixedUpdate()
    {
        //Debug.Log("CalcularRotacion ejecutándose..."); 
        //Debug.Log($"Current Rotation: {vehicle.rotation.eulerAngles}");

        if (active)
        {

            CalcularRotacion();

            /*sliderA.value = A;
            sliderB.value = B;
            sliderC.value = C;*/

            string HexA = DecToHexMove(A);
            string HexB = DecToHexMove(B);
            string HexC = DecToHexMove(C);

            /*engineAHex.text = "Engine A: " + HexA;
            engineBHex.text = "Engine B: " + HexB;
            engineCHex.text = "Engine C: " + HexC;*/

            mUDPDATA.mAppDataField.PlayMotorC = HexC;
            mUDPDATA.mAppDataField.PlayMotorA = HexA;
            mUDPDATA.mAppDataField.PlayMotorB = HexB;


            engineA.text = ((int)A).ToString();
            engineB.text = ((int)B).ToString();
            engineC.text = ((int)C).ToString();

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


