using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class GenericMeshGravity : GravitySource
{
    public bool outward;
    public float gravityDistance = 9f;
    public float gravityStrength = 9.81f;
    public float interpolationStrength = 1f;
    public Vector3 lastCorrectGravity = Vector3.down;
    public override Vector3 GetGravity(Vector3 position)
    {
        Vector3 centre = GetComponent<MeshCollider>().bounds.center;

        RaycastHit[] hits = Physics.SphereCastAll(
            position, interpolationStrength
            , (outward ? 1 : -1) * (centre-position), 
            gravityDistance);

        Debug.DrawLine(position, position + (outward ? 1 : -1) * (centre - position) * gravityDistance, Color.red);

        Vector3 gravityNormal = Vector3.zero;
        RaycastHit closestHit = new RaycastHit();
        bool hadClosestHit = false;

        if (hits.Length > 0)
        {
            hadClosestHit = true;
            closestHit = hits[0];
        }
        foreach (RaycastHit hit in hits)
        {
            if (closestHit.distance < closestHit.distance)
            {
                closestHit = hit;
            }
        }
        if (hadClosestHit) 
        {
            if (closestHit.transform == this.transform)
            {
                MeshCollider collider = (MeshCollider)closestHit.collider;
                Mesh mesh = collider.sharedMesh;
                Vector3[] normals = mesh.normals;
                int[] triangles = mesh.triangles;

                if (normals == null || triangles == null || closestHit.triangleIndex * 3 + 2 >
                    triangles.Length)
                {
                    return lastCorrectGravity;
                }

                Vector3 n0 = normals[triangles[closestHit.triangleIndex * 3 + 0]];
                Vector3 n1 = normals[triangles[closestHit.triangleIndex * 3 + 1]];
                Vector3 n2 = normals[triangles[closestHit.triangleIndex * 3 + 2]];

                Vector3 baryCenter = closestHit.barycentricCoordinate;
                Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
                interpolatedNormal.Normalize();
                interpolatedNormal = closestHit.transform.TransformDirection(interpolatedNormal);

                gravityNormal += interpolatedNormal;
            }
        }

        if (hits.Length > 0 && gravityNormal != Vector3.zero)
        {
            gravityNormal.Normalize();
            lastCorrectGravity = gravityNormal;

            return gravityNormal * gravityStrength;
        }
        else
        {
            return lastCorrectGravity;
        }


    }




}



/* old code
using UnityEngine;

public class GenericMeshGravity : GravitySource
{
    public bool outward;
    public float gravityDistance = 9f;
    public float gravityStrength = 9.81f;
    public float interpolationStrength = 1f;
    public Vector3 lastCorrectGravity = Vector3.down;
    public override Vector3 GetGravity(Vector3 position)
    {
        Vector3 closestPointOnMesh = GetComponent<MeshCollider>().ClosestPoint(position);
        
        Vector3 gravity = (closestPointOnMesh - position) * gravityStrength * (outward ? 1 : -1);
        
        return gravity;
    }
    
}
*/