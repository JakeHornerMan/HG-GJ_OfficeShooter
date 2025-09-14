using UnityEngine;

public class ElevatorButton : Interactable
{
    public override void Interact()
    {
        Debug.Log($"[ElevatorButton] You pressed the elevator button.");
        gameManager.LoadNextScene();
        hasInteracted = true;
    }
}

