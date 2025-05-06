using System.Collections;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
{
    [SerializeField] EnemyMovementType _enemyMovementType;
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

    [SerializeField] int _scoreValue = 10;

    [SerializeField, Range(0,1)] float _shieldChance = .5f;
    bool _shieldActive;
    [SerializeField] GameObject _shieldVisual;

    bool _isDodging = false;
    float _direction = 1;
    Coroutine _dodgeCoroutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _laserContainer = GameObject.Find("LaserContainer").transform;
        _cameraController = Camera.main.GetComponent<CameraController>();

        if (Random.value > _shieldChance)
        {
            _shieldVisual.SetActive(true);
            _shieldActive = true;
        }
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
    }
    
    private void CalculateMovement()
    {
        if (_isDodging)
        {
            transform.Translate(Vector2.right * (_direction * _speed * Time.deltaTime));
        }
        else
        {
            transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        }

        if (transform.position.y <= _bottombounds)
        {
            float randX = Random.Range(_leftBounds, _rightBounds);
            transform.position = new Vector2(randX, _topbounds);
        }

        if (Mathf.Abs(transform.position.x) > _rightBounds)
            _isDodging = false;
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
            if (ShieldCheck() == false)
            {
                other.GetComponentInParent<Player>()?.Damage();
                _cameraController.StartCameraShake(_shakeIntensity, _shakeTime);
                EnemyDeathSequence();
            }
        }
        if (other.CompareTag("Projectile"))
        {
            if (ShieldCheck() == false)
            {
                Laser laser = other.GetComponent<Laser>();
                if (laser != null && !laser.IsEnemyLaser)
                {
                    laser.DestroyObjectAndParent();
                    GameManager.Instance.AddToScore(_scoreValue);
                    EnemyDeathSequence();
                }
            }
            else
            {
                Laser laser = other.GetComponent<Laser>();
                if (laser != null && !laser.IsEnemyLaser)
                    laser.DestroyObjectAndParent();
            }
        }
    }

    private bool ShieldCheck()
    {
        if (!_shieldActive)
            return false;
        else
        {
            _shieldActive = false;
            _shieldVisual.SetActive(false);
            return true;
        }
    }

    private void EnemyDeathSequence()
    {
        SpawnManager.Instance.OnEnemyDeath();
        GameObject go = Instantiate(_explosion, transform.position, Quaternion.identity);
        go.transform.localScale = _explosionScale;
        Destroy(this.gameObject);
    }

    public EnemyMovementType GetEnemyMovementType()
    {
        return _enemyMovementType;
    }

    public void FireAtPowerup()
    {
        FireLaser(_leftLaserPoint.position);
        FireLaser(_rightLaserPoint.position);
    }

    public void DodgeFire(DodgeDirection direction)
    {
        if (_dodgeCoroutine == null)
        {
            _direction = (float)direction;
            _dodgeCoroutine = StartCoroutine(DodgeTime());
        }
    }

    IEnumerator DodgeTime()
    {
        _isDodging = true;
        yield return new WaitForSeconds(1);
        _isDodging = false;
        yield return new WaitForSeconds(1);
        _dodgeCoroutine = null;
    }

    public void Damage()
    {
        if (ShieldCheck() == false)
        {
            GameManager.Instance.AddToScore(_scoreValue);
            EnemyDeathSequence();
        }
    }
}
