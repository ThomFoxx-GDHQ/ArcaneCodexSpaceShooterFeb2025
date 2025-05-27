using System;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float _speed = 5f;
    private bool _isMovingDown;
    private bool _isEnemyLaser;
    private float _speedMultiplier = 1f;
    [SerializeField] float _slowDownMultiplier = .5f;

    public bool IsEnemyLaser => _isEnemyLaser;

    // Update is called once per frame
    void Update()
    {
        if (_isMovingDown)
            MoveDown();
        else
            MoveUp();
    }

    private void MoveUp()
    {
        transform.Translate(_speed * _speedMultiplier * Time.deltaTime * Vector3.up);
        BoundaryCheck();
    }

    private void MoveDown()
    {
        transform.Translate(_speed * _speedMultiplier * Time.deltaTime * Vector3.down);
        BoundaryCheck();
    }

    private void BoundaryCheck()
    {
        if (Mathf.Abs(transform.position.y) > 7 || Mathf.Abs(transform.position.x)>15)
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

    /// <summary>
    /// Assigns the Direction and ownership of the Laser being fired
    /// </summary>
    /// <param name="movingDown"> Is the Laser moving Down the Screen </param>
    /// <param name="enemyLaser"> Is this Laser being Fired by the Enemy Types</param>
    public void AssignLaser(bool movingDown, bool enemyLaser)
    {
        _isMovingDown = movingDown;
        _isEnemyLaser = enemyLaser;
    }

    public void SetSlowLaser()
    {
        _speedMultiplier = _slowDownMultiplier;
    }
}
