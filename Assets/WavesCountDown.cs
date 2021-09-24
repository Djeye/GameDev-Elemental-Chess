using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WavesCountDown : MonoBehaviour
{
    [SerializeField] private Parameters parameters;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI mode;
    [SerializeField] private Button formationButton;
    [SerializeField] private MainCountDown mainTimer;
    [SerializeField] private GhostSpawner ghostSpawner;

    private float maxSpawnTime, minSpawnTime;
    private string[] elemList = new string[5] { "fire", "water", "metal", "wood", "earth" };
    private HashSet<string> ElemSet = new HashSet<string>();
    private HashSet<int> numSet = new HashSet<int>();

    private float curTime, mainTimerStartTime;
    private bool isSpawnLine = true;

    private Color lineColor = new Color(0, 1, 0.9f, 0.9f), wedgeColor = new Color(1, 0.7f, 0, 0.9f);

    void Start()
    {
        maxSpawnTime = parameters._maxTimeToSpawnWave;
        minSpawnTime = parameters._minTimeToSpawnWave;

        mainTimerStartTime = mainTimer.GetStartTime();
        curTime = maxSpawnTime;

        formationButton.onClick.AddListener(TaskOnClick);
        formationButton.image.color = lineColor;
        mode.text = "Line";
    }

    void FixedUpdate()
    {
        if (!Field.endOfGame) curTime -= 1 * Time.fixedDeltaTime;

        if (curTime <= 0)
        {
            curTime = Mathf.CeilToInt(Mathf.Lerp(minSpawnTime, maxSpawnTime, mainTimer.GetCurTime() / mainTimerStartTime));
            if (isSpawnLine)
            {
                ghostSpawner.SpawnGhostLine();
            }
            else
            {
                StartCoroutine(ghostSpawner.SpawnGhostWedge());
            }
        }

        timer.text = string.Format("{0:0.0}", curTime);
    }

    void TaskOnClick()
    {
        isSpawnLine = !isSpawnLine;
        mode.text = isSpawnLine ? "LINE" : "WEDGE";
        formationButton.image.color = isSpawnLine ? lineColor : wedgeColor;
    }
}
