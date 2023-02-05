using Music_Player_Maui.Models;
using Plugin.Maui.Audio;

namespace Music_Player_Maui.Services; 

public class TrackQueue {
  private readonly IAudioManager _audioManager;

  // public event EventHandler<TrackEventArgs> NewSongSelected;

  public List<Track> NextUpTracks { get; private set; } = new();
  public List<Track> QueuedTracks { get; private set; } = new();
  //public List<Track> TrackHistory { get; private set; } = new (); //todo: implement properly!
  public List<Track> AllTracks { get; private set; } = new();

  public Track CurrentTrack {
    get => this._currentTrack;
    private set {
      //if (this._currentTrack != null)
      //this.TrackHistory.Add(this._currentTrack);

      this._currentTrack = value;
    //  this.NewSongSelected?.Invoke(this, new TrackEventArgs(value));
      this._wasPaused = false;
    }
  }

  public int Index { get; private set; } //todo: make index setting public?

  private Track _currentTrack;

  private bool _wasPaused; //indicates if the track was already paused or if its the first play   

  private IAudioPlayer? _currentPlayer;
  // private readonly IMediaManager _mediaManager;


  public bool IsShuffle { get; private set; }

  public TrackQueue(IAudioManager audioManager) {
    this._audioManager = audioManager;


    //  var manager = CrossMediaManager.Current;
  //  this._mediaManager = manager;
  //  manager.MediaItemFinished += this._MediaItemFinished;
  }

  public void FullyCreateQueue(List<Track> nextUps, List<Track> queued, Track current) {
    this.NextUpTracks = nextUps;
    this.QueuedTracks = queued;
    this.CurrentTrack = current;
    this._ReloadFullQueue();
  }

  public void ChangeQueue(List<Track> tracks) {
    this.QueuedTracks = tracks;
  //  this.CurrentTrack = tracks.Dequeue();
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
  /// Removes first occurence of this track
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

  //private void _MediaItemFinished(object sender, MediaManager.Media.MediaItemEventArgs e) => this.Next();

  public void Play(Track track) {
    this.CurrentTrack = track;

    var stream = File.OpenRead(track.Path);

    this._currentPlayer?.Stop();

    this._currentPlayer = this._audioManager.CreatePlayer(stream);
    this._currentPlayer.Play();

    //this._mediaManager.Play(track.Path);
  }

  //todo: works fine with first start but doesnt with new song selected, differentiate with check of crossmediainitization
  public void Play() {
    

    //  if (!this._wasPaused) {
    //    var progress = this.CurrentTrack.GetProgress();
    //
    //    if (progress != TimeSpan.Zero)
    //      this._PlayAtTime(progress);
    //    else
    //      this._mediaManager.Play(this.CurrentTrack.Path);
    //
    //  } else
    //    this._mediaManager.Play();
  }

  private void _PlayAtTime(TimeSpan time) {
  //  var task = this._mediaManager.Extractor.CreateMediaItem(this.CurrentTrack.Path);
  //  task.Wait();
  //
  //  this._mediaManager.Play(task.Result, time);
  //  this._mediaManager.SeekTo(time);
  }

  public void Pause() {
   // this._mediaManager.Pause();
   // this._wasPaused = true;
  }

  public void Next() {
   // if (this.NextUpTracks.Count > 0)
   //   this.Play(this.NextUpTracks.Dequeue());
   //
   // else if (this.QueuedTracks.Count > 0)
   //   this.Play(this.QueuedTracks.Dequeue());

    ++this.Index;
  }

  //todo: implement again
  public void Previous() {
    //--this.Index;
    //this._PlayTrackAsync();
  }

  public void Shuffle() {
   // this.QueuedTracks.Shuffle();
    var track = this.QueuedTracks[0];
    this.QueuedTracks.RemoveAt(0);
    this.CurrentTrack = track;
    this.IsShuffle = true;
    this._ReloadFullQueue();
    this.Play();
  }


}