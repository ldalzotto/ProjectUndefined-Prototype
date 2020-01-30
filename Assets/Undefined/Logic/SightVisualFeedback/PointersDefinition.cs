using System;
using System.Runtime.InteropServices;

namespace SightVisualFeedback
{
    public unsafe struct SightVisualFeedbackSystemPointer : IDisposable
    {
        private SightVisualFeedbackSystem* ptr;
        
        public static SightVisualFeedbackSystemPointer Allocate()
        {
            SightVisualFeedbackSystemPointer ptr = new SightVisualFeedbackSystemPointer();
            ptr.ptr = (SightVisualFeedbackSystem*) Marshal.AllocHGlobal(sizeof(SightVisualFeedbackSystem));
            return ptr;
        }

        public SightVisualFeedbackSystem* Ref()
        {
            return this.ptr;
        }
        
        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)this.ptr);
        }
    }

    
    public static class ClassPointerExtension
    {
        public static SightVisualFeedbackSystemDefinitionPointer ToPointer(this SightVisualFeedbackSystemDefinition obj)
        {
            return new SightVisualFeedbackSystemDefinitionPointer(GCHandle.Alloc(obj, GCHandleType.Normal));
        }

    }
 
    
    public struct SightVisualFeedbackSystemDefinitionPointer : IDisposable
    {
        private GCHandle ptr;

        public SightVisualFeedbackSystemDefinitionPointer(GCHandle ptr)
        {
            this.ptr = ptr;
        }

        public SightVisualFeedbackSystemDefinition GetValue()
        {
            return this.ptr.Target as SightVisualFeedbackSystemDefinition;
        }

        public void Dispose()
        {
            this.ptr.Free();
        }
    }
}