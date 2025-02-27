using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
        { get { return _instance; } }

    private int _playerScore = 0;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        UIManager.Instance.UpdateScore(0);
    }

    public void AddToScore(int points)
    {
        _playerScore += points;
        UIManager.Instance.UpdateScore(_playerScore);
    }
}
