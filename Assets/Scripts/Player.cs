using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables
    [Header("Speed Settings")]
    [SerializeField] private float _normalSpeed = 5f;
    [SerializeField] private float _speedBooostMultiplier = 3f;
    [SerializeField] private float _thrustBoostMultiplier = 2f;
    private float _thrustMultiplier = 1;
    [SerializeField] private int _maxHealth = 3;
    private int _health = 3;

    float _horizontalInput;
    float _verticalInput;
    Vector3 _direction = Vector3.zero;
    Vector3 _position = Vector3.zero;
    [Header("Boundaries")]
    [SerializeField] float _upperBounds;
    [SerializeField] float _lowerBounds;
    [SerializeField] float _leftRightBounds;

    [Header("Weapon Setting")]
    [SerializeField] Transform _laserContainer;
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] GameObject _tripleShotPrefab;
    [SerializeField] GameObject _scatterShotPrefab;
    [SerializeField] float _fireRate = 0.0f;
    bool _isFiring = false;
    Coroutine _fireCoroutine = null;
    WaitForSeconds _fireTime;
    bool _isTripleActive;
    bool _isSpeedBoostActive;
    [SerializeField] int _startingAmmoCount = 15;
    int _currentAmmoCount = 0;
    bool _isScatterActive;
    bool _canFire = true;
    bool _slowLasers = false;
    
    [Header("Shield Settings")]
    [SerializeField] GameObject _shieldVisual;
    [SerializeField] ShieldVisualization _shieldVisualization;
    bool _isShieldActive;
    int _currentShieldHealth = 0;
    [SerializeField] int _maxShieldHealth = 3;

    [Header("Powerup Settings")]
    [SerializeField, Tooltip("This is the default length added to the Triple Shot Timer when a Powerup is caught.")] 
    float _defaultTripleShotTimerLength = 5f;
    float _tripleShotTimer = 0f;
    Coroutine _tripleShotCoroutine;
    [SerializeField, Tooltip("This is the default length added to the Speed Boost Timer when a Powerup is caught.")]
    float _defaultSpeedBoostTimerLength = 5f;
    float _speedBoostTimer = 0f;
    Coroutine _speedBoostCoroutine;
    [SerializeField, Tooltip("This is the default length added to the Scatter Shot Timer when a Powerup is caught.")]
    float _defaultScatterShotTimerLength = 5f;
    float _scatterShotTimer = 0f;
    Coroutine _scatterShotCoroutine;
    bool _isDead = false;
    float _slowDownMultiplier = 1f;

    [Header("Damage Settings")]
    [SerializeField] private DamageVisuals _damageVisuals;
    [SerializeField] private CameraController _cameraController;
    [Tooltip("Controls the Intensity of the Camera Shake when hit by Laser")]
    [SerializeField, Range(0, 1)] float _shakeIntensity = .5f;
    [SerializeField, Range(0, 1)] float _shakeTime = 1f;

    [Header("Thruster Settings")]
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

    private bool _wasHit = false;

