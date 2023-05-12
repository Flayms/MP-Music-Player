using MP_Music_Player.Enums;
using MP_Music_PLayer.Enums;
using MP_Music_Player.Extensions;
using MP_Music_Player.Models;
using MP_Music_Player.Models.EventArgs;
using MP_Music_Player.Services.Repositories;
using Track = MP_Music_Player.Models.Track;

namespace MP_Music_Player.Services;

//todo: maybe split up in TrackQueue (model) and TrackQueueService (acting on model)
public class TrackQueue {

  private readonly AudioPlayer _player;
  private readonly IQueuedTracksRepository _queuedTrackRepository;

  private readonly MusicContext _context;

  /// <summary>
  /// This <see langword="event"/> raises when the <see cref="CurrentTrack"/> changes.
  /// </summary>
  public event EventHandler<TrackEventArgs>? NewSongSelected;

  /// <summary>
  /// This <see langword="event"/> raises when anything in this <see cref="TrackQueue"/> changes.
  /// </summary>
  public event EventHandler<EventArgs>? QueueChanged;

  //todo: save in settings / db
  public LoopMode LoopMode { get; set; } = LoopMode.None;
  
  //todo: make distinction between previously played and previous track in queue
  public IList<Track> HistoryTracks { get; private set; } = new List<Track>(); //todo: could be stack
  public Track? CurrentTrack { get; private set; }

  public IList<Track> NextUpTracks { get; private set; } = new List<Track>(); //todo: could be queue
  public IList<Track> QueuedTracks { get; private set; } = new List<Track>(); //todo: could be queue

  //the queue, how it was first initiated without any dequeued elements
  private IList<Track> _originalQueue = new List<Track>();

  public bool IsShuffle { get; private set; }

  public TrackQueue(AudioPlayer player, IQueuedTracksRepository queuedTrackRepository, MusicContext context) {
    this._player = player;
    this._queuedTrackRepository = queuedTrackRepository;
    this._context = context;

    player.PlaybackEnded += this._AudioPlayer_PlaybackEnded;
  }

  public void LoadTracksFromDb() {
    var dbCurrentTrack = this._context.CurrentTracks.FirstOrDefault();

    if (dbCurrentTrack != null) {
      var track = this.CurrentTrack = dbCurrentTrack.Track;
      this._player.SetTrack(track);
      this._player.Seek(dbCurrentTrack.ProgressInSeconds);
      this.NewSongSelected?.Invoke(this, new TrackEventArgs(track));
    }

    this.HistoryTracks = this._queuedTrackRepository.HistoryTracks.ToList();
    this.NextUpTracks = this._queuedTrackRepository.NextUpTracks.ToList();
    this.QueuedTracks = this._queuedTrackRepository.QueuedTracks.ToList();
    this._originalQueue = new List<Track>(this.QueuedTracks);
  }


  //public void ChangeCurrentTrack(Track newTrack, bool addLastTrackToHistory = true) {
  //  if (addLastTrackToHistory && this.CurrentTrack != null)
  //    this.HistoryTracks.Add(this.CurrentTrack);

  //  this.CurrentTrack = newTrack;
  //  this._audioPlayer.Stop();

  //  this.NewSongSelected?.Invoke(this, new TrackEventArgs(newTrack));
  //  this._wasPaused = false;
  //}

  /// <summary>
  /// Changes current track without starting playback.
  /// </summary>
  /// <remarks>Doesn't raise <see cref="QueueChanged"/>.</remarks>
  /// <param name="newTrack">The new track to be selected.</param>
  /// <param name="addLastTrackToHistory"><c>True</c> if the last playing <see cref="Track"/> should be added to <see cref="HistoryTracks"/>.</param>
  private void _ChangeCurrentTrack(Track newTrack, bool addLastTrackToHistory = true) {
    if (addLastTrackToHistory && this.CurrentTrack != null)
      this.HistoryTracks.Add(this.CurrentTrack);

    this.CurrentTrack = newTrack;
    this._player.Stop();

    this.NewSongSelected?.Invoke(this, new TrackEventArgs(newTrack));
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
    this._ChangeCurrentTrack(list.Dequeue());
    this.QueuedTracks = list;
    this.QueueChanged?.Invoke(this, EventArgs.Empty);
  }

