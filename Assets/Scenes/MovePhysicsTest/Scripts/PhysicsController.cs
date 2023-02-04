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
    [Range(0f,20f)]
    public float scalar;
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


    void Update()
    {	
        midPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f); // gets halfway point between the two controllers

        // Vector3 BA = B.position - A.position;
        Vector3 BA = midPos - prevMidPos;
        // Vector3 BC = B.position - C.position;
        Vector3 BC = midPos - transform.position;
        // Vector3 AC = C.position - A.position;
        float ba = BA.magnitude;
        float bc = BC.magnitude;
        float ratio = ba/(ba+bc);
        
        Vector3 X = Vector3.Lerp(prevMidPos, transform.position, ratio);

        Vector3 angleBisector = midPos - X;
        Vector3 D = transform.position + angleBisector;
        Vector3 DC = D - transform.position;
        newPos = DC * scalar + transform.position;

        transform.position = newPos;
        prevMidPos = midPos;

        ObjectFollowCursor();
    }

    private float distance = 15.2f;
    void ObjectFollowCursor() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 point = ray.origin + (ray.direction * distance); // point is the next place the object needs to go to
        controller2.position = point;
    }
}