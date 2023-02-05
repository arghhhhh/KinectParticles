using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodySourceViewNew : MonoBehaviour 
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;
    
    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    
    private List<JointType> _joints = new List<JointType>{
        JointType.HandLeft,
        JointType.HandRight,
    };
    
    void Update () 
    {
        #region Get Kinect Data
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
            return;
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
                continue;
                
            if(body.IsTracked)
                trackedIds.Add (body.TrackingId);
        }
        #endregion

        #region Delete Kinect Bodies
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId)) // delete untracked bodies
            {
                Destroy(mBodies[trackingId]); // destroy game object
                mBodies.Remove(trackingId); // remove from list
            }
        }
        #endregion

        #region Create Kinect Bodies
        foreach(var body in data)
        {
            if (body == null)
                continue;
            
            if(body.IsTracked)
            {
                if(!mBodies.ContainsKey(body.TrackingId))
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId); // make sure all tracked bodies are being handled
                
                UpdateBodyObject(body, mBodies[body.TrackingId]);
            }
        }
        #endregion
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        foreach (JointType joint in _joints) {
            GameObject newJoint = Instantiate(mJointObject);
            newJoint.name = joint.ToString();

            newJoint.transform.parent = body.transform;
        }
        
        return body;
    }
    
    private void UpdateBodyObject(Body body, GameObject bodyObject)
    {
        foreach (JointType _joint in _joints) {
            Joint _sourceJoint = body.Joints[_joint];
            Vector3 targetPosition = GetVector3FromJoint(_sourceJoint);
            targetPosition.z = 0;

            Transform jointObject = bodyObject.transform.Find(_joint.ToString());
            jointObject.position = targetPosition;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
