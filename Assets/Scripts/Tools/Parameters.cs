using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters : MonoBehaviour
{
    [Header("Timer Options")]
    public float _allTime = 300;
    public float _maxTimeToSpawnWave = 6f;
    public float _minTimeToSpawnWave = 3.5f;


    [Header("Mana Options")]
    public float _maxMana = 100;
    public float _crystalCost = 5;
    public float _manaGrowSpeed = 1;

    public float _darkStartMana = 0;
    public float _darkManaForPeshkaDied = 1;
    public float _darkManaGrowSpeed = 1;

    [Header("Elems Options")]
    public float _elemManaCost = 6f;

    [Header("Projectiles Options")]
    public float _projectileSpeed = 5f;
    public float _fireDamage = 1f;
    public float _waterDamage = 1f;
    public float _metalDamage = 3f;
    public float _woodDamage = 1f;
    public float _earthDamage = 2f;
    [Space]
    public float _fireFrequency = 2f;
    public float _waterFrequency = 1f;
    public float _metalFrequency = 0.3f;
    public float _woodFrequency = 1.5f;
    public float _earthFrequency = 0.5f;
    [Space]
    public float _defDamageCoef = 1; 
    public float _minDamageCoef = 0; 
    public float _maxDamageCoef = 2f; 
    public float _reducedDamageCoef = 0.5f;

    [Header("Special abilities")]
    public float _fireSplashDamage = 0.5f;
    public float _waterSplashDamage = 2f;
    public float _earthSlowDuration = 3f;
    public float _earthSlowSpeedReduction = 0.5f;
    [Range(0f, 1f)] public float _metalPenetration = 0.2f;
    public float _woodRootDuration = 1f;
    public int _woodHitsForRoute = 6;


    [Header("Tower Options")]
    public float _towerMaxHP;

    [HideInInspector] public Color squareGreen = new Color(0, 1, 0.5f, 0.5f);
    [HideInInspector] public Color squareRed = new Color(1, 0.5f, 0, 0.5f);

    [HideInInspector] public Color hpRingGreen = new Color(0, 0.7f, 0.3f, 0.9f);
    [HideInInspector] public Color hpRingRed = new Color(0.7f, 0.3f, 0,  0.9f);


    private void Awake()
    {
        Field.FieldAwake(); // Инициализируем структуру поля, освобождая память под сеты линий
    }
}
