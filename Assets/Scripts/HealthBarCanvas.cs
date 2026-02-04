using Unity.Netcode;
using UnityEngine;

public class HealthBarCanvas : NetworkBehaviour
{
    public HealthBar health;                 // referència al Health del player
    public RectTransform healthbar;       // el RectTransform del "fill"
    public float maxWidth = 200f;         // amplada total de la barra

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthBar>();
    }

    public override void OnNetworkSpawn()
    {
        if (health == null || healthbar == null) return;

        // Pintar valor inicial
        UpdateBar(health.currentHealth.Value);

        // Hook: quan canviï, actualitza
        health.currentHealth.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (health != null)
            health.currentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateBar(newValue);
    }

    private void UpdateBar(int current)
    {
        float pct = (health.maxHealth <= 0) ? 0f : (float)current / health.maxHealth;
        float w = maxWidth * pct;

        healthbar.sizeDelta = new Vector2(w, healthbar.sizeDelta.y);
    }
}

