using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using UnityScript.Steps;

///
/// TODO
/// Finishing the UnsafeStackV2. It's memory copy when the capacity is doubled doesn't work.
/// Delete UnsafeStackOfPointer -> every unsafe stack must use the UnsafeStackV2.
/// Make the UnsafeList to use the stack for deletion index.
/// Make the UnsafeList generic like the UnsafeStackV2.
/// 

namespace Test.UnsafeTest
{
    public class UnsafeTest : MonoBehaviour
    {
        private void Start()
        {
           
        }

        public unsafe static void FooV2()
        {
            /*
            MyStructHandler* myStruct1 = MyStructHandler.Allocate(MyStruct.Allocate(new MyStruct(1, false, 5)));
            MyStructHandler* myStruct2 = MyStructHandler.Allocate(MyStruct.Allocate(new MyStruct(1, false, 6)));
            MyStructHandler* myStruct3 = MyStructHandler.Allocate(MyStruct.Allocate(new MyStruct(1, false, 7)));

            // Inline
            UnsafeListV2<MyStructHandler> list = new UnsafeListV2<MyStructHandler>(100, 1);
            list.Add(myStruct1);
            list.Add(myStruct2);
            list.Add(myStruct3);

            for (int i = 0; i <= list.MaxSettedIndexValue; i++)
            {
                Debug.Log(list.Get(i)->GetPointer()->Long);
            }

            for (int i = 0; i <= list.MaxSettedIndexValue; i++)
            {
                list.Get(i)->GetPointer()->Long = 99;
            }

            for (int i = 0; i <= list.MaxSettedIndexValue; i++)
            {
                Debug.Log(list.Get(i)->GetPointer()->Long);
            }

            myStruct2->Dispose();

            for (int i = 0; i <= list.MaxSettedIndexValue; i++)
            {
                Debug.Log(list.Get(i)->GetPointer()->Long);
            }

            list.Remove(1);
            list.Add(new MyStruct(1, false, 8));

            for (int i = 0; i <= list.MaxSettedIndexValue; i++)
            {
                Debug.Log(list.Get(i)->Long);
            }

            for (int i = 0; i <= list.MaxSettedIndexValue; i++)
            {
                list.Get(i)->Dispose();
            }

            list.Dispose();
            */
        }

        private void Update()
        {
            new JobAlloc().Schedule().Complete();
        }
    }

    struct JobAlloc : IJob
    {
        public void Execute()
        {
            new List<int>(50);
        }
    }

    struct MyStruct
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
    }

    public unsafe struct UnsafeListV2<T> : IDisposable where T : unmanaged
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
                initialSize = 1;
            }

            if (deleteIndexBufferInitialSize == 0)
            {
                deleteIndexBufferInitialSize = 1;
            }

            this.DeletedFreedIndexes = new UnsafeStackV2<int>(deleteIndexBufferInitialSize, sizeof(int));
            this.ElementSizeInByte = sizeof(IntPtr);
            this.Memory = (void*) Marshal.AllocHGlobal(initialSize * ElementSizeInByte);

            this.Count = 0;
            this.Capacity = initialSize;
            this.MaxSettedIndexValue = -1;
        }

        public T* Get(int index)
        {
            this.CheckIfIndexIsAllowed(index);
            return (T*) ((IntPtr*) (((byte*) Memory) + ElementSizeInByte * index))->ToPointer();
        }

        public void Set(int index, T* input)
        {
            this.CheckIfIndexIsAllowed(index);
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

            Buffer.MemoryCopy(this.Memory, newMemory, newCapacity * ElementSizeInByte, this.Capacity * ElementSizeInByte);

            Marshal.FreeHGlobal((IntPtr) this.Memory);
            this.Memory = newMemory;
            this.Capacity = newCapacity;
        }

        public void Remove(int index)
        {
            this.CheckIfIndexIsAllowed(index);
            if (((void*) this.Get(index)) != null)
            {
                this.DeletedFreedIndexes.Push(index);
                Count -= 1;
                this.SetMaxSettedIndexValueToTheClosetValue();
            }
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

    /*
    public unsafe struct UnsafeStackOfPointer : IDisposable
    {
        public int Count { get; private set; }
        public int Capacity { get; private set; }

        private void* Memory;
        private int ElementSizeInByte;

        public UnsafeStackOfPointer(int initialSize = 2)
        {
            this.Memory = null;
            this.ElementSizeInByte = sizeof(PointerContainer);
            if (initialSize == 0)
            {
                initialSize = 2;
            }

            this.Memory = (void*) Marshal.AllocHGlobal(initialSize * ElementSizeInByte);
            this.Count = 0;
            this.Capacity = initialSize;
        }

        public void Push(IntPtr input)
        {
            if (Count + 1 > Capacity)
            {
                this.DoubleCapacity();
            }

            *(PointerContainer*) (((byte*) Memory) + ElementSizeInByte * Count) = new PointerContainer(input);
            Count += 1;
        }

        private void DoubleCapacity()
        {
            var newCapacity = this.Capacity * 2;
            void* newMemory = (void*) Marshal.AllocHGlobal(newCapacity * ElementSizeInByte);

            for (var i = 0; i < this.Capacity; i++)
            {
                *(PointerContainer*) (((byte*) newMemory) + ElementSizeInByte * i)
                    = new PointerContainer(((PointerContainer*) (((byte*) Memory) + ElementSizeInByte * i))->StoredPointer);
            }
            
            Marshal.FreeHGlobal((IntPtr) this.Memory);
            this.Memory = newMemory;
            this.Capacity = newCapacity;
        }

        public IntPtr? Pop()
        {
            if (this.HasElements())
            {
                Count -= 1;
                var intPtr = ((PointerContainer*) (((byte*) Memory) + ElementSizeInByte * Count))->StoredPointer;
                return intPtr;
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
    */
}