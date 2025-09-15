using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameManager gameManager;
    public bool hasInteracted = false;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hasInteracted = false;
    }

    public virtual void Interact()
    {
        Debug.Log("you have interacted with this object");
        gameManager.AddToMissionLogs("you have interacted with this object");
    }
}
