using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
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
    [SerializeField] bool _isTripleActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnManager = GameObject.FindFirstObjectByType<SpawnManager>();
        _fireTime = new WaitForSeconds(_fireRate);
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
        transform.Translate(Time.deltaTime * _speed * _direction);
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
        _health--;

        if (_health < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void ActivateTripleShot()
    {
        _isTripleActive = true;
        StartCoroutine(TripleShotShutdownRoutine());
    }

    IEnumerator TripleShotShutdownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleActive = false;
    }
}
