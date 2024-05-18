using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteractable : Interactable
{
    public override void Interact()
    {
        GameManager.GetPlayer().PickUpItem(transform.parent.gameObject);
    }
}
