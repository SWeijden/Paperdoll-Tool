static class DPD_Utility
{
    /// <summary> Initialize a referenced OpenFileDialog to load image(s) </summary>
    public static void InitOpenDialog(ref System.Windows.Forms.OpenFileDialog ofd, string title, bool multiSelect)
    {
        ofd.Title = title;
        ofd.Filter = "Image Files (*.png, *.jpg, *.gif, *.bmp)|*.png;*.jpg;*.gif;*.bmp";
        ofd.DefaultExt = "png";
        ofd.Multiselect = multiSelect;
        ofd.InitialDirectory = "../";
        ofd.RestoreDirectory = false;
    }

    /// <summary> Initialize a referenced SaveFileDialog to save a png </summary>
    public static void InitSaveDialog(ref System.Windows.Forms.SaveFileDialog sfd, string title, string fileName)
    {
        sfd.Title = title;// "Save screenshot";
        sfd.Filter = "Image File (*.png)|*.png";
        sfd.FileName = fileName;
        sfd.DefaultExt = "png";
        sfd.InitialDirectory = "../";
        sfd.RestoreDirectory = false;
        sfd.AddExtension = true;
    }
}