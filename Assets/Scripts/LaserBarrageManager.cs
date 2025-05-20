using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class LaserBarrageManager : MonoBehaviour
{
    private Vector3 _orignPosition;
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
    }
    
    [ContextMenu("Start Barrage Test")]
    public void StartBarrage()
    {
        _active = true;

        _gapPoints.Clear();
        //Random Selection of Gap
        int gapIndex = Random.Range(1, _firePoints.Length - 1);

        _gapPoints.Add(_firePoints[gapIndex - 1]);
        _gapPoints.Add(_firePoints[gapIndex]);
        _gapPoints.Add(_firePoints[gapIndex + 1]);

        StartCoroutine(LaserBarrageRoutine());
    }
}
