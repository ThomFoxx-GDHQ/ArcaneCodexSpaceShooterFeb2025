using UnityEngine;

public class MineControl : MonoBehaviour
{
    [SerializeField] GameObject _explosion;
    [SerializeField] Vector3 _explosionScale;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>()?.Damage();

            GameObject go = Instantiate(_explosion, transform.position, Quaternion.identity);
            go.transform.localScale = _explosionScale;
            
            Destroy(this.gameObject);
        }
    }
}
