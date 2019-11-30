using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct TransformStruct
{
    public Vector3 WorldPosition;
    public Vector3 WorldRotationEuler;
    public Vector3 LossyScale;

    public TransformStruct(Transform transform)
    {
        this.WorldPosition = transform.position;
        this.WorldRotationEuler = transform.eulerAngles;
        this.LossyScale = transform.lossyScale;
    }

    public static TransformStruct operator +(TransformStruct t1, TransformStruct t2)
    {
        return new TransformStruct()
        {
            WorldPosition = t1.WorldPosition + t2.WorldPosition,
            WorldRotationEuler = (Quaternion.Euler(t1.WorldRotationEuler) * Quaternion.Euler(t2.WorldRotationEuler)).eulerAngles,
            LossyScale = new Vector3(t1.LossyScale.x * t2.LossyScale.x, t1.LossyScale.y * t2.LossyScale.y, t1.LossyScale.z * t2.LossyScale.z)
        };
    }

    public bool IsEqualTo(TransformStruct other)
    {
        return (this.WorldPosition == other.WorldPosition) && (this.WorldRotationEuler == other.WorldRotationEuler) && (this.LossyScale == other.LossyScale);
    }
}
