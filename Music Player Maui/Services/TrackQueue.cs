using System.Diagnostics;
using Music_Player_Maui.Extensions;
using Music_Player_Maui.Models;
using Plugin.Maui.Audio;
using TagLib.Matroska;
using File = System.IO.File;
using Track = Music_Player_Maui.Models.Track;

namespace Music_Player_Maui.Services; 

//todo: maybe split between queue logic and actual playing logic

public class TrackQueue {
  private readonly IAudioManager _audioManager;

   public event EventHandler<TrackEventArgs>? NewSongSelected;
   public event EventHandler<IsPlayingEventArgs>? IsPlayingChanged;

  public List<Track> NextUpTracks { get; private set; } = new();
  public List<Track> QueuedTracks { get; private set; } = new();
  //public List<Track> TrackHistory { get; private set; } = new (); //todo: implement properly!
  public List<Track> AllTracks { get; private set; } = new();

  public Track? CurrentTrack {
    get => this._currentTrack;
    private set {
      //if (this._currentTrack != null)
      //this.TrackHistory.Add(this._currentTrack);
      this._currentTrack = value ?? throw new ArgumentNullException(nameof(value));
      this.NewSongSelected?.Invoke(this, new TrackEventArgs(value));
      this._wasPaused = false;
    }
  }

  public int Index { get; private set; } //todo: make index setting public?

  public double CurrentTrackDurationInS => this._currentPlayer?.Duration ?? 0;
  public double CurrentTrackPositionInS => this._currentPlayer?.CurrentPosition ?? 0;

  public bool IsPlaying {
    get => this._isPlaying;
    private set {
      this._isPlaying = value; 
      this.IsPlayingChanged?.Invoke(this, new IsPlayingEventArgs(this.IsPlaying));
    }
  }


  private Track? _currentTrack;

  private bool _wasPaused; //indicates if the track was already paused or if its the first play   

  private IAudioPlayer? _currentPlayer;
  private bool _isPlaying;


  public bool IsShuffle { get; private set; }

  public TrackQueue(IAudioManager audioManager) {
    this._audioManager = audioManager;
  }

  public void FullyCreateQueue(List<Track> nextUps, List<Track> queued, Track current) {
    this.NextUpTracks = nextUps;
    this.QueuedTracks = queued;
    this.CurrentTrack = current;
    this._ReloadFullQueue();
  }

  public void ChangeQueue(List<Track> tracks) {
    this.QueuedTracks = tracks;
    this.CurrentTrack = tracks.Dequeue();
    this._ReloadFullQueue();
  }

  public void AddNext(Track track) {
    this.NextUpTracks.Insert(0, track);
    this._ReloadFullQueue();
  }

  public void AddToQueue(Track track) {
    this.NextUpTracks.Add(track);
    this._ReloadFullQueue();
  }

  public void AddToEndOfQueue(Track track) {
    this.QueuedTracks.Add(track);
    this._ReloadFullQueue();
  }

  public void JumpToNextUpTrack(Track track) => this._JumpToClickedTrack(track, this.NextUpTracks);

  public void JumpToQueueTrack(Track track) => this._JumpToClickedTrack(track, this.QueuedTracks);

  private void _JumpToClickedTrack(Track track, List<Track> tracks) {
    for (var i = 0; i < tracks.Count; ++i) {
      if (tracks[i] == track) {
        tracks.RemoveRange(0, i + 1);
        this.Play(track);
        break;
      }
    }

    this._ReloadFullQueue();
  }

  /// <summary>
  /// Removes first occurrence of this track
  /// </summary>
  /// <param name="track">The track</param>
  public void Remove(Track track) {
    if (!this.NextUpTracks.Remove(track))
      this.QueuedTracks.Remove(track);

    this._ReloadFullQueue();
  }

  private void _ReloadFullQueue() {
    //var tracks = new List<Track>(this.TrackHistory) { this.CurrentTrack };
    var tracks = new List<Track> { this.CurrentTrack };
    tracks.AddRange(this.NextUpTracks);
    tracks.AddRange(this.QueuedTracks);
    this.AllTracks = tracks;

    this.Index = 0;
    //this.Index = this.TrackHistory.Count + 1;
  }

  public void Play(Track track) {
    this.CurrentTrack = track;

    this._RemoveCurrentPlayerSafely();
    this._currentPlayer = this._CreatePlayer(track);

    this._currentPlayer.Play();

    this.IsPlaying = true;
  }

  private IAudioPlayer _CreatePlayer(Track track) {
    var stream = File.OpenRead(track.Path);
    var player = this._currentPlayer = this._audioManager.CreatePlayer(stream);
    player.PlaybackEnded += this._CurrentPlayer_PlaybackEnded;

    return player;
  } 

  private void _CurrentPlayer_PlaybackEnded(object? sender, EventArgs e) {
    this._RemoveCurrentPlayerSafely();
    this.Next();
  }

  private void _RemoveCurrentPlayerSafely() {
    if (this._currentPlayer == null)
      return;

    this._currentPlayer.PlaybackEnded -= this._CurrentPlayer_PlaybackEnded;
    this._currentPlayer.Stop();
    this._currentPlayer.Dispose();
    this._currentPlayer = null;
  }

  //todo: works fine with first start but doesn't with new song selected, differentiate with check of cross-media-initialization
  public void Play() {
    this.IsPlaying = true;

    if (this._wasPaused) {
      this._currentPlayer.Play();
      return;
    }

    this._RemoveCurrentPlayerSafely();

    var progress = this._currentPlayer?.CurrentPosition ?? 0;

    if (progress > 0)
      this._PlayAtTime(progress);
    else
      this.Play(this.CurrentTrack);
  }

  private void _PlayAtTime(double timeInSeconds) {
    this._currentPlayer?.Stop();

    var player = this._currentPlayer = this._CreatePlayer(this.CurrentTrack);
    player.Seek(timeInSeconds);
    player.Play();
    this.IsPlaying = true;
  }

  public void Pause() {
    this._currentPlayer!.Pause();
    this._wasPaused = true;
    this.IsPlaying = false;
  }

  public void Next() {
   if (this.NextUpTracks.Count > 0)
     this.Play(this.NextUpTracks.Dequeue());
   
   else if (this.QueuedTracks.Count > 0)
     this.Play(this.QueuedTracks.Dequeue());

   ++this.Index;
  }

  //todo: implement again
  public void Previous() {
    //--this.Index;
    //this._PlayTrackAsync();
  }

  public void Shuffle() {
    this.QueuedTracks.Shuffle();
    var track = this.QueuedTracks[0];
    this.QueuedTracks.RemoveAt(0);
    this.CurrentTrack = track;
    this.IsShuffle = true;
    this._ReloadFullQueue();
    this.Play();
  }

  public void JumpToPercent(double value) {
    if (this.CurrentTrack == null)
      throw new ArgumentNullException(nameof(this.CurrentTrack));

    var duration = this._currentPlayer?.Duration ?? 0; //todo: better null checks needed beforehand
    var position = duration * value;
    this._currentPlayer?.Seek(position);
  }

  public double GetProgressPercent() {
    if (this.CurrentTrack == null)
      return 0;

    var duration = this._currentPlayer?.Duration ?? 0; //todo: throws exception quite often
    var position = this._currentPlayer?.CurrentPosition ?? 0;

    if (duration == 0)
      return 0;

    var result = position / duration;
    return result;
  }

}