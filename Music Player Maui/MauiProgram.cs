using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Music_Player_Maui.Services;
using Music_Player_Maui.ViewModels;
using Music_Player_Maui.Views.Pages;
using Music_Player_Maui.Models;
using Music_Player_Maui.Views.UserControls;
using Plugin.Maui.Audio;

namespace Music_Player_Maui;

public static class MauiProgram {
  public static MauiApp CreateMauiApp() {
    var builder = MauiApp.CreateBuilder();
    var services = builder.Services;

    builder
      .UseMauiApp<App>()
      .UseMauiCommunityToolkit()
      .ConfigureFonts(fonts => {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
      });

    services.AddSingleton<Settings>();

    _CreateDatabaseService(services);

    services.AddSingleton(AudioManager.Current);

    services.AddSingleton<TrackQueue>();
    services.AddSingleton<TagReadingService>();
    services.AddSingleton<MusicFileParsingService>();
    services.AddSingleton<MusicService>();

    services.AddSingleton<SongsPage>();
    services.AddSingleton<SongsViewModel>();

    services.AddSingleton<TabLibraryPage>();
    services.AddSingleton<TabLibraryViewModel>();

    services.AddSingleton<SettingsPage>();
    services.AddSingleton<SettingsViewModel>();

    services.AddSingleton<BottomTrackPlayerView>();
    services.AddSingleton<TrackPlayerViewModel>();

    services.AddScoped<SearchPage>();
    services.AddScoped<SearchViewModel>();

    services.AddSingleton<QueuePage>();
    services.AddSingleton<QueueViewModel>();

#if DEBUG
    builder.Logging.AddDebug();
#endif

    return builder.Build();
  }

  private static void _CreateDatabaseService(IServiceCollection services) {
    services.AddSingleton(provider => {
      const string dbName = "sqliteDb.db";
      var applicationDataPath = FileSystem.Current.AppDataDirectory;
      var dbFilePath = Path.Combine(applicationDataPath, dbName);

      //for preventing migration issues
      if (VersionTracking.IsFirstLaunchForVersion(VersionTracking.CurrentVersion)) {
        if (File.Exists(dbFilePath))
          File.Delete(dbFilePath);

        var settings = provider.GetService<Settings>()!;
        settings.ReadFromCache = false;
      }

      var context = new MusicContext(dbFilePath);
      context.Database.EnsureCreated();

      context.SaveChanges();

      return context;
    });
  }
}
