using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 0.1f; // Время для уничтожения объекта
    void Start()
    {
        StartCoroutine(AutoDestroyAfterTime()); // Запускаем корутину, в которой через время уничтожется носитель скрипта
    }

    IEnumerator AutoDestroyAfterTime()
    {
        yield return new WaitForSeconds(timeToDestroy); // Выжидаем установленное время
        Destroy(this.gameObject); // Уничтожаем объект
    }
}
