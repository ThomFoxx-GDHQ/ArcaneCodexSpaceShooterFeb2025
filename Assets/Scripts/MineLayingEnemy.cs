using UnityEngine;

public class MineLayingEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] EnemyMovementType _enemyMovementType;
    [SerializeField] float _speed = 5f;
    [SerializeField] GameObject _minePrefab;
    [SerializeField] float _mineRate = 2f;
    float _canMine = 0f;

    [Header("Screen Boundaries")]
    [SerializeField] float _topBounds;
    [SerializeField] float _bottomBounds;
    [SerializeField] float _leftRightBounds;

    Player _player;

    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] Vector3 _explosionScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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

        if (transform.position.y < _bottomBounds)
        {
            float rng = Random.Range(-_leftRightBounds, _leftRightBounds);
            transform.position = new Vector3(rng, _topBounds, 0);
        }
    }

    public EnemyMovementType GetEnemyMovementType()
    {
        return _enemyMovementType;
    }
}
