using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _leftRightBounds = 9f;
    [SerializeField] private float _topBound = 7;
    private Vector3 _spawnPos = Vector3.zero;
    [SerializeField] private float _enemySpawnDelay;
    private WaitForSeconds _enemyDelayTimer;
    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private int _maxEnemiesInScene;
    private int _enemiesInScene;
    private bool _isSpawning = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemyDelayTimer = new WaitForSeconds(_enemySpawnDelay);
        _spawnPos.y = _topBound;
        StartCoroutine(EnemySpawner());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EnemySpawner()
    {
        while (_isSpawning)
        {
            if (_enemiesInScene < _maxEnemiesInScene)
            {
                float rng = Random.Range(-_leftRightBounds, _leftRightBounds);
                _spawnPos.x = rng;

                Instantiate(_enemyPrefab, _spawnPos, Quaternion.identity, _enemyContainer);
                _enemiesInScene++;
            }
            yield return _enemyDelayTimer;
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
