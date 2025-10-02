using UnityEngine;

public class RotateWarning : MonoBehaviour
{
    [Header("Assign your warning GameObject here")]
    public GameObject warningPanel;

    void Start()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false); // Hide at start
    }

    void Update()
    {
        // If width > height => landscape
        if (Screen.width > Screen.height)
        {
            if (warningPanel != null && !warningPanel.activeSelf)
                warningPanel.SetActive(true);
        }
        else
        {
            if (warningPanel != null && warningPanel.activeSelf)
                warningPanel.SetActive(false);
        }
    }
}
