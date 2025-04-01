using System.Collections;
using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraController : MonoBehaviour
{
    Camera _camera;   
    Vector3 _homePosition;

    private void Start()
    {
        _camera = Camera.main;
        _homePosition = transform.position;
    }

    public void StartCameraShake(float intensity, float time)
    {
        StartCoroutine(CameraShake(intensity, time));
    }

    IEnumerator CameraShake(float intensity, float time)
    {
        float timer = 0;
        Vector3 currentPosition = _homePosition;
        while (timer <= time)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            float randomX = Random.Range(-1, 1) * intensity;
            float randomY = Random.Range(-1, 1) * intensity;
            currentPosition.x = randomX;
            currentPosition.y = randomY;
            transform.position = currentPosition;
        }
        transform.position = _homePosition;
    }
}
