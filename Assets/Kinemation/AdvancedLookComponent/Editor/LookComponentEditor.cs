// Designed by Kinemation, 2023

using Kinemation.AdvancedLookComponent.Runtime;
using UnityEditor;
using UnityEngine;

namespace Kinemation.AdvancedLookComponent.Editor
{
    [CustomEditor(typeof(LookComponent), true)]
    public class LookLayerEditor : UnityEditor.Editor
    {
        private string[] tabs = {"Blending", "Aim Offset", "Leaning"};
        private int selectedTab;
        
        private SerializedProperty layerAlpha;
        private SerializedProperty lerpSpeed;
        private SerializedProperty handsAlpha;
        private SerializedProperty handsLerp;
        private SerializedProperty pelvisAlpha;
        private SerializedProperty pelvisLerp;
        
        private SerializedProperty pelvisOffset;
        private SerializedProperty lookUpOffset;
        private SerializedProperty lookRightOffset;
        private SerializedProperty enableAutoDistribution;
        private SerializedProperty enableManualSpineControl;
        private SerializedProperty aimUp;
        private SerializedProperty aimRight;
        private SerializedProperty smoothAim;
        
        private SerializedProperty leanDirection;
        private SerializedProperty leanAmount;
        private SerializedProperty leanSpeed;
        
        private SerializedProperty detectZeroFrames;
        private SerializedProperty checkZeroFootIK;
        private SerializedProperty useRightOffset;
        private SerializedProperty useIK;
        
        private SerializedProperty pelvis;
        private SerializedProperty rightHand;
        private SerializedProperty leftHand;
        private SerializedProperty rightFoot;
        private SerializedProperty leftFoot;
        
        private LookComponent owner;

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }
            
            owner = (LookComponent) target;
            
            layerAlpha = serializedObject.FindProperty("layerAlpha");
            lerpSpeed = serializedObject.FindProperty("layerInterpSpeed");
            handsAlpha = serializedObject.FindProperty("handsLayerAlpha");
            handsLerp = serializedObject.FindProperty("handsLerpSpeed");
            pelvisAlpha = serializedObject.FindProperty("pelvisAlpha");
            pelvisLerp = serializedObject.FindProperty("pelvisLerpSpeed");

            pelvisOffset = serializedObject.FindProperty("pelvisOffset");
            lookUpOffset = serializedObject.FindProperty("lookUpOffset");
            lookRightOffset = serializedObject.FindProperty("lookRightOffset");
            enableAutoDistribution = serializedObject.FindProperty("enableAutoDistribution");
            enableManualSpineControl = serializedObject.FindProperty("enableManualSpineControl");
            aimUp = serializedObject.FindProperty("aimUp");
            aimRight = serializedObject.FindProperty("aimRight");
            smoothAim = serializedObject.FindProperty("smoothAim");

            leanDirection = serializedObject.FindProperty("leanDirection");
            leanAmount = serializedObject.FindProperty("leanAmount");
            leanSpeed = serializedObject.FindProperty("leanSpeed");

            detectZeroFrames = serializedObject.FindProperty("detectZeroFrames");
            checkZeroFootIK = serializedObject.FindProperty("checkZeroFootIK");
            useRightOffset = serializedObject.FindProperty("useRightOffset");
            useIK = serializedObject.FindProperty("useIK");
            
            pelvis = serializedObject.FindProperty("pelvis");
            rightHand = serializedObject.FindProperty("rightHand");
            leftHand = serializedObject.FindProperty("leftHand");
            rightFoot = serializedObject.FindProperty("rightFoot");
            leftFoot = serializedObject.FindProperty("leftFoot");
        }

        private void DrawBlendingTab()
        {
            EditorGUILayout.PropertyField(layerAlpha);
            EditorGUILayout.PropertyField(lerpSpeed);
            EditorGUILayout.PropertyField(handsAlpha);
            EditorGUILayout.PropertyField(handsLerp);
            EditorGUILayout.PropertyField(pelvisAlpha);
            EditorGUILayout.PropertyField(pelvisLerp);
        }
        
        private void DrawOffsetTab()
        {
            EditorGUILayout.PropertyField(pelvisOffset);
            EditorGUILayout.PropertyField(lookUpOffset);
            EditorGUILayout.PropertyField(lookRightOffset);
            EditorGUILayout.PropertyField(enableAutoDistribution);
            EditorGUILayout.PropertyField(enableManualSpineControl);
            EditorGUILayout.PropertyField(aimUp);
            EditorGUILayout.PropertyField(aimRight);
            EditorGUILayout.PropertyField(smoothAim);
        }
        
        private void DrawLeanTab()
        {
            EditorGUILayout.PropertyField(leanDirection);
            EditorGUILayout.PropertyField(leanAmount);
            EditorGUILayout.PropertyField(leanSpeed);
        }

        private void DrawDefault()
        {
            EditorGUILayout.PropertyField(pelvis);
            EditorGUILayout.PropertyField(rightHand);
            EditorGUILayout.PropertyField(leftHand);
            EditorGUILayout.PropertyField(rightFoot);
            EditorGUILayout.PropertyField(leftFoot);
            EditorGUILayout.PropertyField(detectZeroFrames);
            //EditorGUILayout.PropertyField(checkZeroFootIK);
            EditorGUILayout.PropertyField(useRightOffset);
            EditorGUILayout.PropertyField(useIK);
        }
        
        private void RenderAnimButtons()
        {
            if (GUILayout.Button(new GUIContent("Setup rig", "Will find and assign bones")))
            {
                owner.SetupBones();
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Preview animation"))
            {
                owner.EnableEditorPreview();
            }

            if (GUILayout.Button("Reset pose"))
            {
                owner.DisableEditorPreview();
            }

            GUILayout.EndHorizontal();
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefault();
            GUILayout.BeginVertical();
            selectedTab = GUILayout.Toolbar(selectedTab, tabs);
            GUILayout.EndVertical();
            switch (selectedTab)
            {
                case 0:
                    DrawBlendingTab();
                    break;
                case 1:
                    DrawOffsetTab();
                    break;
                case 2:
                    DrawLeanTab();
                    break;
            }
            
            serializedObject.ApplyModifiedProperties();
            RenderAnimButtons();
        }
    }
}