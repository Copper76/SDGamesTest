using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryInfo : MonoBehaviour
{
    public GameObject[] items;
    public int[] item_id;
    public int[] stack;
    public Dictionary<int, int> resourceCount;

    public int[] maxStackDict;//an array representing stack limit for item based on id
    public Dictionary<int, int> equipSlot;

    public GameObject[] pickUpPrefabs;

    private int inventoryOffset = 4;

    public void SetInventoryOffset(int offset)
    {
        inventoryOffset = offset;
    }

    //Add to empty slot

    public void AddItem(Sprite item, int id, int index, int count = 1)
    {
        items[index].GetComponent<Image>().sprite = item;
        item_id[index] = id;
        stack[index] = count;
        TextMeshProUGUI text = items[index].GetComponentInChildren<TextMeshProUGUI>();
        text.enabled = true;
        text.text = stack[index].ToString();
    }

    public void RemoveItem(int index, int count = 1)
    {
        if (stack[index] == count)
        {
            items[index].GetComponent<Image>().sprite = null;
            item_id[index] = 0;
            stack[index] = 0;
            TextMeshProUGUI text = items[index].GetComponentInChildren<TextMeshProUGUI>();
            text.enabled = false;
        }
        else
        {
            stack[index] -= count;
            TextMeshProUGUI text = items[index].GetComponentInChildren<TextMeshProUGUI>();
            text.text = stack[index].ToString();
        }
    }

    public void StackItem(int index, int count = 1)
    {
        stack[index] += count;
        TextMeshProUGUI text = items[index].GetComponentInChildren<TextMeshProUGUI>();
        text.text = stack[index].ToString();
    }

    public int FindStackSlot(int id)
    {
        int target_index;
        int start_index = inventoryOffset;
        do
        {
            target_index = Array.IndexOf(item_id, id, start_index);
            start_index = target_index + 1;
        } while (target_index != -1 && stack[target_index] >= maxStackDict[id] && start_index < item_id.Length);
        return target_index;
    }

    public int FindAvailableSlot()
    {
        for (int i = inventoryOffset; i < stack.Length; i++)
        {
            if (item_id[i] == 0)
            {
                return i;
            }
        }
        return -1;
    }

    public int FindItem(int id)
    {
        for (int i = inventoryOffset; i < stack.Length; i++)
        {
            if (item_id[i] == id)
            {
                return i;
            }
        }
        return -1;
    }
}
