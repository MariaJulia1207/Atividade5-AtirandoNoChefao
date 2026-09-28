using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager responsável apenas por manter o singleton, o estado da cena e
/// as transições entre cenas que o projeto usa no momento.
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Iniciando,
        MenuPrincipal,
        Gameplay
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

    [SerializeField]
    private GameState initialState = GameState.Iniciando;

    public GameState State { get; private set; }
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        MapSceneToState(SceneManager.GetActiveScene().name);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MapSceneToState(scene.name);
    }

    private void MapSceneToState(string sceneName)
    {
        switch (sceneName)
        {
            case "_Boot":
                SetState(GameState.Iniciando);
                break;
            case "MenuPrincipal":
                SetState(GameState.MenuPrincipal);
                break;
            case "SampleScene":
                SetState(GameState.Gameplay);
                break;
        }
    }

    public bool RequestSceneChange(string sceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning("GameManager: já está carregando uma cena.");
            return false;
        }

        if (State == GameState.Iniciando)
        {
            Debug.LogWarning($"GameManager: mudança para '{sceneName}' negada — estado atual: {State}");
            return false;
        }

        StartCoroutine(LoadSceneCoroutine(sceneName, LoadSceneMode.Single));
        return true;
    }

    public void ForceSceneChange(string sceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning("GameManager: já está carregando uma cena.");
            return;
        }

        StartCoroutine(LoadSceneCoroutine(sceneName, LoadSceneMode.Single));
    }

    public void StartBootLoad(string targetSceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning("GameManager: já está carregando uma cena.");
            return;
        }

        StartCoroutine(LoadSceneFromBootCoroutine(targetSceneName));
    }

    public void SetState(GameState newState)
    {
        if (State == newState)
            return;

        State = newState;
        Debug.Log($"GameManager: estado alterado para {State}");
        OnStateChanged?.Invoke(State);
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode mode)
    {
        _isLoadingScene = true;

        Debug.Log($"GameManager: carregando cena '{sceneName}' (mode={mode})...");

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
        if (op == null)
        {
            Debug.LogError($"GameManager: cena '{sceneName}' não encontrada ou LoadSceneAsync retornou null.");
            _isLoadingScene = false;
            yield break;
        }

        while (!op.isDone)
            yield return null;

        MapSceneToState(sceneName);
        _isLoadingScene = false;
    }

    private IEnumerator LoadSceneFromBootCoroutine(string targetSceneName)
    {
        _isLoadingScene = true;

        var currentScene = SceneManager.GetActiveScene();
        Debug.Log($"GameManager: Boot sequence — carregando '{targetSceneName}' additivamente...");

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        if (loadOp == null)
        {
            Debug.LogError($"GameManager: falha ao iniciar carregamento de '{targetSceneName}'.");
            _isLoadingScene = false;
            yield break;
        }

        while (!loadOp.isDone)
            yield return null;

        var loadedScene = SceneManager.GetSceneByName(targetSceneName);
        if (loadedScene.IsValid())
            SceneManager.SetActiveScene(loadedScene);

        var unloadOp = SceneManager.UnloadSceneAsync(currentScene);
        if (unloadOp != null)
        {
            while (!unloadOp.isDone)
                yield return null;
        }

        MapSceneToState(targetSceneName);
        _isLoadingScene = false;

        Debug.Log($"GameManager: Boot sequence complete. Loaded '{targetSceneName}'.");
    }
}