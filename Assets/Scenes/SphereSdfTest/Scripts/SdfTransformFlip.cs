using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SdfTransformFlip : MonoBehaviour
{
    public Transform transformToTransform;
    private Transform thisTransform;
    void Awake()
    {
        thisTransform = gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        Vector3 getPos = new Vector3(-transformToTransform.position.x, transformToTransform.position.y, -transformToTransform.position.z);
        thisTransform.position = getPos;
    }
}
