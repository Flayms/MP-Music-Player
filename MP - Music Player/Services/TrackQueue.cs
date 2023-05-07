using MP_Music_Player.Enums;
using MP_Music_PLayer.Enums;
using MP_Music_Player.Extensions;
using MP_Music_Player.Models;
using MP_Music_Player.Models.EventArgs;
using MP_Music_Player.Services.Repositories;
using Track = MP_Music_Player.Models.Track;

namespace MP_Music_Player.Services;

public class TrackQueue {

  private readonly AudioPlayer _audioPlayer;
  private readonly IQueuedTracksRepository _queuedTrackRepository;

  private readonly MusicContext _context;

  public event EventHandler<TrackEventArgs>? NewSongSelected;
  public event EventHandler<IsPlayingEventArgs>? IsPlayingChanged;

  //todo: save in settings / db
  public LoopMode LoopMode { get; set; } = LoopMode.None;
  
  //todo: make distinction between previously played and previous track in queue
  public IList<Track> HistoryTracks { get; private set; } = new List<Track>(); //todo: could be stack
  public Track? CurrentTrack { get; private set; }

  public IList<Track> NextUpTracks { get; private set; } = new List<Track>(); //todo: could be queue
  public IList<Track> QueuedTracks { get; private set; } = new List<Track>(); //todo: could be queue

  //the queue, how it was first initiated without any dequeued elements
  private IList<Track> _originalQueue = new List<Track>();

  //todo: maybe move into audio-player
  public bool IsPlaying {
    get => this._isPlaying;
    private set {
      this._isPlaying = value;
      this.IsPlayingChanged?.Invoke(this, new IsPlayingEventArgs(this.IsPlaying));
    }
  }

  //indicates if the track was already paused or if its the first play
  private bool _wasPaused;
  private bool _isPlaying;

  public bool IsShuffle { get; private set; }

  public TrackQueue(AudioPlayer audioPlayer, IQueuedTracksRepository queuedTrackRepository, MusicContext context) {
    this._audioPlayer = audioPlayer;
    this._queuedTrackRepository = queuedTrackRepository;
    this._context = context;

    audioPlayer.PlaybackEnded += this._AudioPlayer_PlaybackEnded;
  }

  public void LoadTracksFromDb() {
    var dbCurrentTrack = this._context.CurrentTracks.FirstOrDefault();

    if (dbCurrentTrack != null) {
      var track = this.CurrentTrack = dbCurrentTrack.Track;
      this._audioPlayer.SetTrack(track);
      this._audioPlayer.Seek(dbCurrentTrack.ProgressInSeconds);
      this.NewSongSelected?.Invoke(this, new TrackEventArgs(track));
    }

    this.HistoryTracks = this._queuedTrackRepository.HistoryTracks.ToList();
    this.NextUpTracks = this._queuedTrackRepository.NextUpTracks.ToList();
    this.QueuedTracks = this._queuedTrackRepository.QueuedTracks.ToList();
    this._originalQueue = new List<Track>(this.QueuedTracks);
  }

  /// <summary>
  /// Changes current track without starting playback.
  /// </summary>
  /// <param name="newTrack">The new track to be selected.</param>
  /// <param name="addLastTrackToHistory"><c>True</c> if the last playing <see cref="Track"/> should be added to <see cref="HistoryTracks"/>.</param>
  public void ChangeCurrentTrack(Track newTrack, bool addLastTrackToHistory = true) {
    if (addLastTrackToHistory && this.CurrentTrack != null)
      this.HistoryTracks.Add(this.CurrentTrack);

    this.CurrentTrack = newTrack;
    this._audioPlayer.Stop();

    this.NewSongSelected?.Invoke(this, new TrackEventArgs(newTrack));
    this._wasPaused = false;
  }

  /// <summary>
  /// Changes the current-queue to the newly provided tracks, including setting the first track as <see cref="CurrentTrack"/>.
  /// </summary>
  /// <remarks>Doesn't change <see cref="NextUpTracks"/>, because user specific picks should be kept.</remarks>
  /// <param name="tracks">The new tracks to use as the queue.</param>
  public void ChangeQueue(IEnumerable<Track> tracks) {
    //todo: should be done with enumeration instead of changing to list
    var list = tracks.ToList();

    this._originalQueue = new List<Track>(list);
    this.ChangeCurrentTrack(list.Dequeue());
    this.QueuedTracks = list;
  }

