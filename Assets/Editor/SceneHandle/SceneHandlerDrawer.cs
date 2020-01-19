using System;
using System.Reflection;
using AIObjects;
using Editor_MainGameCreationWizard;
using GeometryIntersection;
using RangeObjects;
using UnityEditor;
using UnityEngine;

public static class SceneHandlerDrawer
{
    public static void Draw(object drawableObject, Transform objectTransform, CommonGameConfigurations CommonGameConfigurations)
    {
        if (drawableObject != null && drawableObject.GetType().GetCustomAttribute<SceneHandleDrawAttribute>(true) != null)
        {
            var fields = ReflectionHelper.GetAllFields(drawableObject.GetType());
            foreach (var field in fields)
            {
                var DrawConfigurationAttribute = field.GetCustomAttribute<DrawConfigurationAttribute>() as DrawConfigurationAttribute;
                if (DrawConfigurationAttribute != null)
                {
                    var configurationAsset = CommonGameConfigurations.GetConfiguration(DrawConfigurationAttribute.ConfigurationType);
                    if (configurationAsset != null)
                    {
                        configurationAsset.GetEntryTry((Enum) field.GetValue(drawableObject), out var configurationDataObject);
                        if (configurationDataObject != null) Draw(configurationDataObject, objectTransform, CommonGameConfigurations);
                    }
                }

                var DrawNestedAttribute = field.GetCustomAttribute<DrawNestedAttribute>() as DrawNestedAttribute;
                if (DrawNestedAttribute != null) Draw(field.GetValue(drawableObject), objectTransform, CommonGameConfigurations);

                var AbstractSceneHandleAttributes = field.GetCustomAttributes<AbstractSceneHandleAttribute>(true);
                if (AbstractSceneHandleAttributes != null)
                {
                    foreach (var AbstractSceneHandleAttribute in AbstractSceneHandleAttributes)
                    {
                        if (AbstractSceneHandleAttribute.GetType() == typeof(WireArcAttribute))
                        {
                            var WireArcAttribute = (WireArcAttribute) AbstractSceneHandleAttribute;
                            var semiAngle = GetFieldValue<float>(drawableObject, field);

                            SetupColors(WireArcAttribute.GetColor());

                            DrawLabel(field.Name, WireArcAttribute.Radius, objectTransform);
                            Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, semiAngle, WireArcAttribute.Radius);
                            Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, -semiAngle, WireArcAttribute.Radius);
                        }
                        else if (AbstractSceneHandleAttribute.GetType() == typeof(WireCircleAttribute))
                        {
                            var WireCircleAttribute = (WireCircleAttribute) AbstractSceneHandleAttribute;

                            var radius = GetFieldValue<float>(drawableObject, field);

                            SetupColors(WireCircleAttribute.GetColor());

                            DrawLabel(field.Name, radius, objectTransform);
                            DrawLabel(field.Name, radius, objectTransform);
                            Handles.DrawWireDisc(objectTransform.position, objectTransform.up, radius);
                        }
                        else if (AbstractSceneHandleAttribute.GetType() == typeof(WireCircleWorldAttribute))
                        {
                            var WireCircleWorldAttribute = (WireCircleWorldAttribute) AbstractSceneHandleAttribute;

                            Vector3 localPositionCenter = Vector3.zero;
                            float radius;

                            if (!WireCircleWorldAttribute.UseTransform)
                            {
                                localPositionCenter = GetPositionFromObject(drawableObject.GetType().GetField(WireCircleWorldAttribute.PositionFieldName).GetValue(drawableObject));
                            }

                            if (!string.IsNullOrEmpty(WireCircleWorldAttribute.PositionOffsetFieldName))
                            {
                                localPositionCenter += (Vector3) drawableObject.GetType().GetField(WireCircleWorldAttribute.PositionOffsetFieldName).GetValue(drawableObject);
                            }

                            if (!string.IsNullOrEmpty(WireCircleWorldAttribute.RadiusFieldName))
                            {
                                radius = (float) drawableObject.GetType().GetField(WireCircleWorldAttribute.RadiusFieldName).GetValue(drawableObject);
                            }
                            else
                            {
                                radius = WireCircleWorldAttribute.Radius;
                            }

                            Vector3 worldPositionCenter = localPositionCenter;
                            if (WireCircleWorldAttribute.UseTransform)
                            {
                                worldPositionCenter = objectTransform.localToWorldMatrix.MultiplyPoint(localPositionCenter);
                            }

                            SetupColors(WireCircleWorldAttribute.GetColor());
                            DrawLabel(field.Name, radius, worldPositionCenter);
                            DrawLabel(field.Name, radius, worldPositionCenter);
                            Handles.DrawWireDisc(worldPositionCenter, Vector3.up, radius);
                        }
                        else if (AbstractSceneHandleAttribute.GetType() == typeof(WireBoxAttribute))
                        {
                            var WireBoxAttribute = (WireBoxAttribute) AbstractSceneHandleAttribute;

                            var center = (Vector3) drawableObject.GetType().GetField(WireBoxAttribute.CenterFieldName).GetValue(drawableObject);
                            var size = (Vector3) drawableObject.GetType().GetField(WireBoxAttribute.SizeFieldName).GetValue(drawableObject);

                            SetupColors(WireBoxAttribute.GetColor());
                            HandlesHelper.DrawBox(center, size, objectTransform, WireBoxAttribute.GetColor(), drawableObject.GetType().Name, MyEditorStyles.SceneDrawDynamicLabelStyle);
                        }
                        else if (AbstractSceneHandleAttribute.GetType() == typeof(WireFrustumAttribute))
                        {
                            var WireFrustumAttribute = (WireFrustumAttribute) AbstractSceneHandleAttribute;
                            var frustum = (FrustumV2) field.GetValue(drawableObject);
                            SetupColors(WireFrustumAttribute.GetColor());
                            DrawFrustum(frustum, objectTransform, false);
                        }
                        else if (AbstractSceneHandleAttribute.GetType() == typeof(WireRoundedFrustumAttribute))
                        {
                            var WireRoundedFrustumAttribute = (WireRoundedFrustumAttribute) AbstractSceneHandleAttribute;
                            var frustum = (FrustumV2) field.GetValue(drawableObject);
                            SetupColors(WireRoundedFrustumAttribute.GetColor());
                            DrawFrustum(frustum, objectTransform, true);
                        }
                        else if (AbstractSceneHandleAttribute.GetType() == typeof(WireDirectionalLineAttribute))
                        {
                            var WireLineAttribute = (WireDirectionalLineAttribute) AbstractSceneHandleAttribute;
                            var lineLength = (float) field.GetValue(drawableObject);
                            SetupColors(WireLineAttribute.GetColor());
                            Handles.DrawLine(objectTransform.transform.position, objectTransform.transform.position + new Vector3(WireLineAttribute.dX, WireLineAttribute.dY, WireLineAttribute.dZ) * lineLength);
                        }
                        else if (AbstractSceneHandleAttribute.GetType() == typeof(WireArrowLinkAttribute))
                        {
                            var WireArrowLinkAttribute = (WireArrowLinkAttribute) AbstractSceneHandleAttribute;

                            Vector3 Source = WireArrowLinkAttribute.Source;
                            Vector3 Target = WireArrowLinkAttribute.Target;

                            if (!string.IsNullOrEmpty(WireArrowLinkAttribute.SourceFieldName))
                            {
                                Source = GetPositionFromObject(drawableObject.GetType().GetField(WireArrowLinkAttribute.SourceFieldName).GetValue(drawableObject));
                            }

                            if (!string.IsNullOrEmpty(WireArrowLinkAttribute.TargetFieldName))
                            {
                                Target = GetPositionFromObject(drawableObject.GetType().GetField(WireArrowLinkAttribute.TargetFieldName).GetValue(drawableObject));
                            }

                            SetupColors(WireArrowLinkAttribute.GetColor());
                            HandlesHelper.DrawArrow(Source, Target, WireArrowLinkAttribute.GetColor(), WireArrowLinkAttribute.ArrowSemiAngle, WireArrowLinkAttribute.ArrowLength);
                        }
                        else if (AbstractSceneHandleAttribute.GetType() == typeof(WireArrowAttribute))
                        {
                            var WireArrowAttribute = (WireArrowAttribute) AbstractSceneHandleAttribute;

                            Vector3 Origin = WireArrowAttribute.Origin;
                            if (!string.IsNullOrEmpty(WireArrowAttribute.OriginFieldName))
                            {
                                Origin = GetPositionFromObject(drawableObject.GetType().GetField(WireArrowAttribute.OriginFieldName).GetValue(drawableObject));
                            }

                            SetupColors(WireArrowAttribute.GetColor());

                            Vector3? WorldEulerAngles = GetRotationFromObject(drawableObject.GetType().GetField(WireArrowAttribute.OriginFieldName).GetValue(drawableObject));
                            if (WorldEulerAngles.HasValue)
                            {
                                Vector3 Target = Origin + (Quaternion.Euler(WorldEulerAngles.Value) * (Vector3.forward * WireArrowAttribute.ArrowLength * 1.5f));

                                HandlesHelper.DrawArrow(Origin, Target, WireArrowAttribute.GetColor(), WireArrowAttribute.ArrowSemiAngle, WireArrowAttribute.ArrowLength);
                            }
                        }
                    }
                }
            }
        }
    }

    private static T GetFieldValue<T>(object drawableObject, FieldInfo field)
    {
        var value = default(T);
        /*
        if (typeof(IByEnumProperty).IsAssignableFrom(field.GetValue(drawableObject).GetType()))
        {
            IByEnumProperty IByEnumProperty = (IByEnumProperty)field.GetValue(drawableObject);
            object objectValue;
            IByEnumProperty.TryGetValue(IObjectGizmoDisplayEnableArea.GetEnumParameter(IByEnumProperty.GetType()), out objectValue);
            if (objectValue != null)
            {
                value = (T)objectValue;
            }
        }
        else
        {
        */
        value = (T) field.GetValue(drawableObject);
        // }

        return value;
    }

    private static void SetupColors(Color color)
    {
        Handles.color = color;
        MyEditorStyles.SceneDrawDynamicLabelStyle.normal.textColor = color;
    }

    private static void DrawLabel(string label, float height, Transform objectTransform)
    {
        Handles.Label(objectTransform.position + Vector3.up * height, label, MyEditorStyles.SceneDrawDynamicLabelStyle);
    }

    private static void DrawLabel(string label, float height, Vector3 worldPosition)
    {
        Handles.Label(worldPosition + Vector3.up * height, label, MyEditorStyles.SceneDrawDynamicLabelStyle);
    }

    private static void DrawFrustum(FrustumV2 frustum, Transform transform, bool isRounded)
    {
        frustum.CalculateFrustumWorldPositionyFace(out var LocalFrustumPointPositions, new TransformStruct {WorldPosition = Vector3.zero, WorldRotationEuler = Vector3.zero, LossyScale = Vector3.one});
        var frustumWorldPositions = new RangeFrustumWorldPositioning {LocalFrustumPositions = LocalFrustumPointPositions}.GetWorldFrustumPositions(transform.localToWorldMatrix);
        DrawFace(frustumWorldPositions.FC1, frustumWorldPositions.FC2, frustumWorldPositions.FC3, frustumWorldPositions.FC4);
        DrawFace(frustumWorldPositions.FC1, frustumWorldPositions.FC5, frustumWorldPositions.FC6, frustumWorldPositions.FC2);
        DrawFace(frustumWorldPositions.FC2, frustumWorldPositions.FC6, frustumWorldPositions.FC7, frustumWorldPositions.FC3);
        DrawFace(frustumWorldPositions.FC3, frustumWorldPositions.FC7, frustumWorldPositions.FC8, frustumWorldPositions.FC4);
        DrawFace(frustumWorldPositions.FC4, frustumWorldPositions.FC8, frustumWorldPositions.FC5, frustumWorldPositions.FC1);
        DrawFace(frustumWorldPositions.FC5, frustumWorldPositions.FC6, frustumWorldPositions.FC7, frustumWorldPositions.FC8);

        if (isRounded) Handles.DrawWireDisc(transform.position, transform.up, frustum.FaceDistance);
    }

    private static void DrawFace(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
    {
        Handles.DrawLine(C1, C2);
        Handles.DrawLine(C2, C3);
        Handles.DrawLine(C3, C4);
        Handles.DrawLine(C4, C1);
    }

    private static Vector3 GetPositionFromObject(object position)
    {
        if (position.GetType() == typeof(Vector3))
        {
            return (Vector3) position;
        }
        else if (position.GetType() == typeof(AIMoveToActionInputData))
        {
            return ((AIMoveToActionInputData) position).WorldPosition;
        }

        return Vector3.zero;
    }

    private static Vector3? GetRotationFromObject(object rotation)
    {
        if (rotation.GetType() == typeof(Vector3))
        {
            return (Vector3) rotation;
        }
        else if (rotation.GetType() == typeof(AIMoveToActionInputData))
        {
            return ((AIMoveToActionInputData) rotation).GetWorldRotation();
        }

        return default(Nullable<Vector3>);
    }
}