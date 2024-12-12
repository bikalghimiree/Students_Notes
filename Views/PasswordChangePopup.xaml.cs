using CommunityToolkit.Maui.Views;

namespace Students_Notes.Views
{
    public partial class PasswordChangePopup : Popup
    {
        public PasswordChangePopup()
        {
            InitializeComponent();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            var result = new PasswordChangeResult
            {
                CurrentPassword = CurrentPasswordEntry.Text,
                NewPassword = NewPasswordEntry.Text,
                ConfirmPassword = ConfirmPasswordEntry.Text
            };
            Close(result);
        }
    }

    public class PasswordChangeResult
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
} 