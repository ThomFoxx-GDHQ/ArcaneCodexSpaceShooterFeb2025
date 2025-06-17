using TMPro.Examples;
using UnityEngine;

public class MineLayingEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] EnemyMovementType _enemyMovementType;
    [SerializeField] float _speed = 5f;
    [SerializeField] GameObject _minePrefab;
    [SerializeField] float _mineRate = 2f;
    float _canMine = 0f;

    //[Header("Screen Boundaries")]
    //[SerializeField] float _topBounds;
    //[SerializeField] float _bottomBounds;
    //[SerializeField] float _leftRightBounds;

    Player _player;

    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] Vector3 _explosionScale;
    [SerializeField] CameraController _cameraController;
    [Tooltip("Controls the Intensity of the Camera Shake when hitting Player")]
    [SerializeField, Range(0, 1)] float _shakeIntensity = .5f;
    [SerializeField, Range(0, 1)] float _shakeTime = 1f;

    [SerializeField] int _scoreValue = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _cameraController = Camera.main.GetComponent<CameraController>();
        _cameraController = Camera.main.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (transform.position.y < _player.transform.position.y && _canMine < Time.time)
        {
            Instantiate(_minePrefab, transform.position, Quaternion.identity);
            _canMine = Time.time + _mineRate;
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (transform.position.y < GameManager.Instance.EnemyBounds.bottom)
        {
            float rng = Random.Range(GameManager.Instance.EnemyBounds.left, GameManager.Instance.EnemyBounds.right);
            transform.position = new Vector3(rng, GameManager.Instance.EnemyBounds.top, 0);
        }
    }

    public EnemyMovementType GetEnemyMovementType()
    {
        return _enemyMovementType;
    }

    public void FireAtPowerup()
    {
        // Not Implemented
    }

    public void Damage()
    {
        GameManager.Instance.AddToScore(_scoreValue);
        EnemyDeathSequence();
    }

    private void EnemyDeathSequence()
    {
        SpawnManager.Instance.OnEnemyDeath();
        GameObject go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = _explosionScale;
        Destroy(this.gameObject);
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
                GameManager.Instance.AddToScore(_scoreValue);
                EnemyDeathSequence();
            }
            else
            {               
                if (laser != null && !laser.IsEnemyLaser)
                    laser.DestroyObjectAndParent();
            }
        }
    }
}
