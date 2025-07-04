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
    private bool _isPowerupSpawning = true;
    [SerializeField] private GameObject[] _powerupPrefabs;
    [SerializeField] private int[] _powerupChances;
    private int _powerupChanceTotal = 0;
    [SerializeField] private float[] _powerupPercents;

    [SerializeField] private int _waveMultiplier = 5;    
    private int _enemyWaveCount = 5;
    private int _spawnedEnemyCount = 0;

    [SerializeField] private int _wave = 0;

    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private float _bossSpawnPOS;

    private void Awake()
    {
        _instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemyDelayTimer = new WaitForSeconds(_enemySpawnDelay);
        
        StartCoroutine(WaveAdvance());
        //StartCoroutine(EnemySpawner());
        StartCoroutine(PowerupSpawner());

        foreach (int chance in _powerupChances) //Add up all Powerup Chances
        {
            _powerupChanceTotal += chance;
        }

        _powerupPercents = new float[_powerupChances.Length];

        for (int i = 0;i<_powerupChances.Length;i++)
        {
            _powerupPercents[i]= (float)_powerupChances[i]/_powerupChanceTotal;
        }

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
            yield return _enemyDelayTimer;
        }
    }

    IEnumerator EnemySpawner()
    {
        _spawnedEnemyCount = 0;
        int waveSegement = _wave % 5;
        if (waveSegement == 0)
        { 
            Instantiate(_bossPrefab, new Vector3(0,_bossSpawnPOS,0), Quaternion.identity, _enemyContainer);
            _isSpawning = false;
            yield return _enemyDelayTimer;
            UIManager.Instance.UpdateWaveText(_wave, false);
        }
        else
        {
            while (_isSpawning && _spawnedEnemyCount < _enemyWaveCount)
            {
                yield return _enemyDelayTimer;
                UIManager.Instance.UpdateWaveText(_wave, false);

                if (_enemiesInScene < _maxEnemiesInScene)
                {
                    int randomEnemy = Random.Range(0, _enemyPrefabs.Length);

                    EnemyMovementType type = _enemyPrefabs[randomEnemy].GetComponent<IEnemy>().GetEnemyMovementType();

                    float rng;
                    switch (type)
                    {
                        case EnemyMovementType.TopDown:
                            _spawnPos.y = _topBound;
                            rng = Random.Range(-_leftRightBounds, _leftRightBounds);
                            _spawnPos.x = rng;
                            break;
                        case EnemyMovementType.RightLeft:
                            Debug.LogWarning("Not Implemented Yet");
                            break;
                        case EnemyMovementType.LeftRight:
                            _spawnPos.x = -_leftRightBounds - 2.5f;
                            rng = Random.Range(1, _topBound - 2);
                            _spawnPos.y = rng;
                            break;
                        default:
                            break;
                    }

                    Instantiate(_enemyPrefabs[randomEnemy], _spawnPos, Quaternion.identity, _enemyContainer);
                    _enemiesInScene++;
                    _spawnedEnemyCount++;
                }
            }
        }
        yield return _enemyDelayTimer;       
    }

    IEnumerator PowerupSpawner()
    {
        yield return _enemyDelayTimer;
        while (_isPowerupSpawning)
        {
            _spawnPos.y = _topBound;
            float randomSpawnPOS = Random.Range(-_leftRightBounds, _leftRightBounds);
            _spawnPos.x = randomSpawnPOS;
            int randomPowerup = PickRandomPowerUp();

            Instantiate(_powerupPrefabs[randomPowerup], _spawnPos, Quaternion.identity);
            float RandomSpawnTime = Random.Range(3, 7);
            yield return new WaitForSeconds(RandomSpawnTime);
        }
    }

    private int PickRandomPowerUp()
    {
        int rng = Random.Range(0, _powerupChanceTotal);

        for (int i = 0; i < _powerupChances.Length; i++)
        {
            if (rng < _powerupChances[i])
                return i;
            else
                rng -= _powerupChances[i];
        }
        return 0;
    }

    private int PickRandomPowerupPercentage()
    {
        float rng = Random.value;
        float runningTotal = 0;

        Debug.Log($"Random Number = {rng}%.");

        for (int i = 0; i < _powerupPercents.Length; i++)
        {            
            if (rng < _powerupPercents[i] + runningTotal)
            {

                Debug.Log($"Picked item #{i} : {_powerupPercents[i] + runningTotal}");
                return i;
            }
            else
            {
                Debug.Log($"Adding {_powerupPercents[i]} to {runningTotal} = {runningTotal + _powerupPercents[i]}.");
                runningTotal += _powerupPercents[i];
            }
        }
        return 0;
    }

    public void OnEnemyDeath()
    {
        _enemiesInScene--;
    }

    public void OnPlayerDeath()
    {
        _isSpawning = false;
        _isPowerupSpawning = false;
    }

    public void RestartSpawning()
    {
        _isSpawning = true;
        StartCoroutine(WaveAdvance());
    }
}
