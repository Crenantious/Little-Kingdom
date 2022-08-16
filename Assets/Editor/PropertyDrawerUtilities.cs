using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropertyDrawerUtilities
{
    public static class PropertyDrawerExtentions
    {
        static readonly Color defaultSeperatorColour = new(26f / 255, 26f / 255, 26f / 255, 1);
        static readonly Dictionary<PropertyDrawer, PropertyInfo> propertyInfos = new();
        static readonly List<PropertyDrawer> forceRepaint = new();

        class PropertyInfo
        {
            public PropertyInfo(Rect propertyRect, float verticalSpacing = 0, float horizontalSpacing = 0)
            {
                this.propertyRect = propertyRect;
                this.verticalSpacing = verticalSpacing;
                this.horizontalSpacing = horizontalSpacing;
            }
            public Rect propertyRect;
            public float height = 0;
            public float verticalSpacing;
            public float horizontalSpacing;
            public int horizontalRectsAmount = 0;
            public int horizontalRectsRemaining = 0;
            public float horizontalFieldsWidth = 0;
            public float heightFromSeperators = 0;
        }

        /// <summary>
        /// Setup the property to utilise the layout extension features.
        /// <see cref="EditorGUI.BeginChangeCheck"/> is called in this method.
        /// </summary>
        /// <param name="verticalSpacing">The default amount of vertical space between each field.</param>>
        /// <param name="verticalSpacing">The default amount of horizontal space between each field.</param>>
        static public void BeginProperty(this PropertyDrawer propertyDrawer, Rect propertyRect,
                                         SerializedProperty property, GUIContent label,
                                         float verticalSpacing = 1, float horizontalSpacing = 1)
        {
            propertyInfos[propertyDrawer] = new(propertyRect, verticalSpacing, horizontalSpacing);
            EditorGUI.BeginProperty(propertyRect, label, property);
            EditorGUI.BeginChangeCheck();
        }

        /// <summary>
        /// End the property started with BeginProperty correctly, running required checks and calculations.
        /// <see cref="EditorGUI.EndChangeCheck"/> is called in this method.
        /// </summary>
        static public void EndProperty(this PropertyDrawer propertyDrawer, SerializedProperty property)
        {
            if (EditorGUI.EndChangeCheck())
                RecalculatePropertyHeight(propertyDrawer, property);

            if (forceRepaint.Contains(propertyDrawer))
            {
                RepaintInspector(property.serializedObject);
                forceRepaint.Remove(propertyDrawer);
            }
            EditorGUI.EndProperty();
        }

        static public float GetActivePropertyHeight(this PropertyDrawer propertyDrawer, SerializedProperty property)
        {
            if (propertyInfos.TryGetValue(propertyDrawer, out PropertyInfo propertyInfo))
                return propertyInfo.height;

            // If BeginProperty has not been called for the property (i.e. the PropertyDrawer has just been created)
            // it needs to be redrawn after initial height calculations.
            forceRepaint.Add(propertyDrawer);
            return 0;
        }

        /// <summary>
        /// Draw a line spanning the width of the property, or inspector.
        /// </summary>
        /// <param name="topPadding">Height above the seperator. Uses the property's verticalSpacing if null.</param>
        /// <param name="bottomPadding">Height below the seperator. Uses the property's verticalSpacing if null.</param>
        /// <param name="thickness">Height of the seperator</param>
        /// <param name="colour">Colour of the seperator</param>
        /// <param name="expandWidthToInspector">
        /// Expands the width to match that of the inspector the property is being drawn in;
        /// this draws outside of the property rect.
        /// </param>
        static public void Seperator(this PropertyDrawer propertyDrawer, float? topPadding = null,
                                     float? bottomPadding = null, float thickness = 1, Color? colour = null,
                                     bool expandWidthToInspector = true)
        {
            var info = propertyInfos[propertyDrawer];
            if (topPadding == null)
                topPadding = info.verticalSpacing;
            if (bottomPadding == null)
                bottomPadding = info.verticalSpacing;
            if (colour == null)
                colour = defaultSeperatorColour;

            float x = expandWidthToInspector ? 0 : info.propertyRect.x;
            float width = expandWidthToInspector ? EditorGUIUtility.currentViewWidth : info.propertyRect.width;

            float top = (float)topPadding;
            float bottom = (float)bottomPadding;

            EditorGUI.DrawRect(new Rect(x, info.propertyRect.y + info.height + top, width, thickness), (Color)colour);

            info.heightFromSeperators += top + bottom + thickness;
            info.height += info.heightFromSeperators;
        }

        /// <summary>
        /// Adds a blank space to push down the next verical rect from GetNextRect().
        /// </summary>
        static public void VerticalSpace(this PropertyDrawer propertyDrawer, float height) =>
            propertyInfos[propertyDrawer].height += height;

        /// <summary>
        /// The next amount of GetNextRect() calls will be laid out horizontally.
        /// </summary>
        static public void MakeNextRectsHorizontal(this PropertyDrawer propertyDrawer, int amount) =>
            propertyInfos[propertyDrawer].horizontalRectsAmount =
            propertyInfos[propertyDrawer].horizontalRectsRemaining =
            amount;

        /// <summary>
        /// Gets the next Rect based on the  current layout settings. Defaults to a new line.
        /// </summary>
        /// <param name="width">Overrides the width. If 0, it will be calculated from the current layout settings.</param>
        /// <param name="height">Overrides the height. If 0, it will be calculated from the current layout settings.</param>
        static public Rect GetNextRect(this PropertyDrawer propertyDrawer, SerializedProperty property = null,
                                       float width = 0, float height = 0)
        {
            if (!propertyInfos.ContainsKey(propertyDrawer))
                throw new MissingReferenceException($"No active property. Make sure to set one via " +
                                                    $"{nameof(PropertyDrawerExtentions.BeginProperty)}().");

            var info = propertyInfos[propertyDrawer];
            float x = info.propertyRect.x;
            int horizontalRectNumber = info.horizontalRectsAmount - info.horizontalRectsRemaining;

            if (height == 0)
                height = property == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight(property);

            if (width == 0)
            {
                width = info.propertyRect.width;

                if (info.horizontalRectsRemaining > 0)
                {
                    width -= info.horizontalFieldsWidth - info.horizontalSpacing * (info.horizontalRectsRemaining - 1);
                    width /= info.horizontalRectsRemaining;
                }
            }

            if (info.horizontalRectsRemaining > 0)
            {
                x += info.horizontalFieldsWidth + info.horizontalSpacing;
                info.horizontalFieldsWidth += width + info.horizontalSpacing;
                info.horizontalRectsRemaining--;
            }

            Rect rect = new(x, info.propertyRect.y + info.height, width, height);

            if (info.horizontalRectsRemaining == 0)
                info.height += height + info.verticalSpacing;

            return rect;
        }

        static float RecalculatePropertyHeight(PropertyDrawer propertyDrawer, SerializedProperty property)
        {
            float height = GetHeightOfAllVisibleChildren(property.Copy());
            height += propertyInfos[propertyDrawer].heightFromSeperators;
            return height;
        }

        static float GetHeightOfAllVisibleChildren(SerializedProperty property)
        {
            float height = 0;
            while (property.NextVisible(true))
                height += EditorGUI.GetPropertyHeight(property);
            return height;
        }

        static void RepaintInspector(SerializedObject BaseObject)
        {
            foreach (var item in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                if (item.serializedObject == BaseObject)
                {
                    item.Repaint();
                    return;
                }
            }
        }
    }
}