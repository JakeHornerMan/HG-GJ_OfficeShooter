using UnityEngine;

public class PickUpPackage : Interactable
{
    public override void Interact()
    {
        Debug.Log($"[PickUpPackage] You picked up a Package.");
        gameManager.PickedUpPackage();
        gameManager.AddToMissionLogs("You have picked up the Picked Up The Package lets Deliver it, proceed to the next floor");
        gameManager.InformPlayerHud("Package Recieved");
        gameObject.SetActive(false);
        hasInteracted = true;
    }
}
