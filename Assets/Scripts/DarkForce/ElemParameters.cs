using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElemParameters : MonoBehaviour
{

    [SerializeField] public Parameters parameters; // �������� ���������
    [SerializeField] public ManaBar manaBar; // ������� ����
    [SerializeField] public Canvas _fieldCanvas; // ��������� ��������� ������� �������� � ���������
    [SerializeField] public LayerMask groundLayerMask; // ����, �� �������� ����� ����������� Drag'n'Drop
    [SerializeField] public Image _fieldSquare; // ���������, ������� ��� �������, ������������ ����������� ��������� �����
    [SerializeField] public Image _hpBar; // �������  ��������, � ��� �� ��������� ��� � ��������� ����
    [SerializeField] TextMeshProUGUI counterText;

    [Header("Element Effects")]
    [SerializeField] public GameObject _fireEffect; // ����� ����
    [SerializeField] public GameObject _waterEffect; // ����� ���� 
    [SerializeField] public GameObject _woodEffect; // ������ �� ����� ������
    [SerializeField] public GameObject _earthEffect; // ������ �� ���������� �����
    [SerializeField] public GameObject _metalEffect; // ������������� ������ �������

    // ������� ������ ����� Next � Previous ��� ���������� ������ ����������� ����� ��������
    private LinkedList<string> elements = new LinkedList<string>(); // ��, ������� ������, ���������� xD
    private int woodCounter = 0, maxHits;


    void Awake()
    {
        elements.AddLast("fire");  // ��������� 5 ������ � ������� ������ ������, ����� Next �� "���������" ������� �� ��������� ������
        elements.AddLast("metal"); // ����� �������, ��� ��������� ������� ��������� ��� ���������� ���������
        elements.AddLast("wood");  // �# �� ������������ ������� ������
        elements.AddLast("earth");
        elements.AddLast("water");
        elements.AddLast("fire");
        elements.AddLast("metal");
        elements.AddLast("wood");  // ������ ������ - ������ ������ ��������� ����� ������ ����

        maxHits = parameters._woodHitsForRoute; // �������� ������������ ����� ��� ���� �� ����������
    }

    // ���������� ����������� ��� ��������� ����� ��� ����� �� "�������"(element) �� ������� �� "�������"(projElement)
    public float GetDamageCoef(string element, string projElement)
    {
        float damageCoef = parameters._defDamageCoef; // ��������� ����������� �����
        if (element.Equals(projElement)) return parameters._minDamageCoef; // ���������� ������ = ����������� ����
        if (elements.Find(projElement).Next.Value.Equals(element)) damageCoef = parameters._maxDamageCoef; // ������ ��������� - ���������� ����
        if (elements.Find(projElement).Next.Next.Next.Value.Equals(element)) damageCoef = parameters._reducedDamageCoef; // ������ ����������� - ���������� ����

        return damageCoef;
    }

    // ����������� ����� ���������, � ���������� �� ������
    public void IncreaseWoodHits()
    {
        woodCounter = (woodCounter + 1) % maxHits; // ����������� �������
        counterText.text = woodCounter.ToString(); // �������� � ������, ���������� ��������

        counterText.color = IsLastWoodHit() ? Color.red : Color.white; // ���������� � �������, ���� ��������� ������� - ��������� 
    }

    // ���������� ������, ���� ��������� ������� - ���������
    public bool IsLastWoodHit() { return woodCounter + 1 == maxHits; }
}
