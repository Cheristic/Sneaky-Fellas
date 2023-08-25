using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(menuName = "new Item")]
public abstract class ItemClass : MonoBehaviour
{
    [Header("Item")]
    public string itemName;
    public Sprite droppedSprite;
    public Sprite pickedUpSprite;
    public GameObject objectInstance;

    public abstract void PickUp();


    public abstract void Use();
}
