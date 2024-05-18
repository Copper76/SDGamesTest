using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class InventoryInitialiser : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] InventoryInfo inventory;

    [SerializeField] private int inventoryLength;
    [SerializeField] private int inventoryHeight;

    [SerializeField] GameObject weaponSlot;
    [SerializeField] GameObject armourSlot;
    [SerializeField] GameObject legSlot;
    [SerializeField] GameObject braceletSlot;

    private int inventoryOffset = 4;
    private float gap = 100.0f;


    // Start is called before the first frame update
    void Awake()
    {
        //inventory.items = new ItemInfo[inventoryLength];
        int inventorySize = inventoryLength * inventoryHeight;
        inventory.items = new GameObject[inventorySize+ inventoryOffset];
        inventory.item_id = new int[inventorySize + inventoryOffset];
        inventory.stack = new int[inventorySize + inventoryOffset];
        inventory.items[0] = weaponSlot;
        inventory.items[1] = armourSlot;
        inventory.items[2] = legSlot;
        inventory.items[3] = braceletSlot;
        for (int j = 0; j < inventoryHeight; j++) 
        {
            for (int i = 0; i < inventoryLength; i++)
            {
                int index = j * inventoryLength + i+ inventoryOffset;
                GameObject itemObject = new GameObject(index.ToString());
                itemObject.transform.parent = transform;
                itemObject.layer = 6;
                Image image = itemObject.AddComponent<Image>();
                image.color = new Color(1f, 1f, 1f, 0.2f);
                itemObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-50f+i*gap,100f-j*gap, 0.0f);
                itemObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                GameObject count = new GameObject("Count");
                count.transform.parent = itemObject.transform;
                TextMeshProUGUI text = count.AddComponent<TextMeshProUGUI>();
                text.text = "0";
                text.color = new Color(0.0f,0.0f,0.0f,1.0f);
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 12;
                count.GetComponent<RectTransform>().anchoredPosition = new Vector3(20f, -20f, 0.0f);
                text.enabled = false;
                inventory.items[index] = itemObject;
                inventory.item_id[index] = 0;
                inventory.stack[index] = 0;
            }
        }
        transform.parent.gameObject.SetActive(false);
        inventory.maxStackDict = new int[] { 1, 64, 64, 1, 1, 1, 1, 1 };
        inventory.equipSlot = new Dictionary<int, int> { {3, 0}, {4, 1}, { 5, 1 }, {6, 2}, {7, 3} };
        inventory.SetInventoryOffset(inventoryOffset);
        Destroy(this);
    }
}
