using UnityEngine;

public class RaycastAngle : MonoBehaviour
{
    public delegate void OnRaycastAngle(int newAngle);
    public OnRaycastAngle valueUpdate;

    [SerializeField] private GameObject AngleTextObject;
    public float CurrentAngle { get; private set; }
    private int previousAngle;
    void FixedUpdate()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;
    
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            float angle = Vector3.Angle(transform.forward, hit.transform.forward);
            float cosAngle = Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad));
            previousAngle = Mathf.RoundToInt(CurrentAngle);
            CurrentAngle = (Mathf.Acos(cosAngle) * Mathf.Rad2Deg);
            
            int currentAngleRounded = Mathf.RoundToInt(CurrentAngle);
            if (currentAngleRounded != previousAngle)
            {
                SampleUtil.AssignStringToTextComponent(AngleTextObject ? AngleTextObject : gameObject, "Angle:\n" + currentAngleRounded);
                valueUpdate?.Invoke(currentAngleRounded);
            }
            //angle.text = (Mathf.Acos(cosAngle) * Mathf.Rad2Deg).ToString();
            //Debug.Log("Did Hit " + hit.transform.name + ", angle:  " + angle + " cos: " + cosAngle + "acos: " + Mathf.Acos(cosAngle) * Mathf.Rad2Deg);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //SampleUtil.AssignStringToTextComponent(AngleTextObject ? AngleTextObject : gameObject, "Angle: ?");
            //Debug.Log("Did not Hit");
        }
        
    }
}
