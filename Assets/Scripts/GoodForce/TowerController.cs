using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerController : MonoBehaviour
{
    // �������������� ������� �� ��������� 
    [SerializeField] private Parameters parameters; // ����� ��������� 
    [SerializeField] private ElementsMaterials elemMaterials; // ��������� �������� � ������� - �������� ��������
    [SerializeField] Image hpSquare; // ������� ��������
    [SerializeField] Canvas fieldCanvas; // ������ ��� ��������� ������� ��������
    [SerializeField] private Transform crystal, woodPanels, banners; // ���������� ������� ��� ����� ������
    [SerializeField] private Transform bulletSpawner; // ���������� ������� ��� ����� ������
    [SerializeField] private GameObject projectile; // ������ ������� �� ��������
    [SerializeField] ParticleSystem towerCloud; // ������������ ������

    private Renderer crystalRend, woodPanelsRend, bannersRend;

    private float projFrequency, projDamage; // �������� ���������, ���� ��������
    private float _timeToSpawn, _shootTime; // ����� ��� ��������, ����� ����� ����������

    private string curElement; // ������� ������� ������
    private float _HP, _maxHP; // ������� �������� 
    private bool _dead = false;


    void Start()
    {
        Initialize(); // ��������� ��������� ���������
    }

    void FixedUpdate()
    {
        ShotProjectiles(); // ������� �������
    }

    // �������������� ��������� ���������
    private void Initialize()
    {
        // ��������� ���������, ��������
        _maxHP = parameters._towerMaxHP;
        _HP = _maxHP;

        // �������� ���������� ���������� �����:
        crystalRend = crystal.GetComponent<Renderer>(); // ��������,
        woodPanelsRend = woodPanels.GetComponent<Renderer>(); // ���������� ������,
        bannersRend = banners.GetComponent<Renderer>(); // �������

        // �������� ��������� ������� ����� � ���������� ���. 
        string startElement = Field.GetFreeElemental();
        SetElement(startElement);
        SpawnTowerCloud(); // ������� ������ ������

        ApplyProjectileParameters(); // ��������� ��������� �������� � ����������� �� �������� ��������

        hpSquare = Instantiate(hpSquare, transform.position + new Vector3(-0.5f, 0.01f, -0.5f), Quaternion.identity, fieldCanvas.transform); // ������� �������
        hpSquare.transform.localRotation = Quaternion.identity; // ������������ ��� � ��������� �������
        hpSquare.enabled = false; // ������ ���������
    }


    // ������� ������ � ��������� �����������, ��������� ����� ����� ����������
    private void ShotProjectiles()
    {
        if (Time.time > _timeToSpawn && IsLineOccupied()) // ���� ����� ���� ������ ������� ��� �������� � ����� �� ������
        {
            Instantiate(projectile, bulletSpawner.position, Quaternion.identity, transform); // ������� ������
            _timeToSpawn = Time.time + _shootTime; // ������ ����� ��� ���������� ��������
        }
    }

    // ��������� ��������� ������� � ����������� �� �������� 
    void ApplyProjectileParameters()
    {
        projDamage = elemMaterials.GetBulletDamage(curElement); // ����
        projFrequency = elemMaterials.GetBulletFreq(curElement); // ������� ���������
        _shootTime = 1 / projFrequency; // ����� ����� ����������
        _timeToSpawn = Time.time + _shootTime / 3f; // ��������� ����� ��� ������ ������ �������
    }

    // ���������� ������, ���� ����� ����� ������ � ����� ��������
    private bool IsLineOccupied()
    {
        return Field.IsLineOccupied(Mathf.RoundToInt(transform.position.z));
    }

    // ������� ������ ��� ������������ ������
    private void SpawnTowerCloud()
    {
        towerCloud = Instantiate(towerCloud, transform.position + new Vector3(0, 0.8f, 0), // ������� ������ ���� ���� ������ �����
            Quaternion.Euler(270, 0, 0), transform).GetComponent<ParticleSystem>();
        var col = towerCloud.colorOverLifetime;
        col.color = elemMaterials.GetGradient(curElement); // ������ ���� ������ � ����������� �� ��������
    }


    // ��������� ������� ������� �����
    public void SetElement(string newElement)
    {
        // ��������� ������� ������, �������� �������� �� ����� ������
        curElement = newElement;
        Material material = elemMaterials.GetMaterial(curElement);

        // ��������� �������� � ��������, �������� � ���������� �������
        crystalRend.material = material;
        bannersRend.material = material;
        woodPanelsRend.material = material;

        // ������ ���� ������ ������ � ����������� �� ��������
        var col = towerCloud.colorOverLifetime;
        col.color = elemMaterials.GetGradient(curElement);

        // ��������� ��������� ������� � ����������� �� ������ ��������
        ApplyProjectileParameters();
    }

    // ��������� ����� �� ������ ���������
    public void TakeDamage(float damage)
    {
        _HP -= damage; // �������� ���������� ����
        hpSquare.fillAmount = _HP / _maxHP; // ������ ������������� ������� ��������

        if (_HP < 0 && !_dead) // �������� � ������, ���� ����������� ��������
        {
            _dead = true; // ����� ���������� ���������� ����������� ������ ���� ���
            Field.aliveTowers[Mathf.RoundToInt(transform.position.z)] = false; // ������� ����� �� ������ ����� �����, ������ ����� ������

            Destroy(this.gameObject); // ���������� ����� ��� ������
            Destroy(hpSquare.gameObject); // ���������� ������� ��������
        }
    }

    public void SetHpBarVisible() { hpSquare.enabled = true; }

    public float GetProjDamage() { return projDamage; }

    public string GetProjElement() { return curElement; }

    public Material GetProjMaterial() { return crystalRend.material; }

    public Parameters GetMainParameters() { return parameters; }

    public GameObject GetSmoke(string element) { return elemMaterials.GetSmoke(element); }
}
