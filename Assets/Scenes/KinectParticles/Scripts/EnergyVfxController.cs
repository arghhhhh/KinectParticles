using UnityEngine;
using MyLib = JossLib.MyUtilityFuncs;

public class EnergyVfxController : MonoBehaviour
{
    public GameObject leftEmitterObj;
    public GameObject rightEmitterObj;
    public GameObject emitterTargetObj;
    private GameObject midPointObj;
    void Start()
    {
        midPointObj = new GameObject("Emitters Midpoint");
        midPointObj.AddComponent<Rigidbody>();
        midPointObj.GetComponent<Rigidbody>().useGravity = false;
        midPointObj.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        midPointObj.transform.parent = gameObject.transform;

        Transform body = gameObject.transform.parent;
        leftEmitterObj.transform.parent = MyLib.FindChildWithTag(body.gameObject, "LeftHand").transform;
        rightEmitterObj.transform.parent = MyLib.FindChildWithTag(body.gameObject, "RightHand").transform;
    }


    [SerializeField]
    private float G = 1f;
    private float r;
    private float m1;
    private float m2;

    public float gravityStopDist = 1f;
    public float maxDist = 100f;
    public float massMult = 4000f;
    public float minMass = 0.25f;
    public float pushStopDist = 0.1f;
    void FixedUpdate()
    {
        midPointObj.transform.position = Vector3.Lerp(leftEmitterObj.transform.position, rightEmitterObj.transform.position, 0.5f);

        r = Vector3.Distance(midPointObj.transform.position, emitterTargetObj.transform.position);
        float adaptMass = Mathf.InverseLerp(1f, maxDist, r) * massMult;
        if (adaptMass < minMass) adaptMass = minMass;
        midPointObj.GetComponent<Rigidbody>().mass = adaptMass;


        m1 = emitterTargetObj.GetComponent<Rigidbody>().mass;
        m2 = midPointObj.GetComponent<Rigidbody>().mass;
        
        
        if (r > gravityStopDist) 
            emitterTargetObj.GetComponent<Rigidbody>().AddForce((midPointObj.transform.position-emitterTargetObj.transform.position).normalized * (G * (m1 * m2) / (r * r)));

        if (r > pushStopDist) 
            PushToMiddle();

        if((emitterTargetObj.transform.position-midPointObj.transform.position).magnitude<gravityStopDist && emitterTargetObj.GetComponent<Rigidbody>().velocity.magnitude<(gravityStopDist / 2f)){
            emitterTargetObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }


    public float pushSpeed = 8f;
    public AnimationCurve moveTowardsCurve;
    void PushToMiddle() {
        float loc = Vector3.Distance(midPointObj.transform.position, emitterTargetObj.transform.position);
        float dist = Vector3.Distance(leftEmitterObj.transform.position, rightEmitterObj.transform.position);
        dist = dist / 2f;
        
        float relativeDistance = Mathf.InverseLerp(0, dist, loc);
        relativeDistance = moveTowardsCurve.Evaluate(relativeDistance);

        float step = relativeDistance * Time.deltaTime * pushSpeed;
        
        emitterTargetObj.GetComponent<Rigidbody>().position = Vector3.MoveTowards(emitterTargetObj.transform.position, midPointObj.transform.position, step);
    }
}
