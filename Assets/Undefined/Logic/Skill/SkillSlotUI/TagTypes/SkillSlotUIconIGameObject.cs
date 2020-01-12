using UnityEngine;
using UnityEngine.UI;

namespace Skill
{
    public class SkillSlotUIconIGameObject : MonoBehaviour
    {
        private Image SkilIcon;

        public void Init()
        {
            this.SkilIcon = GetComponent<Image>();
        }
        
        public void SetUIIcon(Sprite Icon)
        {
            this.SkilIcon.sprite = Icon;
        }
    }
}