using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Parameters parameters; // �������� ���������
    [SerializeField] private Image preview; // �������������� ����������� ������ ���������
    [SerializeField] private bool isDarkManaBar = false; // ����������� �� ������� ���� ������ �����

    private Slider slider; // ������� - �������
    private float maxMana; // �������� �� ����������
    private float manaSpeed; // �������� �� ����������

    private float fadeSpeed = 0.05f; // �������� �������� ����

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        slider = gameObject.GetComponent<Slider>();

        maxMana = parameters._maxMana; // �������� ��������� ��������
        manaSpeed = parameters._manaGrowSpeed;

        slider.maxValue = maxMana; // ������ ��������� ��������
        // ������� ������� - ������������ 
        slider.value = isDarkManaBar ? parameters._darkStartMana : slider.maxValue;

        ReturnPreview(); // ��������� ��������� ������ ����
    }

    // � ������ ����� ��������� ��������, ����� ������������ ����
    private void Start()
    {
        if (!isDarkManaBar) StartCoroutine(manaGrow()); // ������ ��� ������� �������
    }

    // ��������, ������ ���������� ���� �� ������� ��������
    public void SpendMana(float cost)
    {
        StartCoroutine(ManaFade(cost, slider.value));
    }

    // ���������� ������, ���� ���� ���������� ��� ������� ��������
    public bool IsEnoughMana(float cost)
    {
        return slider.value - cost >= 0;
    }

    // ���������� ������������ �������� ����
    public float getMaxMana() { return maxMana; }

    // ���������� ������� �������� ����
    public float getSliderValue() { return slider.value; }

    // ��������, ������ ���������� ����
    IEnumerator ManaFade(float cost, float curMana)
    {
        while ((slider.value > curMana - cost && cost > 0) || (slider.value < curMana - cost && cost < 0)) // ���� ������� �������� ���� �� ����� ����������
        {
            slider.value -= cost * fadeSpeed; // �������� ���� � �����-�� ���������
            yield return new WaitForSeconds(fadeSpeed * .1f); // ���� �����-�� �����, ����� ���� ���������� ������
        }

        ReturnPreview(); // � ����� �������� , ������ ���������� ����� ���������� ����
    }

    // ��������, ����� ����������� ����
    IEnumerator manaGrow()
    {
        // ��������, �� ������� ���� ����� ������������� �� ��������
        float manaGrow = 0.01f * manaSpeed;
        while (true && !Field.endOfGame) // �������� ��� ����, ���� ��� �� ����������
        {
            // ���� ���� �� ��������� ���������, ����������� �������� ���� � ������, ��������� �� ������ � �����
            if (slider.value != slider.maxValue) preview.fillAmount = Mathf.Clamp(preview.fillAmount + manaGrow, 0f, 1f);
            slider.value = Mathf.Clamp(slider.value + manaGrow * slider.maxValue, slider.minValue, slider.maxValue);

            yield return new WaitForSeconds(.5f);
        }
    }

    // ������������� �������� ������, ����� ��������, ������� ��������� � ������ ��������
    public void ShowPreview(float cost)
    {
        preview.fillAmount = (slider.value - cost) / slider.maxValue;
    }

    // ������������� ������ ������ �������� �������� ����
    public void ReturnPreview()
    {
        preview.fillAmount = slider.value / slider.maxValue;
    }
}
