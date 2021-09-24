using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElemMotion : MonoBehaviour
{
    private const float DESK_OFFSET = 0.05f; // Оффсет по вертикальной координате, чтобы элемы ходили по клеткам
    [SerializeField] private ElemParameters elemParameters; // Зависимости между стихиями, визуальные эффекты

    [Header("Changable Parameters")]
    [SerializeField] float _speed; // Скорость и максимальное здоровье
    [SerializeField] float _maxHp; // Скорость и максимальное здоровье
    [SerializeField] float _attackDamage, _attackFreq; // Урон от атаки, и скорость атаки
    [SerializeField] private string _element; // Стихия
    [SerializeField] private float manaCost; // Стоимость элема 
    [SerializeField] private bool _isGhost = false; // Истина, если элем - призрак

    // Параметры, получаемые из ElemParameters
    private Parameters parameters; // Основные параметры
    private ManaBar manaBar; // Полоска маны
    private Canvas _fieldCanvas; // Плоскость отрисовки полоски здоровья и квадратов
    LayerMask groundLayerMask; // Слой, по которому будет происходить Drag'n'Drop
    private Image _fieldSquare; // Квадратик, зеленый или красный, отображающий возможность поставить элема
    private Image _hpBar; // Полоска  здоровья, в той же плоскость что и квадратик выше

    private Vector3 startPos, roundedPosition; // Начальная позиция, и позиция внутри клетки (округленная)
    private float minZ = -0.5f, maxZ = 4.5f, minX = -0.5f, maxX = 7.5f; // Крайние стороны поля

    private bool enteredInGame = false, posibleToPlace = false, enterTower = false; // Введен ли в игру, возможно ли выставить, вошел ли в башню
    private float lerpSpeed = 3f, defaultSpeed = 0.35f; // Скорость визуализации здоровья, нормализирущий параметр скорости
    private float hp, startSpeed;  // Уровень здоровья, начальная скорость
    private float earthSlowTime; // Время замедления от земли

    private Animator animator; // Компонент аниматора смены флагов анимаций 
    private CapsuleCollider capsCol; // Компонент коллайдера
    private GameObject splash, earthSlow; // Визаульные объекты, с которыми придется взаимодйствовать 

    // Вызывается при первоначальной инициализации до вызова Start
    private void Awake()
    {
        // Для призраков получаем компонент, из которого получаем остальные параметры при старте
        if (_isGhost) elemParameters = transform.parent.GetComponentInParent<ElemParameters>();
    }

    void Start()
    {
        GetMainElemsParameters(); // Получаем основные параметры из объекта с общими параметрами
        if (_isGhost) // Если призрак
        {
            InitializeGhost(); // Инициализируем параметры для призрака
        }
        else
        {
            InitializeElem(); // Инициализируем параметры для элема
        }
    }

    private void GetMainElemsParameters()
    {
        animator = GetComponentInChildren<Animator>(); // Получаем компонент аниматора, для смены анимаций
        hp = _maxHp; // При спавне начальное здоровье равно максимальному
        startSpeed = _speed; // Сохраняем начальную скорость, потому что во время боя может меняться

        parameters = elemParameters.parameters; // Получаем основные параметры
        manaBar = elemParameters.manaBar; // Получаем полоску маны
        _fieldCanvas = elemParameters._fieldCanvas; // Получаем канвас для отрисовки полосок здоровья
        groundLayerMask = elemParameters.groundLayerMask; // Получаем слой для регистрации позиции курсора
        _fieldSquare = elemParameters._fieldSquare; // Получаем спрайт квадрата для выставления
        _hpBar = elemParameters._hpBar; // Получаем спрайт полоски здоровья

        capsCol = GetComponent<CapsuleCollider>(); // Получаем компонент коллайдера
    }

    private void InitializeElem()
    {
        tag = "InactiveElem";
        name = name.Replace("(Clone)", "").Trim(); // Меняем имя новому элему, чтобы не было бесконечных (Clone) в названии
        startPos = transform.position; // Сохраняем начальную позицию

        _fieldSquare = Instantiate(_fieldSquare, Vector3.zero, Quaternion.identity, _fieldCanvas.transform); // Создаем квадрат
        _fieldSquare.transform.localRotation = Quaternion.identity; // Поворачиваем его в плоскости канваса
        _fieldSquare.enabled = false; // Делаем невидимым
    }

    private void InitializeGhost()
    {
        enteredInGame = true; // Он сразу введен в игру
        SpawnHpBar(); // Создаем полоску здоровья
    }

    private void FixedUpdate()
    {
        if (enteredInGame) // Если элем введен в игру, и он не мертв
        {
            if (!enterTower) // Если не подошел близко к башне
            {
                if (!Field.IsCellOccupied(transform.position) || isGhost()) // Если клетка не занята либо элем - призрак
                {
                    float fieldSpeed = -1 * defaultSpeed * _speed * Time.fixedDeltaTime;
                    Vector3 changePosition = new Vector3(fieldSpeed, 0, 0); // Считаем расстояние, которое должен элем пройти
                    transform.position += changePosition; // Меняем позицию на это расстояние
                    _hpBar.transform.localPosition += changePosition; // Перемещаем полоску здоровья вместе с элемом
                }

                ApplyEarthSlow(); // Применяем эффект замедления
            }

            // Считаем позицию по оси Z == номер линии
            int roundedZ = Mathf.RoundToInt(transform.position.z);
            Field.field[roundedZ].Add(transform); // Обновляем позицию в сете трансформов для сета линий поля. Происходит перезапись

            // Меняем параметры полоски здоровья
            _hpBar.fillAmount = Mathf.Lerp(_hpBar.fillAmount, hp / _maxHp, lerpSpeed * Time.fixedDeltaTime); // Уменьшаем видимую часть
            _hpBar.color = Color.Lerp(parameters.hpRingRed, parameters.hpRingGreen, hp / _maxHp); // Меняем цвет визуализации


            // Условия для смерти элема: Здоровье меньше нуля, или он вошел в башню == уничтожил линию,
            // Или для него кончилась линия, или на линии упала башня, или произошел конец игры
            if (hp <= 0 || transform.position.x < 0 || !Field.aliveTowers[roundedZ] || Field.endOfGame) // || enterTower 
            {
                animator.SetTrigger("isDead"); // Вклчаем анимацию смерти
                capsCol.enabled = false;
                enteredInGame = false;
                Destroy(_hpBar.gameObject);
                Field.field[roundedZ].Remove(transform); // Удаляем информацию об этом элеме из сета - его больше нет в игре
                if (isGhost()) manaBar.SpendMana(-manaCost); // Восстанавливаем ману при смерти призрака
            }
        }
    }

    // В методе Update уменьшает время замедления, если закончится - удаляем эффект, возвращаем скорость
    private void ApplyEarthSlow()
    {
        if (earthSlowTime > 0)
        {
            // Ограничиваем время замедления от 0 до установленного, уменьшаем со временем
            earthSlowTime = Mathf.Clamp(earthSlowTime - Time.fixedDeltaTime, 0, parameters._earthSlowDuration);
            if (earthSlowTime == 0f)
            {
                _speed = startSpeed; // Возвращаем скорость
                if (earthSlow) Destroy(earthSlow.gameObject); // Если есть эффект замедления - удаляем
            }
        }
    }

    // TODO Переписать корутину стана как для замедления, возможны наложения стана на стан -> прекращение его раньше, а не продление
    private void ApplyWoodRoot()
    {

    }

    // Действия при зажатии мыши (Drag'n'Drop)
    private void OnMouseDrag()
    {
        if (!enteredInGame && !Field.endOfGame && manaBar.IsEnoughMana(manaCost)) // Если еще не введен в игру, и игра не закончена и достаточно маны
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Кастуем луч для Drag'n'Drop 
            RaycastHit hit;
            float onDeskY = startPos.y + DESK_OFFSET;
            if (Physics.Raycast(ray, out hit, 30f, groundLayerMask)) // Если луч касается слоя для каста в пределах 30 у.е.
            {
                // Перемещаем элема внутри поля, ограничив последней линией +- оффсет
                transform.position = new Vector3(Mathf.Clamp(hit.point.x, maxX - 0.7f, maxX + 0.7f), onDeskY,
                                                 Mathf.Clamp(hit.point.z, minZ + 0.3f, maxZ - 0.3f));
            }

            int roundedX = Mathf.RoundToInt(transform.position.x); // Округлённые значения координат, 
            int roundedZ = Mathf.RoundToInt(transform.position.z); // Для корректной отрисовки квадрата 
            roundedPosition = new Vector3(roundedX, onDeskY, roundedZ); // И последующего выставления элема


            manaBar.ShowPreview(manaCost); // Визуализируем сколько маны будет стоить элем при постановке

            if (isObjectInField()) // Если элем находится внутри поля
            {
                // Возможность поставить на точку - На линии есть башня, и элем либо призрак, либо клетка не занята
                posibleToPlace = Field.aliveTowers[roundedZ] && (isGhost() || !Field.IsCellOccupied(roundedPosition));
                _fieldSquare.color = posibleToPlace ? parameters.squareGreen : parameters.squareRed; // Меняем цвет квадрата в зависимости от этого условия
                _fieldSquare.enabled = true; // Делаем квадрат видимым
                _fieldSquare.transform.localPosition = new Vector3(roundedX, roundedZ, 0); // Перемещаем квадрат вместе с элемом, но по клеткам
            }
            else
            {
                posibleToPlace = false; // Невозможно выставить элема вне поля
                _fieldSquare.enabled = false; // Делаем квадрат невидимым
            }
        }
    }

    // Действия при отпускании мыши
    private void OnMouseUp()
    {
        if (!enteredInGame)
        {
            if (posibleToPlace) // Если не введен в игру и есть возможность поставить
            {
                enteredInGame = true; // Введен в игру
                tag = "Elem";
                Instantiate(this.gameObject, startPos, Quaternion.identity, transform.parent); // Создаем копию объекта на стартовой позиции
                transform.position = roundedPosition; // Выставляем точно в центр выбранной клетки
                SpawnHpBar(); // Создаем полоску здоровья
                Destroy(_fieldSquare.gameObject); // Уничтожаем квадрат - для этого элема он больше не нужен

                manaBar.SpendMana(manaCost);
                animator.SetTrigger("enteredInGame");
            }
            else
            {
                transform.position = startPos;
                manaBar.ReturnPreview();
            }
        }
        if (_fieldSquare) _fieldSquare.enabled = false; // Если квадрат еще существует, то при неправильной постановке - отключаем  

    }

    private bool isObjectInField()
    {
        return (transform.position.x > minX && transform.position.x < maxX) &&
            (transform.position.z > minZ && transform.position.z < maxZ);
    }

    // Действия при касании других объектов
    private void OnTriggerEnter(Collider other)
    {
        // Происходит только при введении элема в игру
        if (enteredInGame)
        {
            // Если тэг объекта - снаряд, и этот объект не собственный сплэш
            if (other.CompareTag("Projectile") && !other.gameObject.Equals(splash))
            {
                ProjectileMotion projectile = other.GetComponent<ProjectileMotion>();
                float projDamage = projectile.GetProjDamage();
                string projElement = projectile.GetProjElement();
                hp -= projDamage * elemParameters.GetDamageCoef(_element, projElement);

                ProjectileEffect(projectile);
            }
            else if (other.CompareTag("Tower") && !enterTower)
            {
                _speed = 0f;
                animator.SetTrigger("isAttack");
                enterTower = true;
                TowerController tower = other.GetComponent<TowerController>();
                tower.SetHpBarVisible();
                StartCoroutine(AttackTower(tower));
            }
        }
    }

    private IEnumerator AttackTower(TowerController tower)
    {
        while (true)
        {
            tower.TakeDamage(_attackDamage);
            yield return new WaitForSeconds(1 / _attackFreq);
        }
    }

    private void ProjectileEffect(ProjectileMotion projectile)
    {
        if (!projectile.IsSplash()) // Если снаряд - не эффект от снаряда, то срабатывает эффект снаряда ._.
        {
            switch (projectile.GetProjElement()) // В зависимости от элемента снаряда создаем соответствующий эффект
            {
                case "fire":
                    splash = Instantiate(elemParameters._fireEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity, transform);
                    break;
                case "water":
                    splash = Instantiate(elemParameters._waterEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity, transform);
                    break;
                case "metal":
                    projectile.MetalPenetrateReduceDamage(); // Уменьшаем урон снаряда с каждым попаданием по элему
                    splash = Instantiate(elemParameters._metalEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity, transform);
                    break;
                case "wood":
                    WoodProcessHits();
                    break;
                case "earth":
                    EarthReduceSpeed();
                    break;
            }
        }
    }

    private void WoodProcessHits()
    {
        if (elemParameters.IsLastWoodHit())
        {
            Instantiate(elemParameters._woodEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.Euler(-90, 0, 0), transform);
            StartCoroutine(WoodRoot());
        }
        elemParameters.IncreaseWoodHits();
    }

    private void EarthReduceSpeed()
    {
        if (!earthSlow)
        {
            earthSlow = Instantiate(elemParameters._earthEffect, transform.position + new Vector3(0, 0.3f, 0), Quaternion.Euler(-90, 0, 0), transform);
        }
        earthSlowTime = parameters._earthSlowDuration;
        _speed = startSpeed * parameters._earthSlowSpeedReduction;
    }

    private IEnumerator WoodRoot()
    {
        _speed = 0f;
        animator.SetTrigger("isRooted");
        yield return new WaitForSecondsRealtime(parameters._woodRootDuration);
        _speed = startSpeed;
        animator.SetTrigger("enteredInGame");
    }

    // Создаем полоску здоровья. Для призраков - сразу, для элемов - после выставления
    private void SpawnHpBar()
    {
        _hpBar = Instantiate(_hpBar, Vector3.zero, Quaternion.identity, _fieldCanvas.transform);
        _hpBar.color = parameters.hpRingGreen; // Меняем цвет полного круга - зеленый
        _hpBar.transform.localRotation = Quaternion.identity; // Поворачиваем вдоль канваса
        _hpBar.transform.localPosition = new Vector3(transform.position.x, transform.position.z, 0); // Создаем в месте элема
    }

    // Истина, если элем - призрак
    public bool isGhost() { return _isGhost; }

    public Parameters GetMainParameters() { return parameters; }
}
