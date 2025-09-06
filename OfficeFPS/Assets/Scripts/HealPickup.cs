using UnityEngine;

public class HealPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public bool isHealth = false;
    public bool isShield = false;
    public float amount = 25f; // Amount to heal or shield

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"HealPickup OnTriggerEnter {other.gameObject.name}. Tag: {other.gameObject.tag}");
        if (other.CompareTag("Player"))
        {
            HealthShieldSystem healthSystem = other.GetComponent<HealthShieldSystem>();
            if (healthSystem != null)
            {
                bool applied = false;
                if (isHealth && healthSystem.CurrentHealth < healthSystem.MaxHealth)
                {
                    healthSystem.AddHealth(amount);
                    applied = true;
                }
                else if (isShield && healthSystem.CurrentShield < healthSystem.MaxShield)
                {
                    healthSystem.AddShield(amount);
                    applied = true;
                }

                if (applied)
                {
                    gameObject.SetActive(false);
                    transform.position = new Vector3(-500, -500, -500);
                }
            }
            else
            {
                Debug.LogWarning("Player does not have a HealthShieldSystem component!");
            }
        }
    }
}
