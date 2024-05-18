using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEditor;

enum InteractMode
{
    NORMAL, INVENTORY, BUILDER, STAT
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Equipper))]
public class PlayerController : MonoBehaviour
{
    public Statistics playerStat;
    private Equipper playerEquipper;

    //Basic movement system
    [Header("Movement")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float airControl;

    private Rigidbody rb;
    private float horizontalSens = 5.0f;
    private float verticalSens = 5.0f;
    private float verticalRotationMinimum = 0f;
    private float verticalRotationMaximum = 30f;

    private float moveSpeed = 15.0f;
    private float limitSpeed = 10.0f;
    private float jumpForce = 240.0f;

    private bool isGrounded = false;
    private float groundCheckDist = 1.3f;

    private InteractMode interactMode = InteractMode.NORMAL;

    private Vector2 moveVector;

    //Necessary Components
    [Header("References")]
    [SerializeField] private TextMeshProUGUI promptText;

    [SerializeField] private Interactable touchedObject;

    [SerializeField] private ParticleSystem particleSys;

    [Header("Statistics")]
    [SerializeField] private string playerDataFile;
    [SerializeField] private StatInfo statInfo;
    private GameObject statMenu;

    //Inventory system related
    [Header("Inventory")]
    [SerializeField] private InventoryInfo inventory;
    private GameObject inventoryMenu;

    [SerializeField] private GameObject selected;
    [SerializeField] private Vector3 dropOffset;
    private int selectedID;
    private int selectedStack;
    private int prevSlot;

    [SerializeField] private GameObject descriptor;

    public int copperCount = 0;
    public int goldCount = 0;

    //Builder related
    [Header("Builder")]
    private GameObject builderMenu;
    [SerializeField] private BuilderInfo builder;

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        playerEquipper = GetComponent<Equipper>();

        inventoryMenu = inventory.transform.parent.gameObject;
        builderMenu = builder.gameObject;
        statMenu = statInfo.gameObject;

        GameManager.SetPlayerController(this);
    }

