using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainCountDown : MonoBehaviour
{
    [SerializeField] Parameters parameters; // �������� ���������
    [SerializeField] TextMeshProUGUI timerText; // ����� �������
    [SerializeField] Slider slider; // ��������� ��������. ��� ����������
    [SerializeField] Image fillImage; // �����������, ���� �������� ������
    [SerializeField] Gradient gradient; // ������ �������� - ����� ������� ����� �������� ��� ����� �������
    //[SerializeField] int _numSectors;

    private float startTime;
    private float curTime, min, sec;
    //int sectorTime;

    void Start()
    {
        startTime = parameters._allTime; // �������� ����� ����� ���
        curTime = startTime; // ������������� ������� ����� - �� ����� ������ �����

        slider.maxValue = startTime; // ������ ��������� ��������
        slider.value = slider.maxValue; // ������ ������� �������� �� ������������

        //sectorTime = Mathf.RoundToInt(startTime / _numSectors);
    }

    void Update()
    {
        if (!Field.endOfGame) curTime -= 1 * Time.deltaTime; // ������� ����� �� ����� ���
        if (curTime <= 0) // ��������� �����
        {
            curTime = 0; // ������ �� ������
            Field.endOfGame = true; // ���� ����� ����
        }
        else
        {
            // ����� ������� ��������������� ����� ���������
            //int stepValue = (int)curTime / sectorTime * (int)startTime / _numSectors;

            slider.value = Mathf.FloorToInt(curTime); // ���������� ����� ��� ��� ����� ��������
            fillImage.color = gradient.Evaluate(1 - slider.normalizedValue); // ������ ���� ������� ����
        }


        min = Mathf.FloorToInt(curTime / 60); // �������� ������ ��� ������������ ������� ���
        sec = Mathf.FloorToInt(curTime % 60); // ������� ������� ���. ���������� � ������ �������
        timerText.text = string.Format("{0:0}:{1:00}", min, sec); // ���������� ����� ��� � ������ �������
    }

    // ���������� ������� ����� ���
    public float GetCurTime() { return curTime; }

    // ���������� ����� ����� �����
    public float GetStartTime() { return startTime; }
}
