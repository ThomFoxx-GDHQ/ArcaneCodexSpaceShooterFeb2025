using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpinnerBehaviour : MonoBehaviour
{
    List<Transform> _firePoints = new List<Transform>();
    [SerializeField] float _fallSpeed = 5f;
    [SerializeField] float _spinSpeed = 5f;
    [SerializeField] Transform _model;
    [SerializeField] GameObject _laserPrefab;
    [SerializeField] bool _isBurstFiring = false;
    [SerializeField] float _fireRate = 1f;
    float _fireTime = 0;
    GameObject _laser;
    Transform _laserContainer;
    int _laserIndex = 0;

    private void Start()
    {
        _firePoints = _model.GetComponentsInChildren<Transform>().ToList();
        _firePoints.Remove(_model);
        _laserContainer = GameObject.Find("LaserContainer").transform;
    }

    private void Update()
    {
        CalculateMovement();
        if (_fireTime < Time.time)
            FireLasers();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_fallSpeed * Time.deltaTime), Space.World);
        _model.transform.Rotate(Vector3.forward, _spinSpeed * Time.deltaTime);

        if (transform.position.y < -10)
            Destroy(this.gameObject);
    }

    void FireLasers()
    {
        _fireTime = Time.time +_fireRate;

        if (_isBurstFiring )
        {
            foreach (Transform t in _firePoints)
            {
                _laser = Instantiate(_laserPrefab, transform.position, t.rotation, _laserContainer);
                _laser.GetComponent<Laser>()?.AssignLaser(true, true);
            }    
        }
        else
        {
            _laser = Instantiate(_laserPrefab, transform.position, _firePoints[_laserIndex].rotation, _laserContainer);
            _laser.GetComponent<Laser>()?.AssignLaser(true, true);

            _laserIndex++;
            if (_laserIndex >= _firePoints.Count)
                _laserIndex = 0;
        }
    }

    public void SetBurstFire(bool isBurstFireOn)
    {
        _isBurstFiring = isBurstFireOn;
    }
}
