using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using Unity.Mathematics;
using UnityEngine;

public class CameraPanningTest : MonoBehaviour
{
    public Transform CameraPivotPoint;
    private Camera camera;

    public Transform WorldPoint;

    public Vector3 ScreenDirection;

    // Start is called before the first frame update
    void Start()
    {
        this.camera = Camera.main;
        this.initialCameraPivotPosition = this.CameraPivotPoint.transform.position;
    }

    private Vector3 initialCameraPivotPosition;

    private Vector3 currentDelta;
    private float t;

    // Update is called once per frame
    void Update()
    {
        //DoCalculation();
        DoCalculationMath();
    }

    private void DoCalculation()
    {
        /// [-1,-1] [1,1]
        var screenPosition = (camera.projectionMatrix * camera.worldToCameraMatrix).MultiplyPoint(this.WorldPoint.transform.position);
        Debug.Log(MyLog.Format(screenPosition));
        t += Time.deltaTime;

        /// Convert [-1,1]-[1,1] to world delta
        var worldDirection = (camera.projectionMatrix * camera.worldToCameraMatrix).inverse.MultiplyVector(this.ScreenDirection);
        this.currentDelta = Vector3.Lerp(this.currentDelta, worldDirection, Time.deltaTime * 10f);
        this.CameraPivotPoint.transform.position = this.initialCameraPivotPosition + this.currentDelta;
    }


    private void DoCalculationMath()
    {
        float4x4 projectionMatrixMath = camera.projectionMatrix;
        float4x4 worldToCameraMatrixMath = camera.worldToCameraMatrix;

        t += Time.deltaTime;


        var worldDirectionCorrect = (camera.projectionMatrix * camera.worldToCameraMatrix).inverse.MultiplyVector(this.ScreenDirection);
        /// Convert [-1,1]-[1,1] to world delta
        var worldDirection =
            math.inverse(math.mul(projectionMatrixMath, worldToCameraMatrixMath)).MultiplyVector(this.ScreenDirection);

        this.currentDelta = math.lerp(this.currentDelta, worldDirection, Time.deltaTime * 10);
        
        Debug.Log(camera.worldToCameraMatrix.ToString("F4"));
        
        
        
        this.CameraPivotPoint.transform.position = this.initialCameraPivotPosition + this.currentDelta;
    }

    private Vector3 WorldDirection;
}