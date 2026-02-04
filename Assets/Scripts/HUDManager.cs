using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public RectTransform healthBarHUD; 
    public float maxWidthHUD = 400f;
    private HealthBar localPlayerHealth;

    public void SetupHUD(HealthBar health)
    {
        localPlayerHealth = health;

        UpdateHUDVisual(0, health.currentHealth.Value);
        localPlayerHealth.currentHealth.OnValueChanged += UpdateHUDVisual;
    }

    private void UpdateHUDVisual(int oldVal, int newVal)
    {
        if (healthBarHUD == null) return;
        float pct = (float)newVal / localPlayerHealth.maxHealth;
        healthBarHUD.sizeDelta = new Vector2(maxWidthHUD * pct, healthBarHUD.sizeDelta.y);
    }
}