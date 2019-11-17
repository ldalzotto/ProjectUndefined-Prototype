using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    public class HealthUIFullBarType : MonoBehaviour
    {
        public void SetHealth01(float health01)
        {
            var localScale = this.gameObject.transform.localScale;
            this.gameObject.transform.localScale = new Vector3(health01, localScale.y, localScale.z);
        }
    }
}