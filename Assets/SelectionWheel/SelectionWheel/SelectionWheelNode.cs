using System;
using UnityEngine;
using UnityEngine.UI;

namespace SelectionWheel
{
    public class SelectionWheelNode : MonoBehaviour
    {
        public const string COOLDWON_TIMER_PATTERN = "ss\\.ff";
        private const string COOLDOWN_OBJECT_NAME = "Cooldown";
        private const string REMAINING_EXECUTION_AMOUNT_OBJECT_NAME = "RemainingExecutionAmount";
        private const string DESCRIPTIOn_TEXT_OBJECT_NAME = "DescriptionText";

        private SelectionWheelNodeCooldownDisplayManager selectionWheelNodeCooldownDisplayManager;
        private SelectionWheelNodeRemainngExecutionAmoutDisaplyManager selectionWheelNodeRemainngExecutionAmoutDisaplyManager;

        #region Specific node data

        private SelectionWheelNodeData wheelNodeData;

        #endregion

        public float TargetWheelAngleDeg
        {
            get => targetWheelAngleDeg;
            set => targetWheelAngleDeg = value;
        }

        public float CurrentAngleDeg
        {
            get => currentAngleDeg;
            set => currentAngleDeg = value;
        }

        public Image ImageComponent
        {
            get => imageComponent;
            set => imageComponent = value;
        }

        public SelectionWheelNodeData WheelNodeData
        {
            get => wheelNodeData;
            set => wheelNodeData = value;
        }

        internal SelectionWheelNodeCooldownDisplayManager SelectionWheelNodeCooldownDisplayManager
        {
            get => selectionWheelNodeCooldownDisplayManager;
            set => selectionWheelNodeCooldownDisplayManager = value;
        }

        public static SelectionWheelNode Instantiate(SelectionWheelNodeData wheelNodeData)
        {
            var obj = Instantiate(SelectionWheelGlobalConfigurationGameObject.Get().SelectionWheelGlobalConfiguration.ActionWheelNodePrefab);
            var wheelActionNode = obj.GetComponent<SelectionWheelNode>();
            wheelActionNode.imageComponent = obj.GetComponent<Image>();
            wheelActionNode.descriptionText = obj.gameObject.FindChildObjectRecursively(DESCRIPTIOn_TEXT_OBJECT_NAME).GetComponent<Text>();
            wheelActionNode.descriptionText.text = wheelNodeData.NodeText;

            wheelActionNode.imageComponent.sprite = wheelNodeData.NodeSprite;
            wheelActionNode.WheelNodeData = wheelNodeData;

            wheelActionNode.selectionWheelNodeCooldownDisplayManager = new SelectionWheelNodeCooldownDisplayManager(wheelActionNode.gameObject.FindChildObjectRecursively(COOLDOWN_OBJECT_NAME), wheelActionNode);
            wheelActionNode.selectionWheelNodeRemainngExecutionAmoutDisaplyManager = new SelectionWheelNodeRemainngExecutionAmoutDisaplyManager(wheelActionNode.gameObject.FindChildObjectRecursively(REMAINING_EXECUTION_AMOUNT_OBJECT_NAME), wheelActionNode);

            return wheelActionNode;
        }

        public void Tick(float d)
        {
            selectionWheelNodeCooldownDisplayManager.Tick();
            selectionWheelNodeRemainngExecutionAmoutDisaplyManager.Tick();
        }

        public void SetMaterial(Material material)
        {
            imageComponent.material = material;
        }

        public void SetActiveText(bool value)
        {
            descriptionText.gameObject.SetActive(value);
        }

        #region Internal properties

        private float targetWheelAngleDeg;
        private float currentAngleDeg;
        private Image imageComponent;
        private Text descriptionText;

        #endregion
    }


    internal class SelectionWheelNodeCooldownDisplayManager
    {
        private GameObject cooldownObject;
        private Text cooldownText;
        private Color nodeDarkerImageColor;
        private Image nodeImage;

