using System;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using GoPlay.Utils;
using UnityEngine.UI;

namespace GoPlay.UI.Common
{
    // [ExecuteInEditMode]
    public class UIKeepInScreen : MonoBehaviour
    {
#if ODIN_INSPECTOR_3
        [BoxGroup("Property")]
#endif 
        public bool Horizontal;
#if ODIN_INSPECTOR_3
        [BoxGroup("Property")]
#endif 
        public bool Vertical;
#if ODIN_INSPECTOR_3
        [BoxGroup("Property"), ShowIf("Vertical")]
#endif 
        public bool RotateVertical;
#if ODIN_INSPECTOR_3
        [BoxGroup("Offset"), ShowIf("Horizontal")]
#endif 
        public float Left;
#if ODIN_INSPECTOR_3
        [BoxGroup("Offset"), ShowIf("Horizontal")]
#endif 
        public float Right;
#if ODIN_INSPECTOR_3
        [BoxGroup("Offset"), ShowIf("Vertical")]
#endif 
        public float Up;
#if ODIN_INSPECTOR_3
        [BoxGroup("Offset"), ShowIf("Vertical")]
#endif 
        public float Down;

#if ODIN_INSPECTOR_3
        [BoxGroup("Status"), ReadOnly]
#endif
        public bool IsRotatedVertical;
        
#if ODIN_INSPECTOR_3
        [BoxGroup("Status"), ReadOnly]
#endif
        public Vector2 OriginPos;
        
#if ODIN_INSPECTOR_3
        [BoxGroup("Status"), ReadOnly]
#endif
        public Vector2 ScreenSize;
        
        
        private Canvas _canvas;
        private Canvas Canvas
        {
            get
            {
                if (!_canvas) _canvas = GetComponentInParent<Canvas>();
                return _canvas;
            }
        }

        private void Awake()
        {
            ScreenSize = new Vector2(Screen.width, Screen.height);
        }

        private void OnEnable()
        {
            var rectTransform = transform as RectTransform;
            OriginPos = rectTransform.anchoredPosition;
            LateUpdate();
        }

        private void OnDisable()
        {
            var rectTransform = transform as RectTransform;
            rectTransform.anchoredPosition = OriginPos;
        }

        private void LateUpdate()
        {
            var rectTransform = transform as RectTransform;
            // rectTransform.anchoredPosition = OriginPos; //Reset position
            
            // var camera = Canvas.GetCamera();
            var camera = Camera.main;
            var screenPos = camera.WorldToScreenPoint(transform.position);

            if (Horizontal) CheckHorizontal(rectTransform, ref screenPos);
            if (Vertical) CheckVertical(rectTransform, ref screenPos);
        }

        private void CheckHorizontal(RectTransform rectTransform, ref Vector3 screenPos)
        {
            var left = screenPos.x - rectTransform.rect.width * rectTransform.pivot.x;
            var right = screenPos.x + rectTransform.rect.width * (1f - rectTransform.pivot.x);

            if (left < Left)
            {
                screenPos.x += (Left - left);
                screenPos.x = Mathf.CeilToInt(screenPos.x);
            }

            if (right > (ScreenSize.x - Right))
            {
                screenPos.x -= (right - (ScreenSize.x - Right));
                screenPos.x = Mathf.FloorToInt(screenPos.x);
            }

            var camera = Camera.main;
            transform.position = camera.ScreenToWorldPoint(screenPos);
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.transform as RectTransform, screenPos,
            //     Camera.main, out var anchoredPosition);
            // rectTransform.anchoredPosition = anchoredPosition;
        }

        private void CheckVertical(RectTransform rectTransform, ref Vector3 screenPos)
        {
            if (RotateVertical)
            {
                if (IsRotatedVertical)
                {
                    var up = screenPos.y + rectTransform.rect.height * (1f - rectTransform.pivot.y);
                    if (up <= (ScreenSize.y - Up))
                    {
                        transform.Rotate(Vector3.forward, 180f);
                        IsRotatedVertical = false;
                    }
                }
                else
                {
                    var up = screenPos.y + rectTransform.rect.height * (1f - rectTransform.pivot.y);
                    if (up > (ScreenSize.y - Up))
                    {
                        transform.Rotate(Vector3.forward, 180f);
                        IsRotatedVertical = true;
                    }
                }
            }
            else
            {
                var up = screenPos.y + rectTransform.rect.height * (1f - rectTransform.pivot.y);
                var down = screenPos.y - rectTransform.rect.height * rectTransform.pivot.y;

                if (up > (ScreenSize.y - Up))
                {
                    screenPos.y -= (up - (ScreenSize.y - Up));
                    screenPos.y = Mathf.CeilToInt(screenPos.y);
                }

                if (down < Down)
                {
                    screenPos.y += (Down - down);
                    screenPos.y = Mathf.FloorToInt(screenPos.y);
                }

                transform.position = Canvas.GetCamera().ScreenToWorldPoint(screenPos);
                // RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, screenPos,
                //     Canvas.GetCamera(), out var anchoredPosition);
                // rectTransform.anchoredPosition = anchoredPosition;
            }
        }
    }
}