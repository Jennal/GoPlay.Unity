using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoPlay.UI.Common
{
    [RequireComponent(typeof(Animator))]
    public class UIAnimatedButton : Button
    {
        [Serializable]
        public class AnimationTriggers
        {
            [SerializeField] public string normalTrigger = "Normal";
            [SerializeField] public string downTrigger = "Down";
            [SerializeField] public string upTrigger = "Up";
            [SerializeField] public string disabledTrigger = "Disabled";
            
            public const string kDefaultNormalAnimName = "Normal";
            public const string kDefaultDisabledAnimName = "Disabled";
            public const string kDefaultDownAnimName = "Down";
            public const string kDefaultUpAnimName = "Up";

            public bool CheckTriggerValid(string triggerName) {
                return triggerName == normalTrigger ||
                       triggerName == downTrigger ||
                       triggerName == upTrigger ||
                       triggerName == disabledTrigger;
            }
        }

        [SerializeField]
        private AnimationTriggers m_AnimatedTriggers = new AnimationTriggers();

        [SerializeField] private float m_ClickDelay = 0.5f;
        private float m_lastClickTime;
        
        public AnimationTriggers animatedTriggers
        {
            get { return m_AnimatedTriggers; }
            set { m_AnimatedTriggers = value; }
        }
        
        protected override void Awake()
        {
            base.Awake();
            transition = Transition.Animation;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!IsActive() || !IsInteractable())
                return;

            if (transition == Transition.Animation)
            {
                TriggerAnimation(animatedTriggers.downTrigger);
            }
            
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            var delta = Time.realtimeSinceStartup - m_lastClickTime;
            if (delta < m_ClickDelay) return;

            m_lastClickTime = Time.realtimeSinceStartup;
            base.OnPointerClick(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!IsActive() || !IsInteractable())
                return;
            
            if (transition == Transition.Animation)
            {
                TriggerAnimation(animatedTriggers.upTrigger);
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            string triggername = "";
            switch (state)
            {
                case SelectionState.Normal:
                    triggername = animationTriggers.normalTrigger;
                    break;
                case SelectionState.Highlighted:
                    triggername = animationTriggers.highlightedTrigger;
                    break;
                case SelectionState.Pressed:
//                    triggername = animationTriggers.pressedTrigger;
                    break;
                case SelectionState.Disabled:
                    triggername = animationTriggers.disabledTrigger;
                    break;
                default:
                    triggername = string.Empty;
                    break;
            }
            if (!gameObject.activeInHierarchy)
                return;
            switch (transition)
            {
                case Transition.ColorTint:
                case Transition.SpriteSwap:
                    base.DoStateTransition(state, instant);
                    break;
                case Transition.Animation:
                    TriggerAnimation(triggername);
                    break;
            }
        }
        
        private void TriggerAnimation(string triggername)
        {
            if (transition != Transition.Animation || animator == null || 
                (!animator.isActiveAndEnabled || !animator.hasBoundPlayables) || 
                ! animatedTriggers.CheckTriggerValid(triggername))
                return;
            
            var at = animatedTriggers;
            
            animator.ResetTrigger(at.normalTrigger);
            animator.ResetTrigger(at.downTrigger);
            animator.ResetTrigger(at.upTrigger);
            animator.ResetTrigger(at.disabledTrigger);
            animator.SetTrigger(triggername);
        }
    }
}