using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
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

    public static bool CanMove { get; set; }
    public static bool CanLook { get; set; }

    private void Awake()
    {
        Persistent.itemDatabase.Add(Resources.Load<Item>("ScriptableObjects/AppleItem"));
        Persistent.itemDatabase.Add(Resources.Load<Item>("ScriptableObjects/BallItem"));

        Persistent.playerInventory.AddItem(Persistent.itemDatabase[0]);
        Persistent.playerInventory.AddItem(Persistent.itemDatabase[1]);

        hotbar.Init();

        for (int i = 0; i < hotbar.itemSlots.Length; i++)
        {
            if (i < Persistent.playerInventory.inventory.Count)
                hotbar.AssignItemToSlot(Persistent.playerInventory.inventory[i], i);
        }

        xAxisClamp = 0.0f;

        characterController = GetComponent<CharacterController>();
        CanMove = true;
        movementSpeed = walkSpeed;

        LockCursor();
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CanLook = true;
    }

    public static void ReleaseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        CanLook = false;
    }

    void Update()
    {
        if (hotbar.selectedItem != null)
        {
            if (hotbar.selectedItem.item.type == ItemType.Spawnable)
            {
                itemSpawner.Track(true);
                if (Input.GetMouseButtonDown(0))
                {
                    itemSpawner.SpawnItem(hotbar.selectedItem.item);
                    hotbar.UpdateSlot();
                }
            }
        }

        if (CanLook)
            MouseLook();

        Movement();

        LookCast();

        if (lookedAtObject && Input.GetKeyDown(KeyCode.F))
            Interact();
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
            ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
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

    private void Interact()
    {
        if (lookedAtObject)
        {
            if (lookedAtObject.CompareTag("Pet"))
            {
                lookedAtObject.GetComponent<Pet>().PetPet();
            }
            else if (lookedAtObject.CompareTag("Shop"))
            {
                
            }
        }
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}
