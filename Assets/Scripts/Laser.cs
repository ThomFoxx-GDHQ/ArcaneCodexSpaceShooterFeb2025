using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.up);
        if (transform.position.y > 7)
        {
            DestroyObjectAndParent();
        }
    }

    public void DestroyObjectAndParent()
    {
        if (transform.parent.childCount <= 1 && !transform.parent.CompareTag("Container"))
            Destroy(transform.parent.gameObject);
        else
            transform.parent = null;

        Destroy(this.gameObject);
    }
}
