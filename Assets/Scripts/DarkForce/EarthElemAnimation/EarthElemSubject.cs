using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthElemSubject : MonoBehaviour
{
    // Прилинковываем частички камней
    [SerializeField] private GameObject particles;
    private EarthElemMain mainComponent; // Компонент - родитель, который будет уничтожать этот объект

    void Start()
    {
        mainComponent = GetComponentInParent<EarthElemMain>(); // Получаем компонент-родителя
    }

    // Ивент, который вызывается в конце анимации смерти
    public void DieEvent()
    {
        // Спавним частички камней, которые сразу разлетятся
        particles = Instantiate(particles, transform.position, Quaternion.identity, transform.parent);
        particles.SetActive(true); // Делаем активыми частицы, потому что по умаолчанию они выключениы
        gameObject.SetActive(false); // Делаем неактивным(невидимым) основного земляного элема
        mainComponent.DestroyAfterDie(); // Запускаем функцию уничтожения для родителя (если запускать ее здесь, частицы уничтожатся вместе с элемом)
    }
}
