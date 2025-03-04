using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
        { get { return _instance; } }

    private int _playerScore = 0;
    private bool _isGameOver = false;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        UIManager.Instance.UpdateScore(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }

    }

    public void AddToScore(int points)
    {
        _playerScore += points;
        UIManager.Instance.UpdateScore(_playerScore);
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
}
