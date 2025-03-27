using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public int FramesPerSec { get; protected set; }
    public int MinFPS { get; protected set; } = int.MaxValue;
    public int MaxFPS { get; protected set; } = int.MinValue;
    public float AvgFPS { get; protected set; }

    [SerializeField] private float frequency = 0.5f;

    private TextMeshProUGUI counter;
    private int totalFrames;
    private float totalTime;
    private string filePath;

    private void Start()
    {
        counter = GetComponent<TextMeshProUGUI>();
        if (counter == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on this GameObject.");
            enabled = false;
            return;
        }
        counter.text = "";
        // filePath = Path.Combine(Application.dataPath, "FPSLog.csv");
        // WriteToCSV("Time,FramesPerSec");
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        for (;;)
        {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);

            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
            MinFPS = Mathf.Min(MinFPS, FramesPerSec);
            MaxFPS = Mathf.Max(MaxFPS, FramesPerSec);
            totalFrames += frameCount;
            totalTime += timeSpan;
            AvgFPS = totalFrames / totalTime;

            counter.text = $"FPS: {FramesPerSec}\nMin FPS: {MinFPS}\nMax FPS: {MaxFPS}\nAvg FPS: {AvgFPS:F2}";

            // WriteToCSV($"{Time.realtimeSinceStartup},{FramesPerSec}");
        }
    }

    private void WriteToCSV(string data)
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(data);
        }
    }
}