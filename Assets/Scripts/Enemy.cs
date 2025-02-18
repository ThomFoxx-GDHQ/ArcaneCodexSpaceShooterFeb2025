using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

    [Header("Boundary")]
    [SerializeField] float _topbounds;
    [SerializeField] float _bottombounds;
    [SerializeField] float _leftBounds;
    [SerializeField] float _rightBounds;
    private SpawnManager _spawnManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnManager = GameObject.FindFirstObjectByType<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is Null!!!");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"Hit: {other.name}");
        if (other.CompareTag("Player"))
        { 
            Debug.Log("Hit Player");
            other.GetComponent<Player>()?.Damage();

            /*
            //player player = getcomponent<player>();
            //if (player != null)
            //{
            //    player.damage();
            //}*/

            EnemyDeathSequence();
        }
        if (other.CompareTag("Projectile"))
        {
            other.GetComponent<Laser>()?.DestroyObjectAndParent();
            EnemyDeathSequence();
        }
    }

    private void EnemyDeathSequence()
    {
        _spawnManager.OnEnemyDeath();
        Destroy(this.gameObject);
    }
}
