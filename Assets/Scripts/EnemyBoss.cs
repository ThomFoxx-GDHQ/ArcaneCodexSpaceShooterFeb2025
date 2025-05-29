using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBoss : MonoBehaviour, IEnemy
{
    // Boss behavior states
    enum BossState
    {
        None,
        Intro,
        Idle,
        Attack
    }

    // Types of attacks the boss can perform
    enum AttackStates
    {
        Missile,
        Tractor,
        Bullet,
        LaserBarrage,
        LaserWall,
        Spinner,
        BurstSpinner
    }

    private BossState _currentState = BossState.Intro;
    private AttackStates _currentAttack;

    [SerializeField] private float _yStopPosition;
    [SerializeField] private float _speed = 3.5f;
    private int _idleDirection = 1;
    private bool _isAttacking = false;
    private float _randomTime = 0;

    [SerializeField] private List<Transform> _missileFireLocations = new List<Transform>();
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private LaserBarrageManager _laserBarrage;
    [SerializeField] private GameObject _spinnerPrefab;
    [SerializeField] private LineRenderer _tractorBeamRender;

    Transform _player;

    void Start()
    {
        // Initialization logic (currently unused)
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // State machine controlling the boss's current behavior
        switch (_currentState)
        {
            case BossState.None:
                _randomTime = 0;
                break;
            case BossState.Intro:
                BossEntry();
                break;
            case BossState.Idle:
                BossIdle();
                break;
            case BossState.Attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private void BossEntry()
    {
        // Move downward until reaching stop position, then transition to idle
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (transform.position.y < _yStopPosition)
            StartCoroutine(StateChangeDelay(1.5f, BossState.Idle));
    }

    private void BossIdle()
    {
        // Horizontal movement with direction reversal at screen bounds
        transform.Translate(Vector3.right * (_idleDirection * _speed * Time.deltaTime));
        if (transform.position.x > 6 || transform.position.x < -6)
            _idleDirection *= -1;

        // Random timer triggers attack after 5–10 seconds
        if (_randomTime <= 0)
        {
            _randomTime = Random.Range(5, 10f) + Time.time;
        }
        else if (_randomTime <= Time.time)
        {
            StartCoroutine(StateChangeDelay(1, BossState.Attack));
        }
        else
        {
            _randomTime -= Time.deltaTime;
        }
    }

    private void Attack()
    {
        if (_isAttacking) return;

        // Pick and execute a random attack (currently only Missile implemented)
        int varlength = Enum.GetValues(typeof(AttackStates)).Length;
        _currentAttack = (AttackStates)Random.Range(0, varlength);
        Debug.Log($"{_currentAttack.ToString()} is the next attack");

        switch (_currentAttack)
        {
            //case AttackStates.Missile:
            //    StartCoroutine(MissileAttackRoutine());
            //    break;
            case AttackStates.LaserBarrage:
                _laserBarrage.StartBarrage();
                break;
            case AttackStates.LaserWall:
                _laserBarrage.StartLaserWall();
                break;
            //case AttackStates.Spinner:
            //    StartCoroutine(SpinnerAttack(false));
            //    break;
            //case AttackStates.BurstSpinner:
            //    StartCoroutine(SpinnerAttack(true));
            //    break;
            //case AttackStates.Tractor:
            //    StartCoroutine(TractorBeamRoutine());
            //    break;
            default:
                break;
        }
    }

    IEnumerator SpinnerAttack(bool isBursting)
    {
        _isAttacking = true;

        GameObject go = Instantiate(_spinnerPrefab, transform.position, Quaternion.identity);
        go.GetComponent<SpinnerBehaviour>()?.SetBurstFire(isBursting);
        yield return new WaitForSeconds(5);
        _isAttacking = false;
        StartCoroutine(StateChangeDelay(5, BossState.Idle));
    }

    IEnumerator MissileAttackRoutine()
    {
        _isAttacking = true;

        // Choose random fire points and launch missiles
        int attacks = Random.Range(3, 6);
        List<Transform> attackPoints = new List<Transform>();

        for (int i = 0; i < attacks; i++)
        {
            int randomPoint = Random.Range(0, _missileFireLocations.Count - 1);
            attackPoints.Add(_missileFireLocations[randomPoint]);
            _missileFireLocations.RemoveAt(randomPoint);
        }

        foreach (Transform attackPoint in attackPoints)
        {
            Instantiate(_missilePrefab, attackPoint.position, attackPoint.rotation);
        }

        // Return used fire points back to the pool
        for (int j = attacks - 1; j >= 0; j--)
        {
            _missileFireLocations.Add(attackPoints[j]);
            attackPoints.RemoveAt(j);
        }

        yield return null;
        _isAttacking = false;

        //_randomTime = 0;
        StartCoroutine(StateChangeDelay(5, BossState.Idle));
    }

    IEnumerator TractorBeamRoutine()
    {
        _isAttacking = true;
        _player.GetComponent<Player>()?.CaughtByTractor(true);

        _tractorBeamRender.positionCount = 2;
        _tractorBeamRender.SetPosition(0, transform.position);
        _tractorBeamRender.SetPosition(1, _player.position);

        yield return new WaitForSeconds(5);

        _tractorBeamRender.positionCount = 0;

        _player.GetComponent<Player>()?.CaughtByTractor(false);

        yield return null;
        _isAttacking = false;
        StartCoroutine(StateChangeDelay(5, BossState.Idle));
    }

    IEnumerator StateChangeDelay(float delay, BossState state)
    {
        // Pause current action, then switch to the next state after delay
        _currentState = BossState.None;
        yield return new WaitForSeconds(delay);
        _currentState = state;
    }

    public void IsAttackState(bool isAttacking)
    {
        _isAttacking = isAttacking;
    }

    public void StartStateDelay()
    {
        StartCoroutine(StateChangeDelay(5, BossState.Idle));
    }

    // --- IEnemy Interface Implementation ---

    public void Damage()
    {
        // To be implemented
        throw new System.NotImplementedException();
    }

    public void FireAtPowerup()
    {
        // Boss does not target powerups
        Debug.LogWarning("Not Implemented Here", this.gameObject);
    }

    public EnemyMovementType GetEnemyMovementType()
    {
        return EnemyMovementType.TopDown;
    }
}