using System;
using System.Runtime.InteropServices;
namespace InteractiveObjects
{
 public static class ClassPointerExtension
{
        public static CoreInteractiveObjectPointer Allocate(this InteractiveObjects.CoreInteractiveObject obj)
        {
            return new CoreInteractiveObjectPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }
}    public struct CoreInteractiveObjectPointer : IDisposable
    {
        private GCHandle ptr;

        public CoreInteractiveObjectPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public InteractiveObjects.CoreInteractiveObject GetValue()
        {
            return this.ptr.Target as InteractiveObjects.CoreInteractiveObject;
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