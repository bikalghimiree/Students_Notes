using Microsoft.Extensions.Logging;
using Students_Notes.Data;
using CommunityToolkit.Maui;

namespace Students_Notes
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register your pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<Views.ProfilePage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DatabaseHelper>(s => 
                new DatabaseHelper(Path.Combine(FileSystem.AppDataDirectory, "students.db3")));

            return builder.Build();
        }
    }
}
