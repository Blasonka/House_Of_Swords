using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraSpeedKeyboard = 10f;
    public float cameraSpeedMouse = 1f;
    public float cameraDragSensitivity = 5f;

    private KeyCode[] movementKeys = new KeyCode[4] { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A }; // FEL JOBBRA LE BALRA

    private GameObject topLeftBorder;
    private GameObject bottomRightBorder;

    private BuildingsManager buildingsManager;

    private Vector3 initialMousePos;

    /// <summary>
    /// F�ggv�ny a kamera-mozg�s billenty�inek be�ll�t�s�ra.
    /// </summary>
    /// <param name="up">Felfel� mozg�s - null, ha nem v�ltozik.</param>
    /// <param name="right">Jobbra mozg�s - null, ha nem v�ltozik.</param>
    /// <param name="down">Lefel� mozg�s - null, ha nem v�ltozik.</param>
    /// <param name="left">Balra mozg�s - null, ha nem v�ltozik.</param>
    public void changeMovementKeys(KeyCode? up = null, KeyCode? right = null, KeyCode? down = null, KeyCode? left = null)
    {
        if (up != null) movementKeys[0] = (KeyCode)up;
        if (right != null) movementKeys[1] = (KeyCode)right;
        if (down != null) movementKeys[2] = (KeyCode)down;
        if (left != null) movementKeys[3] = (KeyCode)left;
    }

    private void clampCameraToBounds()
    {
        if (transform.position.x < topLeftBorder.transform.position.x) transform.position = new Vector3(topLeftBorder.transform.position.x, transform.position.y, transform.position.z);
        if (transform.position.x > bottomRightBorder.transform.position.x) transform.position = new Vector3(bottomRightBorder.transform.position.x, transform.position.y, transform.position.z);
        if (transform.position.y > topLeftBorder.transform.position.y) transform.position = new Vector3(topLeftBorder.transform.position.x, topLeftBorder.transform.position.y, transform.position.z);
        if (transform.position.y < bottomRightBorder.transform.position.y) transform.position = new Vector3(topLeftBorder.transform.position.x, bottomRightBorder.transform.position.y, transform.position.z);
    }

    private void Start()
    {
        topLeftBorder = GameObject.Find("TopLeftBorder");
        bottomRightBorder = GameObject.Find("BottomRightBorder");

        buildingsManager = GameObject.Find("BuildingsManager").GetComponent<BuildingsManager>();
    }

    private void Update()
    {
        // A KAMERA VISSZA�LL�T�SA ALAPHELYZETBE (TESZTEL�SHEZ)
        if (Input.GetKeyDown(KeyCode.R)) transform.position = new Vector3(0, 0, -10);

        // KAMERAMOZG�S EG�RREL
        if (Input.GetMouseButtonDown(0))
        {
            initialMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0) && !buildingsManager.windowIsOpen)
        {
            Vector3 delta = (Input.mousePosition - initialMousePos) * cameraSpeedMouse;


            delta *= Time.deltaTime;
            transform.position -= delta;

            clampCameraToBounds();

            //Mathf.Clamp(transform.position.x, topLeftBorder.transform.position.x, bottomRightBorder.transform.position.x);
            //Mathf.Clamp(transform.position.y, bottomRightBorder.transform.position.y, topLeftBorder.transform.position.y);

            //if (isWithinBounds(transform.position - delta))
            //{
            //}

            initialMousePos = Input.mousePosition;
            return;
        }

        // KAMERAMOZG�S BILLENTY�KKEL, HA AZ EG�RREL NEM MOZOG
        if (Input.GetKey(movementKeys[0]) && transform.position.y < topLeftBorder.transform.position.y) transform.position = new Vector3(transform.position.x, transform.position.y + cameraSpeedKeyboard * Time.deltaTime, transform.position.z);
        if (Input.GetKey(movementKeys[1]) && transform.position.x < bottomRightBorder.transform.position.x) transform.position = new Vector3(transform.position.x + cameraSpeedKeyboard * Time.deltaTime, transform.position.y, transform.position.z);
        if (Input.GetKey(movementKeys[2]) && transform.position.y > bottomRightBorder.transform.position.y) transform.position = new Vector3(transform.position.x, transform.position.y - cameraSpeedKeyboard * Time.deltaTime, transform.position.z);
        if (Input.GetKey(movementKeys[3]) && transform.position.x > topLeftBorder.transform.position.x) transform.position = new Vector3(transform.position.x - cameraSpeedKeyboard * Time.deltaTime, transform.position.y, transform.position.z);
    }
}
