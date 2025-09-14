using UnityEngine;

public class DeliverPackage : Interactable
{
    public override void Interact()
    {
        Debug.Log($"[PickUpPackage] You Delivered the Package.");
        gameManager.DeliveredUpPackage();
        gameManager.AddToMissionLogs("You have Delivered the Package, Task completed now back to the basement with you!");
        gameManager.InformPlayerHud("Package Delivered");
        gameObject.SetActive(false);
        hasInteracted = true;
    }
}
