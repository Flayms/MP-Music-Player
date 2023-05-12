using MP_Music_Player.Models;
using MP_Music_Player.Models.EventArgs;
using Plugin.Maui.Audio;
using Exception = System.Exception;

namespace MP_Music_Player.Services;

/// <summary>
/// Save-Wrapper for all the audio-playback-logic
/// </summary>
public partial class AudioPlayer {

  private readonly IAudioManager _audioManager;
  private IAudioPlayer? _currentPlayer;
  public event EventHandler? PlaybackEnded;

  /// <summary>
  /// This <see langword="event"/> raises whenever the playback starts / stops changes.
  /// </summary>
  public event EventHandler<IsPlayingEventArgs>? IsPlayingChanged;

  public bool HasTrackSelected => this._currentTrack != null;

  /// <summary>
  /// Gets the position of the current track in seconds or 0 if nothing is playing.
  /// </summary>
  public double PositionInS => this._currentPlayer?.CurrentPosition ?? 0;

  /// <summary>
  /// <see langword="true"/> if this player is currently playing.
  /// </summary>
  public bool IsPlaying {
    get => this._isPlaying;
    private set {
      if (value == this._isPlaying)
        return;

      this._isPlaying = value;
      this.IsPlayingChanged?.Invoke(this, new IsPlayingEventArgs(this.IsPlaying));
    }
  }

  private bool _isPlaying;

  /// <summary>
  /// <see langword="true"/> if there is a paused, selected track.
  /// </summary>
  public bool IsPaused { get; private set; }

  private Track? _currentTrack;

  public AudioPlayer(IAudioManager audioManager) {
    this._audioManager = audioManager;
  }

  /// <summary>
  /// Starts playback on the given <see cref="Track"/>.
  /// </summary>
  /// <param name="track">The track to play.</param>
  public void Play(Track track) {
    this._CreatePlayer(track);
    this.Play();
  }

  /// <summary>
  /// Adds the given <see cref="Track"/> without starting playback.
  /// </summary>
  /// <param name="track">The track to set.</param>
  public void SetTrack(Track track) => this._CreatePlayer(track);

  /// <summary>
  /// Starts playback on the given <see cref="Track"/> at a specific position.
  /// </summary>
  /// <param name="track">The track to play.</param>
  /// <param name="timeInSeconds">The position to seek to in seconds.</param>
  public void PlayAtTime(Track track, double timeInSeconds) {
    var player = this._CreatePlayer(track);
    player.Seek(timeInSeconds);
    this.Play();
  }

  /// <summary>
  /// Sets the current playback position.
  /// </summary>
  /// <param name="positionInS">The position in seconds to seek to.</param>
  /// <exception cref="Exception">Throws if no track is selected.</exception>
  public void Seek(double positionInS) {
    var player = this._currentPlayer;
    if (player == null) {
      if (this._currentTrack == null)
        throw new NullReferenceException("Can't seek without a selected track.");

      player = this._CreatePlayer(this._currentTrack);
    }

    player.Seek(positionInS);
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
    this.IsPlaying = false;
    this.IsPaused = false;
  }

  private IAudioPlayer _CreatePlayer(Track track) {
    this._RemoveCurrentPlayerSafely();

    var stream = File.OpenRead(track.Path);
    var player = this._audioManager.CreatePlayer(stream);

    player.PlaybackEnded += this._CurrentPlayer_PlaybackEnded;

    this._currentTrack = track;
    this._currentPlayer = player;

    this.HandlePlatformSpecificPopup();

    return player;
  }

  public partial void HandlePlatformSpecificPopup();

  /// <summary>
  /// Begins or continues playback.
  /// </summary>
  public void Play() {
    if (this._currentPlayer == null)
      throw new NullReferenceException(nameof(this._currentPlayer));

    this._currentPlayer.Play();
    this.IsPlaying = true;
    this.IsPaused = false;
  }

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
    } catch (System.Runtime.InteropServices.COMException) { }

    this.IsPlaying = false;
    this.IsPaused = false;
  }

  public double GetProgressPercent() {
    if (this._currentTrack == null)
      return 0;

    var duration = this._currentTrack.Duration.TotalSeconds;
    var position = this.PositionInS;

    if (duration == 0)
      return 0;

    var result = position / duration;
    return result;
  }

  public void JumpToPercent(double value) {
    if (this._currentTrack == null)
      throw new NullReferenceException(nameof(this._currentTrack));

    var duration = this._currentTrack.Duration.TotalSeconds;
    var position = duration * value;

    this.Seek(position);
  }
}