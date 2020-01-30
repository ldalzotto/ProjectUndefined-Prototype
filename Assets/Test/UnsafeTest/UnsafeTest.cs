using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using Unity.Collections;

namespace Test.UnsafeTest
{
    public unsafe class UnsafeTest : MonoBehaviour
    {
        private void Start()
        {
            
            using (UnsafeListV2<MyStruct> warmList = new UnsafeListV2<MyStruct>(1, 1))
            {
                MyStruct* MyStructPtr = MyStruct.Allocate(new MyStruct(1, true, 5));
                warmList.Add(MyStructPtr);
                Marshal.FreeHGlobal((IntPtr) MyStructPtr);
            }


            Profiler.BeginSample("UnsafeTest : Allocation");
            var list = new UnsafeListV2<MyStruct>(1024, 2);
            for (int i = 0; i < 1024; i++)
            {
                MyStruct* MyStructPtrTmp = MyStruct.Allocate(new MyStruct(1, true, 5));
                list.Add(MyStructPtrTmp);
            }

            Profiler.EndSample();


            Profiler.BeginSample("UnsafeTest : DeAlloc");
            for (int i = 0; i < 1000; i++)
            {
                var foundPtr = list.Get(i);
                if ((IntPtr) foundPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal((IntPtr) foundPtr);
                }
            }

            list.Dispose();

            Profiler.EndSample();
        }

        public unsafe static void FooV2()
        {
        }

        private void Update()
        {
        }
    }

    struct MyStruct : IEquatable<MyStruct>
    {
        public int Int;
        public bool Bool;
        public long Long;

        public MyStruct(int i, bool b, long l)
        {
            Int = i;
            Bool = b;
            Long = l;
        }

        public static unsafe MyStruct* Allocate(MyStruct MyStruct)
        {
            MyStruct* MyStructHandlerPointer = (MyStruct*) Marshal.AllocHGlobal(sizeof(MyStruct));
            *MyStructHandlerPointer = MyStruct;
            return MyStructHandlerPointer;
        }

