using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FlowManager : MonoBehaviour
{
    public static FlowManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private TextMeshProUGUI _startText;
    [SerializeField] private Button _startButton;
    
    [Header("Game Over UI")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private Button _restartButton;
    
    [Header("Game Objects")]
    [SerializeField] private GameObject _player;
    [SerializeField] private List<GameObject> _managers = new List<GameObject>();

    [Header("Fade Settings")]
    [SerializeField] private float _fadeDuration = 1f;
    
    private bool _isGameStarted;
    private Coroutine _currentFadeCoroutine;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        _isGameStarted = false;
        
        // UI 초기화
        InitializeUI();
        
        // 플레이어와 매니저들 비활성화
        SetGameObjectsActive(false);
    }

    private void InitializeUI()
    {
        // 시작 UI 설정
        if (_startPanel != null)
        {
            _startPanel.SetActive(true);
            SetPanelAlpha(_startPanel, 1f);
            
            if (_startButton != null)
            {
                _startButton.onClick.RemoveAllListeners();
                _startButton.onClick.AddListener(StartGame);
            }
        }
        
        // 게임 오버 UI 설정
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
            SetPanelAlpha(_gameOverPanel, 0f);
            
            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveAllListeners();
                _restartButton.onClick.AddListener(RestartGame);
            }
        }
    }
    
    private void StartGame()
    {
        if (_isGameStarted) return;
        
        _isGameStarted = true;
        
        // 시작 UI 페이드 아웃
        if (_startPanel != null)
        {
            StartFadeCoroutine(_startPanel, 1f, 0f, () => {
                _startPanel.SetActive(false);
                ActivateGame();
            });
        }
        else
        {
            ActivateGame();
        }
    }
    
    private void ActivateGame()
    {
        SetGameObjectsActive(true);
        SetCursorState(true);
    }
    
    public void GameOver()
    {
        if (!_isGameStarted) return;
        
        _isGameStarted = false;
        
        // 게임 오버 UI 페이드 인
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            StartFadeCoroutine(_gameOverPanel, 0f, 1f, () => {
                DeactivateGame();
            });
        }
        else
        {
            DeactivateGame();
        }
    }
    
    private void DeactivateGame()
    {
        SetGameObjectsActive(false);
        SetCursorState(false);
    }
    
    private void RestartGame()
    {
        // 게임 오버 UI 페이드 아웃
        if (_gameOverPanel != null)
        {
            StartFadeCoroutine(_gameOverPanel, 1f, 0f, () => {
                _gameOverPanel.SetActive(false);
                StartGame();
            });
        }
        else
        {
            _gameOverPanel.SetActive(false);
            StartGame();
        }
    }
    
    private void StartFadeCoroutine(GameObject panel, float startAlpha, float targetAlpha, System.Action onComplete = null)
    {
        if (_currentFadeCoroutine != null)
        {
            StopCoroutine(_currentFadeCoroutine);
        }
        
        _currentFadeCoroutine = StartCoroutine(FadeCoroutine(panel, startAlpha, targetAlpha, onComplete));
    }
    
    private IEnumerator FadeCoroutine(GameObject panel, float startAlpha, float targetAlpha, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / _fadeDuration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            
            SetPanelAlpha(panel, currentAlpha);
            
            yield return null;
        }
        
        SetPanelAlpha(panel, targetAlpha);
        onComplete?.Invoke();
    }
    
    private void SetPanelAlpha(GameObject panel, float alpha)
    {
        if (panel == null) return;
        
        foreach (var graphic in panel.GetComponentsInChildren<Graphic>())
        {
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
    
    private void SetGameObjectsActive(bool active)
    {
        if (_player != null)
        {
            _player.SetActive(active);
        }
        
        foreach (var manager in _managers)
        {
            if (manager != null)
            {
                manager.SetActive(active);
            }
        }
    }

    private void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
    
    public bool IsGameStarted => _isGameStarted;
} 