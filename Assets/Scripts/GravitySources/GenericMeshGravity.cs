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
        bool checkSphere = CheckSphereExtra(GetComponent<MeshCollider>(), 
            FindObjectOfType<PlayerMovement>().PlayerSphereCollider,
            out Vector3 closestPointOnMesh, out Vector3 surfaceNormal);
        if (checkSphere)
        {
            return surfaceNormal * gravityStrength * (outward ? -1 : 1);
        }
        else
        {
            return lastCorrectGravity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            CustomGravity.Instance.SetGravitySource(this);
        }



    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            CustomGravity.Instance.SetGravitySource(this);
        }

    }

    public static bool CheckSphereExtra(Collider target_collider, SphereCollider sphere_collider, out Vector3 closest_point, out Vector3 surface_normal)
    {
        closest_point = Vector3.zero;
        surface_normal = Vector3.zero;
        float surface_penetration_depth = 0;

        Vector3 sphere_pos = sphere_collider.transform.position;
        if (Physics.ComputePenetration(target_collider, target_collider.transform.position, target_collider.transform.rotation, sphere_collider, sphere_pos, Quaternion.identity, out surface_normal, out surface_penetration_depth))
        {
            closest_point = sphere_pos + (surface_normal * (sphere_collider.radius - surface_penetration_depth));

            surface_normal = -surface_normal;

            return true;
        }

        return false;
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