using System;
using UnityEditor;
using UnityEngine;

namespace Anoho.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    public class ReadonlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var isReadonly = CheckIsReadonly(property);

            EditorGUI.BeginDisabledGroup(isReadonly);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }

        private bool CheckIsReadonly(SerializedProperty property)
        {
            if (attribute is not ReadonlyAttribute readonlyAttr)
            {
                return false;
            }

            var memberName = readonlyAttr.MemberName;
            if (memberName == null)
            {
                return true;
            }

            var obj = property.serializedObject.targetObject;
            var objType = obj.GetType();
            var bindingFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance;

            var propInfo = objType.GetField(memberName, bindingFlags);
            if (propInfo != null && propInfo.FieldType == typeof(bool))
            {
                return (bool)propInfo.GetValue(obj);
            }

            var methodInfo = objType.GetMethod(memberName, bindingFlags);
            if (methodInfo != null && methodInfo.ReturnType == typeof(bool))
            {
                return (bool)methodInfo.Invoke(obj, Array.Empty<object>());
            }

            return false;
        }
    }
}
