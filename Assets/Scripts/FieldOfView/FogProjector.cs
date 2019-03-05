using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Projector))]
public class FogProjector : MonoBehaviour
{
    [SerializeField]
    private Material projectorMaterial;

    [SerializeField]
    private RenderTexture fogTexture;

    private RenderTexture prevTexture;
    private RenderTexture currTexture;
    private Projector projector;
    
    private void Awake()
    {
        projector = GetComponent<Projector>();
    }

    private void OnEnable()
    {
        prevTexture = fogTexture;
        currTexture = fogTexture;

        // Projector materials aren't instanced, resulting in the material asset getting changed.
        // Instance it here to prevent us from having to check in or discard these changes manually.
        projector.material = new Material(projectorMaterial);

        projector.material.SetTexture("_PrevTexture", prevTexture);
        projector.material.SetTexture("_CurrTexture", currTexture);
    }

    private void Update()
    {
        // Swap the textures
        Graphics.Blit(currTexture, prevTexture);
        Graphics.Blit(fogTexture, currTexture);
    }
}