using UnityEngine;

public class Powerup : MonoBehaviour
{   

    [SerializeField] float _speed = 5f;
    [SerializeField] float _bottomBounds = -10f;
    [SerializeField] PowerupType _powerTypeID;

    [Tooltip("If Powerup is refill type, enter amount to be refilled, otherwise it is ignored.")]
    [SerializeField] int _powerupAmount = 5;

    [SerializeField] AudioClip _clip;

    private void Start()
    {
        //Debug.Break();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (transform.position.y < _bottomBounds)
        {
            //Debug.Log(transform.position.y);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"Hit: {other.name}");
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponentInParent<Player>();
            switch (_powerTypeID)
            {
                case PowerupType.TripleShot:
                    player?.ActivateTripleShot();
                    break;
                case PowerupType.SpeedBoost:
                    player?.ActivateSpeedBoost();
                    break;
                case PowerupType.Shield:
                    player?.ActivateShield();
                    break;
                case PowerupType.Ammo:
                    player?.RefillAmmo(_powerupAmount);
                    break;
                case PowerupType.Health:
                    player?.RestoreHealth(_powerupAmount);
                    break;
                case PowerupType.ScatterShot:
                    player?.ActivateScatterShot();
                    break;
                case PowerupType.SlowDown:
                    player?.ActivateSlowdown(_powerupAmount);
                    break;
                case PowerupType.WeaponJam:
                    player?.ActivateWeaponJam();
                    break;
                case PowerupType.SlowLasers:
                    player?.ActivateSlowLasers();
                    break;
                default:
                    break;
            }
            AudioSource.PlayClipAtPoint(_clip, transform.position);
            Destroy(this.gameObject);
        }
    }
}
