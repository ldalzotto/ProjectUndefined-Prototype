using CoreGame;
using UnityEngine;

namespace VisualFeedback
{
    public class DottedLineManager : GameSingleton<DottedLineManager>
    {
        public GameObject DottedLineContainer { get; private set; }

        public DottedLineManager()
        {
            this.DottedLineContainer = new GameObject();
            this.DottedLineContainer.transform.position = Vector3.zero;
            this.DottedLineContainer.transform.rotation = Quaternion.identity;
            this.DottedLineContainer.transform.localScale = Vector3.one;
        }
    }
}