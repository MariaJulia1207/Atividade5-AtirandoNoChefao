using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Iniciando,
        MenuPrincipal,
        Gameplay,
        EndGame
    }

    public enum EndGameResult
    {
        Victory,
        Defeat
    }

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
#if UNITY_2022_2_OR_NEWER
                _instance = FindAnyObjectByType<GameManager>();
#else
                _instance = FindObjectOfType<GameManager>();
#endif
                if (_instance == null)
                {
                    var go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private GameState initialState = GameState.Iniciando;

    public GameState State { get; private set; }
    public static EndGameResult LastEndGameResult { get; private set; }
    public event Action<GameState> OnStateChanged;

    private bool _isLoadingScene;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        State = initialState;
    }

    public void FinishGame(EndGameResult result)
    {
        if (_isLoadingScene) return;

        LastEndGameResult = result;
        StartCoroutine(FinishGameCoroutine());
    }

    private IEnumerator FinishGameCoroutine()
    {
        _isLoadingScene = true;

        // 1. Descarrega a cena GUI se estiver aberta
        Scene guiScene = SceneManager.GetSceneByName("GUI");
        if (guiScene.isLoaded)
        {
            AsyncOperation unloadGuiOp = SceneManager.UnloadSceneAsync("GUI");
            while (unloadGuiOp != null && !unloadGuiOp.isDone)
                yield return null;
        }

        // 2. Carrega a cena EndGame em modo Aditivo sobre a cena Shooter
        AsyncOperation loadEndGameOp = SceneManager.LoadSceneAsync("EndGame", LoadSceneMode.Additive);
        while (loadEndGameOp != null && !loadEndGameOp.isDone)
            yield return null;

        SetState(GameState.EndGame);
        _isLoadingScene = false;
    }

    public void ForceSceneChange(string sceneName)
    {
        if (_isLoadingScene) return;

        if (sceneName == "Shooter")
        {
            StartCoroutine(LoadGameplayCoroutine());
        }
        else
        {
            StartCoroutine(LoadSingleSceneCoroutine(sceneName));
        }
    }

    private IEnumerator LoadGameplayCoroutine()
    {
        _isLoadingScene = true;

        // 1. Carrega a cena de gameplay principal (Shooter)
        AsyncOperation opShooter = SceneManager.LoadSceneAsync("Shooter", LoadSceneMode.Single);
        while (!opShooter.isDone)
            yield return null;

        // 2. Carrega a cena GUI de forma aditiva por cima da Shooter
        AsyncOperation opGUI = SceneManager.LoadSceneAsync("GUI", LoadSceneMode.Additive);
        while (!opGUI.isDone)
            yield return null;

        SetState(GameState.Gameplay);
        _isLoadingScene = false;
    }

    private IEnumerator LoadSingleSceneCoroutine(string sceneName)
    {
        _isLoadingScene = true;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!op.isDone)
            yield return null;

        MapSceneToState(sceneName);
        _isLoadingScene = false;
    }

    public void SetState(GameState newState)
    {
        if (State == newState) return;

        State = newState;
        OnStateChanged?.Invoke(State);
    }

    private void MapSceneToState(string sceneName)
    {
        switch (sceneName)
        {
            case "_Boot": SetState(GameState.Iniciando); break;
            case "MenuPrincipal": SetState(GameState.MenuPrincipal); break;
            case "Shooter": SetState(GameState.Gameplay); break;
            case "EndGame": SetState(GameState.EndGame); break;
        }
    }
}