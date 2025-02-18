using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField, Range(0,1)] private float _speed;
    [SerializeField] private MeshRenderer _renderer;
    private Vector2 _movement = Vector2.zero;


    private void Update()
    {
        _movement.y = Time.time * _speed;
        _renderer.material.mainTextureOffset = _movement;
    }
}
