using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;


public class PartsBarElement : MonoBehaviour
{
//#pragma warning disable 0219
//#pragma warning restore 0219

    // *** Public variables ***

    // Event triggered when hiding this PartsBar
    [HideInInspector] public UnityEvent hideElementEvent = new UnityEvent();
    // Index of current element, starts at 1
    [HideInInspector] public int partID = 1;
    // Index of current element comboCode offset
    [HideInInspector] public int comboCodeID = 0;

    // *** Public/Inspector variables ***

    [Header("Objects")]
    // Image prefab
    [SerializeField] private GameObject ImagePrefab;

    [Header("Images")]
    // Transparent image
    [SerializeField] private Texture transparentImage;

    [Header("Buttons")]
    // Hide PartsBar Button
    public Button hideElementButton;
    // Navigation buttons
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;

    // *** Private variables ***

    // Container for displaying images
    private GameObject ImageContainer;
    // Text to display the combo code
    private Text comboCodeText;
    // Current image to be displayed
    private RawImage displayReference;

    // List of textures for the current PartsBar
    private List<Texture> textureList = new List<Texture>();
    // List of names of the current textures
    private List<string> namesList = new List<string>();

    // Dialog to load the images of the current PartsBar
    private System.Windows.Forms.OpenFileDialog loadPartImagesDialog;

    // Index of currently displayed image
    private int imageIndex = 0;

    #region Native Functions

    void Awake()
    {
        ImageContainer = GameObject.FindGameObjectWithTag("PaperDollContainer");
        comboCodeText = GameObject.FindGameObjectWithTag("ComboCode").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Reference to set active image
        displayReference = Instantiate(ImagePrefab, ImageContainer.transform, false).GetComponent<RawImage>();

        // Create and initialize the file dialog
        loadPartImagesDialog = new System.Windows.Forms.OpenFileDialog();
        DPD_Utility.InitOpenDialog(ref loadPartImagesDialog, "Select images", true);
    }

    // Update is called once per frame
    void Update() { }

    #endregion

    #region Public Functions

    /// <summary> Load images for parts via a popup dialog </summary>
    public void LoadPartsBarImagesFromDisk()
    {
        // Load new textures from the selected directory
        System.Windows.Forms.DialogResult dialogResult = loadPartImagesDialog.ShowDialog();
        if (dialogResult == System.Windows.Forms.DialogResult.OK)
        {
            // Reset the state of the PartsBar
            ResetPartsBarState();

            for (int i = 0; i < loadPartImagesDialog.FileNames.Length; i++)
            {
                StartCoroutine(LoadTexturesFromDiskAsync(
                    loadPartImagesDialog.FileNames[i],                    // Full path
                    loadPartImagesDialog.SafeFileNames[i],                // Filename
                    (i == loadPartImagesDialog.FileNames.Length-1))       // Boolean if last
                    );
            }
        }
    }

    /// <summary> Add default characters to combo code text </summary>
    public void AppendComboCode()
    {
        // Show section of the displayed combo code text
        string comboText = comboCodeText.text;
        comboText = comboText.Insert(comboCodeID, "__");
        comboCodeText.text = comboText;
    }

    /// <summary> Hide the current PartsBar </summary>
    public void HidePartsBarElement()
    {
        // Remove section of the displayed combo code text
        string comboText = comboCodeText.text;
        comboText = comboText.Remove(comboCodeID, 2);
        comboCodeText.text = comboText;

        // Reset the state of the current PartsBar
        ResetPartsBarState();
        // Disable the current object
        gameObject.SetActive(false);
        // Trigger an update for the PartsBarManager when hiding an element
        hideElementEvent.Invoke();
    }

    /// <summary> Button Input. Change displayed texture of current PartsBar </summary>
    public void NavigateArrow(bool moveRight)
    {
        // Increment or loop index around
        if (moveRight)
        {
            if (imageIndex < textureList.Count-1)
                imageIndex++;
            else
                imageIndex = 0;
        }
        // Decrement or loop index around
        else
        {
            if (imageIndex > 0)
                imageIndex--;
            else
                imageIndex = textureList.Count-1;
        }

        SetDisplayImage(imageIndex);
        UpdateComboCodeText();
    }

    #endregion

    #region Private Functions

    /// <summary> Update section of the combo code </summary>
    private void UpdateComboCodeText()
    {
        string comboText = comboCodeText.text;
        comboText = comboText.Remove(comboCodeID, 2);

        // Get first 2 characters of filename
        char[] fileChars = { '_', '_' };
        if (namesList[imageIndex].Length > 0)
            fileChars[0] = namesList[imageIndex][0];
        if (namesList[imageIndex].Length > 1)
            fileChars[1] = namesList[imageIndex][1];

        // Insert apply newNameCode to comboText
        string newNameCode = "" + fileChars[0] + fileChars[1];
        comboText = comboText.Insert(comboCodeID, newNameCode);
        comboCodeText.text = comboText;
    }

    /// <summary> Control enabling and disabling the texture swapping navigation arrows </summary>
    private void UpdateNavigationArrows()
    {
        // Updated navigation arrows
        if (textureList.Count < 2)
        {
            leftArrow.interactable = false;
            rightArrow.interactable = false;
        }
        else
        {
            leftArrow.interactable = true;
            rightArrow.interactable = true;
        }
    }

    /// <summary> Update the current displayed image </summary>
    private void SetDisplayImage(int id)
    {
        // Set image texture if index is valid
        if (textureList.Count > id)
            displayReference.texture = textureList[id];
    }

    /// <summary> Reset the state of the PartsBar </summary>
    private void ResetPartsBarState()
    {
        imageIndex = 0;
        namesList.Clear();
        textureList.Clear();
        displayReference.texture = transparentImage;
        UpdateNavigationArrows();
    }

    #region Enumerators

    /// <summary> Asynchronous texture loading </summary>
    IEnumerator LoadTexturesFromDiskAsync(string fullPath, string fullname, bool lastItem)
    {
        // Retrieve texture from path
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(fullPath);
        yield return uwr.SendWebRequest();

        // Add texture and name to lists
        textureList.Add(((DownloadHandlerTexture)uwr.downloadHandler).texture);
        namesList.Add(fullname.Split('.')[0]);

        // Inside enum to account for delayed trigger. Append empty name and update the UI
        if (lastItem)
        {
            // Set last image as empty
            textureList.Add(transparentImage);
            namesList.Add("__");
            UpdateComboCodeText();

            // Set the first loaded texture
            imageIndex = 0;
            SetDisplayImage(imageIndex);

            UpdateNavigationArrows();
        }
    }

    #endregion

    #endregion
}
