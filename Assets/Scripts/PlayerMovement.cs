using System;
using UnityEngine;
using KBCore.Refs;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    Transform _playerInputSpace;

    [SerializeField, Self]
    private Rigidbody _rb;

    private Vector3 _velocity, _desiredVelocity;

    private bool _desiredJump;

    public SphereCollider PlayerSphereCollider;

    // Player Parameters

    // Metres
    [SerializeField, Range(0f, 10f)]
    public float _jumpHeight = 2f;

    // Metres per second
    [SerializeField, Range(0f, 100f)]
    public float _maxSpeed = 10f;

    // Metres per second square
    [SerializeField, Range(0f, 100f)]
    public float _maxAcceleration = 10f;
    
    // Metres per second square
    [SerializeField, Range(0f, 100f)] 
    private float _maxAirAcceleration = 1f;
        
    [SerializeField, Range(0, 5)]
    int _maxAirJumps = 0;
    
    // Degrees
    [SerializeField, Range(0f, 90f)]
    float _maxGroundAngle = 25f;
    
    // Metres
    [SerializeField, Min(0f)]
    float _probeDistance = 1f;
    
    // Metres per second
    [SerializeField, Range(0f, 100f)]
    float _maxSnapSpeed = 100f;
    
    // Player State
    bool OnGround => _groundContactCount > 0;
    bool OnSteep => _steepContactCount > 0;
    
    int _groundContactCount, _steepContactCount;
    
    int _jumpPhase;

    private Vector3 _contactNormal, _steepNormal;

    private int _stepsSinceLastGrounded, _stepsSinceLastJump;
    
    // Calculated Values

    float _minGroundDotProduct;

    // Gravity Related Stuff
    Vector3 _upAxis, _rightAxis, _forwardAxis;
    Vector3 lookDirection = Vector3.forward;

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(_maxGroundAngle * Mathf.Rad2Deg);
    }

    private void Awake()
    {
        _rb.useGravity = false;
        OnValidate();
    }

    void Update()
    {
        //Visuals
        HandlePlayerVisuals();
        
        // Inputs
        Vector2 playerInput;

        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        // Uses the transform of the Player Input Space
        if (_playerInputSpace)
        {
            _rightAxis = ProjectDirectionOnPlane(_playerInputSpace.right, _upAxis);
            _forwardAxis = ProjectDirectionOnPlane(_playerInputSpace.forward, _upAxis);
        }
        else
        {
            _rightAxis = ProjectDirectionOnPlane(Vector3.right, _upAxis);
            _forwardAxis = ProjectDirectionOnPlane(Vector3.forward, _upAxis);
        }
        
        _desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * _maxSpeed;
        
        _desiredJump |= Input.GetButtonDown("Jump");
    }

    private void HandlePlayerVisuals()
    {
        Vector2 playerInput;

        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        // Uses the transform of the Player Input Space
        if (_playerInputSpace)
        {
            _rightAxis = ProjectDirectionOnPlane(_playerInputSpace.right, _upAxis);
            _forwardAxis = ProjectDirectionOnPlane(_playerInputSpace.forward, _upAxis);

        }
        else
        {
            _rightAxis = ProjectDirectionOnPlane(Vector3.right, _upAxis);
            _forwardAxis = ProjectDirectionOnPlane(Vector3.forward, _upAxis);
        }

        if (playerInput.magnitude > 0.01f)
        {
            lookDirection = _rightAxis * playerInput.x + _forwardAxis * playerInput.y;
        }
        
        Quaternion rotation = Quaternion.LookRotation(lookDirection, _upAxis);
        transform.rotation = rotation;
    }

    private void FixedUpdate()
    {
        //Set Up Axis
        Vector3 gravity = CustomGravity.Instance.GetGravity(_rb.position, out _upAxis);
        
        UpdateState();
        AdjustVelocity();

        if (_desiredJump) {
            _desiredJump = false;
            Jump(gravity);
        }
        
        // Add gravity
        _velocity += gravity * Time.deltaTime;

        _rb.velocity = _velocity;

        ClearState();
    }

    private void ClearState()
    {
        _groundContactCount = 0;
        _steepContactCount = 0;
        _contactNormal = Vector3.zero;
        _steepNormal = Vector3.zero;
    }

    void AdjustVelocity ()
    {
        Vector3 xAxis = ProjectDirectionOnPlane(_rightAxis, _contactNormal);
        Vector3 zAxis = ProjectDirectionOnPlane(_forwardAxis, _contactNormal);
        
        float currentX = Vector3.Dot(_velocity, xAxis);
        float currentZ = Vector3.Dot(_velocity, zAxis);

        float acceleration = OnGround ? _maxAcceleration : _maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX =
            Mathf.MoveTowards(currentX, _desiredVelocity.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, _desiredVelocity.z, maxSpeedChange);
        
        _velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private void UpdateState()
    {
        _stepsSinceLastGrounded++;
        _stepsSinceLastJump++;
        
        _velocity = _rb.velocity;
        if (OnGround || SnapToGround() || CheckSteepContacts())
        {
            _stepsSinceLastGrounded = 0;
            if (_stepsSinceLastJump > 1)
            {
                _jumpPhase = 0;
            }

            _jumpPhase = 0;
            if (_groundContactCount > 1)
                _contactNormal.Normalize();;
        }
        else
        {
            _contactNormal = _upAxis;
        }
    }

    bool SnapToGround ()
    {
        //If didn't just leave the ground or player just jumped
        if (_stepsSinceLastGrounded > 1 || _stepsSinceLastJump <= 2)
        {
            return false;
        }
        
        // If the player is too fast, don't snap to ground
        float speed = _velocity.magnitude;
        if (speed > _maxSnapSpeed)
        {
            return false;
        }

        // If the player has nothing downwards
        if (!Physics.Raycast(_rb.position, -_upAxis, out RaycastHit hit, _probeDistance)) 
        {
            return false;
        }
        
        float upDot = Vector3.Dot(_upAxis, hit.normal);
        if (upDot < _minGroundDotProduct) 
        {
            return false;
        }
        // If the player is too steep

        // It's a valid ground
        _groundContactCount = 1;
        _contactNormal = hit.normal;
        float dot = Vector3.Dot(_velocity, hit.normal);
        
        // If the player is moving up, else don't change
        if (dot > 0f) 
        {
            _velocity = (_velocity - hit.normal * dot).normalized * speed;
        }
        return true;
    }

    private void Jump(Vector3 gravity)
    {
        Vector3 jumpDirection;

        // If on ground, jump in the normal direction
        if (OnGround)
            jumpDirection = _contactNormal;
        // If on steep, jump along it
        else if (OnSteep)
        {
            jumpDirection = _steepNormal;
            _jumpPhase = 0;
        }

        // If in air, jump in the normal direction
        else if (_maxAirJumps > 0 && _jumpPhase <= _maxAirJumps)
        {
            // Set jump phase to 1 if player was on ground when he jumped
            if (_jumpPhase == 0)
                _jumpPhase = 1;
            jumpDirection = _contactNormal;
        }

        else
            return;
        
        _stepsSinceLastJump = 0;
        _jumpPhase++;
        
        float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * _jumpHeight);
        jumpDirection = (jumpDirection + _upAxis).normalized;
        float alignedSpeed = Vector3.Dot(_velocity, jumpDirection);
        if (alignedSpeed > 0f) 
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }

        _velocity += jumpDirection * jumpSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay (Collision collision) 
    {
        EvaluateCollision(collision);
    }

    void OnCollisionExit (Collision collision) 
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(_upAxis, normal);
            if (upDot >= _minGroundDotProduct)
            {
                _groundContactCount++;
                _contactNormal += normal;
            }
            else if (upDot > -0.01f)
            {
                _steepContactCount++;
                _steepNormal += normal;
            }
        }
    }
    
    bool CheckSteepContacts () {
        if (_steepContactCount > 1) {
            _steepNormal.Normalize();
            float upDot = Vector3.Dot(_upAxis, _steepNormal);
            if (upDot >= _minGroundDotProduct) {
                _groundContactCount = 1;
                _contactNormal = _steepNormal;
                return true;
            }
        }
        return false;
    }
    
    Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal) 
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }
}
