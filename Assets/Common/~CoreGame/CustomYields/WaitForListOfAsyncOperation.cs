using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaitForListOfAsyncOperation : CustomYieldInstruction
{

    private List<AsyncOperation> asyncOperations;

    public WaitForListOfAsyncOperation(List<AsyncOperation> asyncOperations)
    {
        this.asyncOperations = asyncOperations;
    }

    public override bool keepWaiting
    {
        get
        {
            foreach (var asyncOperation in this.asyncOperations)
            {
                if (!asyncOperation.isDone)
                {
                    if (asyncOperation.allowSceneActivation == false && asyncOperation.progress >= 0.9f)
                    {
                        return false;
                    }
                    return true;
                }

            }
            return false;
        }
    }

}
