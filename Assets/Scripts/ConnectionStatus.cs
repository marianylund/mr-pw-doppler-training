using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatus : MonoBehaviour
{
    [SerializeField] private BLEBehaviour _ble;
    [SerializeField] private Text status;

    private bool prevConnection;
    
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
            status.color = Color.Lerp(Color.white, Color.green, 0.3f);
        }
        else
        {
            status.text = "BLE\nDisconnected";
            status.color = Color.Lerp(Color.white, Color.red, 0.3f);
        }
    }

}
