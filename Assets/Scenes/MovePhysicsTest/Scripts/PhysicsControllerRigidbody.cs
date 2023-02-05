using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsControllerRigidbody : MonoBehaviour
{
    public Transform controller1;
    public Transform controller2;
    public GameObject followObj;
    public GameObject midPointPrefab;
    private GameObject midPoint;

    readonly float G = 1f;
    float r;
    float prevR = 0;
    float m1;
    float m2;

    public float minG = 1f;
    public float maxDist = 100f;
    public float massMult = 4000f;
    public float minMass = 0.25f;
    public float minS = 0.1f;
    public AnimationCurve massCurve;
    void Start()
    {
        // InitialVelocity();
        midPoint = Instantiate(midPointPrefab);
        midPoint.transform.position = Vector3.Lerp(controller1.position, controller2.position, 0.5f);
    }

    void FixedUpdate()
    {
        midPoint.transform.position = Vector3.Lerp(controller1.position, controller2.position, 0.5f);

        r = Vector3.Distance(midPoint.transform.position, followObj.transform.position);
        float adaptMass = Mathf.InverseLerp(1f, maxDist, r) * massMult;
        // adaptMass = massCurve.Evaluate(adaptMass);
        if (adaptMass < minMass) adaptMass = minMass;
        midPoint.GetComponent<Rigidbody>().mass = adaptMass;


        m1 = followObj.GetComponent<Rigidbody>().mass;
        m2 = midPoint.GetComponent<Rigidbody>().mass;
        
        
        if (r > minG) 
            Gravity();

        if (r > minS) 
            SteerToMiddle();

        if((followObj.transform.position-midPoint.transform.position).magnitude<minG && followObj.GetComponent<Rigidbody>().velocity.magnitude<minG){
            followObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        ObjectFollowCursor();

        prevR = r;
    }

    void Gravity()
    {
        followObj.GetComponent<Rigidbody>().AddForce((midPoint.transform.position-followObj.transform.position).normalized * (G * (m1 * m2) / (r * r)));
    }

    void InitialVelocity()
    {
        followObj.transform.LookAt(midPoint.transform);
        followObj.GetComponent<Rigidbody>().velocity += followObj.transform.right * Mathf.Sqrt((G * m2) / r);
    }

    private float distance = 15.2f;
    // float time = 0f;
    void ObjectFollowCursor() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 point = ray.origin + (ray.direction * distance); // point is the next place the object needs to go to
        // point.z = Mathf.Sin(3f*time) * 3f;
        // Debug.Log(point.z);
        controller2.position = point;
        // time += Time.fixedDeltaTime;
    }

    public float speed;
    public AnimationCurve moveTowardsCurve;
    
    void SteerToMiddle() {
        float loc = Vector3.Distance(midPoint.transform.position, followObj.transform.position);
        float dist = Vector3.Distance(controller1.position, controller2.position); // get distance between controllers
        dist = dist / 2f; // bring value closer to loc
        
        float relativeDistance = Mathf.InverseLerp(0, dist, loc); // returns value between 0-1
        relativeDistance = moveTowardsCurve.Evaluate(relativeDistance); //ease out

        float step = relativeDistance * Time.deltaTime * speed;
        
        followObj.GetComponent<Rigidbody>().position = Vector3.MoveTowards(followObj.transform.position, midPoint.transform.position, step);
    }
}
