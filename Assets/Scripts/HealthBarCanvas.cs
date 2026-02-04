using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class HealthBarCanvas : NetworkBehaviour
{
    public HealthBar health;
    public RectTransform healthbar;
    public float maxWidth = 200f;

    private HUDManager hud;

    public override void OnNetworkSpawn()
    {
        if (health == null) health = GetComponent<HealthBar>();

        UpdateLocalBar(health.currentHealth.Value);
        health.currentHealth.OnValueChanged += (oldVal, newVal) => UpdateLocalBar(newVal);

        if (IsOwner)
        {
            hud = FindFirstObjectByType<HUDManager>();
            if (hud != null)
            {
                hud.SetupHUD(health);
            }
        }
    }

    private void UpdateLocalBar(int current)
    {
        if (healthbar == null) return;
        float pct = (float)current / health.maxHealth;
        healthbar.sizeDelta = new Vector2(maxWidth * pct, healthbar.sizeDelta.y);
    }
}