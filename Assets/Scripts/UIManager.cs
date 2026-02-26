using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Scoring")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _notesCountText;
    [SerializeField] private TextMeshProUGUI _mistakesText;

    [Header("Combo")]
    [SerializeField] private Slider _comboGaugeSlider;
    [SerializeField] private TextMeshProUGUI _multiplierText;

    [SerializeField] private GameObject _startGameUI;

    public void ShowStartGame()
    {
        _startGameUI.SetActive(true);
    }

    public void HideStartGame()
    {
        _startGameUI.SetActive(false);
    }

    public void UpdateScoreText(int score, int notesHit, int notesTotal, int mistakes)
    {
        _scoreText.text = string.Format("Score: {0}", score);
        _notesCountText.text = string.Format("Notes: {0} / {1}", notesHit, notesTotal);
        _mistakesText.text = string.Format("Mistakes: {0}", mistakes);
    }

    public void UpdateCombo(int comboCount, int multiplier)
    {
        _comboGaugeSlider.value = comboCount;
        _multiplierText.text = string.Format("Multiplier: X{0}", multiplier);
    }
}
