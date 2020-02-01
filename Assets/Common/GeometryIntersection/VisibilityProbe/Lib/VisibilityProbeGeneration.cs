using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GeometryIntersection
{
    public static class VisibilityProbeGeneration
    {
        public static VisibilityProbe GenerateAndAlocateVisibilityProbeLocalPointsFrom(float ProbeDensityPerUnit, BoxDefinition BoxDefinition)
        {
            Intersection.ExtractBoxColliderLocalPoints(BoxDefinition.LocalCenter, BoxDefinition.LocalSize,
                out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);

            float BoxWidth = Vector3.Distance(C1, C2);
            float BoxHeight = Vector3.Distance(C1, C4);
            float BoxDepth = Vector3.Distance(C1, C5);

            CalculateProbeNumberAndIntervalFromDensity(BoxWidth, ProbeDensityPerUnit, out int WidthNumberOfProbe, out float WidthDistanceBetweenProbes);
            CalculateProbeNumberAndIntervalFromDensity(BoxHeight, ProbeDensityPerUnit, out int HeightNumberOfProbe, out float HeightDistanceBetweenProbes);
            CalculateProbeNumberAndIntervalFromDensity(BoxDepth, ProbeDensityPerUnit, out int DepthNumberOfProbe, out float DepthDistanceBetweenProbes);

            int totalNumberOfProbe = (WidthNumberOfProbe + HeightNumberOfProbe - 2 + DepthNumberOfProbe - 2) * 4;
            VisibilityProbe VisibilityProbe = VisibilityProbe.Allocate(totalNumberOfProbe);

            int probeCounter = 0;

            FeedVisibilityProbe(C1, C2, true, true, WidthNumberOfProbe, WidthDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);
            FeedVisibilityProbe(C4, C3, true, true, WidthNumberOfProbe, WidthDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);
            FeedVisibilityProbe(C5, C6, true, true, WidthNumberOfProbe, WidthDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);
            FeedVisibilityProbe(C8, C7, true, true, WidthNumberOfProbe, WidthDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);


            FeedVisibilityProbe(C1, C4, false, false, HeightNumberOfProbe, HeightDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);
            FeedVisibilityProbe(C2, C3, false, false, HeightNumberOfProbe, HeightDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);
            FeedVisibilityProbe(C5, C8, false, false, HeightNumberOfProbe, HeightDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);
            FeedVisibilityProbe(C6, C7, false, false, HeightNumberOfProbe, HeightDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);


            FeedVisibilityProbe(C1, C5, false, false, DepthNumberOfProbe, DepthDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);
            FeedVisibilityProbe(C2, C6, false, false, DepthNumberOfProbe, DepthDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);
            FeedVisibilityProbe(C4, C8, false, false, DepthNumberOfProbe, DepthDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);
            FeedVisibilityProbe(C3, C7, false, false, DepthNumberOfProbe, DepthDistanceBetweenProbes, ref VisibilityProbe, ref probeCounter);

            return VisibilityProbe;
        }

        private static void CalculateProbeNumberAndIntervalFromDensity(float ComparedDistance, float Density, out int NumberOfProbes, out float DistanceBetweenProbes)
        {
            NumberOfProbes = Mathf.Max(Mathf.RoundToInt(Density / ComparedDistance), 2);
            DistanceBetweenProbes = ComparedDistance / (NumberOfProbes - 1);
        }

        private static void FeedVisibilityProbe(Vector3 StartPosition, Vector3 EndPosition,
            bool GenerateStart, bool GenerateEnd, int NumberOfProbes, float DistanceBetweenProbes, ref VisibilityProbe visibilityProbe, ref int CurrentProbeCounter)
        {
            if (GenerateStart)
            {
                visibilityProbe[CurrentProbeCounter] = StartPosition;
                CurrentProbeCounter += 1;
            }

            Vector3 OffsetDirectionNormalize = (EndPosition - StartPosition).normalized;

            for (int i = 1; i < NumberOfProbes; i++)
            {
                if (i == NumberOfProbes - 1 && !GenerateEnd)
                {
                    break;
                    ;
                }

                Vector3 probePosition;
                if (i == NumberOfProbes - 1)
                {
                    probePosition = EndPosition;
                }
                else
                {
                    probePosition = StartPosition + (OffsetDirectionNormalize * DistanceBetweenProbes * i);
                }

                visibilityProbe[CurrentProbeCounter] = probePosition;
                CurrentProbeCounter += 1;
            }
        }
    }
}