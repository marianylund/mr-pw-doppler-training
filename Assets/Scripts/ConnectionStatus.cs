using System;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatus : MonoBehaviour
{
    public bool ShowBLE = true;
    [SerializeField] private BLEBehaviour _ble;
    [SerializeField] private Text status;

    private readonly Color connectedColour = Color.Lerp(Color.white, Color.green, 0.3f);
    private readonly Color disconnectedColour = Color.Lerp(Color.white, Color.red, 0.3f);

    private bool prevConnection;

    private void Start()
    {
        if (!ShowBLE)
        {
            status.text = "";
        }
        Debug.Log("BLE is disabled, so turning off ConnectionStatus printing");
        this.enabled = false;
    }

    private void Update()
    {
        if (prevConnection != _ble.isConnected)
        {
            prevConnection = _ble.isConnected;
            BleConnection(_ble.isConnected);
        }
    }

    private void BleConnection(bool connected)
    {
        if (connected)
        {
            status.text = "BLE\nConnected";
            status.color = connectedColour;
        }
        else
        {
            status.text = "BLE\nDisconnected";
            status.color = disconnectedColour;
        }
    }

}
