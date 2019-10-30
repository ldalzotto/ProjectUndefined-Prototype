using UnityEngine;

[System.Serializable]
public struct Vector3Binarry
{
    [SerializeField]
    public float x;
    [SerializeField]
    public float y;
    [SerializeField]
    public float z;

    public Vector3Binarry(Vector3 vector3)
    {
        this.x = vector3.x;
        this.y = vector3.y;
        this.z = vector3.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public struct QuaternionBinarry
{
    [SerializeField]
    public float x;
    [SerializeField]
    public float y;
    [SerializeField]
    public float z;
    [SerializeField]
    public float w;

    public QuaternionBinarry(Quaternion quaternion)
    {
        this.x = quaternion.x;
        this.y = quaternion.y;
        this.z = quaternion.z;
        this.w = quaternion.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}

[System.Serializable]
public struct TransformBinarry
{
    [SerializeField]
    public Vector3Binarry position;
    [SerializeField]
    public QuaternionBinarry rotation;
    [SerializeField]
    public Vector3Binarry localScale;

    public TransformBinarry(Transform transform)
    {
        this.position = new Vector3Binarry(transform.position);
        this.rotation = new QuaternionBinarry(transform.rotation);
        this.localScale = new Vector3Binarry(transform.localScale);
    }

    public TransformBinarryFormatted Format()
    {
        return new TransformBinarryFormatted(
                this.position.ToVector3(),
                this.rotation.ToQuaternion(),
                this.localScale.ToVector3()
            );
    }

}

public struct TransformBinarryFormatted
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public TransformBinarryFormatted(Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        this.position = position;
        this.rotation = rotation;
        this.localScale = localScale;
    }
}