    private void Start()
    {
        string filePath = Path.Combine(Application.dataPath, playerDataFile);
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            playerStat = JsonUtility.FromJson<Statistics>(jsonString);
            playerStat.HP = playerStat.MAXHP;
        }
        else
        {
            Debug.Log("File not found: " + filePath);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selected.activeInHierarchy)
        {
            selected.GetComponent<RectTransform>().position = Input.mousePosition;
        }
        else
        {
            int slot = IsPointerOverSlot();
            if (slot > -1 && inventory.item_id[slot] != 0)
            {
                descriptor.SetActive(true);
                TextMeshProUGUI[] textFields = descriptor.GetComponentsInChildren<TextMeshProUGUI>();
                Item itemInfo = inventory.pickUpPrefabs[inventory.item_id[slot]].GetComponent<Item>();
                textFields[0].text = itemInfo.pickUpName;
                textFields[1].text = itemInfo.description;
            }
            else
            {
                descriptor.SetActive(false);
            }
        }
        if (descriptor.activeInHierarchy)
        {
            descriptor.GetComponent<RectTransform>().position = Input.mousePosition + new Vector3(50f, -100f, 0f);
        }
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, groundCheckDist);
        Vector3 moveDir = new Vector3(moveVector.x * moveSpeed * playerStat.MOV, 0.0f, moveVector.y * moveSpeed * playerStat.MOV);
        moveDir = transform.TransformDirection(moveDir);
        moveDir = GetSlopeAdjustment(moveDir, transform.localScale.y * 0.5f);
        if (!isGrounded)
        {
            moveDir *= airControl;
        }
        else
        {
            if (moveDir != Vector3.zero)
            {
                var trans = particleSys.transform;
                trans.rotation = Quaternion.LookRotation(-rb.velocity);
                if (!particleSys.isPlaying)
                {
                    particleSys.Play();
                }
            }
            else
            {
                particleSys.Stop();
            }
        }

        rb.AddForce(moveDir * rb.mass);

        float adjsutedLimitSpeed = limitSpeed * playerStat.MOV;

        if (Math.Abs(rb.velocity.x) > adjsutedLimitSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x < 0 ? -adjsutedLimitSpeed : adjsutedLimitSpeed, rb.velocity.y, rb.velocity.z);
        }
        if (Math.Abs(rb.velocity.z) > adjsutedLimitSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z < 0 ? -adjsutedLimitSpeed : adjsutedLimitSpeed);
        }
    }

    private Vector3 GetSlopeAdjustment(Vector3 moveDir, float checkDist)
    {
        RaycastHit slopeHit;
        if (Physics.Raycast(transform.position, -transform.up, out slopeHit, checkDist + 1.0f))
        {
            return Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
        }
        return moveDir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * (transform.localScale.y * 0.5f + 1.0f));
    }

    public void RotateX(InputAction.CallbackContext context)
    {
        if (interactMode == InteractMode.NORMAL)
        {
            float rotationY = horizontalSens * context.ReadValue<float>() * Time.deltaTime;
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.y += rotationY;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    public void RotateY(InputAction.CallbackContext context)
    {
        if (interactMode == InteractMode.NORMAL)
        {
            float rotationX = verticalSens * context.ReadValue<float>() * Time.deltaTime;
            Vector3 cameraRotation = Camera.main.transform.rotation.eulerAngles;
            cameraRotation.x -= rotationX;
            if ((cameraRotation.x > verticalRotationMinimum && rotationX > 0) || (cameraRotation.x < verticalRotationMaximum && rotationX < 0))
            {
                Camera.main.transform.parent.rotation = Quaternion.Euler(cameraRotation);
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (interactMode == InteractMode.NORMAL)
        {
            moveVector = context.ReadValue<Vector2>();
        }
        else
        {
            moveVector = Vector2.zero;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed && interactMode == InteractMode.NORMAL)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0));
        }
    }

    public void ToggleInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (interactMode)
            {
                case InteractMode.NORMAL:
                    inventoryMenu.SetActive(true);
                    interactMode = InteractMode.INVENTORY;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
                case InteractMode.INVENTORY:
                    inventoryMenu.SetActive(false);
                    interactMode = InteractMode.NORMAL;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case InteractMode.BUILDER:
                    builderMenu.SetActive(false);
                    inventoryMenu.SetActive(true);
                    interactMode = InteractMode.INVENTORY;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
                case InteractMode.STAT:
                    statMenu.SetActive(false);
                    inventoryMenu.SetActive(true);
                    interactMode = InteractMode.INVENTORY;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
            }
        }
    }

    public void ToggleStatMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (interactMode)
            {
                case InteractMode.NORMAL:
                    statMenu.SetActive(true);
                    interactMode = InteractMode.STAT;
                    break;
                case InteractMode.INVENTORY:
                    inventoryMenu.SetActive(false);
                    statMenu.SetActive(true);
                    interactMode = InteractMode.STAT;
                    break;
                case InteractMode.BUILDER:
                    builderMenu.SetActive(false);
                    statMenu.SetActive(true);
                    interactMode = InteractMode.STAT;
                    break;
                case InteractMode.STAT:
                    statMenu.SetActive(false);
                    interactMode = InteractMode.NORMAL;
                    break;
            }
            if (interactMode == InteractMode.STAT)
            {
                statInfo.UpdateStatText();
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && touchedObject != null)
        {
            touchedObject.Interact();
        }
    }

    public void Press(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryMenu.activeInHierarchy)
        {
            int selectedSlot = IsPointerOverSlot();
            if (selectedSlot > -1 && inventory.item_id[selectedSlot] != 0)
            {
                selected.SetActive(true);
                descriptor.SetActive(false);
                Image image = inventory.items[selectedSlot].GetComponent<Image>();
                selected.GetComponent<Image>().sprite = image.sprite;
                selectedID = inventory.item_id[selectedSlot];
                selectedStack = inventory.stack[selectedSlot];
                inventory.RemoveItem(selectedSlot, selectedStack);
                prevSlot = selectedSlot;
            }
        }
    }

    public void Release(InputAction.CallbackContext context)
    {
        if (context.performed && selected.activeInHierarchy)
        {
            selected.SetActive(false);
            int selectedSlot = IsPointerOverSlot();
            if (selectedSlot > -1)//The pointer is on a valid slot
            {
                int targetID = inventory.item_id[selectedSlot];
                int targetStack = inventory.stack[selectedSlot];
                if (((!inventory.equipSlot.ContainsKey(selectedID) || inventory.equipSlot[selectedID] != selectedSlot) && selectedSlot < 4) || (targetID != 0 && prevSlot < 4 && (!inventory.equipSlot.ContainsKey(targetID) || inventory.equipSlot[targetID] != prevSlot)))//The target is invalid or the item in the target slot cannot be swapped to the current slot
                {
                    inventory.AddItem(selected.GetComponent<Image>().sprite, selectedID, prevSlot, selectedStack);
                    return;
                }

                if (targetID == selectedID)//The target slot has the same type of item
                {
                    if (targetStack + selectedStack <= inventory.maxStackDict[selectedID])//The item can be fully transferred
                    {
                        inventory.StackItem(selectedSlot, selectedStack);
                    }
                    else//There are too many items to transfer
                    {
                        inventory.AddItem(selected.GetComponent<Image>().sprite, selectedID, prevSlot, selectedStack + targetStack - inventory.maxStackDict[selectedID]);
                        inventory.StackItem(selectedSlot, inventory.maxStackDict[selectedID] - targetStack);

                    }
                    TextMeshProUGUI text = inventory.items[selectedSlot].GetComponentInChildren<TextMeshProUGUI>();
                    text.text = targetStack.ToString();
                }
                else//The target slot has a different kind of item
                {
                    if (targetID != 0)
                    {
                        inventory.AddItem(inventory.items[selectedSlot].GetComponent<Image>().sprite, targetID, prevSlot, targetStack);
                    }
                    inventory.AddItem(selected.GetComponent<Image>().sprite, selectedID, selectedSlot, selectedStack);
                }

                if (selectedSlot < 4)
                {
                    playerEquipper.OnEquip(selectedID);
                    if (targetID != 0)
                    {
                        playerEquipper.OnUnequip(targetID);
                    }
                }
                if (prevSlot < 4)
                {
                    playerEquipper.OnUnequip(selectedID);
                    if (targetID != 0)
                    {
                        playerEquipper.OnEquip(targetID);
                    }
                }
            }
            else if (selectedSlot == -1)//The pointer is not in on a slot but still in menu, reset the transfer
            {
                inventory.AddItem(selected.GetComponent<Image>().sprite, selectedID, prevSlot, selectedStack);
            }
            else//The pointer is outside the menu so the items should be dropped
            {
                GameObject droppedItem = Instantiate(inventory.pickUpPrefabs[selectedID], transform.position+(transform.rotation * dropOffset), Quaternion.identity);
                //droppedItem.name = selectedID.ToString();
                droppedItem.name = inventory.pickUpPrefabs[selectedID].name;
                droppedItem.GetComponent<Item>().stack = selectedStack;
                if (selectedID == 1)
                {
                    copperCount -= selectedStack;
                }
                if (selectedID == 2)
                {
                    goldCount -= selectedStack;
                }
            }
        }
    }

    public void Split(InputAction.CallbackContext context)
    {
        if (context.performed && selected.activeInHierarchy)
        {
            int half = selectedStack / 2;
            if (half != 0)
            {
                inventory.AddItem(selected.GetComponent<Image>().sprite, selectedID, prevSlot, half);
                int emptySlot = inventory.FindAvailableSlot();
                if (emptySlot == -1)
                {
                    inventory.StackItem(prevSlot, selectedStack - half);
                }
                else
                {
                    inventory.AddItem(selected.GetComponent<Image>().sprite, selectedID, emptySlot, selectedStack - half);
                }
            }
            else
            {
                inventory.AddItem(selected.GetComponent<Image>().sprite, selectedID, prevSlot, selectedStack);
            }
            selected.SetActive(false);
        }
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public int IsPointerOverSlot()
    {
        return IsPointerOverSlot(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private int IsPointerOverSlot(List<RaycastResult> eventSystemRaysastResults)
    {
        bool hitMenu = false;
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("Slot"))
            {
                return Convert.ToInt32(curRaysastResult.gameObject.name);
            }
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("InventoryMenu"))
            {
                hitMenu = true;
            }
        }
        return hitMenu ? -1 : -2;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public void PickUpItem(GameObject other)
    {
        Item item = other.GetComponent<Item>();
        int target_id = item.item_id;
        int count = item.stack;
        while (count > 0)
        {
            int target_index = inventory.FindStackSlot(target_id);
            if (target_index == -1)//Cannot stack
            {
                //add to an empty slot
                target_index = inventory.FindAvailableSlot();
                if (target_index != -1)
                {
                    inventory.AddItem(item.displayImage, target_id, target_index, count);
                    if (target_id == 1)
                    {
                        copperCount += count;
                    }
                    if (target_id == 2)
                    {
                        goldCount += count;
                    }
                    break;
                }
                else
                {
                    //Inventory is full, anything necessary?
                }
            }
            else
            {
                //stack to an existing stack
                int stackAmount = Mathf.Min(inventory.maxStackDict[target_id] - inventory.stack[target_index], count);
                inventory.StackItem(target_index, stackAmount);
                count -= stackAmount;
                if (target_id == 1)
                {
                    copperCount += stackAmount;
                }
                if (target_id == 2)
                {
                    goldCount += stackAmount;
                }
            }
        }
        Destroy(other);
        promptText.gameObject.SetActive(false);
    }

    public void ConsumeItem(int target_id, int amount)
    {
        int remaining = amount;
        while (remaining > 0)
        {
            int target_index = inventory.FindItem(target_id);
            if (target_index == -1)//Item not found
            {
                Debug.Log("Consuming non-existant item");
                return;
            }
            else
            {
                int consumeAmount = Mathf.Min(inventory.stack[target_index], remaining);
                inventory.RemoveItem(target_index, consumeAmount);
                remaining -= consumeAmount;
            }
        }
    }

    public void ToggleBuildingMenu(BuildInteractable builderInteractable)
    {
        switch (interactMode)
        {
            case InteractMode.NORMAL:
                builderMenu.SetActive(true);
                interactMode = InteractMode.BUILDER;
                break;
            case InteractMode.INVENTORY:
                inventoryMenu.SetActive(false);
                builderMenu.SetActive(true);
                interactMode = InteractMode.BUILDER;
                break;
            case InteractMode.BUILDER:
                builderMenu.SetActive(false);
                interactMode = InteractMode.NORMAL;
                break;
            case InteractMode.STAT:
                statMenu.SetActive(false);
                builderMenu.SetActive(true);
                interactMode = InteractMode.BUILDER;
                break;
        }
        if (interactMode == InteractMode.BUILDER)
        {
            builder.UpdateUI(builderInteractable);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectible")
        {
            PickUpItem(other.gameObject);
            promptText.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (touchedObject == null)
        {
            if (other.tag == "Pickup" || other.tag == "Builder")
            {
                touchedObject = other.GetComponent<Interactable>();
            }
        }
        else
        {
            promptText.gameObject.SetActive(true);
            switch (touchedObject.tag)
            {
                case "Pickup":
                    promptText.SetText("PRESS E TO PICKUP");
                    break;
                case "Builder":
                    promptText.SetText("PRESS E TO BUILD");
                    break;

            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickup" || other.tag == "Builder")
        {
            promptText.gameObject.SetActive(false);
            touchedObject = null;
        }
    }
}
