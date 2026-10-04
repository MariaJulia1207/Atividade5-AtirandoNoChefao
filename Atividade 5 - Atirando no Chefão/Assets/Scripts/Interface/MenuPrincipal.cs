using UnityEngine;

/// <summary>
/// Controlador simples para o Menu Principal.
/// Configure os nomes das cenas no Inspector e ligue os botões às funções abaixo.
/// </summary>
public class MenuPrincipalController : MonoBehaviour
{
    [Header("Cenas")]
    public string shooterScene = "Shooter";

    /// <summary>
    /// Botão: Seleção de Bolinhas
    /// </summary>
    public void OnShooterClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RequestSceneChange(shooterScene);
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