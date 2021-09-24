using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthElemMain : MonoBehaviour
{
    [SerializeField] float _timeBeforeDestroy;
    public void DestroyAfterDie()
    {
        StartCoroutine(DestroyParticles()); // Запуск корутины для уничтожения элема с частицами через время
    }

    private IEnumerator DestroyParticles()
    {
        yield return new WaitForSeconds(_timeBeforeDestroy); // Выжидаем нужное время
        Destroy(this.gameObject); // Уничтожаем родителя (вместе с элемом и частицами)
    }
}
