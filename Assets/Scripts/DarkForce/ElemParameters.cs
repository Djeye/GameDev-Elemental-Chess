using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElemParameters : MonoBehaviour
{

    [SerializeField] public Parameters parameters; // Основные параметры
    [SerializeField] public ManaBar manaBar; // Полоска маны
    [SerializeField] public Canvas _fieldCanvas; // Плоскость отрисовки полоски здоровья и квадратов
    [SerializeField] public LayerMask groundLayerMask; // Слой, по которому будет происходить Drag'n'Drop
    [SerializeField] public Image _fieldSquare; // Квадратик, зеленый или красный, отображающий возможность поставить элема
    [SerializeField] public Image _hpBar; // Полоска  здоровья, в той же плоскость что и квадратик выше
    [SerializeField] TextMeshProUGUI counterText;

    [Header("Element Effects")]
    [SerializeField] public GameObject _fireEffect; // Сплэш огня
    [SerializeField] public GameObject _waterEffect; // Сплэш воды 
    [SerializeField] public GameObject _woodEffect; // Облако от стана дерева
    [SerializeField] public GameObject _earthEffect; // Облако от замедления земли
    [SerializeField] public GameObject _metalEffect; // Пронизывающий эффект металла

    // Связный список имеет Next и Previous для идеального поиска зависимости между стихиями
    private LinkedList<string> elements = new LinkedList<string>(); // Да, связный список, пригодился xD
    private int woodCounter = 0, maxHits;


    void Awake()
    {
        elements.AddLast("fire");  // Добавляем 5 стихий в связный список дважды, чтобы Next на "последний" элемент не возвращал ошибку
        elements.AddLast("metal"); // Всяко быстрее, чем проверять наличие следующих или предыдущих элементов
        elements.AddLast("wood");  // С# не закцикливает связный список
        elements.AddLast("earth");
        elements.AddLast("water");
        elements.AddLast("fire");
        elements.AddLast("metal");
        elements.AddLast("wood");  // Худший случай - искать Третий следующий после первой Воды

        maxHits = parameters._woodHitsForRoute; // Получаем максимальное число для рута из параметров
    }

    // Возвращает коэффициент при получении урона для элема со "стихией"(element) от снаряда со "стихией"(projElement)
    public float GetDamageCoef(string element, string projElement)
    {
        float damageCoef = parameters._defDamageCoef; // Дефолтный коэффициент урона
        if (element.Equals(projElement)) return parameters._minDamageCoef; // Одинаковые стихии = минимальный урон
        if (elements.Find(projElement).Next.Value.Equals(element)) damageCoef = parameters._maxDamageCoef; // Стихия побеждает - повышенный урон
        if (elements.Find(projElement).Next.Next.Next.Value.Equals(element)) damageCoef = parameters._reducedDamageCoef; // Стихия проигрывает - пониженный урон

        return damageCoef;
    }

    // Увеличиваем число попаданий, и отображаем на экране
    public void IncreaseWoodHits()
    {
        woodCounter = (woodCounter + 1) % maxHits; // Увеличиваем счетчик
        counterText.text = woodCounter.ToString(); // Приводим к строке, отображаем значение

        counterText.color = IsLastWoodHit() ? Color.red : Color.white; // Окрашиваем в красный, если следующий выстрел - последний 
    }

    // Возвращает истину, если следующий выстрел - последний
    public bool IsLastWoodHit() { return woodCounter + 1 == maxHits; }
}
