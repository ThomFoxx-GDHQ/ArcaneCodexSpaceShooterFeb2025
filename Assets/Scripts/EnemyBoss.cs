using System;
using System.Collections;
using UnityEngine;

public class EnemyBoss : MonoBehaviour, IEnemy
{
    enum BossState
    {
        None,
        Intro,
        Idle,
        Attack
    }

    private BossState _currentState = BossState.Intro;
    [SerializeField] private float _yStopPosition;
    [SerializeField] private float _speed = 3.5f;
    private int _idleDirection = 1;
       
    void Start()
    {
        
    }

    void Update()
    {
        switch (_currentState)
        {
            case BossState.Intro:
                BossEntry();
                break;
            case BossState.Idle:
                BossIdle();
                break;
            case BossState.Attack:
                break;
            default:
                break;
        }
    }

    private void BossEntry()
    {
        //if (transform.position.y > _yStopPosition)
        //    transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        //else
        //    _currentState = BossState.Attack;

        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (transform.position.y < _yStopPosition)
            StartCoroutine(StateChangeDelay(1.5f,BossState.Idle));
    }

    private void BossIdle()
    {

        transform.Translate(Vector3.right * (_idleDirection * _speed * Time.deltaTime));

        if (transform.position.x > 6 || transform.position.x < -6)
            _idleDirection *= -1;
    }

    IEnumerator StateChangeDelay(float delay, BossState state)
    {
        _currentState = BossState.None;
        yield return new WaitForSeconds(delay);
        _currentState = state;
    }


    public void Damage()
    {
        throw new System.NotImplementedException();
    }

    public void FireAtPowerup()
    {
        Debug.LogWarning("Not Implemented Here", this.gameObject);
    }

    public EnemyMovementType GetEnemyMovementType()
    {
        return EnemyMovementType.TopDown;
    }
}
