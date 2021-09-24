using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthElemSubject : MonoBehaviour
{
    // �������������� �������� ������
    [SerializeField] private GameObject particles;
    private EarthElemMain mainComponent; // ��������� - ��������, ������� ����� ���������� ���� ������

    void Start()
    {
        mainComponent = GetComponentInParent<EarthElemMain>(); // �������� ���������-��������
    }

    // �����, ������� ���������� � ����� �������� ������
    public void DieEvent()
    {
        // ������� �������� ������, ������� ����� ����������
        particles = Instantiate(particles, transform.position, Quaternion.identity, transform.parent);
        particles.SetActive(true); // ������ �������� �������, ������ ��� �� ���������� ��� ����������
        gameObject.SetActive(false); // ������ ����������(���������) ��������� ��������� �����
        mainComponent.DestroyAfterDie(); // ��������� ������� ����������� ��� �������� (���� ��������� �� �����, ������� ����������� ������ � ������)
    }
}
