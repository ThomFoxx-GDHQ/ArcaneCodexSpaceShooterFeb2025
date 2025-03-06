using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public void DestroyExplosion()
    {
        Destroy(this.gameObject);
    }
}