  public void ChangeNextUp(IEnumerable<Track> tracks) {
    //todo: should be done with enumeration instead of changing to list
    var list = tracks.ToList();

    this._originalQueue = new List<Track>(list);
    this._ChangeCurrentTrack(list.Dequeue());
    this.NextUpTracks = list;
    this.QueueChanged?.Invoke(this, EventArgs.Empty);
  }

  /// <summary>
  /// Adds a track to the first position of the <see cref="NextUpTracks"/> (it's gonna play as the next track).
  /// </summary>
  /// <param name="track">The track to insert at the front of the queue.</param>
  public void InsertNextUp(Track track) {
    this.NextUpTracks.Insert(0, track);
    this.QueueChanged?.Invoke(this, EventArgs.Empty);
  }

  /// <summary>
  /// Adds track to the end of the <see cref="NextUpTracks"/>
  /// (it's gonna play after all the other user picks but before the common queue).
  /// </summary>
  /// <param name="track"></param>
  public void AddToNextUp(Track track) {
    this.NextUpTracks.Add(track);
    this.QueueChanged?.Invoke(this, EventArgs.Empty);
  }

  /// <summary>
  /// Adds the track to the very last playback position.
  /// </summary>
  /// <param name="track"></param>
  public void AddToEndOfQueue(Track track) {
    this.QueuedTracks.Add(track);
    this.QueueChanged?.Invoke(this, EventArgs.Empty);
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

    this.QueueChanged?.Invoke(this, EventArgs.Empty);
  }

  /// <inheritdoc cref="_ChangeCurrentTrack"/>
  /// <summary>
  /// Changes the current track and starts playback.
  /// </summary>
  /// <param name="track">The new track to play.</param>
  /// <param name="addLastTrackToHistory"></param>
  public void Play(Track track, bool addLastTrackToHistory = true) {
    this._ChangeCurrentTrack(track, addLastTrackToHistory);
    this.QueueChanged?.Invoke(this, EventArgs.Empty);
    this._player.Play(track);
  }

  private void _AudioPlayer_PlaybackEnded(object? _, EventArgs __) {
    if (this.LoopMode == LoopMode.Current)
      this._player.Play(this.CurrentTrack!);
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
    if (this._player.IsPaused) {
      this._player.Play();
      return;
    }

    var playbackPosition = this._player.PositionInS;

    if (playbackPosition <= 0)
      //start first time playback from start
      this._player.Play(this.CurrentTrack);
    else
      //start playing at specific time for the first time
      this._player.PlayAtTime(this.CurrentTrack, playbackPosition);
  }

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
    if (this.HistoryTracks.Count <= 0) {
      this._player.Seek(0);
      return;
    }

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

  //todo: db logic shouldn't be here i think
  public void SaveToDb() {

    //todo: don't delete history
    this._context.QueuedTracks.Clear();

    var queuedTracks = this.QueuedTracks
      .Select(t => new DbQueuedTrack(t, QueuedType.Queued))
      .Concat(this.NextUpTracks
        .Select(t => new DbQueuedTrack(t, QueuedType.NextUp)))
      .Concat(this.HistoryTracks
        .Select(t => new DbQueuedTrack(t, QueuedType.History)));

    this._context.CurrentTracks.Clear();
    var currentPosition = this._player.PositionInS;
    if (this.CurrentTrack != null)
      this._context.CurrentTracks.Add(new DbCurrentTrack { Track = this.CurrentTrack, ProgressInSeconds = currentPosition });

    this._context.QueuedTracks.AddRange(queuedTracks);

    this._context.SaveChanges();
  }

}