#endregion

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
        UIManager.Instance.UpdateShieldMeter(_currentShieldHealth);
        _shieldVisualization.UpdateShieldColor(_currentShieldHealth);

        _currentAmmoCount = _startingAmmoCount;
        UIManager.Instance.UpdateAmmo(_currentAmmoCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;
        if (_wasHit)
            _wasHit = false;



        ThrusterCalculations();
        CalculateMovement();
        Bounds();

        if (_canFire)
        {
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
        else if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
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
            transform.Translate(Time.deltaTime * (_normalSpeed * _speedBooostMultiplier * _thrustMultiplier * _slowDownMultiplier) * _direction);
        else
            transform.Translate(Time.deltaTime * (_normalSpeed * _thrustMultiplier * _slowDownMultiplier) * _direction);        
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
        if (_currentAmmoCount == 0)
        {
            AudioManager.Instance.PlayEmptyClip();
            return;
        }

        GameObject go = null;

        if (_isTripleActive && _tripleShotPrefab != null)
            go = Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity, _laserContainer);

        if (_isScatterActive && _scatterShotPrefab != null)
            go = Instantiate(_scatterShotPrefab, transform.position, Quaternion.identity, _laserContainer);

        if ((!_isTripleActive && !_isScatterActive) && _laserPrefab != null)
            go = Instantiate(_laserPrefab, transform.position, Quaternion.identity, _laserContainer);

        if (go != null && _slowLasers == true)
            foreach (Laser laser in go.GetComponentsInChildren<Laser>())
            {
                laser.SetSlowLaser();
            }

        _currentAmmoCount--;
        UIManager.Instance.UpdateAmmo(_currentAmmoCount);
    }


    /// <summary>
    /// Applies one point of health Damage to the player.
    /// </summary>
    public void Damage()
    {
        if (_wasHit) return;

        _wasHit = true;

        if (_isShieldActive)
        {
            _currentShieldHealth--;

            if (_currentShieldHealth == 0)
            {
                _isShieldActive = false;
                _shieldVisual?.SetActive(false);
            }

            UIManager.Instance.UpdateShieldMeter(_currentShieldHealth);
            _shieldVisualization.UpdateShieldColor(_currentShieldHealth);
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
        {
            if (_isScatterActive) //Stop Scatter Shot Timer
                _scatterShotTimer = -1;

            _tripleShotCoroutine = StartCoroutine(TripleShotShutdownRoutine());
        }
        else
            _tripleShotTimer += _defaultTripleShotTimerLength;

        if (_currentAmmoCount <= 5)
            RefillAmmo(5);
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
        _currentShieldHealth = _maxShieldHealth;
        UIManager.Instance.UpdateShieldMeter(_currentShieldHealth);
        _shieldVisualization.UpdateShieldColor(_currentShieldHealth);
    }

    public void RefillAmmo(int amount)
    {
        _currentAmmoCount += amount;

        if (_currentAmmoCount < 0)
            _currentAmmoCount = 0;

        UIManager.Instance.UpdateAmmo(_currentAmmoCount);
    }

    public void RestoreHealth(int amount)
    {
        //Debug.Log($"Health Amount = {amount}");
        _health += amount;

        if (_health > _maxHealth)
            _health = _maxHealth;

        UIManager.Instance.UpdateLives(_health);
        _damageVisuals?.ApplyVisualDamage(_health);
    }

    public void ActivateScatterShot()
    {
        _isScatterActive = true;
        if (_scatterShotCoroutine == null)
        {
            if (_isTripleActive)    //Stop Triple Shot Timer
                _tripleShotTimer = -1;

            _scatterShotCoroutine = StartCoroutine(ScatterShotShutdownRoutine());
        }
        else
            _scatterShotTimer += _defaultScatterShotTimerLength;

        if (_currentAmmoCount <= 5)
            RefillAmmo(5);
    }

    IEnumerator ScatterShotShutdownRoutine()
    {
        _scatterShotTimer = _defaultScatterShotTimerLength;
        while (_scatterShotTimer >=0)
        {
            _scatterShotTimer -= Time.deltaTime;
            yield return null;
        }
        _isScatterActive = false;
        _scatterShotTimer = 0;
        _scatterShotCoroutine = null;
    }

    public void ActivateSlowdown(int amount)
    {
        _slowDownMultiplier = 1f / amount;
        Debug.Log($"SlowDown = {_slowDownMultiplier}");
        StartCoroutine(SlowDownRoutine());
    }

    IEnumerator SlowDownRoutine()
    {
        yield return new WaitForSeconds(_defaultSpeedBoostTimerLength);
        _slowDownMultiplier = 1;
    }

    public void ActivateWeaponJam()
    {
        _canFire = false;
        StartCoroutine(WeaponJamCoolDown());
    }

    IEnumerator WeaponJamCoolDown()
    {
        yield return new WaitForSeconds(5);
        _canFire = true;
    }

    public void ActivateSlowLasers()
    {
        _slowLasers = true;
        StartCoroutine(SlowLaserCoolDown());
    }

    IEnumerator SlowLaserCoolDown()
    {
        yield return new WaitForSeconds(5);
        _slowLasers = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.CompareTag("Projectile"))
        { 
            Laser laser = other.GetComponent<Laser>();
            if (laser != null && laser.IsEnemyLaser)
            {
                Damage();
                _cameraController.StartCameraShake(_shakeIntensity, _shakeTime);
                Destroy(other.gameObject);
            }
        }
    }
}
