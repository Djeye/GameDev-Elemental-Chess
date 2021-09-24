using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    private const float Y_OFFSET = 0.5f; // Оффсет для "полета" кристалла над полем
    // Прилинкованные объекты
    [SerializeField] Parameters parameters; // Параметры
    [SerializeField] LayerMask groundLayerMask; // Слой в эдиторе, навешенный на невидимую плоскость, вдоль которой происходит перемещение
    [SerializeField] private string element; // Элемент стихии кристалла
    [SerializeField] private ManaBar manaBar; // Мана Бар светлой стороны

    float manaCost;     // Стоимость для смены стихии
    private Vector3 startPos; // Стартовая позиция, для возвращения и спавна нового кристалла
    private bool availableToChange = false; // Возможно ли сменить стихию
    private Outline towerOutline; // Обводка башни при наведении кристалла на нее
    private TowerController tower; // Башня для смены стихии

    void Start()
    {
        Initialize();
    }

    // Инициализация начальных параметров
    private void Initialize()
    {
        startPos = transform.position; // Запоминаем изначальное положение
        manaCost = parameters._crystalCost; // Стоимость выставления кристалла
    }

    // Перемещение кристалла Drag'n'Drop-ом
    private void OnMouseDrag()
    {
        if (manaBar.IsEnoughMana(manaCost)) // Возможно только если достаточно маны
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Каст луча от позиции мыши на экране
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20f, groundLayerMask)) // Если каст луча попажает в плоскость в пределах 20 единиц
            {
                transform.position = new Vector3(Mathf.Clamp(hit.point.x, -0.3f, 7.3f), Y_OFFSET, // Перемещаем кристалл внутри поля
                                                 Mathf.Clamp(hit.point.z, -1.3f, 4.3f));
            }

            manaBar.ShowPreview(manaCost); // Визуализируем сколько потратим маны
        }
    }

    // Действия при отпускании мыши/пальца
    private void OnMouseUp()
    {
        if (availableToChange)
        {
            tower.SetElement(element); // Выставляем новый элемент для башни
            manaBar.SpendMana(manaCost); // Тратим ману
        }
        else
        {
            manaBar.ReturnPreview(); // Превью маны = уровню маны
        }

        transform.position = startPos; // В любом случае кристалл возвращается на свое место
    }


    // Действия при нахождении кристалла внутри других объектов
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tower")) // Если объект - башня
        {
            tower = other.GetComponent<TowerController>(); // Храним башню, стихию которой сможем поменять
            bool isSameElement = tower.GetProjElement().Equals(element); // Истина, если стихия башни и кристала совпадают
            float dstBetweenTowerAndCrystal = Mathf.Abs(other.transform.position.z - transform.position.z); // Расстояние от кристала до башни
            towerOutline = other.GetComponent<Outline>(); // Подсветка для башни

            availableToChange = !isSameElement && dstBetweenTowerAndCrystal < 0.5f; // Возможно поменять стихию, если элемент башни другой, и она ближайшая для кристалла
            towerOutline.enabled = availableToChange; // Подсветка зависит от возможности поменять стихию
        }
    }

    // Действия при покидании другого объекта
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tower")) // Если объект - башня
        {
            other.GetComponent<Outline>().enabled = false; // Отключаем подсветку
            availableToChange = false; // Невозможно применить стихию
        }
    }
}
