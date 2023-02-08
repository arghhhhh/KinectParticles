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
    float m1;
    float m2;

    public float minG = 1f;
    public float maxDist = 100f;
    public float massMult = 4000f;
    public float minMass = 0.25f;
    public float minS = 0.1f;
    public AnimationCurve massCurve;

    float maxVelocity = 0;
    public int startMeasuringAt = 60;
    int frameCount = 0;
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

        // if (Time.frameCount > startMeasuringAt)
        ScaleOvershoot();

        // if (followObj.GetComponent<Rigidbody>().velocity == Vector3.zero)
        //     Debug.Log("no velo!");

        ObjectFollowCursor();
    }

    public Vector3 defaultScale = new Vector3(2f,2f,2f);
    public Vector2 mixMaxScale = new Vector2(3.25f, 1f);
    public float scaleInc = 0.2f;
    float maxVelocityCap = 50f;
    public AnimationCurve scaleCurve;
    void ScaleOvershoot()
    {
        // if (followObj.GetComponent<Rigidbody>().velocity.sqrMagnitude > maxVelocity) {
        //     maxVelocity = followObj.GetComponent<Rigidbody>().velocity.sqrMagnitude;
        //     Debug.Log(maxVelocity);
        // }
            
        float V = Mathf.InverseLerp(0, maxVelocityCap, followObj.GetComponent<Rigidbody>().velocity.sqrMagnitude);
        V = scaleCurve.Evaluate(V);

        // float S = Mathf.Lerp(scaleInc, -scaleInc, V); // could use curve.eval on V to add a nice effect
        // Vector3 scaleChange = Vector3.one * S;

        // Vector3 minScale = Vector3.one * mixMaxScale.x;
        // Vector3 maxScale = Vector3.one * mixMaxScale.y;

        // followObj.transform.localScale += scaleChange;

        // if (followObj.transform.localScale.sqrMagnitude < minScale.sqrMagnitude)
        //     followObj.transform.localScale = minScale;
        // else if (followObj.transform.localScale.sqrMagnitude > maxScale.sqrMagnitude)
        //     followObj.transform.localScale = maxScale;

        float S = Mathf.LerpUnclamped(mixMaxScale.x, mixMaxScale.y, V);
        followObj.transform.localScale = Vector3.one * S;

        // if (followObj.GetComponent<Rigidbody>().velocity.sqrMagnitude < 1f && followObj.transform.localScale.sqrMagnitude > defaultScale.sqrMagnitude)
        //     followObj.transform.localScale -= scaleChange * 1.15f;

    }

    void Gravity()
    {
        followObj.GetComponent<Rigidbody>().AddForce((midPoint.transform.position-followObj.transform.position).normalized * (G * (m1 * m2) / (r * r))); // physics formula for gavitational force
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

    // float D = 0;
    // float prevD = 0;
    // float deltaD = 0;
    // float totalDeltaD = 0;
    // public int DframeTolerance = 5;
    // bool scaleDir = false;
    // bool prevScaleDir = false;
    // int dirDuration = 0;
    // public int maxDirDuration = 180;
    // void OvershootBasedOnControllerDist() {
    //     D = Vector3.Distance(controller1.position, controller2.position);
    //     deltaD = D - prevD;
    //     totalDeltaD += deltaD;
    //     if (deltaD > 0)
    //         scaleDir = true;
    //     if (deltaD < 0)
    //        scaleDir = false;
    //     // ignore all instances where scaleDir == 0 because it will equal 0 a lot between movements
    //     if (scaleDir != prevScaleDir)
    //     {
    //         if (dirDuration > DframeTolerance) 
    //             ScaleOvershootOld();

    //         dirDuration = 0;
    //         totalDeltaD = 0;
    //     }

    //     dirDuration++;
    //     prevD = D;
    //     prevScaleDir = scaleDir;
    // }

    // public float overshootDistMult = 0.1f;
    // public float overshootTimeMult = 1f;
    // public Vector2 minMaxScale = new Vector2(0.25f, 5f);
    // void ScaleOvershootOld()
    // {
    //     Debug.Log(totalDeltaD+", "+dirDuration);
    //     if (dirDuration > maxDirDuration)
    //         dirDuration = maxDirDuration;
    //     float currScale = followObj.transform.localScale.x; // object is uniformly scaled, so only need to get one value from the vector3
    //     // lerp val * 2f will give scalar val with median of 1f
    //     float overshootDist = totalDeltaD * overshootDistMult; 
    //     float overshootDur = dirDuration * overshootTimeMult;
    //     float dd = overshootDist / overshootDur; // larger, quicker movements should result in larger, quicker overshoots
    //     float scaler = dd;
    //     StartCoroutine(ScaleToSize(scaler, dirDuration));
    // }

    // private IEnumerator ScaleToSize(float scaleChange, float duration) 
    // {
    //     // Debug.Log("started coroutine!");
    //     float animationTime = 0;
    //     Vector3 scaleVector = new Vector3(scaleChange, scaleChange, scaleChange);
 
    //     while(animationTime < duration)
    //     {
    //         animationTime ++;
    //         followObj.transform.localScale += scaleVector;
    //         yield return null;
    //     }
    // }
}
