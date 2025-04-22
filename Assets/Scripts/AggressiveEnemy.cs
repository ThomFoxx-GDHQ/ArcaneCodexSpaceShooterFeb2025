using UnityEngine;
using ArcaneCodex.Utilities;

public class AggressiveEnemy : MonoBehaviour
{
    [SerializeField] EnemyMovementType _enemyMovementType;
    [SerializeField] float _speed = 5f;

    [Header("Boundary")]
    [SerializeField] float _topbounds;
    [SerializeField] float _bottombounds;
    [SerializeField] float _leftBounds;
    [SerializeField] float _rightBounds;

    [Space(10)]
    [SerializeField] GameObject _explosion;
    [SerializeField] Vector3 _explosionScale;
    [SerializeField] CameraController _cameraController;
    [Tooltip("Controls the Intensity of the Camera Shake when hitting Player")]
    [SerializeField, Range(0, 1)] float _shakeIntensity = .5f;
    [SerializeField, Range(0, 1)] float _shakeTime = 1f;

    [SerializeField] int _scoreValue = 10;

    Player _player;

    [SerializeField] float _attackDistance = 2f;
    bool _canRamPlayer;

    [SerializeField] GameObject _model;
    Quaternion _defaultRotation;

    private void Start()
    {
        _player = GameObject.FindFirstObjectByType<Player>();
        _cameraController = Camera.main.GetComponent<CameraController>();
        _defaultRotation = _model.transform.rotation;
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(_player.transform.position, transform.position) < _attackDistance)
        {
            _canRamPlayer = true;
        }
        else if (Vector2.Distance(_player.transform.position, transform.position) > _attackDistance + 1)
        {
            _canRamPlayer = false;
        }
    }

    private void Update()
    {
        if (_canRamPlayer == false)
        {
            transform.Translate(Vector3.down * (_speed * Time.deltaTime));

            if (_model.transform.rotation != _defaultRotation)
                _model.transform.rotation = _defaultRotation;
        }
        else
        {
            //transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);


            _model.transform.rotation = Utilities.LookAt2D(_player.transform.position, transform.position);

        }
    }
}
