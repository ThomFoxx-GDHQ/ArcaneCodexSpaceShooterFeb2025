using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.up);
        if (transform.position.y > 7)
            Destroy(this.gameObject);        
    }
}
