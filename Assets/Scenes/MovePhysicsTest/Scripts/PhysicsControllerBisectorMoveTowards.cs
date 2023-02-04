using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class PhysicsControllerBisectorMoveTowards : MonoBehaviour
{
    public Transform controller1;
    public Transform controller2;

    private Vector3 prevMidPos;
    private Vector3 midPos;
    private Vector3 newPos;

    private float timeElapsed;
    [Range(0f,1.5f)]
    public float intensity;
    private float acceleration;
    public AnimationCurve accelerationCurve;

    float prevVelocity;
    float velocity;

    void Start() 
    {
        midPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f);
        prevMidPos = midPos;
        prevVelocity = 0f;
    }


    void Update()
    {	
        midPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f); // gets halfway point between the two controllers
        if (midPos != prevMidPos) // only do calculations on non-zero accelerations
        {
            Vector3 BA = midPos - prevMidPos;
            Vector3 BC = midPos - transform.position;
            float ba = BA.magnitude;
            float bc = BC.magnitude;
            float bisectorRatio = ba/(ba+bc);
            
            Vector3 bisectorPos = Vector3.Lerp(prevMidPos, transform.position, bisectorRatio);

            Vector3 angleBisector = midPos - bisectorPos;
            Vector3 D = transform.position + angleBisector;
            Vector3 DC = D - transform.position;

            float velocity = ba / timeElapsed; // velocity is the change in position over time
            float midAccel = (velocity - prevVelocity) / timeElapsed; // acceleration is the change in velocity over time

            midAccel = Mathf.InverseLerp(-900f, 900f, midAccel); // clamps accel between specified range and returns value between 0-1
            midAccel = accelerationCurve.Evaluate(midAccel);
            float scalar = midAccel * intensity; // 

            newPos = DC * scalar + transform.position;

            transform.position = newPos;

            prevMidPos = midPos;
            prevVelocity = velocity;
            timeElapsed = 0;
        }

        SteerToMiddle();
        ObjectFollowCursor();

        timeElapsed += Time.deltaTime;
    }

    private float distance = 15.2f;
    void ObjectFollowCursor() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 point = ray.origin + (ray.direction * distance); // point is the next place the object needs to go to
        controller2.position = point;
    }

    public float speed;
    public AnimationCurve moveTowardsCurve;
    
    void SteerToMiddle() {
        float loc = Vector3.Distance(midPos, transform.position);
        float dist = Vector3.Distance(controller1.position, controller2.position); // get distance between controllers
        dist = dist / 2f; // bring value closer to loc
        
        float relativeDistance = Mathf.InverseLerp(0, dist, loc); // returns value between 0-1
        relativeDistance = moveTowardsCurve.Evaluate(relativeDistance); //ease out

        float step = relativeDistance * Time.deltaTime * speed;

        transform.position = Vector3.MoveTowards(transform.position, midPos, step);
    }
}