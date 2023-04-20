using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Varneon.VInspector
{
    public static class RangedValueFieldBuilder
    {
        public static VisualElement Build(SerializedProperty property, RangeAttribute rangeAttribute, string customName = null, string tooltip = null)
        {
            VisualElement slider;

            VisualElement valueField;

            string valueType = property.type;

            if (valueType == "float")
            {
                slider = new Slider(string.Empty, rangeAttribute.min, rangeAttribute.max);

                if (!string.IsNullOrEmpty(tooltip))
                {
                    slider.tooltip = tooltip;
                }

                valueField = new FloatField(customName ?? property.displayName);

                ((FloatField)valueField).labelElement.AddToClassList("unity-property-field__label");

                ((FloatField)valueField).RegisterValueChangedCallback(a =>
                {
                    if(a.newValue > rangeAttribute.max) { ((FloatField)valueField).value = rangeAttribute.max; }
                    else if (a.newValue < rangeAttribute.min) { ((FloatField)valueField).value = rangeAttribute.min; }

                    ((BaseSlider<float>)slider).SetValueWithoutNotify(Mathf.Clamp(a.newValue, rangeAttribute.min, rangeAttribute.max));
                });

                ((BaseSlider<float>)slider).RegisterValueChangedCallback(a => ((FloatField)valueField).SetValueWithoutNotify(a.newValue));
            }
            else if (valueType == "int")
            {
                slider = new SliderInt(string.Empty, (int)rangeAttribute.min, (int)rangeAttribute.max);

                valueField = new IntegerField(customName ?? property.displayName);

                ((IntegerField)valueField).labelElement.AddToClassList("unity-property-field__label");

                ((IntegerField)valueField).RegisterValueChangedCallback(a => {
                    if (a.newValue > rangeAttribute.max) { ((IntegerField)valueField).value = Convert.ToInt32(rangeAttribute.max); }
                    else if (a.newValue < rangeAttribute.min) { ((IntegerField)valueField).value = Convert.ToInt32(rangeAttribute.min); }

                    ((BaseSlider<int>)slider).SetValueWithoutNotify(Convert.ToInt32(Mathf.Clamp(a.newValue, rangeAttribute.min, rangeAttribute.max)));
                });

                ((BaseSlider<int>)slider).RegisterValueChangedCallback(a => ((IntegerField)valueField).SetValueWithoutNotify(a.newValue));
            }
            else
            {
                Debug.LogWarning($"Attempting to build a custom ranged value field for type '{valueType}', which hasn't been implemented yet!");

                return !string.IsNullOrEmpty(customName) ? new PropertyField(property, customName) : new PropertyField(property);
            }

            valueField.name = "unity-input-" + property.propertyPath;

            ((BindableElement)slider).bindingPath = property.propertyPath;

            ((BindableElement)valueField).bindingPath = property.propertyPath;

            slider.style.flexGrow = 1;

            VisualElement valueInput = valueField.Q("unity-text-input");

            valueInput.style.flexGrow = 0;
            valueInput.style.flexShrink = 1;

            valueInput.style.width = new StyleLength(50f);

            valueInput.style.marginBottom = 0;
            valueInput.style.marginRight = 0;
            valueInput.style.marginTop = 0;

            valueField.Insert(1, slider);

            Label label = valueField.Q<Label>();

            label.RegisterCallback<MouseUpEvent>((evt) =>
            {
                if (evt.button != 1)
                {
                    return;
                }

                property.serializedObject.Update();

                GenericMenu genericMenu = (GenericMenu)typeof(EditorGUI).GetMethod("FillPropertyContextMenu", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { property, null, null });
                Vector2 vector = new Vector2(label.layout.xMin, label.layout.height);
                vector = label.LocalToWorld(vector);
                Rect position = new Rect(vector, Vector2.zero);
                genericMenu.DropDown(position);
                evt.PreventDefault();
                evt.StopPropagation();
            });

            return valueField;
        }
    }
}
