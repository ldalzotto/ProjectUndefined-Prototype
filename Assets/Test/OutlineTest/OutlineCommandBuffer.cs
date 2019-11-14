using UnityEngine;
using UnityEngine.Rendering;

public class OutlineCommandBuffer : MonoBehaviour
{
    private static OutlineCommandBuffer Instance;

    private MaterialPropertyBlock WhiteOutline;
    private MaterialPropertyBlock BlackNonOutline;
    public static OutlineCommandBuffer Get()
    {
        if (Instance == null)
        {
            Instance = GameObject.FindObjectOfType<OutlineCommandBuffer>();
        }

        return Instance;
    }

    public Material BlitMaterial;
    public Material OutlineMaterial;
    public MeshFilter OutlinedMesh;

    public CommandBuffer OutlineTextureDrawBuffer;
    public CommandBuffer BlitBuffer;

    private void Start()
    {
        this.WhiteOutline = new MaterialPropertyBlock();
        this.WhiteOutline.SetColor("_OutlineColor", Color.white);
        this.WhiteOutline.SetFloat("_OutlineLocalDistance", 0.1f);
        
        this.BlackNonOutline = new MaterialPropertyBlock();
        this.BlackNonOutline.SetColor("_OutlineColor", Color.black);
        this.BlackNonOutline.SetFloat("_OutlineLocalDistance", 0f);
        
        this.OutlineTextureDrawBuffer = new CommandBuffer();
        this.OutlineTextureDrawBuffer.name = "OutlineTextureDrawBuffer";
        
        this.BlitBuffer = new CommandBuffer();
        this.BlitBuffer.name = "BlitBuffer";
    }

    private void Update()
    {
        
        this.BlitBuffer.Clear();
        this.BlitBuffer.Clear();
        
        this.BlitBuffer.DrawMesh(this.OutlinedMesh.mesh, this.OutlinedMesh.transform.localToWorldMatrix, this.OutlineMaterial,0,0,WhiteOutline);
        this.BlitBuffer.DrawMesh(this.OutlinedMesh.mesh, this.OutlinedMesh.transform.localToWorldMatrix, this.OutlineMaterial,0,0,BlackNonOutline);
        
        this.BlitBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
        
        this.BlitBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, BlitMaterial);
    }
}