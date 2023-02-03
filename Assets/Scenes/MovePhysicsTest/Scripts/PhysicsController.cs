using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class PhysicsController : MonoBehaviour
{
    public Transform controller1;
    public Transform controller2;

    private Vector3 prevMidPos;
    private Vector3 midPos;
    private Vector3 objPos;
    private Vector3 newPos;

    private float timeElapsed;
    private float scalar;
    private float acceleration;

    public float initialMagnitude = 1f;
    public float intensity = 2f;
    public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    public Transform testPos;

    void Start() 
    {
        midPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f);
        prevMidPos = midPos;
    }

    float getAngle(Vector3 a, Vector3 b, Vector3 c) {
        Vector3 side1 = a-b;
        Vector3 side2 = c-b;
        return Vector3.Angle(side1, side2);
    }

    Vector3 getBisector(Vector3 a, Vector3 b, Vector3 c) {
        Vector3 side1 = a-b;
        Vector3 side2 = c-b;
        return Vector3.Slerp(side1, side2, 0.5f);
    }

    void Update()
    {	
        midPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f); // gets halfway point between the two controllers
        Vector3 bisector = getBisector(testPos.position, midPos, transform.position);
        // float bisector = getAngle(prevMidPos, midPos, transform.position);

        Debug.DrawLine(midPos, transform.position, Color.green);
        Debug.DrawLine(midPos, bisector, Color.red);
        Debug.DrawLine(midPos, testPos.position, Color.blue);
        
        
        // prevMidPos = midPos;

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
}