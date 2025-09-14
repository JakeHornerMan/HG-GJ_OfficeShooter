using UnityEngine;

public class KeycardInteractable : Interactable
{
    public override void Interact()
    {
        Debug.Log($"[KeycardInteractable] You picked up a Keycard.");
        gameManager.GotKeyCard();
        gameManager.AddToMissionLogs("You have picked up the key card proceed to the next floor");
        gameManager.InformPlayerHud("Key Card Collected");
        gameObject.SetActive(false);
        hasInteracted = true;
    }
}