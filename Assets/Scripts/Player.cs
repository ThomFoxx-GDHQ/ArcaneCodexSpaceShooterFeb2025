using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _normalSpeed = 5f;
    [SerializeField] private float _speedBooostMultiplier = 3f;
    [SerializeField] private float _thrustBoostMultiplier = 2f;
    private float _thrustMultiplier = 1;
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
    bool _isDead = false;
    [SerializeField] private DamageVisuals _damageVisuals;

    private bool _isThrusting;
    [SerializeField] private Vector2 _minMaxHeat;
    private float _currentHeat;
    private bool _engineOverheated = false;
    private bool _canThrust = true;
    [SerializeField] private float _thrustRate = 5f;
    [SerializeField] private float _normalThrustCooldownRate = 4f;
    [SerializeField] private float _overheatThrustCooldownRate = 2f;
    private float _thrustCoolDownRate;
    [SerializeField] private float _overheatLimiter = 50f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager.Instance.UpdateLives(3);
        _fireTime = new WaitForSeconds(_fireRate);
        _shieldVisual?.SetActive(false);

        _damageVisuals ??= GetComponentInChildren<DamageVisuals>();
        _damageVisuals?.ApplyVisualDamage(_health);
        _currentHeat = _minMaxHeat.x;
        UIManager.Instance.UpdateThrustSlider(_currentHeat);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;

        ThrusterCalculations();
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

    private void ThrusterCalculations()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canThrust)
        {
            _thrustMultiplier = _thrustBoostMultiplier;
            _isThrusting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _thrustMultiplier = 1;
            _isThrusting = false;
        }

        //Increase heat while Thursting
        if (_isThrusting && _currentHeat < _minMaxHeat.y)
        {
            _currentHeat += _thrustRate * Time.deltaTime;
        }
        else if (_currentHeat >= _minMaxHeat.y)
        {
            _currentHeat = _minMaxHeat.y;
        }

        if (_currentHeat == _minMaxHeat.y)
        {
            _engineOverheated = true;
            _thrustMultiplier = 1;
            _thrustCoolDownRate = _overheatThrustCooldownRate;
            _canThrust = false;
        }
        else if (_engineOverheated == false)
        {
            _thrustCoolDownRate = _normalThrustCooldownRate;
        }

        if (_engineOverheated && _currentHeat < _overheatLimiter)
        {
            _canThrust = true;
        }

        //Decrease heat while not thrusting
        if (!_isThrusting && _currentHeat > _minMaxHeat.x)
        {
            _currentHeat -= _thrustCoolDownRate * Time.deltaTime;
        }
        else if (_currentHeat < _minMaxHeat.x)
        {
            _currentHeat = _minMaxHeat.x;
            _engineOverheated = false;
        }

        UIManager.Instance.UpdateThrustSlider(_currentHeat);
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
            transform.Translate(Time.deltaTime * _normalSpeed * _speedBooostMultiplier * _thrustMultiplier * _direction);
        else
            transform.Translate(Time.deltaTime * _normalSpeed * _thrustMultiplier * _direction);        
    }

    private void Bounds()
    {
        _position = transform.position;

        _position.y = Mathf.Clamp(_position.y, _lowerBounds, _upperBounds);

        if (Mathf.Abs(_position.x) > _leftRightBounds)
        {
            _position.x = _position.x * -1;

            if (_position.x < 0)
                _position.x += .05f;
            else
                _position.x -= .05f;
        }

        transform.position = _position;
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
        UIManager.Instance.UpdateLives(_health);
        _damageVisuals?.ApplyVisualDamage(_health);

        if (_health < 1)
        {
            SpawnManager.Instance.OnPlayerDeath();
            GameManager.Instance.GameOver();
            //Destroy(this.gameObject); ::Replaced::
            transform.GetChild(0).gameObject.SetActive(false);
            _isDead = true;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.CompareTag("Projectile"))
        { 
            Laser laser = other.GetComponent<Laser>();
            if (laser != null && laser.IsEnemyLaser)
            {
                Damage();
                Destroy(other.gameObject);
            }
        }
    }
}
