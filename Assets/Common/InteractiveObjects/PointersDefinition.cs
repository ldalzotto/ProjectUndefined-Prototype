using System;
using System.Runtime.InteropServices;
using AnimatorPlayable;

namespace InteractiveObjects
{
    public static class ClassPointerExtension
    {
        public static CoreInteractiveObjectPointer ToPointer(this CoreInteractiveObject obj)
        {
            return new CoreInteractiveObjectPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }

    }

    public struct CoreInteractiveObjectPointer : IDisposable
    {
        private GCHandle ptr;

        public CoreInteractiveObjectPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public CoreInteractiveObject GetValue()
        {
            return this.ptr.Target as CoreInteractiveObject;
        }

        public void Dispose()
        {
            this.ptr.Free();
        }
    }
}