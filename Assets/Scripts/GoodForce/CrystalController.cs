using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    private const float Y_OFFSET = 0.5f; // ������ ��� "������" ��������� ��� �����
    // �������������� �������
    [SerializeField] Parameters parameters; // ���������
    [SerializeField] LayerMask groundLayerMask; // ���� � �������, ���������� �� ��������� ���������, ����� ������� ���������� �����������
    [SerializeField] private string element; // ������� ������ ���������
    [SerializeField] private ManaBar manaBar; // ���� ��� ������� �������

    float manaCost;     // ��������� ��� ����� ������
    private Vector3 startPos; // ��������� �������, ��� ����������� � ������ ������ ���������
    private bool availableToChange = false; // �������� �� ������� ������
    private Outline towerOutline; // ������� ����� ��� ��������� ��������� �� ���
    private TowerController tower; // ����� ��� ����� ������

    void Start()
    {
        Initialize();
    }

    // ������������� ��������� ����������
    private void Initialize()
    {
        startPos = transform.position; // ���������� ����������� ���������
        manaCost = parameters._crystalCost; // ��������� ����������� ���������
    }

    // ����������� ��������� Drag'n'Drop-��
    private void OnMouseDrag()
    {
        if (manaBar.IsEnoughMana(manaCost)) // �������� ������ ���� ���������� ����
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ���� ���� �� ������� ���� �� ������
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20f, groundLayerMask)) // ���� ���� ���� �������� � ��������� � �������� 20 ������
            {
                transform.position = new Vector3(Mathf.Clamp(hit.point.x, -0.3f, 7.3f), Y_OFFSET, // ���������� �������� ������ ����
                                                 Mathf.Clamp(hit.point.z, -1.3f, 4.3f));
            }

            manaBar.ShowPreview(manaCost); // ������������� ������� �������� ����
        }
    }

    // �������� ��� ���������� ����/������
    private void OnMouseUp()
    {
        if (availableToChange)
        {
            tower.SetElement(element); // ���������� ����� ������� ��� �����
            manaBar.SpendMana(manaCost); // ������ ����
        }
        else
        {
            manaBar.ReturnPreview(); // ������ ���� = ������ ����
        }

        transform.position = startPos; // � ����� ������ �������� ������������ �� ���� �����
    }


    // �������� ��� ���������� ��������� ������ ������ ��������
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tower")) // ���� ������ - �����
        {
            tower = other.GetComponent<TowerController>(); // ������ �����, ������ ������� ������ ��������
            bool isSameElement = tower.GetProjElement().Equals(element); // ������, ���� ������ ����� � �������� ���������
            float dstBetweenTowerAndCrystal = Mathf.Abs(other.transform.position.z - transform.position.z); // ���������� �� �������� �� �����
            towerOutline = other.GetComponent<Outline>(); // ��������� ��� �����

            availableToChange = !isSameElement && dstBetweenTowerAndCrystal < 0.5f; // �������� �������� ������, ���� ������� ����� ������, � ��� ��������� ��� ���������
            towerOutline.enabled = availableToChange; // ��������� ������� �� ����������� �������� ������
        }
    }

    // �������� ��� ��������� ������� �������
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tower")) // ���� ������ - �����
        {
            other.GetComponent<Outline>().enabled = false; // ��������� ���������
            availableToChange = false; // ���������� ��������� ������
        }
    }
}
