using ArcaneCodex.Utilities;
using UnityEditor;
using UnityEngine;

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
    [SerializeField] float _blastDistance;

    Vector3 _direction = Vector3.up;

    [SerializeField] float _detonationTimer = 5f;
    bool _isDetonating = false;

    Collider2D[] _blastedObjects;

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
            Detonate();
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
        if (other.gameObject == _target || other.transform.parent?.gameObject == _target)
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

    private void Detonate()
    {
        //Spawn the Large Explosion
        GameObject go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = _blastScale;

        //Get a list of all objects in range
        _blastedObjects = Physics2D.OverlapCircleAll(transform.position, _blastDistance);

        //do Damage to all objects that can take damage
        foreach (Collider2D obj in _blastedObjects)
        {
            //Debug.Log(obj.name, this.gameObject);
            if (obj.TryGetComponent<IEnemy>(out IEnemy enemy))
                enemy.Damage();
            else if (obj.TryGetComponent<IEnemy>(out IEnemy enemyAlso))
                enemyAlso.Damage();
            else if (obj.TryGetComponent<Powerup>(out Powerup powerup))
                Destroy(powerup.gameObject);
            else if (obj.TryGetComponent<MineControl>(out MineControl mine))
                Destroy(mine.gameObject);
            else if (obj.CompareTag("Player"))            
                obj.transform.parent.GetComponent<Player>()?.Damage();            
        }

        Destroy(this.gameObject);
    }
}
