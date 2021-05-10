using UnityEngine;
using UnityEngine.InputSystem;

public class MarkerSelection : MonoBehaviour
{
    public LayerMask layer;
    public Camera cam;

    private Ray ray;
    private RaycastHit hit;

    public bool isHovering = false;
    public Vector2 mousePos;

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mousePos = context.ReadValue<Vector2>();
        }
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (context.canceled && isHovering)
        {
            WorldManager._instance.TryMoveToNavPoint(hit.collider.gameObject);
        }
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit, layer))
        {
            isHovering = true;
        }
        else
        {
            isHovering = false;
        }
    }
}
