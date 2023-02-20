﻿using Music_Player_Maui.Enums;
using Music_Player_Maui.Extensions;
using Music_Player_Maui.Models;
using Plugin.Maui.Audio;
using File = System.IO.File;
using Track = Music_Player_Maui.Models.Track;

namespace Music_Player_Maui.Services;

//todo: maybe split between queue logic and actual playing logic

public class TrackQueue {
  private readonly IAudioManager _audioManager;

  private readonly QueuedTracksRepository _queuedTrackRepository;
  // private readonly MusicContext _context;

  public event EventHandler<TrackEventArgs>? NewSongSelected;
  public event EventHandler<IsPlayingEventArgs>? IsPlayingChanged;

  public IReadOnlyCollection<Track> NextUpTracks => this._queuedTrackRepository.NextUpTracks;
  public IReadOnlyCollection<Track> QueuedTracks => this._queuedTrackRepository.QueuedTracks;

  //todo: maybe make repo property for this and read directly from there
  public List<Track> AllTracks { get; private set; } = new();

  public Track? CurrentTrack {
    get => this._queuedTrackRepository.CurrentTrack;

    //todo: maybe does too much for a setter
    private set {
      this._currentTrack = value ?? throw new ArgumentNullException(nameof(value));
      this._queuedTrackRepository.SetNewCurrentTrack(value);

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

  public TrackQueue(IAudioManager audioManager, QueuedTracksRepository queuedTrackRepository) {
    this._audioManager = audioManager;
    this._queuedTrackRepository = queuedTrackRepository;

  }

  //public void FullyCreateQueue(List<Track> nextUps, List<Track> queued, Track current) {
  //  this.NextUpTracks = nextUps;
  //  this.QueuedTracks = queued;
  //  this.CurrentTrack = current;
  //  this._ReloadFullQueue();
  //}

  //also handles all the db-stuff
  public void ChangeQueue(List<Track> tracks) {
    this.CurrentTrack = tracks.Dequeue();
    this._queuedTrackRepository.ChangeQueue(tracks);
    this._ReloadFullQueue();
  }

  public void InsertNextUp(Track track) {
    this._queuedTrackRepository.InsertNextUp(track);
    this._ReloadFullQueue();
  }

  public void AddToNextUp(Track track) {
    this._queuedTrackRepository.AddToNextUp(track);
    this._ReloadFullQueue();
  }

  public void AddToEndOfQueue(Track track) {
    this._queuedTrackRepository.AddToEndOfQueue(track);
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