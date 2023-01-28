using UnityEngine;

[ExecuteInEditMode]
public class SdfTransformFlip : MonoBehaviour
{
    public Transform transformToTransform;

    void Update()
    {
        Vector3 getPos = new Vector3(-transformToTransform.position.x, transformToTransform.position.y, -transformToTransform.position.z);
        gameObject.transform.position = getPos;
    }
}
