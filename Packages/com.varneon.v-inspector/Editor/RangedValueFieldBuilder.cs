using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Varneon.VInspector
{
    public static class RangedValueFieldBuilder
    {
        public static VisualElement Build(UnityEngine.Object target, SerializedProperty property, RangeAttribute rangeAttribute, string customName = null, string tooltip = null)
        {
            VisualElement slider;

            VisualElement valueField;

            string valueType = property.type;

            if (valueType == "float")
            {
                slider = new Slider(customName ?? property.displayName, rangeAttribute.min, rangeAttribute.max);

                if (!string.IsNullOrEmpty(tooltip))
                {
                    slider.tooltip = tooltip;
                }

                valueField = new FloatField(string.Empty);
                ((INotifyValueChanged<float>)valueField).RegisterValueChangedCallback(a => ((INotifyValueChanged<float>)slider).SetValueWithoutNotify(Mathf.Clamp(a.newValue, rangeAttribute.min, rangeAttribute.max)));
            }
            else if (valueType == "int")
            {
                slider = new SliderInt(customName ?? property.displayName, (int)rangeAttribute.min, (int)rangeAttribute.max);

                valueField = new IntegerField(string.Empty);
                ((INotifyValueChanged<int>)valueField).RegisterValueChangedCallback(a => ((INotifyValueChanged<int>)slider).SetValueWithoutNotify(Convert.ToInt32(Mathf.Clamp(a.newValue, rangeAttribute.min, rangeAttribute.max))));
            }
            else
            {
                Debug.LogWarning($"Attempting to build a custom ranged value field for type '{valueType}', which hasn't been implemented yet!");

                return !string.IsNullOrEmpty(customName) ? new PropertyField(property, customName) : new PropertyField(property);
            }

            slider.style.flexGrow = 1;

            ((BindableElement)slider).BindProperty(property);

            valueField.style.width = new StyleLength(50f);

            valueField.style.marginBottom = 0;
            valueField.style.marginRight = 0;
            valueField.style.marginTop = 0;

            ((BindableElement)valueField).BindProperty(property);

            slider.Add(valueField);

            return slider;
        }
    }
}
