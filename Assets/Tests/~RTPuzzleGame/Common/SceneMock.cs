using System;
using Persistence;
using VisualFeedback;

namespace Tests
{
 
    public class MockDottedLineRendererManager : DottedLineRendererManager
    {
        public override void OnComputeBeziersInnerPointEvent(DottedLine DottedLine)
        {
        }

        protected override void OnComputeBeziersInnerPointResponse(ComputeBeziersInnerPointResponse ComputeBeziersInnerPointResponse)
        {
        }

        public override void Tick()
        {
        }
    }
}