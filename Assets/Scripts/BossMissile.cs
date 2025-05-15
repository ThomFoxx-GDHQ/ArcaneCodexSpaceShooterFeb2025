using UnityEngine;

public class BossMissile : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] Vector3 _hitScale = Vector3.one;


    private void Update()
    {
        transform.Translate(Vector3.up *(_speed * Time.deltaTime),Space.Self);

        if (transform.position.x < -12 || transform.position.x > 12)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.transform.parent.TryGetComponent<Player>(out Player player))
        {
            player.Damage();
            GameObject go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            go.transform.localScale = _hitScale;
            Destroy(this.gameObject);
        }
    }
}
