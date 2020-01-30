using System;
using System.Runtime.InteropServices;
namespace AnimatorPlayable
{
 public static class ClassPointerExtension
{
        public static AnimatorPlayableObjectPointer Allocate(this AnimatorPlayable.AnimatorPlayableObject obj)
        {
            return new AnimatorPlayableObjectPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }
}    public struct AnimatorPlayableObjectPointer : IDisposable
    {
        private GCHandle ptr;

        public AnimatorPlayableObjectPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public AnimatorPlayable.AnimatorPlayableObject GetValue()
        {
            return this.ptr.Target as AnimatorPlayable.AnimatorPlayableObject;
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