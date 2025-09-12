using System.Reflection;
using UnityEngine;
using UnityEditor;
using System;

namespace Anoho.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var buttonAttribute = (ButtonAttribute)attribute;
            var buttonContent = new GUIContent(property.displayName, property.tooltip);
            
            if (GUI.Button(position, buttonContent))
            {
                var obj = property.serializedObject.targetObject;
                var objType = obj.GetType();

                var methodName = buttonAttribute.MethodName;
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                
                var methodInfo = objType.GetMethod(methodName, bindingFlags);
                if (methodInfo == null)
                {
                    Debug.LogError($"ButtonAttribute: The specified method '{buttonAttribute.MethodName}' not found.");
                    return;
                }
                
                // パラメータを持っている場合は呼び出し不可
                if (methodInfo.GetParameters().Length > 0)
                {
                    Debug.LogError($"ButtonAttribute: The specified method '{buttonAttribute.MethodName}' has parameters. please specify a method that has no parameter.");
                    return;
                }
                
                methodInfo.Invoke(obj, Array.Empty<object>());
                
                EditorUtility.SetDirty(obj);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 標準的なボタンの高さを返す
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
