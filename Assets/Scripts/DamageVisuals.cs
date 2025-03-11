using System.Collections.Generic;
using UnityEngine;

public class DamageVisuals : MonoBehaviour
{
    [SerializeField] private List<GameObject> _allDamageVisuals = new List<GameObject>();
    private List<GameObject> _enabledVisuals = new List<GameObject>();
    private List<GameObject> _disabledVisuals = new List<GameObject>();

    private int _previousHealth = -1;
    private int _randomNumber;

    /// <summary>
    /// Resets the damage visuals by disabling all and clearing tracking lists.
    /// </summary>
    private void ResetObjectState()
    {
        _enabledVisuals.Clear();

        // Disable all damage visuals and reset the disabled list.
        foreach (var visual in _allDamageVisuals)
        {
            visual.SetActive(false);
        }
        _disabledVisuals = new List<GameObject>(_allDamageVisuals);
    }

    /// <summary>
    /// Adjusts the damage visuals based on health changes.
    /// </summary>
    /// <param name="health">The current health of the object.</param>
    public void ApplyVisualDamage(int health)
    {
        // Initialize visuals on first call.
        if (_previousHealth == -1)
        {
            ResetObjectState();
            _previousHealth = health;
            return;
        }

        // Determine whether to add or remove damage effects.
        if (health < _previousHealth)
            AddDamage();
        else
            RemoveDamage();

        _previousHealth = health;
    }

    /// <summary>
    /// Activates a random disabled damage visual to reflect new damage.
    /// </summary>
    private void AddDamage()
    {
        if (_disabledVisuals == null || _disabledVisuals.Count == 0) return;

        _randomNumber = Random.Range(0, _disabledVisuals.Count);
        _disabledVisuals[_randomNumber].SetActive(true);

        // Move the activated visual from the disabled list to the enabled list.
        _enabledVisuals.Add(_disabledVisuals[_randomNumber]);
        _disabledVisuals.RemoveAt(_randomNumber);
    }

    /// <summary>
    /// Deactivates a random enabled damage visual to reflect healing or repair.
    /// </summary>
    private void RemoveDamage()
    {
        if (_enabledVisuals == null || _enabledVisuals.Count == 0) return;

        _randomNumber = Random.Range(0, _enabledVisuals.Count);
        _enabledVisuals[_randomNumber].SetActive(false);

        // Move the deactivated visual from the enabled list to the disabled list.
        _disabledVisuals.Add(_enabledVisuals[_randomNumber]);
        _enabledVisuals.RemoveAt(_randomNumber);
    }
}
