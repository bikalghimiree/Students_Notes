using Students_Notes.Data;
using Students_Notes.Models;

namespace Students_Notes.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly DatabaseHelper _database;
        private ImageSource _profileImageSource;
        private bool _isTapFrameVisible;
        private string _userName;
        private bool _isNameChanged;

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

        public ProfilePage()
        {
            InitializeComponent();
            _database = new DatabaseHelper(Path.Combine(FileSystem.AppDataDirectory, "students.db3"));
            BindingContext = this;
            LoadUserData();
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
                    Email = "student@gmail.com"
                };
                await _database.SaveUserAsync(user);
                UserName = user.Name;
                ProfileImageSource = "profile_placeholder.png";
            }
        }

        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var user = _database.GetUserAsync().GetAwaiter().GetResult();
                IsNameChanged = user != null && e.NewTextValue != user.Name;
            }
            catch (Exception ex)
            {
                // Handle any potential errors
                IsNameChanged = false;
            }
        }

        private async void OnSaveNameClicked(object sender, EventArgs e)
        {
            try
            {
                var user = await _database.GetUserAsync();
                if (user != null)
                {
                    user.Name = UserName;
                    await _database.SaveUserAsync(user);
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
            // Implement password change logic
            await DisplayAlert("Change Password", "Password change functionality will be implemented here", "OK");
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
            await Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData();
        }
    }
} 