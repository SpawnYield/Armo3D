
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider _HealthSlider; // Ссылка на слайдер здоровья
    [SerializeField] private Humanoid _Humanoid; // Ссылка на объект Humanoid
    [SerializeField] private float animationDuration = 0.5f; // Длительность анимации

    private Coroutine healthCoroutine;

    private void Start()
    {
        // Инициализируем слайдер значениями
        UpdateHealthUI();
        Debug.Log("Started");
        // Подписываемся на событие изменения здоровья
        _Humanoid.OnTakeDamaged += UpdateHealthUI;
        _Humanoid.OnMaxHealthChanged += UpdateHealthUI;
}

    private void UpdateHealthUI()
    {
        // Если корутина уже выполняется, прерываем её
        if (healthCoroutine != null)
        {
            StopCoroutine(healthCoroutine);
        }
        Debug.Log("Channger");
        // Запускаем новую корутину для плавного изменения
        healthCoroutine = StartCoroutine(AnimateHealthChange(_HealthSlider.value, _Humanoid.Health));
    }

    private IEnumerator AnimateHealthChange(float startValue, float endValue)
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // Линейная интерполяция значения
            _HealthSlider.value = Mathf.Lerp(startValue, endValue, t);
            yield return null; // Ждем следующий кадр
        }

        // Устанавливаем конечное значение
        _HealthSlider.value = endValue;
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        _Humanoid.OnTakeDamaged -= UpdateHealthUI;
        _Humanoid.OnMaxHealthChanged -= UpdateHealthUI;
    }
}
