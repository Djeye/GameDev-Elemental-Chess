using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 0.1f; // ����� ��� ����������� �������
    void Start()
    {
        StartCoroutine(AutoDestroyAfterTime()); // ��������� ��������, � ������� ����� ����� ����������� �������� �������
    }

    IEnumerator AutoDestroyAfterTime()
    {
        yield return new WaitForSeconds(timeToDestroy); // �������� ������������� �����
        Destroy(this.gameObject); // ���������� ������
    }
}
