using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthElemMain : MonoBehaviour
{
    [SerializeField] float _timeBeforeDestroy;
    public void DestroyAfterDie()
    {
        StartCoroutine(DestroyParticles()); // ������ �������� ��� ����������� ����� � ��������� ����� �����
    }

    private IEnumerator DestroyParticles()
    {
        yield return new WaitForSeconds(_timeBeforeDestroy); // �������� ������ �����
        Destroy(this.gameObject); // ���������� �������� (������ � ������ � ���������)
    }
}
