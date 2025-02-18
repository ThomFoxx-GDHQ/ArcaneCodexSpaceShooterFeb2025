using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Hit: {other.name}");
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>()?.ActivateTripleShot();
            Destroy(this.gameObject);
        }
    }
}
