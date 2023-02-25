using Music_Player_Maui.Models;
using Plugin.Maui.Audio;
using Exception = System.Exception;

namespace Music_Player_Maui.Services; 

//todo: add doc-comments
//todo: sometimes throws exceptions, sometimes handles stuff safely -> need to be more consistent

/// <summary>
/// Save-Wrapper for all the audio-playback-logic
/// </summary>
public class AudioPlayer {

  private readonly IAudioManager _audioManager;
  private IAudioPlayer? _currentPlayer;
  public event EventHandler? PlaybackEnded;

  public bool HasTrackSelected => this._currentPlayer != null;

  /// <summary>
  /// Gets the duration of the current track in seconds or 0 if nothing is playing.
  /// </summary>
  public double DurationInS => this._currentPlayer?.Duration ?? 0;

  /// <summary>
  /// Gets the position of the current track in seconds or 0 if nothing is playing.
  /// </summary>
  public double PositionInS => this._currentPlayer?.CurrentPosition ?? 0;

  public AudioPlayer(IAudioManager audioManager) {
    this._audioManager = audioManager;
  }

  public void Play(Track track) {
    this._currentPlayer = this._CreatePlayer(track);
    this._currentPlayer.Play();
  }

  public void PlayAtTime(Track track, double timeInSeconds) {
    var player = this._currentPlayer = this._CreatePlayer(track);
    player.Seek(timeInSeconds);
    player.Play();
  }

  public void Seek(double positionInS) {
    if (this._currentPlayer == null)
      throw new Exception("Can't seek if no track is selected!");

    this._currentPlayer.Seek(positionInS);
  }

  /// <summary>
  /// Safely stops playback if is playing currently.
  /// </summary>
  public void Stop() => this._RemoveCurrentPlayerSafely();

  /// <summary>
  /// Pauses the current track.
  /// </summary>
  /// <exception cref="Exception">Throws if no song is selected.</exception>
  public void Pause() {
    if (this._currentPlayer == null)
      throw new Exception("Can't pause if no song is selected!");

    this._currentPlayer.Pause();
  }

  private IAudioPlayer _CreatePlayer(Track track) {
    this._RemoveCurrentPlayerSafely();

    var stream = File.OpenRead(track.Path);
    var player = this._audioManager.CreatePlayer(stream);
    player.PlaybackEnded += this._CurrentPlayer_PlaybackEnded;

    return player;
  }

  /// <summary>
  /// Begins or continues playback.
  /// </summary>
  public void Play() => this._currentPlayer?.Play();

  private void _CurrentPlayer_PlaybackEnded(object? sender, EventArgs e) {
    this._RemoveCurrentPlayerSafely();
    this.PlaybackEnded?.Invoke(this, EventArgs.Empty);
  }

  /// <summary>
  /// Removes current-player safely if exists.
  /// </summary>
  private void _RemoveCurrentPlayerSafely() {
    if (this._currentPlayer == null)
      return;

    try {
      this._currentPlayer.PlaybackEnded -= this._CurrentPlayer_PlaybackEnded;
      this._currentPlayer.Stop();
      this._currentPlayer.Dispose();
      this._currentPlayer = null;

      //throws when a track gets double-clicked instead of once
    } catch (System.Runtime.InteropServices.COMException e) {

    }
  }
}