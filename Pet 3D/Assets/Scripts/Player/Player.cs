using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float movementSpeed;
    public float jumpSpeed;
    public float runMultiplier;
    public float gravity = -9.81f;
    Vector3 velocity;
    public float mouseSensitivity;
    private float xAxisClamp;
    private CharacterController characterController;

    public Hotbar hotbar;

    public ItemSpawner itemSpawner;
    public Transform playerCamera;

    [SerializeField] Shop shop;

    float petInteractionDistance = 2f;
    Pet pet;
    public bool IsWithinPetInteractionDistance { get; set; }
    public bool IsWithinShopInteractionDistance { get; set; }

    public static bool CanMove { get; set; }
    public static bool CanLook { get; set; }

    private void Awake()
    {
        Persistent.itemDatabase.Add(Resources.Load<Item>("ScriptableObjects/AppleItem"));
        Persistent.itemDatabase.Add(Resources.Load<Item>("ScriptableObjects/BallItem"));

        Persistent.playerInventory.AddItemToPlayerInventory(Persistent.itemDatabase[0]);
        Persistent.playerInventory.AddItemToPlayerInventory(Persistent.itemDatabase[1]);

        hotbar.Init();

        for (int i = 0; i < hotbar.itemSlots.Length; i++)
        {
            if (i < Persistent.playerInventory.inventory.Count)
                hotbar.AssignItemToIndex(Persistent.playerInventory.inventory[i], i);
        }

        xAxisClamp = 0.0f;

        characterController = GetComponent<CharacterController>();
        CanMove = true;

        LockCursor();

        itemSpawner = GetComponent<ItemSpawner>();

        pet = FindObjectOfType<Pet>();
        IsWithinShopInteractionDistance = false;
        IsWithinPetInteractionDistance = false;
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
                    itemSpawner.SpawnItem(hotbar.selectedItem.item);
            }
        }


        if (CanLook)
            CameraRotation();

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * x + transform.forward * y;

        if (CanMove)
            characterController.Move(movement * movementSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            characterController.Move(movement * Time.deltaTime * runMultiplier);
        }

        CheckDistanceBetweenPlayerAndPet();

        if (IsWithinPetInteractionDistance)
        {
            if (Input.GetKeyDown(KeyCode.F))
                Pet();
        }
        if (IsWithinShopInteractionDistance)
        {

        }
    }

    private void CheckDistanceBetweenPlayerAndPet()
    {
        float dist = Vector3.Distance(transform.position, pet.transform.position);
        if (dist <= petInteractionDistance)
            IsWithinPetInteractionDistance = true;
        else
            IsWithinPetInteractionDistance = false;
    }

    void Pet()
    {
        pet.PetPet();
    }

    public void CameraRotation()
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

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shop"))
        {
            IsWithinShopInteractionDistance = true;
            shop = other.GetComponentInParent<Shop>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shop"))
        {
            IsWithinShopInteractionDistance = false;
            shop = null;
        }
    }
}
