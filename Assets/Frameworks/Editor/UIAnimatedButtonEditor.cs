using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using GoPlay.UI.Common;

namespace GoPlay.Editor
{
    [CustomEditor(typeof(UIAnimatedButton), true)]
    //使用了 SerializedObject 和 SerializedProperty 系统，因此，可以自动处理“多对象编辑”，“撤销undo” 和 “预制覆盖prefab override”。
    [CanEditMultipleObjects]
    public class UIAnimatedButtonEditor : ButtonEditor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_InteractableProperty;
        private SerializedProperty m_NormalTriggerProperty;
        private SerializedProperty m_DownTriggerProperty;
        private SerializedProperty m_UpTriggerProperty;
        private SerializedProperty m_DisabledTriggerProperty;
        private SerializedProperty m_ClickDelay;
        private SerializedProperty m_OnClickProperty;

        private string[] m_PropertyPathesToExcludeForChildClasses;
        private const float kArrowThickness = 2.5f;
        private const float kArrowHeadSize = 1.2f;

        protected override void OnEnable()
        {
            base.OnEnable();

            this.m_Script = this.serializedObject.FindProperty("m_Script");
            this.m_InteractableProperty = this.serializedObject.FindProperty("m_Interactable");
            this.m_NormalTriggerProperty = this.serializedObject.FindProperty("m_AnimatedTriggers.normalTrigger");
            this.m_DownTriggerProperty = this.serializedObject.FindProperty("m_AnimatedTriggers.downTrigger");
            this.m_UpTriggerProperty = this.serializedObject.FindProperty("m_AnimatedTriggers.upTrigger");
            this.m_DisabledTriggerProperty = this.serializedObject.FindProperty("m_AnimatedTriggers.disabledTrigger");
            this.m_ClickDelay = this.serializedObject.FindProperty("m_ClickDelay");
            this.m_OnClickProperty = this.serializedObject.FindProperty("m_OnClick");
            this.m_PropertyPathesToExcludeForChildClasses = new[]
            {
                this.m_Script.propertyPath,
                this.m_NormalTriggerProperty.propertyPath,
                this.m_DownTriggerProperty.propertyPath,
                this.m_UpTriggerProperty.propertyPath,
                this.m_DisabledTriggerProperty.propertyPath,
                this.m_InteractableProperty.propertyPath,
                this.m_ClickDelay.propertyPath,
                this.m_OnClickProperty.propertyPath,
            };
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_InteractableProperty);
            EditorGUILayout.PropertyField(this.m_ClickDelay);
            Animator animator = (this.target as Selectable).GetComponent<Animator>();

            ++EditorGUI.indentLevel;
            EditorGUILayout.PropertyField(m_NormalTriggerProperty);
            EditorGUILayout.PropertyField(m_DownTriggerProperty);
            EditorGUILayout.PropertyField(m_UpTriggerProperty);
            EditorGUILayout.PropertyField(m_DisabledTriggerProperty);
            --EditorGUI.indentLevel;

            if ((UnityEngine.Object) animator == (UnityEngine.Object) null ||
                (UnityEngine.Object) animator.runtimeAnimatorController == (UnityEngine.Object) null)
            {
                Rect controlRect = EditorGUILayout.GetControlRect();
                controlRect.xMin += EditorGUIUtility.labelWidth;
                if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
                {
                    AnimatorController animatorContoller =
                        GenerateSelectableAnimatorContoller(
                            (this.target as UIAnimatedButton).animatedTriggers, this.target as Selectable);
                    if ((UnityEngine.Object) animatorContoller != (UnityEngine.Object) null)
                    {
                        if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
                            animator = (this.target as Selectable).gameObject.AddComponent<Animator>();
                        AnimatorController.SetAnimatorController(animator, animatorContoller);
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_OnClickProperty);

            this.ChildClassPropertiesGUI();
            this.serializedObject.ApplyModifiedProperties();
        }

        private void ChildClassPropertiesGUI()
        {
            if (this.IsDerivedSelectableEditor())
                return;
            DrawPropertiesExcluding(this.serializedObject, this.m_PropertyPathesToExcludeForChildClasses);
        }

        private bool IsDerivedSelectableEditor()
        {
            return this.GetType() != typeof(SelectableEditor);
        }

        private static AnimatorController GenerateSelectableAnimatorContoller(
            UIAnimatedButton.AnimationTriggers animationTriggers,
            Selectable target)
        {
            if ((UnityEngine.Object) target == (UnityEngine.Object) null)
                return (AnimatorController) null;
            string saveControllerPath = GetSaveControllerPath(target);
            if (string.IsNullOrEmpty(saveControllerPath))
                return (AnimatorController) null;
            string name1 = !string.IsNullOrEmpty(animationTriggers.normalTrigger)
                ? animationTriggers.normalTrigger
                : UIAnimatedButton.AnimationTriggers.kDefaultNormalAnimName;
            string name2 = !string.IsNullOrEmpty(animationTriggers.downTrigger)
                ? animationTriggers.downTrigger
                : UIAnimatedButton.AnimationTriggers.kDefaultDownAnimName;
            string name3 = !string.IsNullOrEmpty(animationTriggers.upTrigger)
                ? animationTriggers.upTrigger
                : UIAnimatedButton.AnimationTriggers.kDefaultUpAnimName;
            string name4 = !string.IsNullOrEmpty(animationTriggers.disabledTrigger)
                ? animationTriggers.disabledTrigger
                : UIAnimatedButton.AnimationTriggers.kDefaultDisabledAnimName;
            AnimatorController controllerAtPath = AnimatorController.CreateAnimatorControllerAtPath(saveControllerPath);
            GenerateTriggerableTransition(name1, controllerAtPath);
            GenerateTriggerableTransition(name2, controllerAtPath);
            GenerateTriggerableTransition(name3, controllerAtPath);
            GenerateTriggerableTransition(name4, controllerAtPath);
            
            // 消除警告
            controllerAtPath.AddParameter("Highlighted", AnimatorControllerParameterType.Trigger);
            controllerAtPath.AddParameter("Pressed", AnimatorControllerParameterType.Trigger);
            
            AssetDatabase.ImportAsset(saveControllerPath);
            return controllerAtPath;
        }

        private static string GetSaveControllerPath(Selectable target)
        {
            string name = target.gameObject.name;
            string message = string.Format("Create a new animator for the game object '{0}':", (object) name);
            return EditorUtility.SaveFilePanelInProject("New Animation Contoller", name, "controller", message);
        }

        private static AnimationClip GenerateTriggerableTransition(string name, AnimatorController controller)
        {
            AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(name);
            AssetDatabase.AddObjectToAsset((UnityEngine.Object) animationClip, (UnityEngine.Object) controller);
            AnimatorState destinationState = controller.AddMotion((Motion) animationClip);
            controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
            var transition = controller.layers[0].stateMachine.AddAnyStateTransition(destinationState);
            transition.AddCondition(AnimatorConditionMode.If, 0.0f, name);
            transition.duration = 0f;

            switch (name)
            {
                case "Up":
                case "Down":
                    var settings = AnimationUtility.GetAnimationClipSettings(animationClip);
                    settings.loopTime = false;
                    AnimationUtility.SetAnimationClipSettings(animationClip, settings);
                    break;
            }

            if (name == "Up")
            {
                var normalState = controller.layers[0].stateMachine.defaultState;
                transition = destinationState.AddTransition(normalState);
                transition.hasExitTime = true;
                transition.exitTime = 1f;
                transition.duration = 0f;
            }

            return animationClip;
        }
    }
}