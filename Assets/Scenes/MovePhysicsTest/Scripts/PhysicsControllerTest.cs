using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class PhysicsControllerTest : MonoBehaviour
{
    public Transform controller1;
    public Transform controller2;

    private Vector3 prevMidPos;
    private Vector3 midPos;
    private Vector3 prevObjPos;
    private Vector3 objPos;

    Vector3 position;
    Vector3 prevPosition;
    Vector3 midVelo;
    Vector3 prevMidVelo;
    Vector3 objVelo;
    Vector3 prevObjVelo;
    float midAccel;
    public float magnitude;
    float prevMagnitude;
    float acceleration;

    Vector3 midDir;
    Vector3 objDir;
    Vector3 goToPos;
    Vector3 goToRef;
    Vector3 projRef;

    float timeElapsed;

    float scalar;

    public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    
    void Awake()
    {

    }

    void Start() 
    {
        midPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f); // gets halfway point between the two controllers
        prevMidPos = midPos; // original refPos is midPos
        // prevGoToPoint = refPos;
    }

    void Update()
    {	
        midPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f); // gets halfway point between the two controllers
        if (midPos != prevMidPos) { // only do calculations on non-zero accelerations
            midVelo = (midPos - prevMidPos) / timeElapsed; // velocity is the change in position over time
            midAccel = (midVelo - prevMidVelo).magnitude / timeElapsed; // acceleration is the change in velocity over time
            midDir = midPos - prevMidPos;
            midAccel = Mathf.InverseLerp(0, 500f, midAccel); // returns value between 0-1
            Debug.Log("1: "+midAccel);
            midAccel = curve.Evaluate(midAccel);
            Debug.Log("2: "+midAccel);
            scalar = midAccel * magnitude; // 

            goToRef = midDir * scalar; // now, we have a reference point to use with the sphere

            objPos = transform.position; // current position of sphere
            objDir = objPos - prevObjPos;
            midDir = goToRef - midPos;

            float distance = Vector3.Distance(prevMidPos, prevObjPos);
            projRef = midDir + (midPos.normalized * distance);
            goToPos = projRef + objDir;

            transform.position = Vector3.MoveTowards(transform.position, goToPos, Time.deltaTime*magnitude);
            // transform.position = midPos;

            timeElapsed = 0;
        }
        
        Debug.DrawRay(prevMidPos, midPos, Color.red);
        Debug.DrawRay(prevObjPos, projRef, Color.cyan);
        Debug.DrawRay(prevMidPos, prevObjPos, Color.yellow);
        // Debug.DrawRay(midPos, goToRef, Color.green);
        // Debug.DrawRay(midPos, goToRef, Color.green);
        

       

        
        
        prevMidPos = midPos;
        prevObjPos = objPos;

        ObjectFollowCursor();

        timeElapsed += Time.deltaTime;

        // Debug.Log("Time: "+Time.deltaTime+", Velocity: "+velocity+", Acceleration: "+acceleration);
    }

    public float distance = 15.2f;
    void ObjectFollowCursor() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 point = ray.origin + (ray.direction * distance); // point is the next place the object needs to go to
        // cube.AddForce(point, ForceMode.Acceleration);
        controller2.position = point;
    }
}



    // this is something, but not much. save as a last resort
    // void FixedUpdate()
    // {
    //     goToPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f);
    //     velocity = (controller2.position - c2PrevPos) / Time.fixedDeltaTime;
    //     c2PrevPos = controller2.position;
    //     transform.position = Vector3.Lerp(transform.position, goToPos + velocity, Time.deltaTime*1.5f);
    // }

        // arrow controls
        // {        
        // if (Input.GetKey(KeyCode.LeftArrow))
        // {
        //     Vector3 position = controller2.position;
        //     position.x = position.x - Time.deltaTime * magnitude;
        //     controller2.position = position;
        // }
        // if (Input.GetKey(KeyCode.RightArrow))
        // {
        //     Vector3 position = controller2.position;
        //     position.x = position.x + Time.deltaTime * magnitude;
        //     controller2.position = position;
        // }
        // if (Input.GetKey(KeyCode.UpArrow))
        // {
        //     Vector3 position = controller2.position;
        //     position.y = position.y + Time.deltaTime * magnitude;
        //     controller2.position = position;
        // }
        // if (Input.GetKey(KeyCode.DownArrow))
        // {
        //     Vector3 position = controller2.position;
        //     position.y = position.y - Time.deltaTime * magnitude;
        //     controller2.position = position;
        // }}


        // centerPoint = Vector3.Lerp(capsule.position, cube.position, 0.5f); // gets halfway point between two controllers

        // velocity = cube.velocity;
        // velocity = (cube.position - previousPos) / Time.deltaTime; // velocity is the change in position over time
        // magnitude = velocity.magnitude;
        // acceleration = (magnitude - prevMagnitude) / Time.deltaTime; // acceleration is the change in velocity over time
        // acceleration /= 1000f; // normalize acceleration a bit

        // // if acceleration is positive, move goToPoint past centerPoint. if negative, move behind centerPoint
        // // if the magnitude of velocity is greater than the magnitude of prevVelocity, the object is accelerating

        // if (acceleration != 0f)
        //     goToPoint = prevGoToPoint * acceleration;
        // else goToPoint = prevGoToPoint;
        
        // goToPoint = Vector3.MoveTowards(transform.position, goToPoint, 5f);

        // transform.position = goToPoint;

        // prevGoToPoint = goToPoint;

        // previousPos = cube.position;    
        // prevVelocity = velocity;
        // prevMagnitude = magnitude;