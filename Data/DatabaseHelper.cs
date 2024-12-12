using SQLite;
using Students_Notes.Models;

namespace Students_Notes.Data
{
    public class DatabaseHelper
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseHelper(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.DropTableAsync<User>().Wait();
            _database.CreateTableAsync<User>(CreateFlags.None).Wait();
        }

        public async Task<User> GetUserAsync()
        {
            var users = await _database.Table<User>().ToListAsync();
            return users.FirstOrDefault();
        }

        public async Task<int> SaveUserAsync(User user)
        {
            var existingUser = await GetUserAsync();
            if (existingUser == null)
            {
                return await _database.InsertAsync(user);
            }
            user.Id = existingUser.Id;
            return await _database.UpdateAsync(user);
        }

        public async Task<int> UpdateUserProfileImageAsync(byte[] imageData)
        {
            var user = await GetUserAsync();
            if (user != null)
            {
                user.ProfileImage = imageData;
                return await _database.UpdateAsync(user);
            }
            return 0;
        }

        public async Task<int> UpdateUserNameAsync(string name)
        {
            var user = await GetUserAsync();
            if (user != null)
            {
                user.Name = name;
                return await _database.UpdateAsync(user);
            }
            return 0;
        }

        public async Task<int> UpdateUserPasswordAsync(string password)
        {
            var user = await GetUserAsync();
            if (user != null)
            {
                user.Password = password;
                return await _database.UpdateAsync(user);
            }
            return 0;
        }
    }
} 