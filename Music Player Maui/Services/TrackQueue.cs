using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Music_Player_Maui.Enums;
using Music_Player_Maui.Extensions;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services.Repositories;
using Plugin.Maui.Audio;
using File = System.IO.File;
using Track = Music_Player_Maui.Models.Track;

namespace Music_Player_Maui.Services;

//todo: maybe split between queue logic and actual playing logic

//todo: separate caching and reading from repo logic
public class TrackQueue {
  private readonly IAudioManager _audioManager;

  private readonly IQueuedTracksRepository _queuedTrackRepository;

  private readonly Settings _settings;

  private readonly MusicContext _context;
  // private readonly MusicContext _context;

  public event EventHandler<TrackEventArgs>? NewSongSelected;
  public event EventHandler<IsPlayingEventArgs>? IsPlayingChanged;

  public IList<Track> NextUpTracks { get; private set; }
  public IList<Track> QueuedTracks { get; private set; }

  //todo: maybe make repo property for this and read directly from there
  public List<Track> AllTracks { get; private set; } = new();

  public Track? CurrentTrack { get; private set; }

  public void ChangeCurrentTrack(Track track) {
    this.CurrentTrack = track;
    Task.Run(() => this._settings.CurrentTrackId = track.Id);

    this.NewSongSelected?.Invoke(this, new TrackEventArgs(track));
    this._wasPaused = false;
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

  private bool _wasPaused; //indicates if the track was already paused or if its the first play   

  private IAudioPlayer? _currentPlayer;
  private bool _isPlaying;


  public bool IsShuffle { get; private set; }

  public TrackQueue(IAudioManager audioManager, IQueuedTracksRepository queuedTrackRepository, Settings settings, MusicContext context) {
    this._audioManager = audioManager;
    this._queuedTrackRepository = queuedTrackRepository;
    this._settings = settings;
    this._context = context;

    //todo: too slow for constructor code
    var id = this._settings.CurrentTrackId;
    if (id != null)
      //todo: firstOrDefault or first??
      this.CurrentTrack = context.Tracks.FirstOrDefault(t => t.Id == id);

    this.NextUpTracks = queuedTrackRepository.NextUpTracks.ToList();
    this.QueuedTracks = queuedTrackRepository.QueuedTracks.ToList();
  }

  //public void FullyCreateQueue(List<Track> nextUps, List<Track> queued, Track current) {
  //  this.NextUpTracks = nextUps;
  //  this.QueuedTracks = queued;
  //  this.CurrentTrack = current;
  //  this._ReloadFullQueue();
  //}

  //also handles all the db-stuff
  public void ChangeQueue(IEnumerable<Track> tracks) {
    var sw = new Stopwatch();
    sw.Start();

    //todo: should be done with enumeration instead of changing to list
    var list = tracks.ToList();
    Trace.WriteLine($"Create linked list: {sw.ElapsedMilliseconds}");
    sw.Restart();

    this.ChangeCurrentTrack(list.Dequeue());
    Trace.WriteLine($"change current track time: {sw.ElapsedMilliseconds}");
    sw.Restart();

    Trace.WriteLine($"db change queue time: {sw.ElapsedMilliseconds}");
    
    this._ReloadFullQueue();

    Trace.WriteLine($"reload queue time: {sw.ElapsedMilliseconds}");
  }

  public void InsertNextUp(Track track) {
    this.NextUpTracks.Insert(0, track);
    this._ReloadFullQueue();
  }

  public void AddToNextUp(Track track) {
    this.NextUpTracks.Add(track);
    this._ReloadFullQueue();
  }

  public void AddToEndOfQueue(Track track) {
    this.QueuedTracks.Add(track);
    this._ReloadFullQueue();
  }

  //todo: implement these again
  //public void JumpToNextUpTrack(Track track) => this._JumpToClickedTrack(track, this.NextUpTracks);
  //
  //public void JumpToQueueTrack(Track track) => this._JumpToClickedTrack(track, this.QueuedTracks);
  //
  //private void _JumpToClickedTrack(Track track, IReadOnlyCollection<Track> tracks) {
  //  for (var i = 0; i < tracks.Count; ++i) {
  //    if (tracks[i] == track) {
  //      tracks.RemoveRange(0, i + 1);
  //      this.Play(track);
  //      break;
  //    }
  //  }
  //
  //  this._ReloadFullQueue();
  //}

  /// <summary>
  /// Removes first occurrence of this track
  /// </summary>
  /// <param name="track">The track</param>
  public void Remove(Track track) {
    this._queuedTrackRepository.RemoveFromQueue(track);
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
    this.ChangeCurrentTrack(track);

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
      this.Play(this.CurrentTrack); //todo: initializing CurrentTrack twice like this
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
    //end of queue reached
    if (!this._queuedTrackRepository.TryDequeueTrack(out var nextTrack))
      return;

    this.Play(nextTrack);

    ++this.Index;
  }

  //todo: implement again
  public void Previous() {
    //--this.Index;
    //this._PlayTrackAsync();
  }

  //todo: check if this still works like intended
  //todo: decide what should happen exactly with nextUp tracks on shuffle
  //todo: check performance
  public void Shuffle() {
    var tracks = new List<Track>(this.QueuedTracks);

    tracks.Shuffle();
    this.ChangeQueue(new LinkedList<Track>(tracks));
    this._ReloadFullQueue();
    this.IsShuffle = true;
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

  public void SaveToDb() {

    //todo: don't delete history
    this._context.QueuedTracks.Clear();

    var queuedTracks = this.QueuedTracks
      .Select(t => new QueuedTrack { Track = t, Type = QueuedType.Queued })
      .Concat(this.NextUpTracks
        .Select(t => new QueuedTrack { Track = t, Type = QueuedType.NextUp}))
      //todo: current track can be null!
      .Concat(new[] {new QueuedTrack {Track = this.CurrentTrack, Type = QueuedType.Current}});

    this._context.QueuedTracks.AddRange(queuedTracks);

    this._context.SaveChanges();
  }

}