using ArcaneCodex.Utilities;
using TMPro;
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

    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] Vector2 _hitScale;
    [SerializeField] Vector2 _blastScale;

    Vector3 _direction = Vector3.up;

    [SerializeField] float _detonationTimer = 5f;
    bool _isDetonating = false;

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
        _detonationTimer -= Time.deltaTime;

        if (_detonationTimer <= 0 && _isDetonating == false)
        {
            _isDetonating = true;
            Denotate();
        }

        if (_startTime + _engageDelay > Time.time)
            transform.Translate(_direction * (_speed * Time.deltaTime));
        else
        {
            _model.rotation = Utilities2D.LookAt2D(_target.transform.position, transform.position);

            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _owner) return;
        if (other.gameObject == _target || other.transform.parent.gameObject == _target)
        {
            if (other.transform.parent.TryGetComponent<Player>(out Player player))
            {
                player.Damage();
                GameObject go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                go.transform.localScale = _hitScale;
                Destroy(this.gameObject);
            }
            else if (other.TryGetComponent<IEnemy>(out IEnemy enemy))
            {
                enemy.Damage();
                GameObject go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                go.transform.localScale = _hitScale;
                Destroy(this.gameObject);
            }
        }

    }

    private void Denotate()
    {
        //Spawn the Large Explosion
        GameObject go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = _blastScale;
        //Get a list of all objects in range
        //do Damage to all objects that can take damage
        Destroy(this.gameObject);
    }
}
