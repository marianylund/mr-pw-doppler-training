using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BLEMenuState : MenuState
{
    public bool printDebug = true;
    public override MenuType GetMenuType() => MenuType.BLE;
    
    // Reference to the BLE Component
    [SerializeField] private Text textIsScanning;
    [SerializeField] private Text textTargetDeviceConnection;
    [SerializeField] private Text textTargetDeviceData;
    [SerializeField] private Text textWriteData;
    [SerializeField] private Text textDiscoveredDevices;

    public void Init(string targetDeviceName)
    {
        textTargetDeviceConnection.text = targetDeviceName + " not found.";
        OnNotScanning();
    }

    public void CleanUp()
    {
        textDiscoveredDevices.text = "";
        textWriteData.text = "";
        textTargetDeviceData.text = "";
        textIsScanning.text = "";
        textTargetDeviceConnection.text = "";
    }
    
    public void UpdateDiscoveredDevices(IDictionary<string, string> discoveredDevices)
    {
        textDiscoveredDevices.text = "";
        foreach (KeyValuePair<string, string> entry in discoveredDevices)
        {
            textDiscoveredDevices.text += "DeviceID: " + entry.Key + "\nDeviceName: " + entry.Value + "\n\n";
            Debug.Log("Added device: " + entry.Key);
        }
    }

    public void OnNotScanning()
    {
        if (textIsScanning.text != "Not scanning.")
        {
            textIsScanning.color = Color.white;
            textIsScanning.text = "Not scanning.";
        }
    }
    
    public void UpdateTextOnDisconnect(string targetDeviceName)
    {
        textTargetDeviceConnection.text = "Disconnected from " + targetDeviceName;
        textTargetDeviceData.text = "";
        textIsScanning.text = "";
        textDiscoveredDevices.text = "";
    }
    
    public void OnScanStart(string targetDeviceName, string serviceUuid, string[] characteristicUuids)
    {
        textIsScanning.color = new Color(244, 180, 26);
        textIsScanning.text = "Scanning...";
        textIsScanning.text +=
            $"Searching for {targetDeviceName} with \nservice {serviceUuid} and \ncharacteristic {characteristicUuids[0]}";
        textDiscoveredDevices.text = "";
    }
    
    public void ReadingThreadTimedOut(float readingTimer)
    {
        textTargetDeviceConnection.text = "Reading thread is timed out, disconnecting ...";
        textDiscoveredDevices.text = "Discovered devices reset.";
        textTargetDeviceData.text = $"Have not been able to get new data for {readingTimer} seconds";
    }
    
    public void NoNewData(float readingTimer)
    {
        if (!printDebug)
            return;
        textTargetDeviceData.text = $"Have not been able to get new data for {readingTimer} seconds";
        Debug.Log(textTargetDeviceData.text);
    }
    
    public void UpdateReadData(string data)
    {
        if (!printDebug)
            return;
        textTargetDeviceData.text = data;
    }

    public void UpdateWriteData(string data)
    {
        if (!printDebug)
            return;
        textWriteData.text = data;
    }
    
    public void OnRestartingConnection(string targetDeviceName)
    {
        textTargetDeviceConnection.text = targetDeviceName + " not found. Restarted ...";
    }
    
    public void UpdateConnectedText(string targetDeviceName)
    {
        textTargetDeviceConnection.text = "Connected to target device:\n" + targetDeviceName;
    }
    
    public void DeviceFoundNotConnected(string targetDeviceName)
    {
        textTargetDeviceConnection.text = "Found target device:\n" + targetDeviceName;
    }

    public override void Show()
    {
        gameObjectMenu.SetActive(true);
    }

    public override void Hide()
    {
        gameObjectMenu.SetActive(false);
    }
}
