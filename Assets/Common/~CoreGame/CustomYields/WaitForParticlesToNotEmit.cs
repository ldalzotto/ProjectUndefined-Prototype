using UnityEngine;
using UnityEditor;

public class WaitForParticlesToNotEmit : CustomYieldInstruction
{

    private ParticleSystem particleSystem;

    public WaitForParticlesToNotEmit(ParticleSystem particleSystem)
    {
        this.particleSystem = particleSystem;
    }

    public override bool keepWaiting => particleSystem.isEmitting;
}