using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;


public class LaserBarrageManager : MonoBehaviour
{
    private Vector3 _orignPosition;
    [SerializeField] private EnemyBoss _enemyBoss;
    [SerializeField] private Transform[] _firePoints;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private Transform _laserContainer;
    [SerializeField] private float _waveSpeed;
    private int _direction = 1;
    [SerializeField] private float _range = 5f;

    [SerializeField] private bool _active = false;
    private Laser _laser;
    [SerializeField] private float _barrageTime = 5f;
    private List<Transform> _gapPoints = new List<Transform>();
    float timer = 0;

    private void Awake()
    {
        _orignPosition = transform.position;
    }

    private void Update()
    {
        if (_active)
        {
            transform.Translate(Vector3.right * (_direction * _waveSpeed * Time.deltaTime));

            if (Mathf.Abs(transform.position.x) > _range)
                _direction *= -1;
        }
    }

    private void FireLaser(Vector3 spawnPOS)
    {
        _laser = Instantiate(_laserPrefab, spawnPOS, Quaternion.identity, _laserContainer).GetComponent<Laser>();
        _laser.AssignLaser(true, true);
        _laser.SetSlowLaser();
        //Debug.Log("Laser Fired",_laser.gameObject);
    }

    IEnumerator LaserBarrageRoutine()
    {        
        timer = Time.time + _barrageTime;
        while (_active && timer > Time.time)
        {
            yield return new WaitForSeconds(.25f);
            foreach (Transform t in _firePoints)
            {
                if (!_gapPoints.Contains(t))
                    FireLaser(t.position);
            }
        }
        _active = false;
        transform.position = _orignPosition;
        _enemyBoss.IsAttackState(false);
        _enemyBoss.StartStateDelay();
    }
    
    [ContextMenu("Start Barrage Test")]
    public void StartBarrage()
    {
        _enemyBoss.IsAttackState(true);
        _active = true;

        _gapPoints.Clear();
        //Random Selection of Gap
        int gapIndex = Random.Range(2, _firePoints.Length - 2);

        _gapPoints.Add(_firePoints[gapIndex - 1]);
        _gapPoints.Add(_firePoints[gapIndex]);
        _gapPoints.Add(_firePoints[gapIndex + 1]);

        StartCoroutine(LaserBarrageRoutine());
    }

    public void StartLaserWall(int numberOfAttacks)
    {
        _enemyBoss.IsAttackState(true);

        StartCoroutine(LaserWallRoutine(numberOfAttacks));
    }

    private void DetermineLaserWallPoints()
    {
        _gapPoints.Clear();

        int gapIndex = Random.Range(3, _firePoints.Length - 3);

        _gapPoints.Add(_firePoints[gapIndex - 2]);
        _gapPoints.Add(_firePoints[gapIndex - 1]);
        _gapPoints.Add(_firePoints[gapIndex]);
        _gapPoints.Add(_firePoints[gapIndex + 1]);
        _gapPoints.Add(_firePoints[gapIndex + 2]);
    }

    IEnumerator LaserWallRoutine(int numberOfAttacks)
    {
        for (int i = 0; i < numberOfAttacks; i++)
        {
            DetermineLaserWallPoints();
            foreach(Transform t in _gapPoints)
            {
                t.GetChild(0).gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(2.1f);
            foreach (Transform t in _gapPoints)
            {
                t.GetChild(0).gameObject.SetActive(false);
            }

            timer = Time.time + 3;
            while (timer > Time.time)
            {
                yield return new WaitForSeconds(.25f);
                foreach (Transform t in _gapPoints)
                {
                    FireLaser(t.position);
                }
            }
            yield return new WaitForSeconds(1);
        }

        _enemyBoss.IsAttackState(false);
        _enemyBoss.StartStateDelay();
    }
}
