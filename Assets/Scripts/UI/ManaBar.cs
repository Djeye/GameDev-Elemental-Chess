using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Parameters parameters; // Основные параметры
    [SerializeField] private Image preview; // Прилинковываем изображение превью манакоста
    [SerializeField] private bool isDarkManaBar = false; // Принадлежит ли полоска маны темным силам

    private Slider slider; // Манабар - слайдер
    private float maxMana; // Значения из параметров
    private float manaSpeed; // Значения из параметров

    private float fadeSpeed = 0.05f; // Скорость убывания маны

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        slider = gameObject.GetComponent<Slider>();

        maxMana = parameters._maxMana; // Получаем начальные значения
        manaSpeed = parameters._manaGrowSpeed;

        slider.maxValue = maxMana; // Меняем параметры слайдера
        // Текущий уровень - максимальный 
        slider.value = isDarkManaBar ? parameters._darkStartMana : slider.maxValue;

        ReturnPreview(); // Заполняем полностью превью маны
    }

    // С начала матча запускаем корутину, вечно восполняющую ману
    private void Start()
    {
        if (!isDarkManaBar) StartCoroutine(manaGrow()); // Только для светлой стороны
    }

    // Корутина, плавно отнимающая ману до нужного значения
    public void SpendMana(float cost)
    {
        StartCoroutine(ManaFade(cost, slider.value));
    }

    // Возвращает истину, если маны достаточно для нужного действия
    public bool IsEnoughMana(float cost)
    {
        return slider.value - cost >= 0;
    }

    // Возвращает максимальное значение маны
    public float getMaxMana() { return maxMana; }

    // Возвращает текущее значение маны
    public float getSliderValue() { return slider.value; }

    // Корутина, плавно отнимающая ману
    IEnumerator ManaFade(float cost, float curMana)
    {
        while ((slider.value > curMana - cost && cost > 0) || (slider.value < curMana - cost && cost < 0)) // Пока текущее значение маны не равно необходиму
        {
            slider.value -= cost * fadeSpeed; // отнимаем ману с какой-то скоростью
            yield return new WaitForSeconds(fadeSpeed * .1f); // Ждем какое-то время, чотбы мана отнималась плавно
        }

        ReturnPreview(); // В конце корутины , превью становится равен количеству маны
    }

    // Корутина, вечно пополняющая ману
    IEnumerator manaGrow()
    {
        // Значение, на которое мана будет увеличиваться со временем
        float manaGrow = 0.01f * manaSpeed;
        while (true && !Field.endOfGame) // Работает всю игру, пока она не закончится
        {
            // Если мана не заполнена полностью, увеличиваем значения маны и превью, ограничив их сверху и снизу
            if (slider.value != slider.maxValue) preview.fillAmount = Mathf.Clamp(preview.fillAmount + manaGrow, 0f, 1f);
            slider.value = Mathf.Clamp(slider.value + manaGrow * slider.maxValue, slider.minValue, slider.maxValue);

            yield return new WaitForSeconds(.5f);
        }
    }

    // Устанавливаем значение превью, чтобы показать, сколько отнимется в случае действия
    public void ShowPreview(float cost)
    {
        preview.fillAmount = (slider.value - cost) / slider.maxValue;
    }

    // Устанавливаем превью равное текущему значению маны
    public void ReturnPreview()
    {
        preview.fillAmount = slider.value / slider.maxValue;
    }
}
