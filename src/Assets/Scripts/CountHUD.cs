using UnityEngine;
using TMPro;

public class CountHUD : MonoBehaviour
{
    [SerializeField] private Bubble bubble;
    [SerializeField] private TextMeshProUGUI counterText;

    private int lastCollectedPoints = -1;
    private int lastTotalPoints = -1;

    private void Start()
    {
        if (bubble == null)
        {
            Debug.LogError("Bubble reference not set in CountHUD!");
            return;
        }
        
        if (counterText == null)
        {
            counterText = GetComponent<TextMeshProUGUI>();
        }

        UpdateCounterText();
    }

    private void Update()
    {
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        var (collected, total) = bubble.GetPointCount();
        
        if (collected != lastCollectedPoints || total != lastTotalPoints)
        {
            counterText.text = $"Puntos: {collected}/{total}";
            lastCollectedPoints = collected;
            lastTotalPoints = total;
        }
    }
}