        private Color nodeInitialImageColor;
        private SelectionWheelNode wheelNodeRef;

        public SelectionWheelNodeCooldownDisplayManager(GameObject cooldownObject, SelectionWheelNode wheelNodeRef)
        {
            this.cooldownObject = cooldownObject;
            cooldownText = cooldownObject.GetComponent<Text>();
            this.wheelNodeRef = wheelNodeRef;
            nodeImage = wheelNodeRef.ImageComponent;
            nodeInitialImageColor = nodeImage.color;
            var darkenColorFactor = 0.8f;
            nodeDarkerImageColor = new Color(nodeInitialImageColor.r * darkenColorFactor, nodeInitialImageColor.g * darkenColorFactor, nodeInitialImageColor.b * darkenColorFactor);
        }

        public void Tick()
        {
            var isOnCooldown = wheelNodeRef.WheelNodeData.IsOnCoolDown;
            cooldownObject.SetActive(isOnCooldown);
            nodeImage.color = nodeInitialImageColor;
            if (isOnCooldown) cooldownText.text = TimeSpan.FromSeconds(wheelNodeRef.WheelNodeData.GetRemainingCooldownTime).ToString(SelectionWheelNode.COOLDWON_TIMER_PATTERN);

            if (!wheelNodeRef.WheelNodeData.CanNodeBeExecuted) nodeImage.color = nodeDarkerImageColor;
        }
    }

    internal class SelectionWheelNodeRemainngExecutionAmoutDisaplyManager
    {
        private Color nodeDarkerOutlineColor;

        private Color nodeInitialOutlineColor;
        private GameObject remainingAmountObject;
        private Outline remainingAmountOutlineText;
        private Text remainingAmountText;
        private SelectionWheelNode wheelNodeRef;

        public SelectionWheelNodeRemainngExecutionAmoutDisaplyManager(GameObject remainingAmountObject, SelectionWheelNode wheelNodeRef)
        {
            this.wheelNodeRef = wheelNodeRef;
            this.remainingAmountObject = remainingAmountObject;
            remainingAmountText = remainingAmountObject.GetComponent<Text>();
            remainingAmountOutlineText = remainingAmountText.GetComponent<Outline>();

            nodeInitialOutlineColor = remainingAmountOutlineText.effectColor;
            var darkenColorFactor = 0.8f;
            nodeDarkerOutlineColor = new Color(nodeInitialOutlineColor.r * darkenColorFactor, nodeInitialOutlineColor.g * darkenColorFactor, nodeInitialOutlineColor.b * darkenColorFactor);

            if (wheelNodeRef.WheelNodeData.GetRemainingExecutionAmount == -1)
            {
                remainingAmountText.text = '\u221E'.ToString();
                this.remainingAmountObject.SetActive(true);
            }
            else if (wheelNodeRef.WheelNodeData.GetRemainingExecutionAmount == -2)
            {
                remainingAmountText.text = "";
            }
            else
            {
                remainingAmountText.text = wheelNodeRef.WheelNodeData.GetRemainingExecutionAmount.ToString();
                this.remainingAmountObject.SetActive(true);
            }
        }

        public void Tick()
        {
            remainingAmountOutlineText.effectColor = nodeInitialOutlineColor;
            if (!wheelNodeRef.WheelNodeData.CanNodeBeExecuted) remainingAmountOutlineText.effectColor = nodeDarkerOutlineColor;
        }
    }

    public abstract class SelectionWheelNodeData
    {
        public abstract object Data { get; }
        public abstract Sprite NodeSprite { get; }
        public abstract bool CanNodeBeExecuted { get; }

        public abstract string NodeText { get; }

        #region

        public abstract int GetRemainingExecutionAmount { get; }

        #endregion

        #region Cooldown management

        public abstract bool IsOnCoolDown { get; }
        public abstract float GetRemainingCooldownTime { get; }

        #endregion
    }
}