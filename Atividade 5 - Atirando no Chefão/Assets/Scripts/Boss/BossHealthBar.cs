using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Text bossStateText;
    [SerializeField] private Color highPhaseColor = new Color(0.2f, 0.85f, 0.4f, 1f);
    [SerializeField] private Color mediumPhaseColor = new Color(1f, 0.7f, 0.2f, 1f);
    [SerializeField] private Color lowPhaseColor = new Color(0.85f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color invulnerableWarningColor = new Color(1f, 1f, 0.65f, 1f);

    public void Refresh(float currentHealth, float maxHealth, Boss.HealthPhase phase, bool isInvulnerable)
    {
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }

        if (fillImage != null)
        {
            fillImage.color = isInvulnerable ? invulnerableWarningColor : GetPhaseColor(phase);
        }

        if (bossStateText != null)
        {
            bossStateText.text = isInvulnerable
                ? "INVULNERÁVEL"
                : $"FASE: {phase.ToString().ToUpper()}";
        }
    }

    private Color GetPhaseColor(Boss.HealthPhase phase)
    {
        return phase switch
        {
            Boss.HealthPhase.High => highPhaseColor,
            Boss.HealthPhase.Medium => mediumPhaseColor,
            _ => lowPhaseColor
        };
    }
}
