using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _notesCountText;
    [SerializeField] private TextMeshProUGUI _mistakesText;
    [SerializeField] private int _scoreAdjustmentPerNote;
    [SerializeField] private GameObject _startGameUI;
    private int _score;
    private int _notesHit;
    private int _notesTotal;
    private int _mistakes;

    public void ShowStartGame()
    {
        _startGameUI.SetActive(true);
    }

    public void HideStartGame()
    {
        _startGameUI.SetActive(false);
    }

    public void AddScore(bool updateNote = false)
    {
        _score += _scoreAdjustmentPerNote;

        if (updateNote)
        {
            _notesHit++;
            _notesTotal++;
        }

        UpdateScoreText();
    }

    public void LowerScore(bool updateNote = false)
    {
        _score -= _scoreAdjustmentPerNote;
        _mistakes++;

        if (updateNote)
        {
            _notesTotal++;
        }

        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        _scoreText.text = string.Format("Score: {0}", _score);
        _notesCountText.text = string.Format("Notes: {0} / {1}", _notesHit, _notesTotal);
        _mistakesText.text = string.Format("Mistakes: {0}", _mistakes);
    }
}
