using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodyVfxConnect : MonoBehaviour
{
    private BodySourceManager bodySourceManager;
    public GameObject bodyPrefab;
    public GameObject jointPrefab;
    public GameObject vfxControllerPrefab;
    private Dictionary<ulong, GameObject> bodies = new Dictionary<ulong, GameObject>();
    private List<JointType> joints = new List<JointType>{
        JointType.HandLeft,
        JointType.HandRight,
    };

    public bool drawSkeleton;

    private Dictionary<JointType, JointType> boneMap = new Dictionary<JointType, JointType>()
    {
        { JointType.FootLeft, JointType.AnkleLeft },
        { JointType.AnkleLeft, JointType.KneeLeft },
        { JointType.KneeLeft, JointType.HipLeft },
        { JointType.HipLeft, JointType.SpineBase },
        
        { JointType.FootRight, JointType.AnkleRight },
        { JointType.AnkleRight, JointType.KneeRight },
        { JointType.KneeRight, JointType.HipRight },
        { JointType.HipRight, JointType.SpineBase },
        
        { JointType.HandTipLeft, JointType.HandLeft },
        { JointType.ThumbLeft, JointType.HandLeft },
        { JointType.HandLeft, JointType.WristLeft },
        { JointType.WristLeft, JointType.ElbowLeft },
        { JointType.ElbowLeft, JointType.ShoulderLeft },
        { JointType.ShoulderLeft, JointType.SpineShoulder },
        
        { JointType.HandTipRight, JointType.HandRight },
        { JointType.ThumbRight, JointType.HandRight },
        { JointType.HandRight, JointType.WristRight },
        { JointType.WristRight, JointType.ElbowRight },
        { JointType.ElbowRight, JointType.ShoulderRight },
        { JointType.ShoulderRight, JointType.SpineShoulder },
        
        { JointType.SpineBase, JointType.SpineMid },
        { JointType.SpineMid, JointType.SpineShoulder },
        { JointType.SpineShoulder, JointType.Neck },
        { JointType.Neck, JointType.Head },
    };

    public Material BoneMaterial;

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
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(bodies[trackingId]);
                bodies.Remove(trackingId);
            }
        }

        if (Input.GetMouseButtonDown(0))
            DeleteAllBodies(knownIds);
        else if (Input.GetMouseButtonDown(1))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        #endregion

        #region Create Kinect Bodies
        foreach(var body in bodyData)
        {
            if (body == null)
                continue;
            
            if(body.IsTracked)
            {
                if(!bodies.ContainsKey(body.TrackingId)) {                    
                    bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);

                    GameObject bodyVfx = Instantiate(vfxControllerPrefab);
                    bodyVfx.name = ("VFX Controller:" + body.TrackingId);
                    bodyVfx.transform.parent = bodies[body.TrackingId].transform;
                }
                    
                
                UpdateBodyObject(body, bodies[body.TrackingId]);

                if (drawSkeleton) 
                    UpdateSkeleton(body, bodies[body.TrackingId]);
            }
        }
        #endregion
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = Instantiate(bodyPrefab);
        body.name = "Body:" + id;

        for (JointType joint = JointType.SpineBase; joint <= JointType.ThumbRight; joint++)
        {
            GameObject newJoint;
            if (drawSkeleton) 
            {
                newJoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
                LineRenderer lr = newJoint.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.material = BoneMaterial;
                lr.startWidth = 0.05f;
                lr.endWidth = 0.05f;
            }
            else
            {
                newJoint = Instantiate(jointPrefab);
            }

            if (joint == JointType.HandLeft) {
                    newJoint.tag = "LeftHand";
                }
            else if (joint == JointType.HandRight) {
                newJoint.tag = "RightHand";
            }

            newJoint.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            newJoint.name = joint.ToString();
            newJoint.transform.parent = body.transform;
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

    private void DeleteAllBodies(List<ulong> ids)
    {
        foreach(ulong trackingId in ids)
        {
            Destroy(bodies[trackingId]);
            bodies.Remove(trackingId);
        }
    }

    private void UpdateSkeleton(Body body, GameObject bodyObject)
    {
        for (JointType joint = JointType.SpineBase; joint <= JointType.ThumbRight; joint++)
        {
            Joint sourceJoint = body.Joints[joint];
            Joint? targetJoint = null;
            
            if(boneMap.ContainsKey(joint))
            {
                targetJoint = body.Joints[boneMap[joint]];
            }
            
            Transform jointObj = bodyObject.transform.Find(joint.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.startColor = ColorSkeleton (sourceJoint.TrackingState);
                lr.endColor = ColorSkeleton (targetJoint.Value.TrackingState);
            }
            else
            {
                lr.enabled = false;
            }
        }
    }

    private static Color ColorSkeleton(TrackingState state)
    {
        switch (state)
        {
        case TrackingState.Tracked:
            return Color.green;

        case TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
}
