using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class PhysicsControllerTest : MonoBehaviour
{
    public Transform controller1;
    public Transform controller2;
    private Vector3 c2prevPos;
    private Vector3 centerPos;
    private Vector3 goToPos;
    private float c2velocity;

    public float magnitude = 2f;
    
    void Awake()
    {
        c2prevPos = controller2.position;
    }

    // this is something, but not much. save as a last resort
    // void FixedUpdate()
    // {
    //     goToPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f);
    //     velocity = (controller2.position - c2PrevPos) / Time.fixedDeltaTime;
    //     c2PrevPos = controller2.position;
    //     transform.position = Vector3.Lerp(transform.position, goToPos + velocity, Time.deltaTime*1.5f);
    // }

    void Update()
    {	
        // arrow controls
        {        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 position = controller2.position;
            position.x = position.x - Time.deltaTime * magnitude;
            controller2.position = position;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 position = controller2.position;
            position.x = position.x + Time.deltaTime * magnitude;
            controller2.position = position;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 position = controller2.position;
            position.y = position.y + Time.deltaTime * magnitude;
            controller2.position = position;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 position = controller2.position;
            position.y = position.y - Time.deltaTime * magnitude;
            controller2.position = position;
        }}


        // Vector3 sDirection = (goToPos - transform.position).normalized; // vector pointing from sphere's current location to the location it needs to go to

        c2velocity = ((controller2.position - c2prevPos).magnitude) / Time.deltaTime; // get velocity of c2
        centerPos = Vector3.Lerp(controller1.position, controller2.position, 0.5f); // gets halfway point between two controllers

        goToPos = Vector3.MoveTowards(transform.position, centerPos, c2velocity*Time.deltaTime);

        transform.position = goToPos;

        c2prevPos = controller2.position;        
    }
}
