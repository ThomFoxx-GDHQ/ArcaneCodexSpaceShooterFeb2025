using UnityEngine;

public class AmmoShuffle : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private BoxCollider2D _boxCollider;
    private float _height = 0;
    private float _width = 0;

    //[ContextMenu("Restart Object")]
    private void Start()
    {
        int rng = Random.Range(0, _sprites.Length);
        if (_spriteRenderer != null )
            _spriteRenderer.sprite = _sprites[rng];

        _height = _sprites[rng].bounds.extents.y * 2;
        _width = _sprites[rng].bounds.extents.x * 2;

        //Debug.Log($"{_sprites[rng].bounds}");

        _boxCollider.size = new Vector2( _width, _height );
    }
}
