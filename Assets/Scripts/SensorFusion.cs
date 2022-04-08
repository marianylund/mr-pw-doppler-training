using UnityEngine;
using Vuforia;

public class SensorFusion : MonoBehaviour
{
    public Transform bleGyroObject;
    public Transform vuforiaProbeObj;
    public Transform mergedObject;
    private BLEBehaviour _ble;
    private TargetStatus status;
    
    private Quaternion _lastBleRotation = Quaternion.identity;
    
    void Start()
    {
        _ble = GetComponent<BLEBehaviour>();
        Debug.Assert(_ble != null, "Requires BLE components to get the data from");
        _ble.OnDataRead += GetData;
    }
    
    /// <summary>
    /// Public method to be called by an EventHandler's Lost/Found Events
    /// </summary>
    /// <param name="observerBehaviour"></param>
    public void TargetStatusChanged(ObserverBehaviour observerBehaviour)
    {
        if (_ble == null)
            return;
        status = observerBehaviour.TargetStatus;
        if (status.Status != Status.TRACKED)
        {
            _ble.StartWritingHandler(vuforiaProbeObj.localRotation);
        }
    }

    /// <summary>
    /// Simulating data from Vuforia as if the Vuforia is tracking
    /// want to apply data from BLE based in the last Vuforia Rotation
    /// </summary>
    public void CalibrateBLEVuforia()
    {
       //Quaternion diff = vuforiaProbeObj.rotation * Quaternion.Inverse(_lastBleRotation);
        //Debug.Log("Rotation difference: " + diff + ", euler: " + diff.eulerAngles);
        _ble.StartWritingHandler(vuforiaProbeObj.localRotation);
    }

    private void FixedUpdate()
    {
        // TODO: how to set it as world rotation, without affecting the parent
        bleGyroObject.rotation = _lastBleRotation;
        if (status.Status == Status.TRACKED && status.StatusInfo == StatusInfo.NORMAL)
        {
            mergedObject.localPosition = vuforiaProbeObj.localPosition;
            mergedObject.localRotation = vuforiaProbeObj.localRotation;
            //mergedObject.SetPositionAndRotation(vuforiaProbeObj.position, vuforiaProbeObj.rotation);
        }
        else
        {
            mergedObject.localPosition = vuforiaProbeObj.localPosition;
            mergedObject.localRotation = _lastBleRotation;
        }
    }

    private void GetData(Quaternion rotation)
    {
        //selectedObject.rotation = Quaternion.Euler(eulerRotation);
        _lastBleRotation = rotation;
        //_lastBleRotation = Quaternion.Inverse(rotation);
    }

    private void OnDestroy()
    {
        _ble.OnDataRead -= GetData;
    }
}
