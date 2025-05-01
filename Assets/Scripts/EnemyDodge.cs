using UnityEngine;

public class EnemyDodge : MonoBehaviour
{
    [SerializeField] DodgeDirection _dodgeDirection;
    [SerializeField] Enemy _enemyParent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            if (other.TryGetComponent<Laser>(out Laser laser) && !laser.IsEnemyLaser)
            {
                _enemyParent.DodgeFire(_dodgeDirection);
            }
        }
    }
}
