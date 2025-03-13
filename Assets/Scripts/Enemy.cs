using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
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



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _laserContainer = GameObject.Find("LaserContainer").transform;
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
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        if (transform.position.y <= _bottombounds)
        {
            float randX = Random.Range(_leftBounds, _rightBounds);
            transform.position = new Vector2(randX, _topbounds);
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
