using UnityEngine;

public class MarkerHighlight : MonoBehaviour
{
    public GameObject rendObject;
    [ColorUsage(true, true)]
    public Color emissionColorOn, emissionColorOff;
    public float minAlpha = .3f, maxAlpha = 1;
    public float highLightScale = 1.2f;
    public float scaleTime = .3f;

    private bool highLighted = false;

    private Renderer rend;
    private Color col;
    private Vector3 normalScale, highLightedScale;
    private float scale = 0;

    private void Awake()
    {
        rend = rendObject.GetComponent<Renderer>();
        col = rend.material.color;
        normalScale = transform.localScale;
        highLightedScale = transform.localScale * highLightScale;
    }

    private void Update()
    {
        if (highLighted && scale < 1)
        {
            scale += Time.deltaTime / scaleTime;
            scale = Mathf.Clamp(scale, 0, 1);
            transform.localScale = Vector3.Lerp(normalScale, highLightedScale, scale);
        }
        
        if(!highLighted && scale > 0)
        {
            scale -= Time.deltaTime / scaleTime;
            scale = Mathf.Clamp(scale, 0, 1);
            transform.localScale = Vector3.Lerp(normalScale, highLightedScale, scale);
        }
    }

    public void Highlight(bool highlight = true)
    {
        highLighted = highlight;
        if (highlight)
        {
            rend.material.SetColor("_EmissionColor", emissionColorOn);
            col.a = maxAlpha;
        }
        else
        {
            rend.material.SetColor("_EmissionColor", emissionColorOff);
            col.a = minAlpha;
        }

        rend.material.SetColor("_Color", col);
    }
}
