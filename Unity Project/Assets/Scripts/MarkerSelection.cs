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

    private MarkerHighlight lastMarker;

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
            UIManager._instance.ShowPrompt(PromptType.Move, false);
            WorldManager._instance.TryMoveToNavPoint(hit.collider.gameObject);
        }
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit, layer))
        {
            isHovering = true;

            if (lastMarker == null)
            {
                lastMarker = hit.collider.GetComponent<MarkerHighlight>();
                lastMarker.Highlight();
            }
        }
        else
        {
            isHovering = false;

            if(lastMarker != null)
            {
                lastMarker.Highlight(false);
                lastMarker = null;
            }
        }
    }
}
