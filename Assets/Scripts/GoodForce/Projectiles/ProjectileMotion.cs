using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    [SerializeField] string splashElement = ""; // Флаг. Пустое значение == обычный снаряд, имя стихии - сплэш 
    private string element = ""; // Элемент снаряда. Пустое значение для изюежания ошибок инициализации
    private float damage; // Урон
    private float penetrationLoss; // Потеря урона при пробитии для металла
    private float _bulletSpeed; // Скорость полета снаряда

    private Renderer projRend; // Компонент рендерера, для смены материала
    private TowerController tower; // Компонент башни-родителя, чтобы брать параметры урона и стихии от нее
    private Parameters parameters; // Основные параметры, которые берем от башни

    private void Start()
    {
        if (splashElement.Equals("")) // Если значение начального сплэшового элемента пусто
        {
            InitializeProj(); //  Инициализируем объект как снаряд
        }
        else 
        {
            InitializeSplash(); // Инициализируем объект как сплэш или другой эффект
        }
    }

    private void InitializeProj()
    {
        projRend = GetComponent<Renderer>(); // Получаем компонент для смены цвета снаряда
        tower = transform.parent.GetComponent<TowerController>(); // Получаем компонент родителя - башню

        parameters = tower.GetMainParameters(); // Нет возможности прилинковать объект с параметрами, поэтому получаем их от башни
        _bulletSpeed = parameters._projectileSpeed; // Скорость снаряда
        penetrationLoss = parameters._metalPenetration; // Потеря урона для металлического снаряда

        damage = tower.GetProjDamage(); // Получаем урон снаряда в зависимости от стихии башни
        element = tower.GetProjElement(); // Получаем стихию снаряда в зависимости от стихии башни
        projRend.material = tower.GetProjMaterial(); // Получаем материал снаряда от стихии башни

        // Создаем дым от летящего снаряда в зависимости от стихии
        Instantiate(tower.GetSmoke(element), transform.position, Quaternion.Euler(0, 0, 90), transform);
    }
    private void InitializeSplash()
    {
        element = splashElement; // Устанавливаем основной тип стихии
        parameters = GetComponentInParent<ElemMotion>().GetMainParameters(); // Получаем параметры от (!) Элема, так как они спавнят сплэш
        if (element.Equals("fire"))
            damage = parameters._fireSplashDamage; // Получаем урон от огненного сплэша
        else if (element.Equals("water"))
            damage = parameters._waterSplashDamage; // Получаем урон от водяного сплэша
    }

    void FixedUpdate()
    {
        if (!IsSplash()) // Только для снарядов выполняем обновление позиции
        {
            transform.position += new Vector3(_bulletSpeed * Time.fixedDeltaTime, 0, 0); // Перемещение снаряда в пространстве - полет
            if (transform.position.x > 7.5f) Destroy(this.gameObject); // Уничтожение снаряда при вылете за область поля
        }
    }

    // Получаем урон от снаряда (или сплэша)
    public float GetProjDamage() { return damage; }

    // Получаем тип стихии от снаряда
    public string GetProjElement() { return element; }

    // Потеря урона при пролете сквозь врага, если снаряд - металл
    public void MetalPenetrateReduceDamage() { damage *= (1 - penetrationLoss); }

    // Вовзращает истину, если источник урона - не снаряд, а сплэш, например
    public bool IsSplash() { return splashElement.Length > 0; ; }

    // Разрушаем снаряд при встрече с врагами
    private void OnTriggerEnter(Collider other)
    {
        // Если не сплэш и враг - элем, и не металлический снаряд
        if (!IsSplash() && other.CompareTag("Elem") && !element.Equals("metal"))
        {
            Destroy(this.gameObject); // Уничтожаем снаряд
        }
    }

}
