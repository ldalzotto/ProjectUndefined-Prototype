using System;
using System.Runtime.InteropServices;
namespace SightVisualFeedback
{
 public static class ClassPointerExtension
{
        public static SightVisualFeedbackSystemDefinitionPointer Allocate(this SightVisualFeedback.SightVisualFeedbackSystemDefinition obj)
        {
            return new SightVisualFeedbackSystemDefinitionPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }
}    public struct SightVisualFeedbackSystemDefinitionPointer : IDisposable
    {
        private GCHandle ptr;

        public SightVisualFeedbackSystemDefinitionPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public SightVisualFeedback.SightVisualFeedbackSystemDefinition GetValue()
        {
            return this.ptr.Target as SightVisualFeedback.SightVisualFeedbackSystemDefinition;
        }

        public void Dispose()
        {
            if (((IntPtr) this.ptr) != IntPtr.Zero)
            {
                this.ptr.Free();
            }
        }
    }    public unsafe struct SightVisualFeedbackSystemPointer : IDisposable
    {
        private SightVisualFeedback.SightVisualFeedbackSystem* ptr;

        public static SightVisualFeedbackSystemPointer Allocate()
        {
            SightVisualFeedbackSystemPointer ptr = new SightVisualFeedbackSystemPointer();
            ptr.ptr = (SightVisualFeedback.SightVisualFeedbackSystem*) Marshal.AllocHGlobal(sizeof(SightVisualFeedback.SightVisualFeedbackSystem));
            return ptr;
        }

        public SightVisualFeedback.SightVisualFeedbackSystem* Ref()
        {
            return this.ptr;
        }

        public void Dispose()
        {
            if (((IntPtr) this.ptr) != IntPtr.Zero)
                Marshal.FreeHGlobal((IntPtr) this.ptr);
        }
    }
}