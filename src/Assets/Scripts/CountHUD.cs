using UnityEngine;
using TMPro;

public class CountHUD : MonoBehaviour
{
    [SerializeField] private Bubble bubble;
    private TextMeshProUGUI counterText;

    private int lastCollectedPoints = -1;
    private int lastTotalPoints = -1;

    private void Start()
    {
        if (bubble == null)
        {
            Debug.LogError("Bubble reference not set in CountHUD!");
            return;
        }
        
        counterText = GetComponent<TextMeshProUGUI>();
        if (counterText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on the same object as CountHUD!");
            return;
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
            counterText.text = $"Bubbles: {collected}/{total}";
            lastCollectedPoints = collected;
            lastTotalPoints = total;
        }
    }
}
