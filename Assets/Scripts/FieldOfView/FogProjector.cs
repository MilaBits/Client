﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Projector))]
public class FogProjector : MonoBehaviour
{
    [SerializeField]
    private Material projectorMaterial;

    [SerializeField]
    private float blendSpeed;

    [SerializeField]
    private int textureScale;

    [SerializeField]
    private RenderTexture fogTexture;

    private RenderTexture prevTexture;
    private RenderTexture currTexture;
    private Projector projector;

    private IEnumerator blendFogCoroutine;

    private void Awake()
    {
        projector = GetComponent<Projector>();
        projector.enabled = true;
    }

    private void OnEnable()
    {
        prevTexture = GenerateTexture();
        currTexture = GenerateTexture();
        
        // Projector materials aren't instanced, resulting in the material asset getting changed.
        // Instance it here to prevent us from having to check in or discard these changes manually.
        projector.material = new Material(projectorMaterial);

        projector.material.SetTexture("_PrevTexture", prevTexture);
        projector.material.SetTexture("_CurrTexture", currTexture);
        
        StartNewBlend();
    }

    private RenderTexture GenerateTexture()
    {
        RenderTexture rt = new RenderTexture(
            fogTexture.width * textureScale,
            fogTexture.height * textureScale,
            0,
            fogTexture.format) {filterMode = FilterMode.Bilinear};
        rt.antiAliasing = fogTexture.antiAliasing;

        return rt;
    }

    public void StartNewBlend()
    {
        StartCoroutine(BlendFog());
    }

    private IEnumerator BlendFog()
    {
        // Swap the textures
        Graphics.Blit(currTexture, prevTexture);
        Graphics.Blit(fogTexture, currTexture);

        for (var blendAmount = 0f; blendAmount < 1; blendAmount += Time.deltaTime * blendSpeed)
        {
            // Set the blend property so the shader knows how much to lerp
            // by when checking the alpha value
            projector.material.SetFloat("_Blend", blendAmount);
            yield return null;
        }

        // once finished blending, swap the textures and start a new blend
        StartNewBlend();
    }
}