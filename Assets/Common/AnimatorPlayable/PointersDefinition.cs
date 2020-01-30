using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AnimatorPlayable
{
    public static class ClassPointerExtension
    {
        public static AnimatorPlayableObjectPointer ToPointer(this AnimatorPlayableObject obj)
        {
            return new AnimatorPlayableObjectPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }

    }

    public struct AnimatorPlayableObjectPointer : IDisposable
    {
        private GCHandle ptr;

        public AnimatorPlayableObjectPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public AnimatorPlayableObject GetValue()
        {
            return this.ptr.Target as AnimatorPlayableObject;
        }

        public void Dispose()
        {
            this.ptr.Free();
        }
    }
}