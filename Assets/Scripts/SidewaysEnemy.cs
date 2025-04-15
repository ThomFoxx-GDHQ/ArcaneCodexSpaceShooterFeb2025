using UnityEngine;

public class SidewaysEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] EnemyMovementType _enemyMovementType;
    [SerializeField] float _speed = 2.5f;

    [Tooltip("X = Negative Boundary && Y = Positive Boundary")]
    [SerializeField] Vector2 _horizontalBounds;
    [Tooltip("X = Negative Boundary && Y = Positive Boundary")]
    [SerializeField] Vector2 _verticalBounds;
    int _direction = 1;

    CameraController _cameraController;
    [Tooltip("Controls the Intensity of the Camera Shake when hitting Player")]
    [SerializeField, Range(0, 1)] float _shakeIntensity = .5f;
    [SerializeField, Range(0, 1)] float _shakeTime = 1f;

    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] Vector3 _explosionScale;

    [SerializeField] int _scoreValue = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraController = Camera.main.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.Translate(_direction * _speed * Time.deltaTime * Vector3.right);

        if (transform.position.x > _horizontalBounds.y)
        {
            _direction = -1;
            transform.position = new Vector3(transform.position.x, GetRandomY(), 0);
        }
        else if (transform.position.x < _horizontalBounds.x)
        {
            _direction = 1;
            transform.position = new Vector3(transform.position.x, GetRandomY(), 0);
        }
    }

    private float GetRandomY()
    {
        float RNG = Random.Range(_verticalBounds.x, _verticalBounds.y);
        return RNG;
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
        }
    }

    private void EnemyDeathSequence()
    {
       SpawnManager.Instance.OnEnemyDeath();
       GameObject go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = _explosionScale;
        Destroy(this.gameObject);
    }

    public EnemyMovementType GetEnemyMovementType()
    {
        return _enemyMovementType;
    }
}