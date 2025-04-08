using System.Collections;
using System.Runtime.CompilerServices;
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

    [SerializeField] private int _waveMultiplier = 5;    
    private int _enemyWaveCount = 5;
    private int _spawnedEnemyCount = 0;

    [SerializeField] private int _wave = 0; 

    private void Awake()
    {
        _instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemyDelayTimer = new WaitForSeconds(_enemySpawnDelay);
        _spawnPos.y = _topBound;
        StartCoroutine(WaveAdvance());
        //StartCoroutine(EnemySpawner());
        StartCoroutine(PowerupSpawner());
    }

    /*private void WaveAdvance()
    {
        _wave++;
        _enemyWaveCount = _wave * _waveMultiplier;
        StartCoroutine(EnemySpawner());
    }*/

    IEnumerator WaveAdvance()
    {
        while (_isSpawning)
        {
            _wave++;
            UIManager.Instance.UpdateWaveText(_wave, true);
            _enemyWaveCount = _wave * _waveMultiplier;
            yield return StartCoroutine(EnemySpawner());
            Debug.Log($"Wave # {_wave} has ended.");
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator EnemySpawner()
    {
        _spawnedEnemyCount = 0;
        while (_isSpawning && _spawnedEnemyCount < _enemyWaveCount)
        {
            yield return _enemyDelayTimer;
            UIManager.Instance.UpdateWaveText(_wave, false);

            if (_enemiesInScene < _maxEnemiesInScene)
            {
                float rng = Random.Range(-_leftRightBounds, _leftRightBounds);
                int randomEnemy = Random.Range(0, _enemyPrefabs.Length);
                _spawnPos.x = rng;

                Instantiate(_enemyPrefabs[randomEnemy], _spawnPos, Quaternion.identity, _enemyContainer);
                _enemiesInScene++;
                _spawnedEnemyCount++;
            }
            
        }
        yield return _enemyDelayTimer;
        //WaveAdvance();
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
