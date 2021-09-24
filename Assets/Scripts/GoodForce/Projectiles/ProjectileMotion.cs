using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    [SerializeField] string splashElement = ""; // ����. ������ �������� == ������� ������, ��� ������ - ����� 
    private string element = ""; // ������� �������. ������ �������� ��� ��������� ������ �������������
    private float damage; // ����
    private float penetrationLoss; // ������ ����� ��� �������� ��� �������
    private float _bulletSpeed; // �������� ������ �������

    private Renderer projRend; // ��������� ���������, ��� ����� ���������
    private TowerController tower; // ��������� �����-��������, ����� ����� ��������� ����� � ������ �� ���
    private Parameters parameters; // �������� ���������, ������� ����� �� �����

    private void Start()
    {
        if (splashElement.Equals("")) // ���� �������� ���������� ���������� �������� �����
        {
            InitializeProj(); //  �������������� ������ ��� ������
        }
        else 
        {
            InitializeSplash(); // �������������� ������ ��� ����� ��� ������ ������
        }
    }

    private void InitializeProj()
    {
        projRend = GetComponent<Renderer>(); // �������� ��������� ��� ����� ����� �������
        tower = transform.parent.GetComponent<TowerController>(); // �������� ��������� �������� - �����

        parameters = tower.GetMainParameters(); // ��� ����������� ������������ ������ � �����������, ������� �������� �� �� �����
        _bulletSpeed = parameters._projectileSpeed; // �������� �������
        penetrationLoss = parameters._metalPenetration; // ������ ����� ��� �������������� �������

        damage = tower.GetProjDamage(); // �������� ���� ������� � ����������� �� ������ �����
        element = tower.GetProjElement(); // �������� ������ ������� � ����������� �� ������ �����
        projRend.material = tower.GetProjMaterial(); // �������� �������� ������� �� ������ �����

        // ������� ��� �� �������� ������� � ����������� �� ������
        Instantiate(tower.GetSmoke(element), transform.position, Quaternion.Euler(0, 0, 90), transform);
    }
    private void InitializeSplash()
    {
        element = splashElement; // ������������� �������� ��� ������
        parameters = GetComponentInParent<ElemMotion>().GetMainParameters(); // �������� ��������� �� (!) �����, ��� ��� ��� ������� �����
        if (element.Equals("fire"))
            damage = parameters._fireSplashDamage; // �������� ���� �� ��������� ������
        else if (element.Equals("water"))
            damage = parameters._waterSplashDamage; // �������� ���� �� �������� ������
    }

    void FixedUpdate()
    {
        if (!IsSplash()) // ������ ��� �������� ��������� ���������� �������
        {
            transform.position += new Vector3(_bulletSpeed * Time.fixedDeltaTime, 0, 0); // ����������� ������� � ������������ - �����
            if (transform.position.x > 7.5f) Destroy(this.gameObject); // ����������� ������� ��� ������ �� ������� ����
        }
    }

    // �������� ���� �� ������� (��� ������)
    public float GetProjDamage() { return damage; }

    // �������� ��� ������ �� �������
    public string GetProjElement() { return element; }

    // ������ ����� ��� ������� ������ �����, ���� ������ - ������
    public void MetalPenetrateReduceDamage() { damage *= (1 - penetrationLoss); }

    // ���������� ������, ���� �������� ����� - �� ������, � �����, ��������
    public bool IsSplash() { return splashElement.Length > 0; ; }

    // ��������� ������ ��� ������� � �������
    private void OnTriggerEnter(Collider other)
    {
        // ���� �� ����� � ���� - ����, � �� ������������� ������
        if (!IsSplash() && other.CompareTag("Elem") && !element.Equals("metal"))
        {
            Destroy(this.gameObject); // ���������� ������
        }
    }

}
