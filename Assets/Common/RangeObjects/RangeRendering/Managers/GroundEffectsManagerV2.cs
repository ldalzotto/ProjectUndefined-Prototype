using System;
using System.Collections.Generic;
using System.Linq;
using CoreGame;
using GeometryIntersection;
using LevelManagement_Interfaces;
using Obstacle;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace RangeObjects
{
    public class GroundEffectsManagerV2 : GameSingleton<GroundEffectsManagerV2>
    {
        private List<GroundEffectType> AffectedGroundEffectsType;

        #region External Dependencies

        private ObstaclesListenerManager ObstaclesListenerManager = ObstaclesListenerManager.Get();

        #endregion

        private List<RangeTypeID> rangeEffectRenderOrder = new List<RangeTypeID>()
        {
            RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
            RangeTypeID.ATTRACTIVE_OBJECT,
            RangeTypeID.SIGHT_VISION,
            RangeTypeID.LAUNCH_PROJECTILE,
            RangeTypeID.LAUNCH_PROJECTILE_CURSOR,
            RangeTypeID.TARGET_ZONE
        };

        private Dictionary<RangeTypeID, Dictionary<int, AbstractRangeRenderData>> rangeRenderDatas;

        #region Command Buffers

        public CommandBuffer RangeDrawCommand { get; private set; }

        #endregion

#if UNITY_EDITOR
        public Dictionary<RangeTypeID, Dictionary<int, AbstractRangeRenderData>> RangeRenderDatas => rangeRenderDatas;
#endif

        public void InitializeEvents()
        {
            rangeRenderDatas = new Dictionary<RangeTypeID, Dictionary<int, AbstractRangeRenderData>>();
            RangeTypeConfiguration = RangeTypeConfigurationGameObject.Get().RangeRenderingConfiguration.RangeTypeConfiguration;
            MasterRangeMaterial = new Material(RangeTypeConfigurationGameObject.Get().RangeRenderingConfiguration.MasterRangeShader);
            RangeRenderingConfiguration = RangeTypeConfigurationGameObject.Get().RangeRenderingConfiguration;

            #region Event Registering

            RangeEventsManager.Get().RegisterOnRangeObjectCreatedEventListener(OnRangeObjectCreated);
            RangeEventsManager.Get().RegisterOnRangeObjectDestroyedEventListener(OnRangeObjectDestroyed);

            #endregion
        }

        public void Init(LevelRangeEffectInherentData LevelRangeEffectInherentData)
        {
          
            RangeDrawCommand = new CommandBuffer();
            RangeDrawCommand.name = GetType().Name + "." + nameof(RangeDrawCommand);

            AffectedGroundEffectsType = GameObject.FindObjectsOfType<GroundEffectType>().ToList();
            foreach (var affectedGroundEffectType in AffectedGroundEffectsType) affectedGroundEffectType.Init();


            //Do static batching of ground effects types
            StaticBatchingUtility.Combine(
                AffectedGroundEffectsType.ConvertAll(groundEffectType => groundEffectType.gameObject)
                    .Union(AffectedGroundEffectsType.ConvertAll(groundEffectType => groundEffectType.AssociatedGroundEffectIgnoredGroundObjectType).SelectMany(s => s).ToList().ConvertAll(groundEffectIgnoredObject => groundEffectIgnoredObject.gameObject))
                    .ToArray(), GameObject.FindWithTag(TagConstants.ROOT_CHUNK_ENVIRONMENT));

            //master range shader color level adjuster
            MasterRangeMaterial.SetFloat("_AlbedoBoost", 1f + LevelRangeEffectInherentData.DeltaIntensity);
            MasterRangeMaterial.SetFloat("_RangeMixFactor", 0.5f + LevelRangeEffectInherentData.DeltaMixFactor);
        }

        public void Tick(float d)
        {
            if (RangeRenderingConfiguration.IsRangeRenderingEnabled)
            {
                Profiler.BeginSample("GroundEffectsManagerV2Tick");
                ForEachRangeRenderData((rangeRenderData) => { rangeRenderData.Tick(d, AffectedGroundEffectsType); });

                #region Buffer data set

                OnCommandBufferUpdate();

                #endregion

                Profiler.EndSample();
            }
            else
            {
                RangeDrawCommand.Clear();
            }
        }


        private void ForEachRangeRenderData(Action<AbstractRangeRenderData> action)
        {
            if (rangeRenderDatas != null)
                foreach (var rangeRenderDatasByCollider in rangeRenderDatas.Values)
                foreach (var rangeRenderData in rangeRenderDatasByCollider.Values)
                    if (rangeRenderData != null)
                        action.Invoke(rangeRenderData);
        }

        private void OnCommandBufferUpdate()
        {
            RangeDrawCommand.Clear();
            RangeDrawCommand.BeginSample("rangeDrawCommand");
            ForEachRangeRenderData((rangeRenderData) => { rangeRenderData.ProcessCommandBuffer(RangeDrawCommand, ref AffectedGroundEffectsType, MasterRangeMaterial); });
            RangeDrawCommand.EndSample("rangeDrawCommand");
        }

        #region Configurations

        private RangeTypeConfiguration RangeTypeConfiguration;
        private Material MasterRangeMaterial;
        private RangeRenderingConfiguration RangeRenderingConfiguration;

        #endregion

        #region External events

        private void OnRangeObjectCreated(RangeObjectV2 RangeObjectV2)
        {
            var rangeTypeID = RangeObjectV2.RangeObjectInitialization.RangeTypeID;
            if (rangeTypeID != RangeTypeID.NOT_DISPLAYED)
            {
                if (!rangeRenderDatas.ContainsKey(rangeTypeID)) rangeRenderDatas[rangeTypeID] = new Dictionary<int, AbstractRangeRenderData>();

                if (RangeObjectV2.GetType() == typeof(SphereRangeObjectV2))
                {
                    var SphereRangeObjectRenderingDataProvider = new SphereRangeObjectRenderingDataProvider((SphereRangeObjectV2) RangeObjectV2, rangeTypeID);
                    var addedRange = new SphereGroundEffectManager(RangeTypeConfiguration.ConfigurationInherentData[rangeTypeID], SphereRangeObjectRenderingDataProvider);
                    addedRange.OnRangeCreated(SphereRangeObjectRenderingDataProvider);
                    rangeRenderDatas[rangeTypeID].Add(SphereRangeObjectRenderingDataProvider.BoundingCollider.GetInstanceID(), new CircleRangeRenderData(addedRange));
                }
                else if (RangeObjectV2.GetType() == typeof(BoxRangeObjectV2))
                {
                    var BoxRangeObjectRenderingDataProvider = new BoxRangeObjectRenderingDataProvider((BoxRangeObjectV2) RangeObjectV2, rangeTypeID);
                    var addedRange = new BoxGroundEffectManager(RangeTypeConfiguration.ConfigurationInherentData[rangeTypeID], BoxRangeObjectRenderingDataProvider);
                    addedRange.OnRangeCreated(BoxRangeObjectRenderingDataProvider);
                    rangeRenderDatas[rangeTypeID].Add(BoxRangeObjectRenderingDataProvider.BoundingCollider.GetInstanceID(), new BoxRangeRenderData(addedRange));
                }
                else if (RangeObjectV2.GetType() == typeof(RoundedFrustumRangeObjectV2))
                {
                    var RoundedFrustumRangeObjectRenderingDataProvider = new FrustumRangeObjectRenderingDataProvider((RoundedFrustumRangeObjectV2) RangeObjectV2, rangeTypeID);
                    var addedRange = new RoundedFrustumGroundEffectManager(RangeTypeConfiguration.ConfigurationInherentData[rangeTypeID], RoundedFrustumRangeObjectRenderingDataProvider);
                    addedRange.OnRangeCreated(RoundedFrustumRangeObjectRenderingDataProvider);
                    rangeRenderDatas[rangeTypeID].Add(RoundedFrustumRangeObjectRenderingDataProvider.BoundingCollider.GetInstanceID(), new RoundedFrustumRenderData(addedRange));
                }
            }
        }

        private void OnRangeObjectDestroyed(RangeObjectV2 RangeObjectV2)
        {
            if (RangeObjectV2.RangeObjectInitialization.RangeTypeID != RangeTypeID.NOT_DISPLAYED)
            {
                rangeRenderDatas[RangeObjectV2.RangeObjectInitialization.RangeTypeID][RangeObjectV2.RangeGameObjectV2.BoundingCollider.GetInstanceID()].OnRangeDestroyed();
                rangeRenderDatas[RangeObjectV2.RangeObjectInitialization.RangeTypeID].Remove(RangeObjectV2.RangeGameObjectV2.BoundingCollider.GetInstanceID());
                if (rangeRenderDatas[RangeObjectV2.RangeObjectInitialization.RangeTypeID].Count == 0) rangeRenderDatas.Remove(RangeObjectV2.RangeObjectInitialization.RangeTypeID);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ForEachRangeRenderData((rangeRenderData) => { rangeRenderData.Dispose(); });
        }

        #endregion
    }

    public abstract class AbstractRangeRenderData
    {
        private Mesh combinedMesh;
        protected IAbstractGroundEffectManager GroundEffectManager;
        protected MaterialPropertyBlock matPro;

        private DynamicComputeBufferManager<FrustumPointsPositions> obstacleFrustumBuffer;

        protected AbstractRangeRenderData()
        {
            matPro = new MaterialPropertyBlock();
            obstacleFrustumBuffer = new DynamicComputeBufferManager<FrustumPointsPositions>(FrustumPointsPositions.GetByteSize(), "FrustumBufferDataBuffer", "_FrustumBufferDataBufferCount", matPro);
        }

#if UNITY_EDITOR
        public DynamicComputeBufferManager<FrustumPointsPositions> ObstacleFrustumBuffer => obstacleFrustumBuffer;
#endif

        public abstract void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial);

        public virtual void Dispose()
        {
            obstacleFrustumBuffer.Dispose();
        }

        protected Mesh CombineMesh(ref List<GroundEffectType> AffectedGroundEffectsType)
        {
            if (GroundEffectManager.MeshMustBeRebuild() || combinedMesh == null)
            {
                combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(
                    GroundEffectManager.GroundEffectTypeToRender().ConvertAll(groundEffectType => new CombineInstance() {mesh = groundEffectType.GroundEffectMesh, transform = groundEffectType.transform.localToWorldMatrix}).ToArray(),
                    true);
            }

            return combinedMesh;
        }

        protected void ProcessObstacleFrustums(CommandBuffer commandBuffer, MaterialPropertyBlock matPro)
        {
            var rangeObjectListener = GroundEffectManager.GetObstacleListener();
            if (rangeObjectListener != null)
                obstacleFrustumBuffer.Tick((bufferData) => { rangeObjectListener.ForEachCalculatedFrustum((calculatedFrustums) => { bufferData.Add(calculatedFrustums); }); });
        }

        public void Tick(float d, List<GroundEffectType> affectedGroundEffectsType)
        {
            GroundEffectManager.Tick(d, affectedGroundEffectsType);
        }

        public void OnRangeDestroyed()
        {
            Dispose();
        }
    }

    public class CircleRangeRenderData : AbstractRangeRenderData
    {
        private DynamicComputeBufferManager<CircleRangeBufferData> circleRangeBuffer;

        public CircleRangeRenderData(SphereGroundEffectManager sphereGroundEffectManager) : base()
        {
            GroundEffectManager = sphereGroundEffectManager;
            circleRangeBuffer = new DynamicComputeBufferManager<CircleRangeBufferData>(CircleRangeBufferData.GetByteSize(), "CircleRangeBuffer", string.Empty, matPro);
        }

        public DynamicComputeBufferManager<CircleRangeBufferData> CircleRangeBuffer => circleRangeBuffer;

        public override void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial)
        {
            var combinedMesh = CombineMesh(ref AffectedGroundEffectsType);
            circleRangeBuffer.Tick((bufferData) => bufferData.Add(((SphereGroundEffectManager) GroundEffectManager).ToSphereBuffer()));
            ProcessObstacleFrustums(commandBuffer, matPro);
            commandBuffer.DrawMesh(combinedMesh, Matrix4x4.identity, MasterRangeMaterial, 0, 0, matPro);
        }

        public override void Dispose()
        {
            base.Dispose();
            circleRangeBuffer.Dispose();
        }
    }

    public class RoundedFrustumRenderData : AbstractRangeRenderData
    {
        private DynamicComputeBufferManager<RoundedFrustumRangeBufferData> roundedFrustumRangeBuffer;

        public RoundedFrustumRenderData(RoundedFrustumGroundEffectManager roundedFrustumGroundEffectManager) : base()
        {
            GroundEffectManager = roundedFrustumGroundEffectManager;
            roundedFrustumRangeBuffer = new DynamicComputeBufferManager<RoundedFrustumRangeBufferData>(RoundedFrustumRangeBufferData.GetByteSize(), "RoundedFrustumRangeBuffer", string.Empty, matPro);
        }

        public DynamicComputeBufferManager<RoundedFrustumRangeBufferData> RoundedFrustumRangeBuffer => roundedFrustumRangeBuffer;

        public override void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial)
        {
            var combinedMesh = CombineMesh(ref AffectedGroundEffectsType);

            roundedFrustumRangeBuffer.Tick((bufferData) => bufferData.Add(((RoundedFrustumGroundEffectManager) GroundEffectManager).ToFrustumBuffer()));
            ProcessObstacleFrustums(commandBuffer, matPro);
            commandBuffer.DrawMesh(combinedMesh, Matrix4x4.identity, MasterRangeMaterial, 0, 3, matPro);
        }

        public override void Dispose()
        {
            base.Dispose();
            roundedFrustumRangeBuffer.Dispose();
        }
    }

    public class BoxRangeRenderData : AbstractRangeRenderData
    {
        private DynamicComputeBufferManager<BoxRangeBufferData> BoxRangeBuffer;

        public BoxRangeRenderData(BoxGroundEffectManager BoxGroundEffectManager) : base()
        {
            GroundEffectManager = BoxGroundEffectManager;
            BoxRangeBuffer = new DynamicComputeBufferManager<BoxRangeBufferData>(BoxRangeBufferData.GetByteSize(), "BoxRangeBuffer", string.Empty, matPro);
        }

        public override void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial)
        {
            var combinedMesh = CombineMesh(ref AffectedGroundEffectsType);
            BoxRangeBuffer.Tick((bufferData) => bufferData.Add(((BoxGroundEffectManager) GroundEffectManager).ToBoxBuffer()));
            ProcessObstacleFrustums(commandBuffer, matPro);
            commandBuffer.DrawMesh(combinedMesh, Matrix4x4.identity, MasterRangeMaterial, 0, 1, matPro);
        }

        public override void Dispose()
        {
            base.Dispose();
            BoxRangeBuffer.Dispose();
        }
    }
}