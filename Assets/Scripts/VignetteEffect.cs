using UnityEngine;


public class VignetteEffect : MonoBehaviour
{
    [SerializeField] private Material vignetteMaterial;
    
    [Header("Vignette Settings")]
    [Range(0f, 1f)]
    public float intensity = 0.5f;
    [Range(0.1f, 3f)]
    public float radius = 1f;
    public Color vignetteColor = Color.black;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (vignetteMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        vignetteMaterial.SetFloat("_Intensity", intensity);
        vignetteMaterial.SetFloat("_Radius", radius);
        vignetteMaterial.SetColor("_VignetteColor", vignetteColor);
        Graphics.Blit(source, destination, vignetteMaterial);
    }
}