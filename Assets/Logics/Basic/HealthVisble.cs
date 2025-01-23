
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider _HealthSlider; // ������ �� ������� ��������
    [SerializeField] private Humanoid _Humanoid; // ������ �� ������ Humanoid
    [SerializeField] private float animationDuration = 0.5f; // ������������ ��������

    private Coroutine healthCoroutine;

    private void Start()
    {
        // �������������� ������� ����������
        UpdateHealthUI();
        Debug.Log("Started");
        // ������������� �� ������� ��������� ��������
        _Humanoid.OnTakeDamaged += UpdateHealthUI;
        _Humanoid.OnMaxHealthChanged += UpdateHealthUI;
}

    private void UpdateHealthUI()
    {
        // ���� �������� ��� �����������, ��������� �
        if (healthCoroutine != null)
        {
            StopCoroutine(healthCoroutine);
        }
        Debug.Log("Channger");
        // ��������� ����� �������� ��� �������� ���������
        healthCoroutine = StartCoroutine(AnimateHealthChange(_HealthSlider.value, _Humanoid.Health));
    }

    private IEnumerator AnimateHealthChange(float startValue, float endValue)
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // �������� ������������ ��������
            _HealthSlider.value = Mathf.Lerp(startValue, endValue, t);
            yield return null; // ���� ��������� ����
        }

        // ������������� �������� ��������
        _HealthSlider.value = endValue;
    }

    private void OnDestroy()
    {
        // ������������ �� ������� ��� ����������� �������
        _Humanoid.OnTakeDamaged -= UpdateHealthUI;
        _Humanoid.OnMaxHealthChanged -= UpdateHealthUI;
    }
}
