using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    [RequireComponent(typeof(Toggle))]
    public class SwitchImageToggleComponent : MonoBehaviour
    {
        private Toggle _toggle;
        private Graphic _bgImage;
        private Graphic _checkMarkImage;

        private void Start()
        {
            _toggle = GetComponent<Toggle>();
            _bgImage = _toggle.targetGraphic;
            _checkMarkImage = _toggle.graphic;
            _toggle.onValueChanged.AddListener(CheckIsOn);
            CheckIsOn(_toggle.isOn);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveAllListeners();
        }

        private void CheckIsOn(bool isOn)
        {
            if (isOn)
            {
                _bgImage.color = new Color(255, 255, 255, 0);
                return;
            }

            _bgImage.color = new Color(255, 255, 255, 255);
        }
    }
}