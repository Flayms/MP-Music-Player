using Microsoft.EntityFrameworkCore;
using Music_Player_Maui.Enums;
using Music_Player_Maui.Models;

namespace Music_Player_Maui.Services; 

//todo: helper method for getting tracks of specific type
public class QueuedTracksRepository {

  private readonly MusicContext _context;
  private DbSet<QueuedTrack> _queuedTracks => this._context.QueuedTracks;

  //todo: decide if its better for these to be properties or methods
  public IReadOnlyCollection<Track> NextUpTracks => this._queuedTracks
    .Where(qt => qt.Type == QueuedType.NextUp)
    .Select(qt => qt.Track)
    .ToList();

  public IReadOnlyCollection<Track> QueuedTracks => this._queuedTracks
    .Where(qt => qt.Type == QueuedType.NextUp)
    .Select(qt => qt.Track)
    .ToList();

  public Track? CurrentTrack => this._queuedTracks.FirstOrDefault(qt => qt.Type == QueuedType.Current)?.Track;

  public QueuedTracksRepository(MusicContext context) {
    this._context = context;
  }

  public void SetNewCurrentTrack(Track track) {
    //only ever has one current track
    var currentDbTrack = this._queuedTracks
      .FirstOrDefault(qt => qt.Type == QueuedType.Current);

    if (currentDbTrack != null) {
      this._queuedTracks.Remove(currentDbTrack);
      this._queuedTracks.Add(new QueuedTrack { Track = currentDbTrack.Track, Type = QueuedType.History });
    }

    //todo: maybe create constructor for queuedTrack
    this._queuedTracks.Add(new QueuedTrack { Track = track, Type = QueuedType.Current });
    this._context.SaveChanges();
  }
  
  /// <summary>
  /// Removes all queued tracks in the repository and adds the new list.
  /// </summary>
  /// <param name="tracks">The tracks to be added.</param>
  public void ChangeQueue(List<Track> tracks) {
    //cleanup db
    var currentlyQueued = this._context.QueuedTracks
      .Where(qt => qt.Type == QueuedType.Queued);
    this._queuedTracks.RemoveRange(currentlyQueued);

    //now add the items
    this._queuedTracks.AddRange(tracks.Select(t => new QueuedTrack { Track = t, Type = QueuedType.Queued }));
    this._context.SaveChanges();
  }

  public void InsertNextUp(Track track) {
    //needs to re-add all values to db, for correct index-order
    var currentlyNextUpTracks = this._queuedTracks
      .Where(qt => qt.Type == QueuedType.NextUp);

    var newNextUps = currentlyNextUpTracks.Select(qt => qt.Track).ToList();
    newNextUps.Add(track);

    this._queuedTracks.RemoveRange(currentlyNextUpTracks);
    this._queuedTracks.AddRange(newNextUps.Select(t => new QueuedTrack { Track = t, Type = QueuedType.NextUp}));
    this._context.SaveChanges();
  }

  public void AddToNextUp(Track track) {
    this._queuedTracks.Add(new QueuedTrack { Track = track, Type = QueuedType.NextUp });
    this._context.SaveChanges();
  }

  public void AddToEndOfQueue(Track track) {
    this._queuedTracks.Add(new QueuedTrack { Track = track, Type = QueuedType.Queued });
    this._context.SaveChanges();
  }

  //also looks through next-ups
  //todo: maybe split into 2 methods
  public void RemoveFromQueue(Track track) {
    var trackToRemove = this._queuedTracks.FirstOrDefault(qt => qt.Type == QueuedType.NextUp);

    if (trackToRemove != null)
      trackToRemove = this._queuedTracks.FirstOrDefault(qt => qt.Type == QueuedType.Queued);

    if (trackToRemove == null)
      return;

    this._queuedTracks.Remove(trackToRemove);
    this._context.SaveChanges();
  }

  //returns false when end of queue //todo: write full doc comment for this
  public bool TryDequeueTrack(out Track track) {
    var queuedTrack = this._queuedTracks.FirstOrDefault(qt => qt.Type == QueuedType.NextUp);

    if (queuedTrack == null)
      queuedTrack = this._queuedTracks.FirstOrDefault(qt => qt.Type == QueuedType.Queued);

    if (queuedTrack == null) {
      track = null!;
      return false;
    }

    track = queuedTrack.Track;
    return true;
  }

}