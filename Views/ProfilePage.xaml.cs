using Students_Notes.Data;
using Students_Notes.Models;
using CommunityToolkit.Maui.Views;

namespace Students_Notes.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly DatabaseHelper _database;
        private ImageSource _profileImageSource;
        private bool _isTapFrameVisible;
        private string _userName;
        private bool _isNameChanged;
        private bool _isSynced;

        public ImageSource ProfileImageSource
        {
            get => _profileImageSource;
            set
            {
                _profileImageSource = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNameChanged
        {
            get => _isNameChanged;
            set
            {
                _isNameChanged = value;
                OnPropertyChanged();
            }
        }

        public bool IsSynced
        {
            get => _isSynced;
            set
            {
                _isSynced = value;
                OnPropertyChanged();
            }
        }

        public ProfilePage()
        {
            InitializeComponent();
            _database = new DatabaseHelper(Path.Combine(FileSystem.AppDataDirectory, "students.db3"));
            BindingContext = this;
            LoadUserData();
            IsSynced = true;
        }

        private async void LoadUserData()
        {
            var user = await _database.GetUserAsync();
            if (user != null)
            {
                UserName = user.Name;
                if (user.ProfileImage != null)
                {
                    ProfileImageSource = ImageSource.FromStream(() => new MemoryStream(user.ProfileImage));
                }
                else
                {
                    ProfileImageSource = "profile_placeholder.png";
                }
            }
            else
            {
                // Create default user if none exists
                user = new User
                {
                    Name = "John Doe",
                    Email = "student@gmail.com",
                    Password = "password"
                };
                await _database.SaveUserAsync(user);
                UserName = user.Name;
                ProfileImageSource = "profile_placeholder.png";
            }
        }

        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                return;

            IsNameChanged = true;
        }

        private async void OnSaveNameClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    await DisplayAlert("Error", "Name cannot be empty", "OK");
                    return;
                }

                var user = await _database.GetUserAsync();
                if (user != null)
                {
                    await _database.UpdateUserNameAsync(UserName);
                    IsNameChanged = false;
                    await DisplayAlert("Success", "Name updated successfully", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to update name", "OK");
            }
        }

        private async void OnProfileImageTapped(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Pick Profile Image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var imageData = memoryStream.ToArray();

                    // Save to database
                    await _database.UpdateUserProfileImageAsync(imageData);

                    // Update UI
                    ProfileImageSource = ImageSource.FromStream(() => new MemoryStream(imageData));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to load image", "OK");
            }
        }

        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            var popup = new PasswordChangePopup();
            var result = await this.ShowPopupAsync(popup) as PasswordChangeResult;

            if (result == null) // User cancelled
                return;

            try
            {
                var user = await _database.GetUserAsync();

                if (user == null)
                {
                    await DisplayAlert("Error", "No user found in database", "OK");
                    return;
                }

                if (result.CurrentPassword != user.Password)
                {
                    await DisplayAlert("Error", "Current password is incorrect", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(result.NewPassword))
                {
                    await DisplayAlert("Error", "New password cannot be empty", "OK");
                    return;
                }

                if (result.NewPassword != result.ConfirmPassword)
                {
                    await DisplayAlert("Error", "Passwords do not match", "OK");
                    return;
                }

                var updateResult = await _database.UpdateUserPasswordAsync(result.NewPassword);
                if (updateResult > 0)
                {
                    await DisplayAlert("Success", "Password changed successfully", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update password in database", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to update password", "OK");
            }
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "No");
            if (answer)
            {
                // Implement sign out logic
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        // Also override the hardware back button
        protected override bool OnBackButtonPressed()
        {
            Shell.Current.GoToAsync("//MainPage");
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData();
        }
    }
} 