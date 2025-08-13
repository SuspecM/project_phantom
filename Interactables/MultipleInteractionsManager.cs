using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultipleInteractionsManager : MonoBehaviour, IInteractable
{
    List<IInteractable> interactables;

    private void Start()
    {
        interactables = new List<IInteractable>();
        Debug.Log("Running interact check");
        Component[] components = GetComponents<Component>();

        foreach (Component component in components)
        {
            if (component is IInteractable)
            {
                interactables.Add(component as IInteractable);

                Debug.Log(component);
            }
        }
    }

    public void Interact(PlayerPickupDrop player, Transform grapPoint)
    {
        foreach (var interactable in interactables)
        {
            // just so we don't get a stack overflow infinite loop
            if (!(interactable is MultipleInteractionsManager))
                interactable.Interact(player, grapPoint);
        }
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
