using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CoreGame
{
    public static class ClassPointerExtension
    {
        public static GameObjectPointer ToPointer(this GameObject obj)
        {
            return new GameObjectPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }

        public static MeshRendererPointer ToPointer(this MeshRenderer obj)
        {
            return new MeshRendererPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }
        
        public static CameraPointer ToPointer(this Camera obj)
        {
            return new CameraPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }
    }

    public struct GameObjectPointer : IDisposable
    {
        private GCHandle ptr;

        public GameObjectPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public GameObject GetValue()
        {
            return this.ptr.Target as GameObject;
        }

        public void Dispose()
        {
            this.ptr.Free();
        }
    }

    public struct MeshRendererPointer : IDisposable
    {
        private GCHandle ptr;

        public MeshRendererPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public MeshRenderer GetValue()
        {
            return this.ptr.Target as MeshRenderer;
        }

        public void Dispose()
        {
            this.ptr.Free();
        }
    }
    public struct CameraPointer : IDisposable
    {
        private GCHandle ptr;

        public CameraPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public Camera GetValue()
        {
            return this.ptr.Target as Camera;
        }

        public void Dispose()
        {
            this.ptr.Free();
        }
    }
}