        public bool Equals(MyStruct other)
        {
            return Int == other.Int && Bool == other.Bool && Long == other.Long;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Int;
                hashCode = (hashCode * 397) ^ Bool.GetHashCode();
                hashCode = (hashCode * 397) ^ Long.GetHashCode();
                return hashCode;
            }
        }
    }

    public unsafe struct UnsafeListV2<T> : IDisposable where T : unmanaged, IEquatable<T>
    {
        private UnsafeStackV2<int> DeletedFreedIndexes;
        public int Count { get; private set; }
        public int Capacity { get; private set; }
        public int MaxSettedIndexValue { get; private set; }

        private void* Memory;

        private int ElementSizeInByte;

        public UnsafeListV2(int initialSize, int deleteIndexBufferInitialSize)
        {
            if (initialSize == 0)
            {
                initialSize = 2;
            }

            if (deleteIndexBufferInitialSize == 0)
            {
                deleteIndexBufferInitialSize = 2;
            }

            this.DeletedFreedIndexes = new UnsafeStackV2<int>(deleteIndexBufferInitialSize, sizeof(int));
            this.ElementSizeInByte = sizeof(IntPtr);
            this.Memory = (void*) Marshal.AllocHGlobal(initialSize * ElementSizeInByte);
            for (int i = 0; i < initialSize; i++)
            {
                *(IntPtr*) (((byte*) Memory) + ElementSizeInByte * i) = IntPtr.Zero;
            }

            this.Count = 0;
            this.Capacity = initialSize;
            this.MaxSettedIndexValue = -1;
        }

        public T* Get(int index)
        {
            this.CheckIfIndexIsAllowed(index);
            return this.Get_NoCheck(index);
        }

        public T* Get_NoCheck(int index)
        {
            return (T*) ((IntPtr*) (((byte*) Memory) + ElementSizeInByte * index))->ToPointer();
        }

        public void Set(int index, T* input)
        {
            this.CheckIfIndexIsAllowed(index);
            this.Set_NoCheck(index, input);
        }

        public void Set_NoCheck(int index, T* input)
        {
            *(IntPtr*) (((byte*) Memory) + ElementSizeInByte * index) = new IntPtr(input);
            this.MaxSettedIndexValue = Math.Max(this.MaxSettedIndexValue, index);
        }

        public void Add(T* input)
        {
            if (this.DeletedFreedIndexes.Count > 0)
            {
                this.Set((*this.DeletedFreedIndexes.Pop()), input);
            }
            else
            {
                if (!(Count >= 0 && Count < this.Capacity))
                {
                    this.DoubleCapacity();
                }

                this.Set(Count, input);
            }

            Count += 1;
        }

        private void DoubleCapacity()
        {
            var newCapacity = this.Capacity * 2;
            void* newMemory = (void*) Marshal.AllocHGlobal(newCapacity * ElementSizeInByte);
            for (int i = 0; i < newCapacity; i++)
            {
                *(IntPtr*) (((byte*) newMemory) + ElementSizeInByte * i) = IntPtr.Zero;
            }

            Buffer.MemoryCopy(this.Memory, newMemory, newCapacity * ElementSizeInByte, this.Capacity * ElementSizeInByte);

            Marshal.FreeHGlobal((IntPtr) this.Memory);
            this.Memory = newMemory;
            this.Capacity = newCapacity;
        }

        public bool Remove(int index)
        {
            this.CheckIfIndexIsAllowed(index);
            if (((IntPtr) this.Get_NoCheck(index)) != IntPtr.Zero)
            {
                this.Set_NoCheck(index, (T*) IntPtr.Zero);
                this.DeletedFreedIndexes.Push(index);
                Count -= 1;
                this.SetMaxSettedIndexValueToTheClosetValue();
                return true;
            }

            return false;
        }

        public bool Remove(T obj)
        {
            for (int i = 0; i <= MaxSettedIndexValue; i++)
            {
                T* currentPointer = this.Get_NoCheck(i);
                if ((IntPtr) currentPointer != IntPtr.Zero)
                {
                    if (this.Get_NoCheck(i)->Equals(obj))
                    {
                        this.Remove(i);
                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckIfIndexIsAllowed(int index)
        {
            if (!(index >= 0 && index < this.Capacity))
            {
                throw new IndexOutOfRangeException("The index : " + index + " is out of range. Max capacity : " + this.Capacity);
            }
        }

        private void SetMaxSettedIndexValueToTheClosetValue()
        {
            if (Count == 0)
            {
                this.MaxSettedIndexValue = -1;
            }
        }

        public void Dispose()
        {
            this.DeletedFreedIndexes.Dispose();
            if (this.Memory != null)
            {
                Marshal.FreeHGlobal((IntPtr) this.Memory);
            }
        }
    }

    public unsafe struct UnsafeStackV2<T> : IDisposable where T : unmanaged
    {
        public int Count { get; private set; }
        public int Capacity { get; private set; }

        private void* Memory;
        private int ElementSizeInByte;

        public UnsafeStackV2(int initialSize, int elementSizeInByte)
        {
            this.Memory = null;
            this.ElementSizeInByte = elementSizeInByte;
            if (initialSize == 0)
            {
                initialSize = 2;
            }

            this.Memory = (void*) Marshal.AllocHGlobal(initialSize * ElementSizeInByte);
            this.Count = 0;
            this.Capacity = initialSize;
        }

        public void Push(T input)
        {
            if (Count + 1 > Capacity)
            {
                this.DoubleCapacity();
            }

            *(T*) (((byte*) Memory) + ElementSizeInByte * Count) = input;
            Count += 1;
        }

        private void DoubleCapacity()
        {
            var newCapacity = this.Capacity * 2;
            void* newMemory = (void*) Marshal.AllocHGlobal(newCapacity * ElementSizeInByte);

            Buffer.MemoryCopy(this.Memory, newMemory, newCapacity * ElementSizeInByte, this.Capacity * ElementSizeInByte);

            Marshal.FreeHGlobal((IntPtr) this.Memory);
            this.Memory = newMemory;
            this.Capacity = newCapacity;
        }

        public T* Pop()
        {
            if (this.HasElements())
            {
                Count -= 1;
                return (T*) (((byte*) Memory) + ElementSizeInByte * Count);
            }

            return null;
        }

        public bool HasElements()
        {
            return Count > 0;
        }

        public void Dispose()
        {
            if (this.Memory != null)
            {
                Marshal.FreeHGlobal((IntPtr) this.Memory);
            }
        }
    }

}