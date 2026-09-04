using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject targetPrefab;
    public TextMeshProUGUI hitsText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI accuracyText;

    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalAccuracyText;

    public float spawnRadiusX = 2f;
    public float spawnRadiusY = 1.5f;
    public float spawnDistance = 8f;

    private int hits = 0;
    private int shotsFired = 0;
    private float timeRemaining = 30f;
    private bool gameActive = true;

    private Vector3 anchorForward;
    private Vector3 anchorRight;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(InitializeAnchor());
    }

    IEnumerator InitializeAnchor()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 camForward = Camera.main.transform.forward;
        anchorForward = new Vector3(camForward.x, 0f, camForward.z).normalized;
        anchorRight = Vector3.Cross(Vector3.up, anchorForward).normalized;

        SpawnTarget();
    }

    void Update()
    {
        if (!gameActive) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            gameActive = false;
            ShowGameOverScreen();
        }

        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);
        UpdateAccuracy();
    }

    public bool IsGameActive()
    {
        return gameActive;
    }

    public void RegisterShot()
    {
        if (gameActive) shotsFired++;
    }

    public void OnTargetHit()
    {
        if (!gameActive) return;
        hits++;
        hitsText.text = "Hits: " + hits;
        SpawnTarget();
    }

    void SpawnTarget()
    {
        float randX = Random.Range(-spawnRadiusX, spawnRadiusX);
        float randY = Random.Range(-spawnRadiusY, spawnRadiusY);

        Vector3 spawnPos = Camera.main.transform.position
                          + anchorForward * spawnDistance
                          + anchorRight * randX
                          + Vector3.up * randY;

        Instantiate(targetPrefab, spawnPos, Quaternion.identity);
    }

    void UpdateAccuracy()
    {
        if (shotsFired == 0)
        {
            accuracyText.text = "Accuracy: 100%";
            return;
        }
        float acc = (float)hits / shotsFired * 100f;
        accuracyText.text = "Accuracy: " + Mathf.RoundToInt(acc) + "%";
    }

    void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Final Hits: " + hits;

        float acc = shotsFired == 0 ? 100f : (float)hits / shotsFired * 100f;
        finalAccuracyText.text = "Final Accuracy: " + Mathf.RoundToInt(acc) + "%";
    }

    public void RestartGame()
    {
        Target[] existingTargets = FindObjectsOfType<Target>();
        foreach (Target t in existingTargets)
        {
            Destroy(t.gameObject);
        }

        hits = 0;
        shotsFired = 0;
        timeRemaining = 30f;
        gameActive = true;

        gameOverPanel.SetActive(false);
        hitsText.text = "Hits: 0";
        UpdateAccuracy();

        SpawnTarget();
    }
}