using Android.Media;
using System.Reflection;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace MP_Music_Player.Services;

public partial class AudioPlayer {

  private bool _isSubscribed;

  //Handles Local Android Notification
  public partial void HandlePlatformSpecificPopup() {
    var track = this._currentTrack ?? throw new NullReferenceException();

    if (!this._isSubscribed) {
      LocalNotificationCenter.Current.NotificationActionTapped += this._Current_NotificationActionTapped;
      this._isSubscribed = true;
    }
    
    var request = new NotificationRequest {
      NotificationId = 1337,
      Title = track.Title,
      Description = track.CombinedArtistNames,
      BadgeNumber = 42,
      Image = new NotificationImage { ResourceName = "record.png" },
      CategoryType = NotificationCategoryType.Status,
      Silent = true,
      Android = new AndroidOptions {
        IconLargeName = new AndroidIcon("record.png"),
        IconSmallName = new AndroidIcon("record.png")
      }
    };

    request.Show();
  }

  //todo: enum for these numbers, see MauiProgram.cs
  private void _Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e) {
    if (this._currentTrack == null)
      return;

    switch (e.ActionId) {
      case 100:
        break;

      case 101:
        var duration = this._currentTrack.Duration;
        this.Seek(duration.TotalSeconds);


        LocalNotificationCenter.Current.Cancel(e.Request.NotificationId);
        break;
    }
  }

  private MediaPlayer _LoadMediaPlayer() {
    var player = this._currentPlayer ?? throw new NullReferenceException();

    //access private field per reflection
    //todo: cache fieldInfo
    //todo: extra partial platform specific class for this
    var type = typeof(Plugin.Maui.Audio.AudioPlayer);
    var field = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
      .First(f => f.FieldType == typeof(MediaPlayer));

    return (MediaPlayer)field.GetValue(player)!;
  }

}
