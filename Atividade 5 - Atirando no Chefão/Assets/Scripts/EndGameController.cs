using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameController : MonoBehaviour
{
    [Header("UI via Inspector")]
    [SerializeField] private Text resultText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Cores")]
    [SerializeField] private Color victoryColor = new Color(0.22f, 0.78f, 0.32f, 1f);
    [SerializeField] private Color defeatColor = new Color(0.82f, 0.12f, 0.12f, 1f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (SceneManager.GetActiveScene().name != "EndGame")
            return;

        if (Object.FindAnyObjectByType<EndGameController>() != null)
            return;

        var go = new GameObject("EndGameController");
        go.AddComponent<EndGameController>();
    }

    private void Awake()
    {
        if (resultText == null || restartButton == null || menuButton == null)
        {
            BuildEndGameScreen();
        }

        ConfigureEndGameScreen(GameManager.LastEndGameResult);
    }

    public void ConfigureEndGameScreen(GameManager.EndGameResult result)
    {
        if (resultText == null)
            return;

        bool isVictory = result == GameManager.EndGameResult.Victory;
        resultText.text = isVictory ? "Vitória!" : "Game Over";
        resultText.color = isVictory ? victoryColor : defeatColor;

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RetryGame);
        }

        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(true);
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(BackToMenu);
        }
    }

    private void BuildEndGameScreen()
    {
        var canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            var canvasObject = new GameObject("EndGameCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        var root = new GameObject("EndGameRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var background = root.AddComponent<Image>();
        background.color = new Color(0.07f, 0.07f, 0.07f, 0.85f);

        var resultTextObject = new GameObject("ResultText");
        resultTextObject.transform.SetParent(root.transform, false);

        resultText = resultTextObject.AddComponent<Text>();
        resultText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        resultText.fontSize = 72;
        resultText.alignment = TextAnchor.MiddleCenter;
        resultText.color = victoryColor;

        var resultRect = resultTextObject.GetComponent<RectTransform>();
        resultRect.anchorMin = new Vector2(0.5f, 0.7f);
        resultRect.anchorMax = new Vector2(0.5f, 0.7f);
        resultRect.sizeDelta = new Vector2(700f, 120f);
        resultRect.anchoredPosition = Vector2.zero;

        restartButton = CreateButton(root.transform, "RestartButton", "Restart", new Vector2(0.5f, 0.30f), new Vector2(260f, 70f), new Color(0.22f, 0.55f, 0.95f, 1f), RetryGame);
        menuButton = CreateButton(root.transform, "MenuButton", "Voltar ao Menu", new Vector2(0.5f, 0.15f), new Vector2(260f, 70f), new Color(0.95f, 0.68f, 0.12f, 1f), BackToMenu);
    }

    private Button CreateButton(Transform parent, string name, string label, Vector2 anchorPosition, Vector2 size, Color color, UnityEngine.Events.UnityAction onClick)
    {
        var buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);

        var buttonRect = buttonObject.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = size;
        buttonRect.anchoredPosition = anchorPosition;

        var buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = color;

        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(onClick);

        var textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);

        var buttonText = textObject.AddComponent<Text>();
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.text = label;
        buttonText.fontSize = 26;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.black;

        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return button;
    }

    private void RetryGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForceSceneChange("Shooter");
            return;
        }

        SceneManager.LoadScene("Shooter");
    }

    private void BackToMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForceSceneChange("MenuPrincipal");
            return;
        }

        SceneManager.LoadScene("MenuPrincipal");
    }
}
