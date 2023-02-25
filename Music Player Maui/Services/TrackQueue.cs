using Music_Player_Maui.Enums;
using Music_Player_Maui.Extensions;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services.Repositories;
using Track = Music_Player_Maui.Models.Track;

namespace Music_Player_Maui.Services;

//todo: maybe split between queue logic and actual playing logic

//todo: separate caching and reading from repo logic
public class TrackQueue {

 private readonly AudioPlayer _audioPlayer;
 private readonly IQueuedTracksRepository _queuedTrackRepository;

  private readonly Settings _settings;

  private readonly MusicContext _context;

  public event EventHandler<TrackEventArgs>? NewSongSelected;
  public event EventHandler<IsPlayingEventArgs>? IsPlayingChanged;

  public IList<Track> NextUpTracks { get; private set; }
  public IList<Track> QueuedTracks { get; private set; }

  //todo: maybe make repo property for this and read directly from there
  public List<Track> AllTracks { get; private set; } = new();

  public Track? CurrentTrack { get; private set; }

  public int Index { get; private set; } //todo: make index setting public?

  public double CurrentTrackDurationInS => this._audioPlayer.DurationInS;
  public double CurrentTrackPositionInS => this._audioPlayer.PositionInS;

  //todo: maybe move into audio-player
  public bool IsPlaying {
    get => this._isPlaying;
    private set {
      this._isPlaying = value;
      this.IsPlayingChanged?.Invoke(this, new IsPlayingEventArgs(this.IsPlaying));
    }
  }

  private bool _wasPaused; //indicates if the track was already paused or if its the first play   

  //private IAudioPlayer? _currentPlayer;
  private bool _isPlaying;


  public bool IsShuffle { get; private set; }

  public TrackQueue(AudioPlayer audioPlayer, IQueuedTracksRepository queuedTrackRepository, Settings settings, MusicContext context) {
    this._audioPlayer = audioPlayer;
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

    audioPlayer.PlaybackEnded += this._AudioPlayer_PlaybackEnded;
  }

  //todo: shouldn't do the saving in settings
  public void ChangeCurrentTrack(Track track) {
    this.CurrentTrack = track;
    this._audioPlayer.Stop();
    Task.Run(() => this._settings.CurrentTrackId = track.Id);

    this.NewSongSelected?.Invoke(this, new TrackEventArgs(track));
    this._wasPaused = false;
  }

  /// <summary>
  /// Changes the current-queue to the newly provided <see cref="tracks"/>, including setting the first track as <see cref="CurrentTrack"/>.
  /// </summary>
  /// <remarks>Doesn't change <see cref="NextUpTracks"/>, because user specific picks should be kept.</remarks>
  /// <param name="tracks"></param>
  public void ChangeQueue(IEnumerable<Track> tracks) {
    //todo: should be done with enumeration instead of changing to list
    var list = tracks.ToList();

    this.ChangeCurrentTrack(list.Dequeue());
    this.QueuedTracks = list;
    this._ReloadFullQueue();
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
    if (!this.NextUpTracks.Remove(track))
      this.QueuedTracks.Remove(track);

    this._ReloadFullQueue();
  }

  /// <summary>
  /// Refills the Property <see cref="AllTracks"/> with all tracks in current Queue.
  /// </summary>
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
    this._audioPlayer.Play(track);
    this.IsPlaying = true;
  }

  private void _AudioPlayer_PlaybackEnded(object? sender, EventArgs e) => this.Next();

  /// <summary>
  /// Resumes or starts playback.
  /// </summary>
  /// <exception cref="Exception">Throws if there's no current track set.</exception>
  public void Play() {
    if (this.CurrentTrack == null)
      throw new Exception("Can't play if there's no current track set!");

    //resume playback
    if (this._wasPaused) {
      this._audioPlayer.Play();
      this.IsPlaying = true;
      return;
    }

    //start first time playback from start
    this._audioPlayer.Play(this.CurrentTrack);
    this.IsPlaying = true;
  }

  public void Pause() {
    this._audioPlayer.Pause();
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
    this.ChangeQueue(tracks);
    this._ReloadFullQueue();
    this.IsShuffle = true;
    this.Play();
  }

  public void JumpToPercent(double value) {
    if (this.CurrentTrack == null)
      throw new NullReferenceException(nameof(this.CurrentTrack));

    var duration = this._audioPlayer.DurationInS; //todo: better null checks needed beforehand
    var position = duration * value;

    if (!this._audioPlayer.HasTrackSelected)
      this._audioPlayer.PlayAtTime(this.CurrentTrack, position);
    else
      this._audioPlayer.Seek(position);
  }

  public double GetProgressPercent() {
    if (this.CurrentTrack == null)
      return 0;

    var duration = this._audioPlayer.DurationInS; //todo: throws exception quite often
    var position = this._audioPlayer.PositionInS;

    if (duration == 0)
      return 0;

    var result = position / duration;
    return result;
  }

  public void SaveToDb() {

    //todo: don't delete history
    this._context.QueuedTracks.Clear();

    var queuedTracks = this.QueuedTracks
      .Select(t => new DbQueuedTrack { Track = t, Type = QueuedType.Queued })
      .Concat(this.NextUpTracks
        .Select(t => new DbQueuedTrack { Track = t, Type = QueuedType.NextUp }));
      //todo: current track can be null!
     // .Concat(new[] {new DbQueuedTrack {Track = this.CurrentTrack, Type = QueuedType.Current}});

    this._context.CurrentTracks.Clear();
    var currentPosition = this._audioPlayer.PositionInS;
    if (this.CurrentTrack != null)
      this._context.CurrentTracks.Add(new DbCurrentTrack { Track = this.CurrentTrack, ProgressInSeconds = currentPosition });

    this._context.QueuedTracks.AddRange(queuedTracks);

    this._context.SaveChanges();
  }

}