using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class BLEBehaviour : MonoBehaviour
{
    public delegate void BLEEvent(Quaternion rotation);
    public BLEEvent OnDataRead;

    [SerializeField] private BLEMenuState menu;
    
    // Change this to match your device.
    private string _targetDeviceName = "Arduino";
    private string _serviceUuid = "{19b10000-e8f2-537e-4f6c-d104768a1214}";

    private readonly string[] _characteristicUuids = {
        "{19b10001-e8f2-537e-4f6c-d104768a1214}",      // writeData
        "{19b10002-e8f2-537e-4f6c-d104768a1214}",      // bool if previous has been changed
        // "{19b10003-e8f2-537e-4f6c-d104768a1214}",      // unused
        "{19b10004-e8f2-537e-4f6c-d104768a1214}",      // readData
    };

    private BLE _ble;
    private BLE.BLEScan _scan;
    public bool isScanning, isConnected, hasTriedScanning, hasDisconnected;
    private string _deviceId;
    private readonly IDictionary<string, string> _discoveredDevices = new Dictionary<string, string>();
    private int _devicesCount;
    
    private byte[] _valuesToWrite;
    private Quaternion _newRotation;
    private string _result;
    
    // If has not gotten answer before the time runs out, disconnect from Arduino and try again
    private float _readingTimeOut = 3f;
    private float _readingTimer;

    // BLE Threads 
    private Thread _scanningThread, _connectionThread, _readingThread, _writingThread;

    private Coroutine _restartingCoroutine;
    private bool isRestarting;
    
    // For handling UI
    private void Start()
    {
        _ble = new BLE();
        _readingThread = new Thread(ReadBleData);
        menu.Init(_targetDeviceName);
        
        StartScanHandler();
    }

    private void Update()
    {
        if (isRestarting)
            return;
        
        if (isScanning)
        {
            if (_discoveredDevices.Count > _devicesCount)
            {
                menu.UpdateDiscoveredDevices(_discoveredDevices);
                _devicesCount = _discoveredDevices.Count;
            }                
        } else
        {
            menu.OnNotScanning();
        }
        
        if (hasDisconnected)
        {
            menu.UpdateTextOnDisconnect(_targetDeviceName);
            hasDisconnected = false;
        }

        // The target device was found.
        if (_deviceId != null && _deviceId != "-1")
        {
            // Target device is connected and GUI knows.
            if (_ble.isConnected && isConnected)
            {
                TryToReadData();
            }
            // Target device is connected, but GUI hasn't updated yet.
            else if (_ble.isConnected && !isConnected)
            {
                menu.UpdateConnectedText(_targetDeviceName);
                isConnected = true; 
            // Device was found, but not connected yet. 
            } else if (!isConnected)
            {
                menu.DeviceFoundNotConnected(_targetDeviceName);
            } 
        }
    }

    public void StartScanHandler()
    {
        if (!hasTriedScanning)
        {
            _devicesCount = 0;
            isScanning = true;
            hasTriedScanning = true;
            _discoveredDevices.Clear();
            _scanningThread = new Thread(ScanBleDevices);
            _scanningThread.Start();
            menu.OnScanStart(_targetDeviceName, _serviceUuid, _characteristicUuids);
        }
        else if (!_ble.isConnected) // Do not reset if it is connected and well
        {
            CleanUp();
            _ble = new BLE();
            menu.OnRestartingConnection(_targetDeviceName);
            hasTriedScanning = false;
            StartScanHandler();
        }
    }

    private void ScanBleDevices()
    {
        try
        {
            _scan = BLE.ScanDevices();
            Debug.Log("BLE.ScanDevices() started.");
            _scan.Found = (deviceId, deviceName) =>
            {
                if (!_discoveredDevices.ContainsKey(deviceId))
                {
                    Debug.Log("found device with name: " + deviceName);
                    _discoveredDevices.Add(deviceId, deviceName);
                }

                if (this._deviceId == null && deviceName == _targetDeviceName)
                {
                    this._deviceId = deviceId;
                }
            };

            _scan.Finished = () =>
            {
                isScanning = false;
                Debug.Log("scan finished");
                _deviceId ??= "-1";
            };
            while (_deviceId == null) 
                Thread.Sleep(500);
            _scan.Cancel();
            _scanningThread = null;
            isScanning = false;
        
            if (_deviceId == "-1")
            {
                Debug.Log($"Scan is finished. {_targetDeviceName} was not found.");
                Disconnect();
                return;
            }
            Debug.Log($"Found {_targetDeviceName} device with id {_deviceId}.");
            StartConHandler();
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not scan for new devices " + e);
        }
        
    }

    private void StartConHandler()
    {
        try
        {
            if (!_connectionThread?.IsAlive ?? true)
            {
                _connectionThread = new Thread(ConnectBleDevice);
                _connectionThread.Start();
            }
            else
            {
                Debug.LogWarning("Connection thread is still running, but it is trying to connect again");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Could not start connecting to device with ID " + _deviceId + "\n" + e);
        }
    }

    private void ConnectBleDevice()
    {
        if (_deviceId != null)
        {
            try
            {
                Debug.Log($"Attempting to connect to {_targetDeviceName} device with id {_deviceId} ...");
                _ble.Connect(_deviceId,
                    _serviceUuid,
                    _characteristicUuids);
            } catch(Exception e)
            {
                Debug.LogWarning("Could not establish connection to device with ID " + _deviceId + "\n" + e);
                Disconnect();
            }
        }
        if (_ble.isConnected)
            Debug.Log("Connected to: " + _targetDeviceName);
    }

    public void Disconnect()
    {
        CleanUp();
        _ble = new BLE();
        hasTriedScanning = false;
        hasDisconnected = true;
    }

    private void TryToReadData()
    {
        if (isConnected && !_readingThread.IsAlive)
        {
            _readingThread = new Thread(ReadBleData);
            _readingThread.Start();

            menu.UpdateReadData("Rot: " + _newRotation.eulerAngles);
            OnDataRead?.Invoke(_newRotation);
            _readingTimer = 0f;
        }
        else if (_readingTimer > _readingTimeOut)
        {
            CleanUp();
            _ble = new BLE();
            menu.ReadingThreadTimedOut(_readingTimer);
            _readingTimer = 0f;
        }
        else
        {
            if (_readingTimer > _readingTimeOut / 2.0f)
            {
                menu.NoNewData(_readingTimer);
            }
            _readingTimer += Time.deltaTime;
        }
    }

    public void StartWritingHandler(Quaternion newCalibratedRotation)
    {
        if (_deviceId == "-1" || !isConnected || (_writingThread?.IsAlive ?? false))
        {
            Debug.Log($"Cannot write yet. DeviceID: {_deviceId}, isConnected: {isConnected}, writingThread: {_writingThread?.IsAlive}");
            return;
        }

        string strValues = $"{newCalibratedRotation.x},{newCalibratedRotation.y},{newCalibratedRotation.z},{newCalibratedRotation.w};";
        menu.UpdateWriteData("Writing some new: " + strValues);
        
        _valuesToWrite = Encoding.ASCII.GetBytes(strValues);
        _writingThread = new Thread(WriteBleData);
        _writingThread.Start();
    }
    
    private void WriteBleData()
    {
        bool ok = BLE.WritePackage(_deviceId,
            _serviceUuid,
            _characteristicUuids[0],
            _valuesToWrite);
        if(!ok)
            Debug.Log(BLE.GetError());
        // Notify the central that the value is updated
        byte[] bytes = new byte[] {1};
        ok = BLE.WritePackage(_deviceId,
            _serviceUuid,
            _characteristicUuids[1],
            bytes);
        if(!ok)
            Debug.Log(BLE.GetError());
        _writingThread = null;
    }

    private void ReadBleData(object obj)
    {
        // Go through all packages until it is the newest correct package
        byte[] prevPackage = BLE.ReadBytes(out string prevCharId);
        _readingTimer = 0f;
        byte[] packageReceived = BLE.ReadBytes(out string charId);
        _readingTimer = 0f;
        while (packageReceived.Length != 1 && packageReceived[0] == 0x0)
        {
            if (charId == _characteristicUuids[2])
            {
                prevPackage = packageReceived;
                prevCharId = charId;
            }
            packageReceived = BLE.ReadBytes(out charId);
            _readingTimer = 0f;
        }
        if (charId == _characteristicUuids[0])
        {
            Debug.Log("Reading data from writeCharacteristic: " + Encoding.UTF8.GetString(packageReceived));
            return;
        }

        if (prevCharId == _characteristicUuids[2])
        {
            // TODO: one day go through string split by comma and then stop when meeting ;
            _result = Encoding.UTF8.GetString(prevPackage).Split(';')[0]; // ; signals the end of the message data
            // Quaternion arrives of the form: f,f,f,f; where f is a float
            //Debug.Log("result: " + result);
            string[] splitResult = _result.Split(',');
            float x = float.Parse(splitResult[0]);
            float y = float.Parse(splitResult[1]);
            float z = float.Parse(splitResult[2]);
            float w = float.Parse(splitResult[3]);
            
            _newRotation = new Quaternion(x, y, z, w);
        }
    }
    
    private void OnDestroy()
    {
        CleanUp();
    }

    private void OnApplicationQuit()
    {
        CleanUp();
    }

    private void StartCleaningUp()
    {
        if (isRestarting)
        {
            if (_restartingCoroutine != null) StopCoroutine(_restartingCoroutine);
            throw new WarningException("Starting to clean when already restarting");
        }

        isRestarting = true;
        _restartingCoroutine = StartCoroutine(CleaningUp());
    }

    IEnumerator CleaningUp()
    {
        CleanUp();
        yield return new WaitForEndOfFrame();
        isRestarting = false;
    }

    // Prevent threading issues and free BLE stack.
    // Can cause Unity to freeze and lead
    // to errors when omitted.
    private void CleanUp()
    {
        try
        {
            isScanning = false;
            isConnected = false;
            hasTriedScanning = false;
            _readingTimer = 0f;
            _deviceId = null;
            _discoveredDevices.Clear();
            _scan.Cancel();
            _ble.Close();
            _scanningThread.Abort();
            _connectionThread.Abort();
            _readingThread.Abort();
            _writingThread.Abort();
            menu.CleanUp();

        } catch(NullReferenceException e)
        {
            Debug.Log("Thread or object never initialized.\n" + e);
        }        
    }

}
