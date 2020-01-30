using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;

public class GarbageCollectionTest : MonoBehaviour
{
    private GCHandle GarabageCollectionClassGChandle;

    private event Action MyEvent;
    
    private void Start()
    {
        Profiler.BeginSample("GarbageCollectionTest : Start");
        this.UpdateCallback(this.Update);
        this.UpdateCallback(this.Update);
        this.UpdateCallback(this.Update);
        this.UpdateCallback(this.Update);
        this.UpdateCallback(this.Update);
        Profiler.EndSample();
        
   //     var GarabageCollectionClass = new GarabageCollectionClass(10);
    //    this.GarabageCollectionClassGChandle = GCHandle.Alloc(GarabageCollectionClass);
    }

    private void UpdateCallback(Action Aya)
    {
        Aya.Invoke();
    }

    private void Update()
    {
      //  Debug.Log((this.GarabageCollectionClassGChandle.Target as GarabageCollectionClass).Value);
    }

    private void OnDestroy()
    {
    //    this.GarabageCollectionClassGChandle.Free();
    }
}

class GarabageCollectionClass
{
    public int Value;

    public GarabageCollectionClass(int value)
    {
        Value = value;
    }
}