using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class TeleportActivate : MonoBehaviour
{
    //Object that has the teleporting controller script
    public GameObject teleportController = null;
    //reference to the input action reference that contains the button mapping data for activation
    public InputActionReference teleportActivateReference;
    
    //These will group Unity event calls that you can add to in the inspector
    [Space]
    [Header("Teleport Events")]
    public UnityEvent onTeleportActivate;
    public UnityEvent onTeleportCancel;

    void Start()
    {
        //interaction with the teleportactivationreference has been completed 
        //and performs a callback to the teleport mode activate
        teleportActivateReference.action.performed += TeleportModeActivate;

        //an interaction with the teleportActivation reference has been cancelled and performs
        //a callback to the teleportmodecancel
        teleportActivateReference.action.canceled += TeleportModeCancel;
    }

    //This will let us call series of events created in the onTeleportActivate events in the inspector
    private void TeleportModeActivate(InputAction.CallbackContext obj) =>
        onTeleportActivate.Invoke();
    //this will delay the call of the Delayteleportation function for 0.1 of a second
    private void TeleportModeCancel(InputAction.CallbackContext obj) =>
        Invoke("DelayTeleportation", 0.1f);
    //this will let us call a series of events created in the onTeleportCancel events in the inspector
    private void DelayTeleportation() => onTeleportCancel.Invoke();

}
