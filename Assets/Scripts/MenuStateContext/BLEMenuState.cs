using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BLEMenuState : MenuState
{
    private bool printDebug = false;
    public override MenuType GetMenuType() => MenuType.BLE;
    
    // Reference to the BLE Component
    [SerializeField] private Text textIsScanning;
    [SerializeField] private Text textTargetDeviceConnection;
    [SerializeField] private Text textTargetDeviceData;
    [SerializeField] private Text textWriteData;
    [SerializeField] private Text textDiscoveredDevices;
    [SerializeField] private Text textSimpleInfo;
    [SerializeField] private GameObject debugParent;

    public void Init(string targetDeviceName)
    {
        textTargetDeviceConnection.text = targetDeviceName + " not found.";
        textSimpleInfo.text = $"Not connected, please, press Connect to search for {targetDeviceName} to connect to and wait a little bit.";

        OnNotScanning();

        if (printDebug)
        {
            ShowDebugInfo();
        }
        else
        {
            HideDebugInfo();
        }
    }

    public void ToggleDebug()
    {
        printDebug = !printDebug;
        if (printDebug)
        {
            ShowDebugInfo();
        }
        else
        {
            HideDebugInfo();
        }
    }
    
    public void HideDebugInfo()
    {
        debugParent.SetActive(false);
        textSimpleInfo.gameObject.SetActive(true);
    }
    
    public void ShowDebugInfo()
    {
        debugParent.SetActive(true);
        textSimpleInfo.gameObject.SetActive(false);
    }

    public void CleanUp()
    {
        textDiscoveredDevices.text = "";
        textWriteData.text = "";
        textTargetDeviceData.text = "";
        textIsScanning.text = "";
        textTargetDeviceConnection.text = "";
        textSimpleInfo.text = $"Not connected, please, press Connect to search for the device to connect to and wait a little bit.";
    }
    
    public void UpdateDiscoveredDevices(IDictionary<string, string> discoveredDevices)
    {
        textDiscoveredDevices.text = "";

        foreach (KeyValuePair<string, string> entry in discoveredDevices)
        {
            textDiscoveredDevices.text += "DeviceID: " + entry.Key + "\nDeviceName: " + entry.Value + "\n\n";
            Debug.Log("Added device: " + entry.Key);
            textSimpleInfo.text = $"Please, wait while we are looking for the device. We are finding something ...";
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
        textSimpleInfo.text = $"Please, wait while we are looking for {targetDeviceName} ...";
    }
    
    public void ReadingThreadTimedOut(float readingTimer)
    {
        textTargetDeviceConnection.text = "Reading thread is timed out, disconnecting ...";
        textDiscoveredDevices.text = "Discovered devices reset.";
        textTargetDeviceData.text = $"Have not been able to get new data for {readingTimer} seconds";

        textSimpleInfo.text = "Something went wrong with the connection. Please, try to Disconnect and Connect again.";
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
        textSimpleInfo.text = "Please, wait, restarting the connection ...";
    }
    
    public void UpdateConnectedText(string targetDeviceName)
    {
        textTargetDeviceConnection.text = "Connected to target device:\n" + targetDeviceName;
        textSimpleInfo.text = "Connected! Nothing to do here, please, press Next to continue.";
    }
    
    public void DeviceFoundNotConnected(string targetDeviceName)
    {
        textTargetDeviceConnection.text = "Found target device:\n" + targetDeviceName;
        textSimpleInfo.text = $"Good news, found {targetDeviceName}! But have not connected to it yet, please, wait.";
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
