using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerController : MonoBehaviour
{
    // Прилинковываем объекты со скриптами 
    [SerializeField] private Parameters parameters; // Общие параметры 
    [SerializeField] private ElementsMaterials elemMaterials; // Генерации словарей с ключами - название элемента
    [SerializeField] Image hpSquare; // Полоска здоровья
    [SerializeField] Canvas fieldCanvas; // Канвас для отрисовки полоски здоровья
    [SerializeField] private Transform crystal, woodPanels, banners; // Изменяемые объекты при смене стихии
    [SerializeField] private Transform bulletSpawner; // Изменяемые объекты при смене стихии
    [SerializeField] private GameObject projectile; // Объект снаряда из префабов
    [SerializeField] ParticleSystem towerCloud; // Визуализации стихии

    private Renderer crystalRend, woodPanelsRend, bannersRend;

    private float projFrequency, projDamage; // Скорость выстрелов, урон снарядов
    private float _timeToSpawn, _shootTime; // Время для выстрела, время между выстрелами

    private string curElement; // Текущий элемент стихии
    private float _HP, _maxHP; // Уровень здоровья 
    private bool _dead = false;


    void Start()
    {
        Initialize(); // Применяем стартовые настройки
    }

    void FixedUpdate()
    {
        ShotProjectiles(); // Создаем снаряды
    }

    // Инициализируем стартовые параметры
    private void Initialize()
    {
        // Стартовые параметры, здоровье
        _maxHP = parameters._towerMaxHP;
        _HP = _maxHP;

        // Получаем компоненты рендереров башни:
        crystalRend = crystal.GetComponent<Renderer>(); // Кристалл,
        woodPanelsRend = woodPanels.GetComponent<Renderer>(); // Деревянные панели,
        bannersRend = banners.GetComponent<Renderer>(); // Баннеры

        // Получаем свободный элемент башни и выставляем его. 
        string startElement = Field.GetFreeElemental();
        SetElement(startElement);
        SpawnTowerCloud(); // Создаем облако стихии

        ApplyProjectileParameters(); // Применяем параметры снарядов в зависимости от текущего элемента

        hpSquare = Instantiate(hpSquare, transform.position + new Vector3(-0.5f, 0.01f, -0.5f), Quaternion.identity, fieldCanvas.transform); // Создаем квадрат
        hpSquare.transform.localRotation = Quaternion.identity; // Поворачиваем его в плоскости канваса
        hpSquare.enabled = false; // Делаем невидимым
    }


    // Создаем снаряд с заданными параметрами, обновляем время между выстрелами
    private void ShotProjectiles()
    {
        if (Time.time > _timeToSpawn && IsLineOccupied()) // Если время игры больше времени для выстрела и линия не пустая
        {
            Instantiate(projectile, bulletSpawner.position, Quaternion.identity, transform); // Создаем снярад
            _timeToSpawn = Time.time + _shootTime; // Меняем время для следующего выстрела
        }
    }

    // Принимаем параметры снаряда в зависимости от элемента 
    void ApplyProjectileParameters()
    {
        projDamage = elemMaterials.GetBulletDamage(curElement); // Урон
        projFrequency = elemMaterials.GetBulletFreq(curElement); // Частота выстрелов
        _shootTime = 1 / projFrequency; // Время между выстрелами
        _timeToSpawn = Time.time + _shootTime / 3f; // Обновляем время для спавна нового снаряда
    }

    // Вовзращает истину, если линия атаки занята и можно стрелять
    private bool IsLineOccupied()
    {
        return Field.IsLineOccupied(Mathf.RoundToInt(transform.position.z));
    }

    // Создаем облако для визуализации стихии
    private void SpawnTowerCloud()
    {
        towerCloud = Instantiate(towerCloud, transform.position + new Vector3(0, 0.8f, 0), // Создаем эффект чуть выше центра башни
            Quaternion.Euler(270, 0, 0), transform).GetComponent<ParticleSystem>();
        var col = towerCloud.colorOverLifetime;
        col.color = elemMaterials.GetGradient(curElement); // Меняем цвет облака в зависимости от элемента
    }


    // Обновляем текущий элемент башни
    public void SetElement(string newElement)
    {
        // Обновляем текущюю стихию, получаем материал из имени стихии
        curElement = newElement;
        Material material = elemMaterials.GetMaterial(curElement);

        // Применяем материал к кристалу, баннерам и деревянным панелям
        crystalRend.material = material;
        bannersRend.material = material;
        woodPanelsRend.material = material;

        // Меняем цвет облака стихии в зависимости от элемента
        var col = towerCloud.colorOverLifetime;
        col.color = elemMaterials.GetGradient(curElement);

        // Применяем параметры снаряда в зависимости от нового элемента
        ApplyProjectileParameters();
    }

    // Получение урона от любого источника
    public void TakeDamage(float damage)
    {
        _HP -= damage; // Отнимаем получанный урон
        hpSquare.fillAmount = _HP / _maxHP; // Меняем заполненность полоски здоровья

        if (_HP < 0 && !_dead) // Действия в случае, если закончилось здоровье
        {
            _dead = true; // Чтобы дальнейшие инструкции выполнились только один раз
            Field.aliveTowers[Mathf.RoundToInt(transform.position.z)] = false; // Удаляем башню из списка живых башен, удаляя линию элемов

            Destroy(this.gameObject); // Уничтожаем башню как объект
            Destroy(hpSquare.gameObject); // Уничтожаем полоску здоровья
        }
    }

    public void SetHpBarVisible() { hpSquare.enabled = true; }

    public float GetProjDamage() { return projDamage; }

    public string GetProjElement() { return curElement; }

    public Material GetProjMaterial() { return crystalRend.material; }

    public Parameters GetMainParameters() { return parameters; }

    public GameObject GetSmoke(string element) { return elemMaterials.GetSmoke(element); }
}
