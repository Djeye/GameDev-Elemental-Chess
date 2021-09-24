using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    // Структура для хранения позиций элемов
    // Сет сетов, индекс сета - номер линии от 1 до 5. 
    public static List<HashSet<Transform>> field = new List<HashSet<Transform>>();

    // Массив живых башен на поле боя. разрушение i-ой башни -> i-ый элемент массива - false
    public static bool[] aliveTowers = new bool[5] { true, true, true, true, true };
    
    private static int towerCount = 5;
    public static bool endOfGame = false;

    // Лист из разных элементов
    public static string[] existElements = new string[5] { "fire", "water", "metal", "wood", "earth" };
    // Сет для генерации случайных разных элементов (записываются уже использованные)
    public static HashSet<string> elementsSet = new HashSet<string>();
    

    // Инициализация 5 пустых сетов для Transform элемов
    public static void FieldAwake()
    {
        for (int i = 0; i < 5; i++) field.Add(new HashSet<Transform>());
    }


    // Возвращает свободный элемент стихии, не использованный ранее.
    public static string GetFreeElemental()
    {
        string elementName = "";
        bool doExitFromLoop = false; //|| (elementsSet.Count == 5);

        if (elementsSet.Count == existElements.Length - 1) // Оптимизация для малого числа оставшихся элементов
        {
            foreach (string elem in existElements)
            {
                if (!elementsSet.Contains(elem)) return elem; // Возвращаем единственный оставшийся элемент
            }
        }
        else
        {
            while (!doExitFromLoop) // В цикле получаем случайный элемент из оставшихся
            {
                elementName = existElements[Random.Range(0, 5)];
                if (!elementsSet.Contains(elementName)) // Выходим если сгенерированного нет в сете
                {
                    elementsSet.Add(elementName);
                    doExitFromLoop = true;
                }
            }
        }
        return elementName;
    }


    // Возврашает True если клетка занята
    public static bool IsCellOccupied(Vector3 pos)
    {
        int linePos = Mathf.RoundToInt(pos.z);

        foreach (Transform elem in field[linePos])
        {
            float deltaLinePos = elem.position.x - pos.x;
            if (elem.position.z == linePos && // Если совпадают линии
                !pos.Equals(elem.position) && // Элем из сета не аргумент pos
                deltaLinePos > -1f && deltaLinePos < 0 // Элем находится впереди до 1 клетки,
                && !elem.GetComponent<ElemMotion>().isGhost()) // Элем не призрак
                return true;
        }

        return false;
    }

    
    // Возвращает истину, если на линия не пустая, и можно стрелять
    public static bool IsLineOccupied(int linePos)
    {
        return field[linePos].Count > 0;
    }


    // Уменьшение счетчика живых башен, условие конца игры 
    public static void KillTower()
    {
        towerCount--;
        if (towerCount <= 2)
        {
            endOfGame = true;
            elementsSet.Clear(); // Для рестарта - обновление сета элементов
        }
    }
}
