using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _mistakesText;
    [SerializeField] private int _scoreAdjustmentPerNote;
    [SerializeField] private GameObject _startGameUI;
    private int _score;
    private int _mistakes;

    public void ShowStartGame()
    {
        _startGameUI.SetActive(true);
    }

    public void HideStartGame()
    {
        _startGameUI.SetActive(false);
    }

    public void AddScore()
    {
        _score += _scoreAdjustmentPerNote;
        UpdateScoreText();
    }

    public void LowerScore()
    {
        _score -= _scoreAdjustmentPerNote;
        _mistakes++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        _scoreText.text = string.Format("Score: {0}", _score);
        _mistakesText.text = string.Format("Mistakes: {0}", _mistakes);
    }
}
