using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    MouseLock mouseLock;

    public float walkSpeed;
    public float runSpeed;
    private float movementSpeed;

    public float jumpHeight;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private CharacterController characterController;

    public float mouseSensitivity;
    private float xAxisClamp;
    public Transform playerCamera;

    private RaycastHit lookInfo;
    public LayerMask interactable;
    public GameObject lookedAtObject;

    public ItemSpawner itemSpawner;

    public Hotbar hotbar;

    private float interactRange = 2.0f;

    public bool CanMove { get; set; }
    public bool CanLook { get; set; }

    public delegate void OnPetCall();
    public event OnPetCall onPetCall;

    public delegate void OnGameObjectInteraction(string objectInteractedWith);
    public event OnGameObjectInteraction onGameObjectInteraction;

    private void Awake()
    {
        // initialize database on game launch
        if (Persistent.itemDatabase.items.Count == 0)
        {
            Persistent.itemDatabase.items.Add(Resources.Load<Item>("ScriptableObjects/AppleItem"));
            Persistent.itemDatabase.items.Add(Resources.Load<Item>("ScriptableObjects/BallItem"));
            Persistent.itemDatabase.items.Add(Resources.Load<Item>("ScriptableObjects/WhistleItem"));
        }

        Persistent.playerInventory.AddItem(Persistent.itemDatabase.ItemByName("Apple"), 10);
        Persistent.playerInventory.AddItem(Persistent.itemDatabase.ItemByName("Ball"), 10);
        Persistent.playerInventory.AddItem(Persistent.itemDatabase.ItemByName("Whistle"), 1);

        if (hotbar)
            hotbar.Init();

        xAxisClamp = 0.0f;

        characterController = GetComponent<CharacterController>();
        CanMove = true;
        movementSpeed = walkSpeed;

        LockCursor();

        mouseLock = FindObjectOfType<MouseLock>();
    }

    private void Start()
    {
        mouseLock.onMouseLockStateChange += SetCursorLockState;
    }

    void SetCursorLockState()
    {
        //CanLook = (Cursor.lockState != CursorLockMode.Locked);
        if (Cursor.lockState == CursorLockMode.None)
            ReleaseCursor();
        else if (Cursor.lockState == CursorLockMode.Locked)
            LockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CanLook = true;
        CanMove = true;
    }

    public void ReleaseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        CanLook = false;
        CanMove = false;
    }

    void Update()
    {
        if (hotbar)
        {
            if (hotbar.GetSelectedItem() != null)
            {
                if (hotbar.GetSelectedItem().item.type == Item.ItemType.Spawnable)
                {
                    itemSpawner.Track(true);
                    if (Input.GetMouseButtonDown(0))
                    {
                        InventoryItem selectedItem = hotbar.GetSelectedItem();

                        if (selectedItem.item.name.Contains("Ball"))
                            itemSpawner.ThrowItem(selectedItem.item, this);
                        else
                            itemSpawner.SpawnItem(selectedItem.item);
                    }
                }
                else if (hotbar.GetSelectedItem().item.type == Item.ItemType.Usable)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        InventoryItem selectedItem = hotbar.GetSelectedItem();

                        UseItem(selectedItem);
                    }
                }
            }
        }

        if (CanLook)
        {
            MouseLook();
            LookCast();
        }

        Movement();

        if (Input.GetKeyDown(KeyCode.F))
            Interact(KeyCode.F);
        else if (Input.GetKeyDown(KeyCode.E))
            Interact(KeyCode.E);
    }

    private void Movement()
    {
        Vector2 movementInput = Vector2.zero;
        bool jumpInput = false;

        if (CanMove)
        {
            movementInput.x = Input.GetAxis("Horizontal");
            movementInput.y = Input.GetAxis("Vertical");

            jumpInput = Input.GetKey(KeyCode.Space);
        }

        Vector3 moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y;
        if (moveDirection.magnitude > 1.0f)
            moveDirection = moveDirection.normalized;

        if (characterController.isGrounded)
        {
            movementSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

            if (jumpInput)
                velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (velocity.y < 0)
                velocity.y = 0f;
        }

        velocity.x = moveDirection.x * movementSpeed;
        velocity.z = moveDirection.z * movementSpeed;
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    public void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xAxisClamp += mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            //ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            //ClampXAxisRotationToValue(90.0f);
        }

        playerCamera.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void LookCast()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out lookInfo, interactRange, interactable))
        {
            lookedAtObject = lookInfo.collider.gameObject;
        }
        else
            lookedAtObject = null;
    }

    private void Interact(KeyCode keyPressed)
    {
        if (lookedAtObject)
        {
            if (keyPressed == KeyCode.F)
            {
                if (lookedAtObject.CompareTag("Pet"))
                {
                    lookedAtObject.GetComponentInParent<Pet>().PetPet();
                }
                else if (lookedAtObject.CompareTag("Shop"))
                {
                    lookedAtObject.GetComponent<ShopObject>().OpenShop();
                }
                else if (lookedAtObject.CompareTag("Event"))
                {
                    lookedAtObject.GetComponent<EventObject>().OpenEvents();
                }
                else if (lookedAtObject.CompareTag("NPC"))
                {
                    //lookedAtObject.GetComponentInParent<NPC>().SetPlayer(this);
                    lookedAtObject.GetComponentInParent<NPC>().Interact();
                }
                else if (lookedAtObject.CompareTag("Ball"))
                {
                    if (lookedAtObject.GetComponent<Ball>())
                        lookedAtObject.GetComponent<Ball>().Pickup();
                    else if (lookedAtObject.GetComponentInParent<Ball>())
                        lookedAtObject.GetComponentInParent<Ball>().Pickup();
                }
                else if (lookedAtObject.CompareTag("Mailbox"))
                {
                    if (lookedAtObject.transform.parent)
                        onGameObjectInteraction(lookedAtObject.transform.parent.name);
                }
            }
            else if (keyPressed == KeyCode.E)
            {
                if (lookedAtObject.CompareTag("Ball"))
                {
                    if (lookedAtObject.GetComponent<Ball>())
                        lookedAtObject.GetComponent<Ball>().Kick(this.transform, transform.forward + transform.up, 200f);
                    else if (lookedAtObject.GetComponentInParent<Ball>())
                        lookedAtObject.GetComponentInParent<Ball>().Kick(this.transform, transform.forward + transform.up, 200f);
                }
            }
        }
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }

    private void UseItem(InventoryItem item)
    {
        if (item.item.type == Item.ItemType.Usable)
        {
            if (item.item.itemName.Contains("Whistle"))
            {
                onPetCall();
            }
        }
    }
}
