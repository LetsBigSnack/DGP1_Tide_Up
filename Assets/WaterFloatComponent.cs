using System;
using System.IO;
using Helpers.Util;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterFloatComponent : MonoBehaviour
{
  //public properties
    public float AirDrag = 1;
    public float WaterDrag = 10;
    public bool AffectDirection = true;
    public bool AttachToSurface = false;
    public Transform[] FloatPoints;
    public bool IsAnchored = false;

    [Header("Buoyancy")]
    public float BuoyancyStrength = 1f;
    
    //used components
    protected Rigidbody Rigidbody;

    //water line
    protected float WaterLine;
    protected Vector3[] WaterLinePoints;

    //help Vectors
    protected Vector3 smoothVectorRotation;
    protected Vector3 TargetUp;
    protected Vector3 centerOffset;

    public Vector3 Center { get { return transform.position + centerOffset; } }

    // Start is called before the first frame update
    void Awake()
    {
        //get components
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.useGravity = false;

        //compute center
        WaterLinePoints = new Vector3[FloatPoints.Length];
        for (int i = 0; i < FloatPoints.Length; i++)
            WaterLinePoints[i] = FloatPoints[i].position;
        centerOffset = PhysicsHelper.GetCenter(WaterLinePoints) - transform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //default water surface
        var newWaterLine = 0f;
        var pointUnderWater = false;

        //set WaterLinePoints and WaterLine
        for (int i = 0; i < FloatPoints.Length; i++)
        {
            //height
            WaterLinePoints[i] = FloatPoints[i].position;
            WaterLinePoints[i].y = OceanManager.Instance.GetWorldHeight(FloatPoints[i].position);
            newWaterLine += WaterLinePoints[i].y / FloatPoints.Length;
            if (WaterLinePoints[i].y > FloatPoints[i].position.y)
                pointUnderWater = true;
        }

        var waterLineDelta = newWaterLine - WaterLine;
        WaterLine = newWaterLine;

        //compute up vector
        TargetUp = PhysicsHelper.GetNormal(WaterLinePoints);

        //gravity
        var gravity = Physics.gravity;
        Rigidbody.linearDamping = AirDrag;
        if (WaterLine > Center.y)
        {
            Rigidbody.linearDamping = WaterDrag;
            //under water
            if (AttachToSurface)
            {
                //attach to water surface
                Rigidbody.position = new Vector3(Rigidbody.position.x, WaterLine - centerOffset.y, Rigidbody.position.z);
            }
            else
            {
                //go up
                gravity = AffectDirection ? TargetUp * -Physics.gravity.y : -Physics.gravity;
                transform.Translate(Vector3.up * waterLineDelta * 0.9f);
            }
        }
        
        if (IsAnchored)
        {
            // Only allow vertical movement and rotation
            var force = gravity * Mathf.Clamp(Mathf.Abs(WaterLine - Center.y), 0, 1) * BuoyancyStrength;
            force.x = 0;
            force.z = 0;
            Rigidbody.AddForce(force);

            // Freeze XZ velocity
            Vector3 v = Rigidbody.linearVelocity;
            v.x = 0;
            v.z = 0;
            Rigidbody.linearVelocity = v;

            // Optional: Freeze angular XZ velocity too
            Vector3 angular = Rigidbody.angularVelocity;
            angular.x = 0;
            angular.z = 0;
            Rigidbody.angularVelocity = angular;
        }
        else
        {
            // Normal buoyant motion
            Rigidbody.AddForce(gravity * Mathf.Clamp(Mathf.Abs(WaterLine - Center.y), 0, 1) * BuoyancyStrength);

        }
        
        //rotation
        if (pointUnderWater)
        {
            //attach to water surface
            TargetUp = Vector3.SmoothDamp(transform.up, TargetUp, ref smoothVectorRotation, 0.2f);
            Rigidbody.rotation = Quaternion.FromToRotation(transform.up, TargetUp) * Rigidbody.rotation;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (FloatPoints == null)
            return;

        for (int i = 0; i < FloatPoints.Length; i++)
        {
            if (FloatPoints[i] == null)
                continue;

            if (OceanManager.Instance != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(WaterLinePoints[i], Vector3.one * 0.3f);
            }
            
            //draw sphere
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(FloatPoints[i].position, 0.1f);

        }

        //draw center
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, WaterLine, Center.z), Vector3.one * 1f);
            Gizmos.DrawRay(new Vector3(Center.x, WaterLine, Center.z), TargetUp * 1f);
        }
    }

}
