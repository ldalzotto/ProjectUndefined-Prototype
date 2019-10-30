using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class DynamicComputeBufferManager<T> where T : struct
    {
        private ComputeBuffer customComputeBuffer;
        private List<T> computeBufferData;

        private BufferReAllocateStrategy BufferReAllocateStrategy;
        private string materialPropertyName;
        private string materialCountPropertyName;
        private int objectByteSize;

        private AbstractDynamicComputeBufferManagerSetter<T> AbstractDynamicComputeBufferManagerSetter;

        public DynamicComputeBufferManager(int objectByteSize, string materialPropertyName, string materialCountPropertyName, List<Material> materials, BufferReAllocateStrategy BufferReAllocateStrategy = BufferReAllocateStrategy.NONE, ComputeBufferType ComputeBufferType = ComputeBufferType.Default)
        {
            this.CommonInit(objectByteSize, materialPropertyName, materialCountPropertyName, BufferReAllocateStrategy, ComputeBufferType);
            this.AbstractDynamicComputeBufferManagerSetter = new MaterialDynamicComputeBufferManagerSetter<T>(materials);
            this.AbstractDynamicComputeBufferManagerSetter.SetBuffer(this.customComputeBuffer, this.materialPropertyName);
        }

        public DynamicComputeBufferManager(int objectByteSize, string materialPropertyName, string materialCountPropertyName, MaterialPropertyBlock MaterialPropertyBlock, BufferReAllocateStrategy BufferReAllocateStrategy = BufferReAllocateStrategy.NONE, ComputeBufferType ComputeBufferType = ComputeBufferType.Default)
        {
            this.CommonInit(objectByteSize, materialPropertyName, materialCountPropertyName, BufferReAllocateStrategy, ComputeBufferType);
            this.AbstractDynamicComputeBufferManagerSetter = new MaterialPropertyDynamicComputeBufferManagerSetter<T>(MaterialPropertyBlock);
            this.AbstractDynamicComputeBufferManagerSetter.SetBuffer(this.customComputeBuffer, this.materialPropertyName);
        }

        private void CommonInit(int objectByteSize, string materialPropertyName, string materialCountPropertyName, BufferReAllocateStrategy BufferReAllocateStrategy, ComputeBufferType ComputeBufferType)
        {
            this.objectByteSize = objectByteSize;
            this.materialPropertyName = materialPropertyName;
            this.materialCountPropertyName = materialCountPropertyName;

            this.BufferReAllocateStrategy = BufferReAllocateStrategy;

            this.customComputeBuffer = new ComputeBuffer(1, objectByteSize, ComputeBufferType);
            this.computeBufferData = new List<T>();
        }

        public void Tick(Action<List<T>> bufferDataSetter)
        {
            if (this.customComputeBuffer != null && this.customComputeBuffer.IsValid())
            {
                this.computeBufferData.Clear();
                bufferDataSetter.Invoke(this.computeBufferData);

                if ((this.BufferReAllocateStrategy == BufferReAllocateStrategy.NONE && this.customComputeBuffer.count != this.computeBufferData.Count)
                      || (this.BufferReAllocateStrategy == BufferReAllocateStrategy.SUPERIOR_ONLY && this.computeBufferData.Count > this.customComputeBuffer.count))
                {
                    if (this.computeBufferData.Count != 0)
                    {
                        this.Dispose();
                        this.customComputeBuffer = new ComputeBuffer(this.computeBufferData.Count, this.objectByteSize);
                        this.AbstractDynamicComputeBufferManagerSetter.SetBuffer(this.customComputeBuffer, this.materialPropertyName);
                    }
                }

                this.AbstractDynamicComputeBufferManagerSetter.SetCounter(this.computeBufferData, this.materialCountPropertyName);

                this.customComputeBuffer.SetData(this.computeBufferData);
            }

        }

        public void Dispose()
        {
            ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.customComputeBuffer);
        }

        public int GetSize()
        {
            return this.computeBufferData.Count;
        }
    }

    public enum BufferReAllocateStrategy
    {
        NONE = 0,
        SUPERIOR_ONLY = 1
    }

    abstract class AbstractDynamicComputeBufferManagerSetter<T> where T : struct
    {
        public abstract void SetBuffer(ComputeBuffer computeBuffer, string bufferPrepertyName);
        public abstract void SetCounter(List<T> computeBufferData, string countPrepertyName);
    }

    class MaterialDynamicComputeBufferManagerSetter<T> : AbstractDynamicComputeBufferManagerSetter<T> where T : struct
    {
        private List<Material> materials;

        public MaterialDynamicComputeBufferManagerSetter(List<Material> materials)
        {
            this.materials = materials;
        }

        public override void SetBuffer(ComputeBuffer computeBuffer, string bufferPrepertyName)
        {
            if (!string.IsNullOrEmpty(bufferPrepertyName))
            {
                foreach (var material in this.materials)
                {
                    material.SetBuffer(bufferPrepertyName, computeBuffer);
                }
            }
        }

        public override void SetCounter(List<T> computeBufferData, string countPrepertyName)
        {
            if (!string.IsNullOrEmpty(countPrepertyName))
            {
                foreach (var material in this.materials)
                {
                    material.SetInt(countPrepertyName, computeBufferData.Count);
                }
            }
        }
    }

    class MaterialPropertyDynamicComputeBufferManagerSetter<T> : AbstractDynamicComputeBufferManagerSetter<T> where T : struct
    {
        private MaterialPropertyBlock MaterialPropertyBlock;

        public MaterialPropertyDynamicComputeBufferManagerSetter(MaterialPropertyBlock materialPropertyBlock)
        {
            MaterialPropertyBlock = materialPropertyBlock;
        }

        public override void SetBuffer(ComputeBuffer computeBuffer, string bufferPrepertyName)
        {
            if (!string.IsNullOrEmpty(bufferPrepertyName))
            {
                this.MaterialPropertyBlock.SetBuffer(bufferPrepertyName, computeBuffer);
            }
        }

        public override void SetCounter(List<T> computeBufferData, string countPrepertyName)
        {
            if (!string.IsNullOrEmpty(countPrepertyName))
            {
                this.MaterialPropertyBlock.SetFloat(countPrepertyName, computeBufferData.Count);
            }
        }
    }
}
