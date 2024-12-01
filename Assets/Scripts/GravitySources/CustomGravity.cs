using UnityEngine;

public class CustomGravity: MonoBehaviour
{
    public static CustomGravity Instance { get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<CustomGravity>();
                if (_instance)
                {
                    return _instance;
                }
                else
                {
                    //Lazy Initialization
                    GameObject go = new GameObject("CustomGravity");
                    _instance = go.AddComponent<CustomGravity>();
                }
            }
            return _instance;
        }
        private set
        {
            if (!_instance)
            {
                _instance = value;
            }
            else
            {
                Debug.Log("Two instances of CustomGravity. Destroying the new one.");
                Destroy(value);
            }
        }
    }

    private static CustomGravity _instance;

    [SerializeField] private GravitySource _startingGravitySource;

    private GravitySource _currentGravitySource;

    void Awake()
    {
        if (CustomGravity.Instance == null)
        { 
            CustomGravity.Instance = this;
        }
        else if (CustomGravity.Instance != this)
        {
            Destroy(gameObject);
        }
        SetGravitySource(_startingGravitySource);
    }
    public Vector3 GetGravity(Vector3 position)
    {
        Vector3 g = Vector3.zero;
        if (_currentGravitySource)
        {
            g += _currentGravitySource.GetGravity(position);
        }

        else
        {
            g = Vector3.down * -9.81f;
        }

        return g;
    }

    public Vector3 GetUpAxis(Vector3 position)
    {
        Vector3 g = Vector3.zero;
        if (_currentGravitySource)
        {
            g += _currentGravitySource.GetGravity(position);
        }

        else
        {
            g = Vector3.down * -9.81f;
        }

        return -g.normalized;
    }

    public Vector3 GetGravity (Vector3 position, out Vector3 upAxis)
    {
        Vector3 g = Vector3.zero;
        if (_currentGravitySource)
        {
            g += _currentGravitySource.GetGravity(position);
        }

        else
        {
            g = Vector3.down * -9.81f;
        }

        upAxis = -g.normalized;
        return g;
    }

    public void SetGravitySource(GravitySource source)
    {
        _currentGravitySource = source;
    }
    
    public void ClearGravitySource(GravitySource source)
    {
        if (_currentGravitySource == source)
        {
            _currentGravitySource = null;
        }
    }
    
    public void ClearGravitySource()
    {
        _currentGravitySource = null;
    }
}