using UnityEngine;

public class RaycastAngle : MonoBehaviour
{
    public delegate void OnRaycastAngle(int newAngle, float overlap);
    public OnRaycastAngle valueUpdate;
    [SerializeField] private UltrasoundVisualiser visualiser;
    [SerializeField] private GameObject AngleTextObject;
    public float CurrentAngle { get; private set; }
    private int previousAngle;
    private float previousOverlap = Mathf.NegativeInfinity;
    private float overlapAccuracy = 0.1f;
    
    private bool _notifiedAboutNoIntersection = false;

    private DepthWindow depthWindow;

    private void Start()
    {
        depthWindow = GetComponent<DepthWindow>();
    }

    void FixedUpdate()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;
        int i = 0;
        Vector3 localForward = transform.TransformDirection(Vector3.forward);
        Vector3 topHit = Vector3.negativeInfinity, bottomHit = Vector3.negativeInfinity;
    
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, localForward, out hit, Mathf.Infinity, layerMask))
        {
            topHit = hit.point;
            Debug.DrawRay(transform.position, localForward * hit.distance, Color.yellow);
            if(Physics.Raycast(transform.position + localForward, -localForward, out hit, Mathf.Infinity, layerMask))
            {
                
                bottomHit = hit.point;
                // Debug.DrawRay(bottomHit, transform.TransformDirection(Vector3.right), Color.green);
            }
            float overlap = depthWindow.CalculateOverlap(topHit, bottomHit);
            
            float angle = Vector3.Angle(transform.forward, hit.transform.forward);
            float cosAngle = Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad));
            previousAngle = Mathf.RoundToInt(CurrentAngle);
            CurrentAngle = (Mathf.Acos(cosAngle) * Mathf.Rad2Deg);
            
            int currentAngleRounded = Mathf.RoundToInt(CurrentAngle);
            if (currentAngleRounded != previousAngle)
            {
                SampleUtil.AssignStringToTextComponent(AngleTextObject ? AngleTextObject : gameObject, "Angle:\n" + currentAngleRounded);
                valueUpdate?.Invoke(currentAngleRounded, overlap);
                Debug.Log("Notifying different overlap because of angle: " + overlap);
                previousOverlap = overlap;
                visualiser.OnIntersecting(-30 < currentAngleRounded && currentAngleRounded < 30);
                _notifiedAboutNoIntersection = false;
            }else if (Mathf.Abs(overlap - previousOverlap) > overlapAccuracy)
            {
                Debug.Log("Notifying different overlap: " + overlap + " prev: " + previousOverlap);
                valueUpdate?.Invoke(currentAngleRounded, overlap);
                previousOverlap = overlap;
            }
            //angle.text = (Mathf.Acos(cosAngle) * Mathf.Rad2Deg).ToString();
            //Debug.Log("Did Hit " + hit.transform.name + ", angle:  " + angle + " cos: " + cosAngle + "acos: " + Mathf.Acos(cosAngle) * Mathf.Rad2Deg);
            
            // Hit back:
            //Debug.DrawRay(topHit, transform.TransformDirection(Vector3.right), Color.magenta);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //SampleUtil.AssignStringToTextComponent(AngleTextObject ? AngleTextObject : gameObject, "Angle: ?");
            if (!_notifiedAboutNoIntersection)
            {
                visualiser.OnNoIntersect();
                _notifiedAboutNoIntersection = true;
                //Debug.Log("Notified " + _notifiedAboutNoIntersection);
                CurrentAngle = -1000;
            }
        }
        
    }

    
}
