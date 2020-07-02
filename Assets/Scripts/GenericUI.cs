using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GenericUI : MonoBehaviour
{
    // *** Public/Inspector variables ***

    [Header("Images")]
    // Backdrop image
    [SerializeField] private RawImage backdropImage;
    // Background image
    [SerializeField] private RawImage backgroundImage;

    // *** Private variables ***

    // Dialog to load the background image
    private System.Windows.Forms.OpenFileDialog loadBackgroundDialog;
    // Dialog to load the backdrop image
    private System.Windows.Forms.OpenFileDialog loadBackdropDialog;
    // Dialog to save a screenshot
    private System.Windows.Forms.SaveFileDialog saveScreenshotDialog;

    // Text to display the combo code
    private Text comboCodeText;

    // Start is called before the first frame update
    void Start()
    {
        // Create and initialize the file dialogs
        loadBackgroundDialog = new System.Windows.Forms.OpenFileDialog();
        loadBackdropDialog = new System.Windows.Forms.OpenFileDialog();
        saveScreenshotDialog = new System.Windows.Forms.SaveFileDialog();
        DPD_Utility.InitOpenDialog(ref loadBackgroundDialog, "Select background image", false);
        DPD_Utility.InitOpenDialog(ref loadBackdropDialog, "Select backdrop image", false);
        DPD_Utility.InitSaveDialog(ref saveScreenshotDialog, "Save screenshot", "myText");

        comboCodeText = GameObject.FindGameObjectWithTag("ComboCode").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() { }

    #region Public Functions

    /// <summary> Button Input. Load a single image and set as background image </summary>
    public void LoadBackground()
    {
        System.Windows.Forms.DialogResult dialogResult = loadBackgroundDialog.ShowDialog();
        if (dialogResult == System.Windows.Forms.DialogResult.OK)
        {
            Debug.Log("Filename: " + loadBackgroundDialog.FileName);
            StartCoroutine(LoadAndSetTexturesFromDiskAsync(loadBackgroundDialog.FileName, backgroundImage));
        }
    }

    /// <summary> Button Input. Load a single image and set as backdrop image </summary>
    public void LoadBackdrop()
    {
        System.Windows.Forms.DialogResult dialogResult = loadBackdropDialog.ShowDialog();
        if (dialogResult == System.Windows.Forms.DialogResult.OK)
        {
            Debug.Log("Filename: " + loadBackdropDialog.FileName);
            StartCoroutine(LoadAndSetTexturesFromDiskAsync(loadBackdropDialog.FileName, backdropImage));
        }
    }

    /// <summary> Button Input. Load a single image and set as backdrop image </summary>
    public void MakeScreenshot()
    {
        // Reinitialize the save dialog with the current combo code as the filename
        DPD_Utility.InitSaveDialog(ref saveScreenshotDialog, "Save screenshot", comboCodeText.text);

        System.Windows.Forms.DialogResult dialogResult = saveScreenshotDialog.ShowDialog();

        if (dialogResult == System.Windows.Forms.DialogResult.OK)
        {
            Debug.Log("Filename: " + saveScreenshotDialog.FileName);
            ScreenCapture.CaptureScreenshot(saveScreenshotDialog.FileName);
        }

        // TODO: Play screenshot sound?
    }

    /// <summary> Quit the application </summary>
    public void Exit()
    {
        Application.Quit();
    }

    #endregion

    #region Private Functions

    #region Enumerators

    // Load texture file from disk and set as texture of displayImage
    IEnumerator LoadAndSetTexturesFromDiskAsync(string filename, RawImage displayImage)
    {
        // Retrieve texture from path
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filename);
        yield return uwr.SendWebRequest();

        // Display loaded texture
        displayImage.texture = (((DownloadHandlerTexture)uwr.downloadHandler).texture);
    }

    #endregion

    #endregion
}
