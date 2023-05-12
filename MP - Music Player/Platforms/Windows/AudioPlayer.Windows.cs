using Windows.Media.Playback;
using Windows.Media;
using Windows.Media.Core;
using System.Reflection;

namespace MP_Music_Player.Services;

public partial class AudioPlayer {


  public partial void HandlePlatformSpecificPopup() {
    var track = this._currentTrack ?? throw new NullReferenceException();

    var mediaPlayer = this._LoadMediaPlayer();

    //set properties
    var source = (MediaSource)mediaPlayer.Source;
    var mediaItem = MediaPlaybackItem.FindFromMediaSource(source);

    var props = mediaItem.GetDisplayProperties();
    var musicProperties = props.MusicProperties;
    props.Type = MediaPlaybackType.Music;
    musicProperties.Title = track.Title;
    musicProperties.Artist = track.CombinedArtistNames;
    //todo: set thumbnail
    mediaItem.ApplyDisplayProperties(props);
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