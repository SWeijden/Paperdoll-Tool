using UnityEngine;

public class PopupManager : MonoBehaviour
{
    // Reference to popup object
    [SerializeField] private GameObject popupObject;

    public void ShowInfoPanel()
    {
        popupObject.SetActive(true);
    }

    public void HideInfoPanel()
    {
        popupObject.SetActive(false);
    }
}
