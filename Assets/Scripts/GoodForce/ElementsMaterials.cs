using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementsMaterials : MonoBehaviour
{
    private Dictionary<string, Material> materialDict = new Dictionary<string, Material>();
    private Dictionary<string, Gradient> gradientDict = new Dictionary<string, Gradient>();
    private Dictionary<string, GameObject> smokeDict = new Dictionary<string, GameObject>();
    private Dictionary<string, float[]> bulletParamsDict = new Dictionary<string, float[]>();

    [SerializeField] private Parameters parameters;
    [SerializeField] private Material fire;
    [SerializeField] private Material water;
    [SerializeField] private Material wood;
    [SerializeField] private Material metal;
    [SerializeField] private Material earth;

    [SerializeField] private GameObject fireSmoke;
    [SerializeField] private GameObject waterSmoke;
    [SerializeField] private GameObject woodSmoke;
    [SerializeField] private GameObject metalSmoke;
    [SerializeField] private GameObject earthSmoke;

    private Gradient fire_grad, water_grad, earth_grad, metal_grad, wood_grad;

    private void Awake()
    {
        InitializeMaterials();
        InitializeGradients();
        InitializeSmokes();
        InitializeGradientsBulletParameters();
    }

    private void InitializeSmokes()
    {
        smokeDict.Add("fire", fireSmoke);
        smokeDict.Add("water", waterSmoke);
        smokeDict.Add("wood", woodSmoke);
        smokeDict.Add("metal", metalSmoke);
        smokeDict.Add("earth", earthSmoke);
    }

    private void InitializeMaterials()
    {
        materialDict.Add("fire", fire);
        materialDict.Add("water", water);
        materialDict.Add("wood", wood);
        materialDict.Add("metal", metal);
        materialDict.Add("earth", earth);
    }

    private void InitializeGradients()
    {
        fire_grad = new Gradient();
        water_grad = new Gradient();
        earth_grad = new Gradient();
        metal_grad = new Gradient();
        wood_grad = new Gradient();

        fire_grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.red / 3f, 0.0f), new GradientColorKey(Color.red / 2f, 1.0f) },
                          new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
        water_grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.blue / 3f, 0.0f), new GradientColorKey(Color.blue / 2f, 1.0f) },
                          new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
        earth_grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.yellow / 3f, 0.0f), new GradientColorKey(Color.yellow / 2f, 1.0f) },
                          new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
        metal_grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.gray / 3f, 0.0f), new GradientColorKey(Color.gray / 2f, 1.0f) },
                          new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
        wood_grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.green / 3f, 0.0f), new GradientColorKey(Color.green / 2f, 1.0f) },
                          new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

        gradientDict.Add("fire", fire_grad);
        gradientDict.Add("water", water_grad);
        gradientDict.Add("wood", wood_grad);
        gradientDict.Add("metal", metal_grad);
        gradientDict.Add("earth", earth_grad);
    }

    private void InitializeGradientsBulletParameters()
    {
        float[] fireParams = new float[2] { parameters._fireDamage, parameters._fireFrequency };
        float[] waterParams = new float[2] { parameters._waterDamage, parameters._waterFrequency };
        float[] woodParams = new float[2] { parameters._woodDamage, parameters._woodFrequency };
        float[] metalParams = new float[2] { parameters._metalDamage, parameters._metalFrequency };
        float[] earthParams = new float[2] { parameters._earthDamage, parameters._earthFrequency };

        bulletParamsDict.Add("fire", fireParams);
        bulletParamsDict.Add("water", waterParams);
        bulletParamsDict.Add("wood", woodParams);
        bulletParamsDict.Add("metal", metalParams);
        bulletParamsDict.Add("earth", earthParams);
    }

    public Material GetMaterial(string elem)
    {
        return materialDict[elem];
    }

    public Gradient GetGradient(string elem)
    {
        return gradientDict[elem];
    }

    public float GetBulletDamage(string elem)
    {
        return bulletParamsDict[elem][0];
    }

    public float GetBulletFreq(string elem)
    {
        return bulletParamsDict[elem][1];
    }

    public GameObject GetSmoke(string elem)
    {
        return smokeDict[elem];
    }
}
