using UnityEngine;

public class MenuPrincipalController : MonoBehaviour
{
    [Header("Cenas")]
    public string shooterScene = "Shooter";

    /// <summary>
    /// Botão: Iniciar Jogo
    /// </summary>
    public void OnShooterClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ForceSceneChange(shooterScene);
    }

    /// <summary>
    /// Botão: Sair do jogo
    /// </summary>
    public void OnSairClicked()
    {
        Debug.Log("MenuPrincipal: Sair solicitado.");
        Application.Quit();
    }
}