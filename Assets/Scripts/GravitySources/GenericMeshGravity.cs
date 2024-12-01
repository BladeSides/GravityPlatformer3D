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
