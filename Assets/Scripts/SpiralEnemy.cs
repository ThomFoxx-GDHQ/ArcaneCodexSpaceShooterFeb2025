using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class SpiralEnemy : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

    [Header("Boundary")]
    [SerializeField] float _topbounds;
    [SerializeField] float _bottombounds;
    [SerializeField] float _leftBounds;
    [SerializeField] float _rightBounds;

    [Header("Laser Stuff")]
    [SerializeField] Transform _leftLaserPoint;
    [SerializeField] Transform _rightLaserPoint;
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] float _fireRate;
    float _whenCanFire = -1;
    [SerializeField] Transform _laserContainer;
    GameObject _laser;

    [Space(10)]
    [SerializeField] GameObject _explosion;
    [SerializeField] Vector3 _explosionScale;
    [SerializeField] CameraController _cameraController; 
    [Tooltip("Controls the Intensity of the Camera Shake when hitting Player")]
    [SerializeField, Range(0, 1)] float _shakeIntensity = .5f;
    [SerializeField, Range(0, 1)] float _shakeTime = 1f;

    float _shipTime = 0;
    Vector3 _travelLinePosition;

    [SerializeField] float _timeStep = 5;
    [SerializeField] float _radius = 1;

        Vector3 _circleOffset = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _laserContainer = GameObject.Find("LaserContainer").transform;
        _cameraController = Camera.main.GetComponent<CameraController>();
        _travelLinePosition = transform.position;

        //For Randomness in Circling
        //_timeStep = Random.Range(-1, 2) * _timeStep;
        //_radius = Random.Range(1f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _whenCanFire)
        {
            FireLaser(_leftLaserPoint.position);
            FireLaser(_rightLaserPoint.position);

            _whenCanFire = Time.time + _fireRate;
        }

        _shipTime += Time.deltaTime;
    }

    private void CalculateMovement()
    {    
        _travelLinePosition += Vector3.down * (_speed * Time.deltaTime);

        _circleOffset.x = Mathf.Cos(_shipTime * _timeStep) * _radius;
        _circleOffset.y = Mathf.Sin(_shipTime * _timeStep) * _radius;        

        transform.position = _travelLinePosition + _circleOffset;

        if (transform.position.y <= _bottombounds)
        {
            float randX = Random.Range(_leftBounds, _rightBounds);
            transform.position = new Vector2(randX, _topbounds);
            _travelLinePosition = transform.position;
        }
    }

    private void FireLaser(Vector3 spawnPOS)
    {
        _laser = Instantiate(_laserPrefab, spawnPOS, Quaternion.identity, _laserContainer);
        _laser.GetComponent<Laser>()?.AssignLaser(true, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.CompareTag("Player"))
        { 
            other.GetComponentInParent<Player>()?.Damage();
            _cameraController.StartCameraShake(_shakeIntensity, _shakeTime);
            EnemyDeathSequence();
        }
        if (other.CompareTag("Projectile"))
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser != null && !laser.IsEnemyLaser)
            {
                laser.DestroyObjectAndParent();
                GameManager.Instance.AddToScore(10);
                EnemyDeathSequence();
            }
        }
    }

    private void EnemyDeathSequence()
    {
        SpawnManager.Instance.OnEnemyDeath();
        GameObject go = Instantiate(_explosion, transform.position, Quaternion.identity);
        go.transform.localScale = _explosionScale;
        Destroy(this.gameObject);
    }
}
