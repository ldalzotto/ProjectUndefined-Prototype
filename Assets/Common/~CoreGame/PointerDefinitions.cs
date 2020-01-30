using System;
using System.Runtime.InteropServices;
namespace CoreGame
{
 public static class ClassPointerExtension
{
        public static GameObjectPointer Allocate(this UnityEngine.GameObject obj)
        {
            return new GameObjectPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }        public static MeshRendererPointer Allocate(this UnityEngine.MeshRenderer obj)
        {
            return new MeshRendererPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }        public static CameraPointer Allocate(this UnityEngine.Camera obj)
        {
            return new CameraPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }
}    public struct GameObjectPointer : IDisposable
    {
        private GCHandle ptr;

        public GameObjectPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public UnityEngine.GameObject GetValue()
        {
            return this.ptr.Target as UnityEngine.GameObject;
        }

        public void Dispose()
        {
            if (((IntPtr) this.ptr) != IntPtr.Zero)
            {
                this.ptr.Free();
            }
        }
    }    public struct MeshRendererPointer : IDisposable
    {
        private GCHandle ptr;

        public MeshRendererPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public UnityEngine.MeshRenderer GetValue()
        {
            return this.ptr.Target as UnityEngine.MeshRenderer;
        }

        public void Dispose()
        {
            if (((IntPtr) this.ptr) != IntPtr.Zero)
            {
                this.ptr.Free();
            }
        }
    }    public struct CameraPointer : IDisposable
    {
        private GCHandle ptr;

        public CameraPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public UnityEngine.Camera GetValue()
        {
            return this.ptr.Target as UnityEngine.Camera;
        }

        public void Dispose()
        {
            if (((IntPtr) this.ptr) != IntPtr.Zero)
            {
                this.ptr.Free();
            }
        }
    }
}