using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInput playerInput;
    private InputAction touchPressAction;
    [SerializeField] SlotMachine slotMachine;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions.FindAction("TouchPress");
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
    }
    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        slotMachine.StartSpin();
    }
}
