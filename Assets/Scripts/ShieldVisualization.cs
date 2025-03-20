using UnityEngine;

public class ShieldVisualization : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _fullStrength;
    [SerializeField] private Color _halfStrength;
    [SerializeField] private Color _lowStrength;

    public void UpdateShieldColor(int health)
    {
        switch (health)
        {
            case 3:
                _renderer.material.color = _fullStrength;
                break;
            case 2:
                _renderer.material.color = _halfStrength;
                break;
            case 1:
                _renderer.material.color = _lowStrength;
                break;
            default:
                _renderer.material.color = _fullStrength;
                break;
        }
    }
}
