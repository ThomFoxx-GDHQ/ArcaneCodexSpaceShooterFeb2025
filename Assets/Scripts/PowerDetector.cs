using System.Collections;
using UnityEngine;

public class PowerDetector : MonoBehaviour
{
    [SerializeField] GameObject _enemyParent;
    [SerializeField] float _detectionCooldown = 2.5f;
    Collider2D _detectionCollider;

    private void Start()
    {
        _detectionCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Powerup>(out Powerup target))
        {
            _enemyParent.GetComponent<IEnemy>().FireAtPowerup();
            _detectionCollider.enabled = false;
            StartCoroutine(DetctionCooldown());
        }
    }

    IEnumerator DetctionCooldown()
    {
        yield return new WaitForSeconds(_detectionCooldown);
        _detectionCollider.enabled = true;
    }
}
