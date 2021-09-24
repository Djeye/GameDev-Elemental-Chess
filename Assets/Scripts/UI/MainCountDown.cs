using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainCountDown : MonoBehaviour
{
    [SerializeField] Parameters parameters; // Основные параметры
    [SerializeField] TextMeshProUGUI timerText; // Текст таймера
    [SerializeField] Slider slider; // Компонент слайдера. для заполнения
    [SerializeField] Image fillImage; // Изображение, цвет которого меняем
    [SerializeField] Gradient gradient; // Задаем градиент - цвета которые будут меняться при смене времени
    //[SerializeField] int _numSectors;

    private float startTime;
    private float curTime, min, sec;
    //int sectorTime;

    void Start()
    {
        startTime = parameters._allTime; // Получаем общее время боя
        curTime = startTime; // Устанавливаем текущее время - на время начала матча

        slider.maxValue = startTime; // Меняем параметры слайдера
        slider.value = slider.maxValue; // Меняем текущее значение на максимальное

        //sectorTime = Mathf.RoundToInt(startTime / _numSectors);
    }

    void Update()
    {
        if (!Field.endOfGame) curTime -= 1 * Time.deltaTime; // Считаем время до конца боя
        if (curTime <= 0) // Кончилось время
        {
            curTime = 0; // Больше не меняем
            Field.endOfGame = true; // Флаг конца игры
        }
        else
        {
            // Тупая попытка визуализировать время секторами
            //int stepValue = (int)curTime / sectorTime * (int)startTime / _numSectors;

            slider.value = Mathf.FloorToInt(curTime); // Отображаем время боя как шкалу здоровья
            fillImage.color = gradient.Evaluate(1 - slider.normalizedValue); // Меняем цвет заднего фона
        }


        min = Mathf.FloorToInt(curTime / 60); // Получаем минуты для визуализации времени боя
        sec = Mathf.FloorToInt(curTime % 60); // Считаем секунды боя. Округление в нижнюю сторону
        timerText.text = string.Format("{0:0}:{1:00}", min, sec); // Отображаем время боя в нужном формате
    }

    // Возвращаем текущее время боя
    public float GetCurTime() { return curTime; }

    // Возвращает общее время матча
    public float GetStartTime() { return startTime; }
}
