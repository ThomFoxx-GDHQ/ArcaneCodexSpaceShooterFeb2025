using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    public static SpawnManager Instance
    { get { return _instance; } }

    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private float _leftRightBounds = 9f;
    [SerializeField] private float _topBound = 7;
    private Vector3 _spawnPos = Vector3.zero;
    [SerializeField] private float _enemySpawnDelay;
    private WaitForSeconds _enemyDelayTimer;
    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private int _maxEnemiesInScene;
    private int _enemiesInScene;
    private bool _isSpawning = true;
    [SerializeField] private GameObject[] _powerupPrefabs;

    private void Awake()
    {
        _instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemyDelayTimer = new WaitForSeconds(_enemySpawnDelay);
        _spawnPos.y = _topBound;
        StartCoroutine(EnemySpawner());
        StartCoroutine(PowerupSpawner());
    }

    IEnumerator EnemySpawner()
    {
        while (_isSpawning)
        {
            if (_enemiesInScene < _maxEnemiesInScene)
            {
                float rng = Random.Range(-_leftRightBounds, _leftRightBounds);
                int randomEnemy = Random.Range(0, _enemyPrefabs.Length);
                _spawnPos.x = rng;

                Instantiate(_enemyPrefabs[randomEnemy], _spawnPos, Quaternion.identity, _enemyContainer);
                _enemiesInScene++;
            }
            yield return _enemyDelayTimer;
        }
    }

    IEnumerator PowerupSpawner()
    {
        while (_isSpawning)
        {
            float randomSpawnPOS = Random.Range(-_leftRightBounds, _leftRightBounds);
            _spawnPos.x = randomSpawnPOS;
            int randomPowerup = Random.Range(0, _powerupPrefabs.Length);

            Instantiate(_powerupPrefabs[randomPowerup], _spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
    }

    public void OnEnemyDeath()
    {
        _enemiesInScene--;
    }

    public void OnPlayerDeath()
    {
        _isSpawning = false;
    }
}
