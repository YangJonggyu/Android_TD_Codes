using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraResolution : MonoBehaviour
{
    public Color letterBoxColor;
    void Start()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleheight = ((float) Screen.width / Screen.height) / ((float) 9 / 16); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }

        camera.rect = rect;
    }

    public void PreCull(ScriptableRenderContext context, Camera camera)
    {
        Color gamma = new Color(Mathf.Pow(letterBoxColor.r,2.2f),Mathf.Pow(letterBoxColor.g,2.2f),Mathf.Pow(letterBoxColor.b,2.2f),1f);
        GL.Clear(true, true,gamma);
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += PreCull;
    }
    
    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= PreCull;
    }
}