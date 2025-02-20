using UnityEngine;

public class Powerup : MonoBehaviour
{   

    [SerializeField] float _speed = 5f;
    [SerializeField] float _bottomBounds = -10f;
    [SerializeField] PowerupType _powerTypeID;

    private void Start()
    {
        //Debug.Break();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));

        if (transform.position.y < _bottomBounds)
        {
            Debug.Log(transform.position.y);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Hit: {other.name}");
        if (other.CompareTag("Player"))
        {
            switch (_powerTypeID)
            {
                case PowerupType.TripleShot:
                    other.GetComponent<Player>()?.ActivateTripleShot();
                    break;
                case PowerupType.SpeedBoost:
                    other.GetComponent<Player>()?.ActivateSpeedBoost();
                    break;
                default:
                    break;
            }

            Destroy(this.gameObject);
        }
    }
}
