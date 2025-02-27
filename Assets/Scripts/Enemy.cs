using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

    [Header("Boundary")]
    [SerializeField] float _topbounds;
    [SerializeField] float _bottombounds;
    [SerializeField] float _leftBounds;
    [SerializeField] float _rightBounds;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
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
        if (other.CompareTag("Player"))
        { 
            other.GetComponent<Player>()?.Damage();
            EnemyDeathSequence();
        }
        if (other.CompareTag("Projectile"))
        {
            other.GetComponent<Laser>()?.DestroyObjectAndParent();
            GameManager.Instance.AddToScore(10);
            EnemyDeathSequence();
        }
    }

    private void EnemyDeathSequence()
    {
        SpawnManager.Instance.OnEnemyDeath();
        
        Destroy(this.gameObject);
    }
}
