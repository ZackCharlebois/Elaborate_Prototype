using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(Animator))]
public class ValueBar : MonoBehaviour
{
    [SerializeField] private float showDelay = 2f;
    private Slider _slider;
    private Animator _animator;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.maxValue = 10;
        _slider.value = 10;
        
        _animator = GetComponent<Animator>();
        _animator.Play("hide");
    }

    public void SetMax(int max, int startValue)
    {
        _slider.maxValue = max;
        _slider.value = startValue;
    }

    public void Set(int value, bool showChange = true)
    {
        _slider.value = value;

        if (!showChange) return;
        _animator.Play("show");
        Invoke(nameof(Hide), showDelay);
    }

    private void LateUpdate()
    {
        //keep us the same orientation when parent reverses scale
        transform.localScale = transform.parent.transform.localScale;
    }

    public void SetToMax()
    {
        _slider.value = _slider.maxValue;
    }

    private void Hide()
    {
        _animator.Play("hide");
    }
}
