using UnityEngine;

namespace Skill
{
    public class SkillSlitUIInputTextGameObject : MonoBehaviour
    {
        private TMPro.TextMeshProUGUI InputText;

        public void Init()
        {
            this.InputText = this.GetComponent<TMPro.TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            this.InputText.text = text;
        }
    }
}