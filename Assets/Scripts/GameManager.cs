using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MusicPlayer _musicPlayer;
    [SerializeField] private NoteScroller _noteScroller;
    [SerializeField] private NoteJudger _noteJudger;
    [SerializeField] private UIManager _uiManager;

    private PlayerInput _playerInput;
    private InputAction _gameStart;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _gameStart = _playerInput.Player.Start;
    }

    private void Start()
    {
        _musicPlayer.enabled = false;
        _noteScroller.enabled = false;
        _noteJudger.enabled = false;

        _uiManager.ShowStartGame();
    }

    private void OnEnable()
    {
        _gameStart.performed += StartGame;
        _gameStart.Enable();
    }

    private void OnDisable()
    {
        _gameStart.performed -= StartGame;
        _gameStart.Disable();
    }

    private void StartGame(InputAction.CallbackContext context)
    {
        _musicPlayer.enabled = true;
        _noteScroller.enabled = true;
        _noteJudger.enabled = true;

        _uiManager.HideStartGame();
    }
}
