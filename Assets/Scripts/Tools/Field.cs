using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    // ��������� ��� �������� ������� ������
    // ��� �����, ������ ���� - ����� ����� �� 1 �� 5. 
    public static List<HashSet<Transform>> field = new List<HashSet<Transform>>();

    // ������ ����� ����� �� ���� ���. ���������� i-�� ����� -> i-�� ������� ������� - false
    public static bool[] aliveTowers = new bool[5] { true, true, true, true, true };
    
    private static int towerCount = 5;
    public static bool endOfGame = false;

    // ���� �� ������ ���������
    public static string[] existElements = new string[5] { "fire", "water", "metal", "wood", "earth" };
    // ��� ��� ��������� ��������� ������ ��������� (������������ ��� ��������������)
    public static HashSet<string> elementsSet = new HashSet<string>();
    

    // ������������� 5 ������ ����� ��� Transform ������
    public static void FieldAwake()
    {
        for (int i = 0; i < 5; i++) field.Add(new HashSet<Transform>());
    }


    // ���������� ��������� ������� ������, �� �������������� �����.
    public static string GetFreeElemental()
    {
        string elementName = "";
        bool doExitFromLoop = false; //|| (elementsSet.Count == 5);

        if (elementsSet.Count == existElements.Length - 1) // ����������� ��� ������ ����� ���������� ���������
        {
            foreach (string elem in existElements)
            {
                if (!elementsSet.Contains(elem)) return elem; // ���������� ������������ ���������� �������
            }
        }
        else
        {
            while (!doExitFromLoop) // � ����� �������� ��������� ������� �� ����������
            {
                elementName = existElements[Random.Range(0, 5)];
                if (!elementsSet.Contains(elementName)) // ������� ���� ���������������� ��� � ����
                {
                    elementsSet.Add(elementName);
                    doExitFromLoop = true;
                }
            }
        }
        return elementName;
    }


    // ���������� True ���� ������ ������
    public static bool IsCellOccupied(Vector3 pos)
    {
        int linePos = Mathf.RoundToInt(pos.z);

        foreach (Transform elem in field[linePos])
        {
            float deltaLinePos = elem.position.x - pos.x;
            if (elem.position.z == linePos && // ���� ��������� �����
                !pos.Equals(elem.position) && // ���� �� ���� �� �������� pos
                deltaLinePos > -1f && deltaLinePos < 0 // ���� ��������� ������� �� 1 ������,
                && !elem.GetComponent<ElemMotion>().isGhost()) // ���� �� �������
                return true;
        }

        return false;
    }

    
    // ���������� ������, ���� �� ����� �� ������, � ����� ��������
    public static bool IsLineOccupied(int linePos)
    {
        return field[linePos].Count > 0;
    }


    // ���������� �������� ����� �����, ������� ����� ���� 
    public static void KillTower()
    {
        towerCount--;
        if (towerCount <= 2)
        {
            endOfGame = true;
            elementsSet.Clear(); // ��� �������� - ���������� ���� ���������
        }
    }
}
