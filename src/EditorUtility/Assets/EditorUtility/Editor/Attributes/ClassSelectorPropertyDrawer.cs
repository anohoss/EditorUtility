using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Anoho.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ClassSelectorAttribute))]
    public class ClassSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.LabelField(position, label.text, "ClassSelector can only be used with [SerializeReference]");
                return;
            }

            // Find base type from managedReferenceFieldTypename
            var baseTypeName = ExtractTypeName(property.managedReferenceFieldTypename);
            if (baseTypeName == null)
            {
                EditorGUI.LabelField(position, label.text, "Unable to determine field type");
                return;
            }
            var baseType = FindTypeByName(baseTypeName);

            // インスタンスを保持している型を特定し、選択中のインデックスを決定する

            var currentIndex = 0;
            var currentType = property.managedReferenceValue?.GetType();
            var availableTypes = GetAvailableTypes(baseType);

            if (currentType != null)
            {
                var currentTypeFullName = currentType.FullName;
                if (!string.IsNullOrEmpty(currentTypeFullName))
                {
                    for (var i = 0; i < availableTypes.Length; i++)
                    {
                        var typeFullName = availableTypes[i].FullName;
                        if (typeFullName == currentTypeFullName)
                        {
                            currentIndex = i + 1;   // +1 for "None" option
                            break;
                        }
                    }
                }
            }

            int selectedIndex;

            var displayLabel = GetDisplayLabel(property, label);
            var displayTypeNames = GetDisplayTypeNames(availableTypes);
            EditorGUI.BeginChangeCheck();
            {
                position.height = EditorGUIUtility.singleLineHeight;
                selectedIndex = EditorGUI.Popup(position, displayLabel, currentIndex, displayTypeNames);
            }

            if (EditorGUI.EndChangeCheck())
            {
                object newValue = null;
                if (selectedIndex > 0)
                {
                    var selectedType = availableTypes[selectedIndex - 1];
                    try
                    {
                        newValue = Activator.CreateInstance(selectedType);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to create instance of {selectedType.Name}: {e.Message}");
                    }
                }
                property.managedReferenceValue = newValue;
            }

            EditorGUI.PropertyField(position, property, GUIContent.none, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private string ExtractTypeName(string managedReferenceFullTypename)
        {
            if (string.IsNullOrEmpty(managedReferenceFullTypename))
            {
                return null;
            }

            // Assembly-Csharp Namespace.TypeNameのような形式で型名が格納される
            var parts = managedReferenceFullTypename.Split(' ');

            if (parts.Length == 2)
            {
                // Namespace.TypeNameの部分
                return parts[1];
            }
            else
            {
                Debug.LogWarning($"Unexpected managedReferenceFullTypename format: {managedReferenceFullTypename}");
            }

            return null;
        }

        private Type FindTypeByName(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private Type[] GetAvailableTypes(Type baseType)
        {
            return TypeCache.GetTypesDerivedFrom(baseType)
                .Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition)
                .OrderBy(type => GetHierarchicalTypeName(type))
                .ToArray();
        }

        private string GetDisplayLabel(SerializedProperty property, GUIContent label)
        {
            if (property.managedReferenceValue != null)
            {
                var typeName = property.managedReferenceValue.GetType().Name;
                return $"{label.text} ({typeName})";
            }
            return label.text;
        }

        private string[] GetDisplayTypeNames(Type[] types)
        {
            var displayNames = new List<string> { "None" };

            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var hierarchicalName = GetHierarchicalTypeName(type);
                displayNames.Add(hierarchicalName);
            }

            return displayNames.ToArray();
        }

        private string GetHierarchicalTypeName(Type type)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(type.Namespace))
            {
                parts.Add(type.Namespace);
            }

            if (type.DeclaringType != null)
            {
                var declaringTypeNames = new List<string>();

                var currentType = type.DeclaringType;
                while (currentType != null)
                {
                    declaringTypeNames.Insert(0, currentType.Name);
                    currentType = currentType.DeclaringType;
                }
                
                parts.AddRange(declaringTypeNames);
            }

            parts.Add(type.Name);

            return string.Join("/", parts);
        }
    }
}
