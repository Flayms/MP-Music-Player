using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using MP_Music_Player.Services;
using MP_Music_Player.ViewModels;
using MP_Music_Player.Views.Pages;
using MP_Music_Player.Models;
using MP_Music_PLayer.Services;
using MP_Music_Player.Services.Repositories;
using MP_Music_PLayer.ViewModels;
using MP_Music_PLayer.Views.Pages;
using MP_Music_Player.Views.UserControls;
using Plugin.LocalNotification;
using Plugin.Maui.Audio;
using AudioPlayer = MP_Music_Player.Services.AudioPlayer;

namespace MP_Music_Player;

public static class MauiProgram {
  public static MauiApp CreateMauiApp() {
    var builder = MauiApp.CreateBuilder();
    var services = builder.Services;

    builder
      .UseMauiApp<App>()

#if ANDROID
      .UseLocalNotification(_SetupLocalNotification)
#endif
      .UseMauiCommunityToolkit()
      .ConfigureFonts(fonts => {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
      })
      .ConfigureLifecycleEvents(events => {
#if WINDOWS
        events.AddWindows(windows => windows
          .OnClosed((_, _) => ServiceHelper.GetService<TrackQueue>().SaveToDb()));
#endif
      });

    services.AddSingleton<Settings>();

    _CreateDatabaseService(services);
    _AddDefaultServices(services);
    _AddPageAndViewModelServices(services);

#if DEBUG
    builder.Logging.AddDebug();
#endif

    return builder.Build();
  }

  private static void _AddDefaultServices(IServiceCollection services) {
    services.AddSingleton(_ => new AudioPlayer(AudioManager.Current));
    services.AddSingleton<TrackQueue>();
    services.AddSingleton<TagReadingService>();
    services.AddSingleton<MusicFileParsingService>();
    services.AddSingleton<MusicService>();
    services.AddSingleton<TrackOptionsService>();
    services.AddSingleton<IQueuedTracksRepository, QueuedTracksRepository>();
    services.AddSingleton<MusicDirectoryService>();
  }

  private static void _AddPageAndViewModelServices(IServiceCollection services) {
    services.AddSingleton<SongsPage>();
    services.AddSingleton<SongsViewModel>();

    services.AddSingleton<TabLibraryPage>();
    services.AddSingleton<TabLibraryViewModel>();

    services.AddSingleton<SettingsPage>();
    services.AddSingleton<SettingsViewModel>();

    services.AddSingleton<BottomTrackPlayerView>();
    services.AddSingleton<BottomTrackViewModel>();

    services.AddScoped<SearchPage>();
    services.AddScoped<SearchViewModel>();

    services.AddSingleton<QueuePage>();
    services.AddSingleton<QueueViewModel>();

    services.AddScoped<GroupsPage>();
    services.AddScoped<GroupsViewModel>();

    services.AddScoped<HistoryPage>();
    services.AddScoped<HistoryViewModel>();

    services.AddTransient<TrackListViewModel>();

    services.AddSingleton<BigTrackViewModel>();
  }

  private static void _CreateDatabaseService(IServiceCollection services) {
    services.AddSingleton(provider => {
      const string dbName = "sqliteDb.db";
      var applicationDataPath = FileSystem.Current.AppDataDirectory;

      if (!Directory.Exists(applicationDataPath))
        Directory.CreateDirectory(applicationDataPath);

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

  //todo: images not working
  private static void _SetupLocalNotification(ILocalNotificationBuilder builder) {
    builder.AddCategory(new NotificationCategory(NotificationCategoryType.Status) {
      ActionList = new HashSet<NotificationAction>(new List<NotificationAction> {
        new(100) {
          Title = "Previous",
          Android = {
            LaunchAppWhenTapped = true,
            IconName = {
              ResourceName = "record.png"
            }
          }
        },
        new(101) {
          Title = "Next",
          Android = {
            LaunchAppWhenTapped = false,
            IconName = {
              ResourceName = "next_dark.png"
            }
          }
        }
      })
    });
  }
}
