using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaction logic for ServerNameTextBox.xaml
    /// </summary>
    public partial class ServerNameTextBox : TextBox
    {
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            string invalidChars = new(Path.GetInvalidFileNameChars());
            string regexString = "^[^";
            regexString += Regex.Escape(invalidChars);
            regexString += "]+$";
            Regex regex = new(regexString);
            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
            base.OnPreviewTextInput(e);
        }
    }
}
