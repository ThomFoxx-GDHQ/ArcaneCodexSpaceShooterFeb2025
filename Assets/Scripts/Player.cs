using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _normalSpeed = 5f;
    [SerializeField] private float _speedBooostMultiplier = 3f;
    private int _health = 3;

    float _horizontalInput;
    float _verticalInput;
    Vector3 _direction = Vector3.zero;
    Vector3 _position = Vector3.zero;

    [SerializeField] float _upperBounds, _lowerBounds;
    [SerializeField] float _leftRightBounds;

    [SerializeField] Transform _laserContainer;
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] GameObject _tripleShotPrefab;
    [SerializeField] float _fireRate = 0.0f;
    bool _isFiring = false;
    Coroutine _fireCoroutine = null;
    WaitForSeconds _fireTime;
    SpawnManager _spawnManager;
    bool _isTripleActive;
    bool _isSpeedBoostActive;
    [SerializeField] GameObject _shieldVisual;
    bool _isShieldActive;
    [SerializeField, Tooltip("This is the default length added to the Triple Shot Timer when a Powerup is caught.")] 
    float _defaultTripleShotTimerLength = 5f;
    float _tripleShotTimer = 0f;
    Coroutine _tripleShotCoroutine;
    [SerializeField, Tooltip("This is the default length added to the Speed Boost Timer when a Powerup is caught.")]
    float _defaultSpeedBoostTimerLength = 5f;
    float _speedBoostTimer = 0f;
    Coroutine _speedBoostCoroutine;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        _fireTime = new WaitForSeconds(_fireRate);
        _shieldVisual?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        Bounds();

        //Spawn Laser when hit Space bar
        if (Input.GetKeyDown(KeyCode.Space) && !_isFiring)
        {
            _isFiring = true;
            _fireCoroutine = StartCoroutine(FireSequence());
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isFiring = false;
            if (_fireCoroutine != null)
            {
                StopCoroutine(_fireCoroutine);
            }
        }

    }

    private void CalculateMovement()
    {
        //Get Inputs
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        //Assign Inputs to Vector3 Direction
        //_direction = new Vector3(_horizontalInput, _verticalInput);
        _direction.x = _horizontalInput;
        _direction.y = _verticalInput;

        //Use Direction to Translate
        if (_isSpeedBoostActive) 
            transform.Translate(Time.deltaTime *_normalSpeed * _speedBooostMultiplier *_direction);
        else
            transform.Translate(Time.deltaTime * _normalSpeed * _direction);
    }

    private void Bounds()
    {
        _position = transform.position;

        _position.y = Mathf.Clamp(_position.y, _lowerBounds, _upperBounds);

        if (Mathf.Abs(_position.x) >= _leftRightBounds)
            _position.x = _position.x * -1;

        transform.position = _position;

        //if (transform.position.y <= -5)
        //{
        //    position.y = -5;
        //}
        //if (transform.position.y >= 0)
        //{
        //    position.y = 0;
        //}
        //if (transform.position.x <= -9.25f)
        //{
        //    position.x = -9.25f;
        //}
        //if (transform.position.x >= 9.25f)
        //{
        //    position.x = 9.25f;
        //}
    }

    IEnumerator FireSequence()
    {
        while (_isFiring)
        {
            FireLaser();
            yield return _fireTime;
        }
    }

    private void FireLaser()
    {
        if (_isTripleActive && _tripleShotPrefab != null)        
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity, _laserContainer);
        else if (_laserPrefab != null)
            Instantiate(_laserPrefab, transform.position, Quaternion.identity, _laserContainer);
    }

    /// <summary>
    /// Applies one point of health Damage to the player.
    /// </summary>
    public void Damage()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisual?.SetActive(false);
            return;
        }

        _health--;

        if (_health < 1)
        {
            SpawnManager.Instance.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void ActivateTripleShot()
    {
        _isTripleActive = true;
        if (_tripleShotCoroutine == null)
            _tripleShotCoroutine = StartCoroutine(TripleShotShutdownRoutine());
        else
            _tripleShotTimer += _defaultTripleShotTimerLength;        
    }

    IEnumerator TripleShotShutdownRoutine()
    {
        _tripleShotTimer = _defaultTripleShotTimerLength;
        while (_tripleShotTimer >= 0)
        {
            _tripleShotTimer -= Time.deltaTime;
            yield return null;
        }
        _isTripleActive = false;
        _tripleShotTimer = 0;
        _tripleShotCoroutine = null;
    }

    public void ActivateSpeedBoost()
    {
        _isSpeedBoostActive = true;
        if (_speedBoostCoroutine == null)
            _speedBoostCoroutine = StartCoroutine(SpeedBoostShutdownRoutine());
        else
            _speedBoostTimer += _defaultSpeedBoostTimerLength;
    }

    IEnumerator SpeedBoostShutdownRoutine()
    {
        _speedBoostTimer = _defaultSpeedBoostTimerLength;
        while (_speedBoostTimer >= 0)
        {
            _speedBoostTimer -= Time.deltaTime;
            yield return null;
        }
        _isSpeedBoostActive = false;
        _speedBoostTimer = 0;
        _speedBoostCoroutine = null;
    }

    public void ActivateShield()
    {
        _isShieldActive = true;
        _shieldVisual?.SetActive(true);
    }
}
