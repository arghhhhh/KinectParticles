using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodyVfxConnect : MonoBehaviour
{
    private BodySourceManager bodySourceManager;
    public GameObject jointPrefab;
    public GameObject vfxControllerPrefab;
    private Dictionary<ulong, GameObject> bodies = new Dictionary<ulong, GameObject>();
    private List<JointType> joints = new List<JointType>{
        JointType.HandLeft,
        JointType.HandRight,
    };

    void Start()
    {
        bodySourceManager = GameObject.Find("BodyManager").GetComponent<BodySourceManager>();
    }

    void Update()
    {
        #region Get Kinect Data
        Body[] bodyData = bodySourceManager.GetData();
        if (bodyData == null)
            return;
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in bodyData)
        {
            if (body == null)
                continue;
                
            if(body.IsTracked)
                trackedIds.Add (body.TrackingId);
        }
        #endregion

        #region Delete Kinect Bodies
        List<ulong> knownIds = new List<ulong>(bodies.Keys);
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId)) // delete untracked bodies
            {
                Destroy(bodies[trackingId]); // destroy game object
                bodies.Remove(trackingId); // remove from list
            }
        }
        #endregion

        #region Create Kinect Bodies
        foreach(var body in bodyData)
        {
            if (body == null)
                continue;
            
            if(body.IsTracked)
            {
                if(!bodies.ContainsKey(body.TrackingId)) {
                    bodies[body.TrackingId] = CreateBodyObject(body.TrackingId); // make sure all tracked bodies are being handled

                    GameObject bodyVfx = Instantiate(vfxControllerPrefab);
                    bodyVfx.name = ("VFX Controller:" + body.TrackingId);
                    bodyVfx.transform.parent = bodies[body.TrackingId].transform;
                }
                    
                
                UpdateBodyObject(body, bodies[body.TrackingId]);
            }
        }
        #endregion
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        foreach (JointType joint in joints) {
            GameObject newJoint = Instantiate(new GameObject("joint"));
            newJoint.name = joint.ToString();
            newJoint.transform.parent = body.transform;

            if (joint == JointType.HandLeft) {
                newJoint.tag = "LeftHand";
            }
            else if (joint == JointType.HandRight) {
                newJoint.tag = "RightHand";
            }
        }
        
        return body;
    }

    private void UpdateBodyObject(Body body, GameObject bodyObject)
    {
        foreach (JointType joint in joints) {
            Joint sourceJoint = body.Joints[joint];
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);

            Transform jointObject = bodyObject.transform.Find(joint.ToString());
            jointObject.position = targetPosition;
        }
    }

    private static Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