  /// <summary>
  /// Adds a track to the first position of the <see cref="NextUpTracks"/> (it's gonna play as the next track).
  /// </summary>
  /// <param name="track">The track to insert at the front of the queue.</param>
  public void InsertNextUp(Track track) => this.NextUpTracks.Insert(0, track);

  /// <summary>
  /// Adds track to the end of the <see cref="NextUpTracks"/>
  /// (it's gonna play after all the other user picks but before the common queue).
  /// </summary>
  /// <param name="track"></param>
  public void AddToNextUp(Track track) => this.NextUpTracks.Add(track);

  /// <summary>
  /// Adds the track to the very last playback position.
  /// </summary>
  /// <param name="track"></param>
  public void AddToEndOfQueue(Track track) => this.QueuedTracks.Add(track);

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
  }

  /// <inheritdoc cref="ChangeCurrentTrack"/>
  /// <summary>
  /// Changes the current track and starts playback.
  /// </summary>
  /// <param name="track">The new track to play.</param>
  /// <param name="addLastTrackToHistory"></param>
  public void Play(Track track, bool addLastTrackToHistory = true) {
    this.ChangeCurrentTrack(track, addLastTrackToHistory);
    this._audioPlayer.Play(track);
    this.IsPlaying = true;
  }

  private void _AudioPlayer_PlaybackEnded(object? _, EventArgs __) {
    if (this.LoopMode == LoopMode.Current)
      this._audioPlayer.Play(this.CurrentTrack!);
    else
      this.Next();
  }

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

    var playbackPosition = this._audioPlayer.PositionInS;

    if (playbackPosition < 0)
      //start first time playback from start
      this._audioPlayer.Play(this.CurrentTrack);
    else
      //start playing at specific time for the first time
      this._audioPlayer.PlayAtTime(this.CurrentTrack, playbackPosition);

    this.IsPlaying = true;
  }

  public void Pause() {
    this._audioPlayer.Pause();
    this._wasPaused = true;
    this.IsPlaying = false;
  }

  //todo: what if end of queue reached
  public void Next() {
    if (this.NextUpTracks.Count > 0)
      this.Play(this.NextUpTracks.Dequeue());

    else if (this.QueuedTracks.Count > 0)
      this.Play(this.QueuedTracks.Dequeue());

    //repeat queue
    else if (this.LoopMode == LoopMode.Queue) {
      this.QueuedTracks = new List<Track>(this._originalQueue);
      this.Play(this.QueuedTracks.Dequeue());
    }
  }

  public void Previous() {
    if (this.HistoryTracks.Count <= 0)
      return;

    var lastTrack = this.HistoryTracks.Pop();

    if (this.CurrentTrack != null)
      this.InsertNextUp(this.CurrentTrack);

    this.Play(lastTrack, false);
  }

  public void Shuffle() {
    var tracks = new List<Track>(this.QueuedTracks);

    tracks.Shuffle();
    this.ChangeQueue(tracks);
    this.IsShuffle = true;
    this.Play();
  }

  //todo: should rather be logic in audio-player than in queue
  public void JumpToPercent(double value) {
    if (this.CurrentTrack == null)
      throw new NullReferenceException(nameof(this.CurrentTrack));

    var duration = this.CurrentTrack.Duration.TotalSeconds;
    var position = duration * value;

    if (!this._audioPlayer.HasTrackSelected)
      this._audioPlayer.PlayAtTime(this.CurrentTrack, position);
    else
      this._audioPlayer.Seek(position);
  }

  public double GetProgressPercent() {
    if (this.CurrentTrack == null)
      return 0;

    var duration = this.CurrentTrack.Duration.TotalSeconds;
    var position = this._audioPlayer.PositionInS;

    if (duration == 0)
      return 0;

    var result = position / duration;
    return result;
  }

  //todo: db logic shouldn't be here i think
  public void SaveToDb() {

    //todo: don't delete history
    this._context.QueuedTracks.Clear();

    var queuedTracks = this.QueuedTracks
      .Select(t => new DbQueuedTrack(t, QueuedType.Queued))
      .Concat(this.NextUpTracks
        .Select(t => new DbQueuedTrack(t, QueuedType.NextUp)));
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