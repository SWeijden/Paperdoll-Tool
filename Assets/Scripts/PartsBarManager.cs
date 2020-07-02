using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartsBarManager : MonoBehaviour
{
    // *** Public variables ***

    public int visiblePartsBars;

    // *** Public/Inspector variables ***

    [Header("Objects")]
    // PartsBarElement prefab used to instantiate
    [SerializeField] private PartsBarElement PartsBarObject;
    // PartsBar container object
    [SerializeField] private GameObject partsBarContainer;

    [Header("Buttons")]
    // Button to creating a new PartsBarElement
    [SerializeField] private Button ShowPartsBarButton;

    // *** Private variables ***

    // Maximum number of PartsBar elements
    private const int PARTS_LIMIT = 5;

    // List of created PartsBar elements
    private List<PartsBarElement> partsBarList = new List<PartsBarElement>();

    #region Native Functions

    // Start is called before the first frame update
    void Start()
    {
        // Spawn PartsBar elements for given amount
        for (int i = 0; i < PARTS_LIMIT; i++)
        {
            AddPartsBar(i + 1);
        }

        // Set first bar to visible
        if (partsBarList.Count > 0)
        {
            partsBarList[0].gameObject.SetActive(true);
            partsBarList[0].AppendComboCode();
            visiblePartsBars = 1;
        }
    }

    // Update is called once per frame
    void Update() { }

    #endregion

    #region Public Functions

    /// <summary>
    /// Button Input. Add a PartsBar to the displayed list
    /// NOTE: Can only be called when valid, else disabled
    /// </summary>
    public void ShowNextPartsBar()
    {
        // Increment the displayed partsbars
        visiblePartsBars++;

        int partsCounted = 0;
        bool shownNext = false;
        // Enable the next item
        for (int i = 0; i < partsBarList.Count; i++)
        {
            // After the first is shown, allow any element to be hidden
            partsBarList[i].hideElementButton.interactable = true;

            // Show the next PartsBarElement and count newly visible PartsBar
            if (!partsBarList[i].isActiveAndEnabled)
            {
                if (!shownNext)
                {
                    partsBarList[i].gameObject.SetActive(true);
                    UpdateComboCodeIndices();
                    partsBarList[i].AppendComboCode();
                    shownNext = true;
                    partsCounted++;
                }
            }
            // Count enabled PartsBars
            else
                partsCounted++;
        }

        // If the last bar is shown, disable the button to add another PartsBar
        if (partsCounted >= partsBarList.Count)
            UpdateShowNextPartsBarButton(false);
        else
            UpdateShowNextPartsBarButton(true);
    }

    /// <summary> Update whether another PartsBar can be shown </summary>
    public void UpdateShowNextPartsBarButton(bool active)
    {
        // When an element is hidden, it always means an element can be added back again
        ShowPartsBarButton.interactable = active;
    }

    /// <summary> Update whether another PartsBar can be shown </summary>
    public void UpdateButtonHideElement()
    {
        // Update counter for number of visible PartsBars
        visiblePartsBars--;

        // Don't allow removing the last PartsBar
        if (visiblePartsBars < 2)
        {
            foreach (PartsBarElement element in partsBarList)
            {
                if (element.isActiveAndEnabled)
                    element.hideElementButton.interactable = false;
            }
        }

        UpdateComboCodeIndices();
    }

    #endregion

    #region Private Functions

    /// <summary> Add a prefab for loading a set of images for a layer </summary>
    private void AddPartsBar(int id)
    {
        PartsBarElement newPartObject = Instantiate(PartsBarObject);
        newPartObject.transform.SetParent(partsBarContainer.transform);
        newPartObject.transform.localPosition = Vector3.zero;
        newPartObject.transform.localScale = Vector3.one;
        newPartObject.name = "PartsBar_0" + id;
        newPartObject.partID = id;
        newPartObject.gameObject.SetActive(false);
        newPartObject.hideElementEvent.AddListener(() => UpdateShowNextPartsBarButton(true));
        newPartObject.hideElementEvent.AddListener(() => UpdateButtonHideElement());

        // Assign new element to stored list
        partsBarList.Add(newPartObject);

        // Turn on RemoveBarButton for each PartsBarElement
        // TODO - IMPROVE: inefficient, but good enough. Could also store the first and set directly
        PartsBarElement[] existingBars = GetComponentsInChildren<PartsBarElement>();
        foreach (PartsBarElement myBar in existingBars)
        {
            Button[] childButtons = myBar.GetComponentsInChildren<Button>();
            foreach (Button btn in childButtons)
            {
                if (btn.name == "RemoveBarButton")
                    btn.interactable = true;
            }
        }
    }

    /// <summary> Update the combo code offset indices for each PartsBar element </summary>
    private void UpdateComboCodeIndices()
    {
        int activeCountX2 = 0;
        foreach (PartsBarElement element in partsBarList)
        {
            // Set comboCodeID as an even, zero based counter matching the active PartsBars
            if (element.isActiveAndEnabled)
            {
                element.comboCodeID = activeCountX2;
                activeCountX2 += 2;
            }
        }
    }

    #endregion
}
