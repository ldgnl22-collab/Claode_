using UnityEngine;
using UnityEngine.UI;

public class HarpoonMashGaugeView : MonoBehaviour
{
    [SerializeField] private PlayerHarpoonAttack _harpoonAttack;
    [SerializeField] private Slider _slider;

    private void Update()
    {
        // 슬라이더 오브젝트를 꺼도 갱신이 멈추지 않도록, 이 컴포넌트는 슬라이더 바깥(부모)에 둔다
        Harpoon harpoon = _harpoonAttack.Harpoon;
        bool isVisible = harpoon.IsStuck;

        if (_slider.gameObject.activeSelf != isVisible)
        {
            _slider.gameObject.SetActive(isVisible);
        }

        if (isVisible)
        {
            _slider.value = harpoon.MashGauge;
        }
    }
}
