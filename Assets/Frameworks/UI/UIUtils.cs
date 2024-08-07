﻿using UnityEngine;
using UnityEngine.UI;
using GoPlay.Cameras;
using GoPlay.Data;
using GoPlay.Utils;

namespace GoPlay.Framework.UI
{
    public static class UIUtils
    {
        public static void InitCanvas(GameObject gameObject)
        {
            var canvas = gameObject.GetComponent<Canvas>();
            var canvasScaler = gameObject.GetComponent<CanvasScaler>();
            var graphicRaycaster = gameObject.GetComponent<GraphicRaycaster>();

            if (canvas && canvasScaler && graphicRaycaster) return;
            
            if (!canvas)
            {
                gameObject.SetLayer(LayerMask.NameToLayer("UI"));
                canvas = gameObject.AddComponent<Canvas>();
                canvas.sortingLayerName = "UI";
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = UICamera.Instance;
            }

            if (!canvasScaler)
            {
                canvasScaler = gameObject.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 1f;
                canvasScaler.referenceResolution = Consts.Resolution.Design;
            }

            if (!graphicRaycaster)
            {
                graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }
}