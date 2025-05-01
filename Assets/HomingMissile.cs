using ArcaneCodex.Utilities;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] float _speed = 6f;
    [SerializeField] float _engageDelay;
    [SerializeField] Transform _model;

    float _startTime;

    GameObject _owner;
    GameObject _target;

    Vector3 _direction = Vector3.up;

    private void Start()
    {
        //if (!_owner.CompareTag("Player"))
        {
            _direction = Vector3.down;
            _target = GameObject.FindGameObjectWithTag("Player");
        }

        _model.rotation = Utilities2D.LookAt2D(_direction, transform.position);
    }

    private void Update()
    {
        if (_startTime + _engageDelay > Time.time)
            transform.Translate(_direction * (_speed * Time.deltaTime));
        else
        {
            _model.rotation = Utilities2D.LookAt2D(_target.transform.position, transform.position);

            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _speed * Time.deltaTime);
        }
    }
}
