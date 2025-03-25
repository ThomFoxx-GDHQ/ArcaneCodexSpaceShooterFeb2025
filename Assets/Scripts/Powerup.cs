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
            switch (_powerTypeID)
            {
                case PowerupType.TripleShot:
                    other.GetComponentInParent<Player>()?.ActivateTripleShot();
                    break;
                case PowerupType.SpeedBoost:
                    other.GetComponentInParent<Player>()?.ActivateSpeedBoost();
                    break;
                case PowerupType.Shield:
                    other.GetComponentInParent<Player>()?.ActivateShield();
                    break;
                case PowerupType.Ammo:
                    other.GetComponentInParent<Player>()?.RefillAmmo(_powerupAmount);
                    break;
                default:
                    break;
            }
            AudioSource.PlayClipAtPoint(_clip, transform.position);
            Destroy(this.gameObject);
        }
    }
}
