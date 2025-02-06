using UnityEngine;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    float _horizontalInput;
    float _verticalInput;
    Vector3 _direction = Vector3.zero;
    Vector3 _position = Vector3.zero;

    [SerializeField] float _upperBounds, _lowerBounds;
    [SerializeField] float _leftRightBounds;

    [SerializeField] Transform _laserContainer;
    [SerializeField] GameObject _laserPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        Bounds();

        //Spawn Laser when hit Space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_laserPrefab != null)
                Instantiate(_laserPrefab, transform.position, Quaternion.identity, _laserContainer);
        }


    }

    private void CalculateMovement()
    {
        //Get Inputs
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        //Assign Inputs to Vector3 Direction
        //_direction = new Vector3(_horizontalInput, _verticalInput);
        _direction.x = _horizontalInput;
        _direction.y = _verticalInput;

        //Use Direction to Translate
        transform.Translate(Time.deltaTime * _speed * _direction);
    }

    private void Bounds()
    {
        _position = transform.position;

        _position.y = Mathf.Clamp(_position.y, _lowerBounds, _upperBounds);

        if (Mathf.Abs(_position.x) >= _leftRightBounds)
            _position.x = _position.x * -1;

        transform.position = _position;

        //if (transform.position.y <= -5)
        //{
        //    position.y = -5;
        //}
        //if (transform.position.y >= 0)
        //{
        //    position.y = 0;
        //}
        //if (transform.position.x <= -9.25f)
        //{
        //    position.x = -9.25f;
        //}
        //if (transform.position.x >= 9.25f)
        //{
        //    position.x = 9.25f;
        //}
    }
}
