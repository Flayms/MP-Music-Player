using Microsoft.EntityFrameworkCore;
using Music_Player_Maui.Enums;
using Music_Player_Maui.Models;

namespace Music_Player_Maui.Services.Repositories;

//todo: helper method for getting tracks of specific type
public class QueuedTracksRepository : IQueuedTracksRepository {

  private readonly MusicContext _context;
  private DbSet<QueuedTrack> _queuedTracks => this._context.QueuedTracks;

  //todo: decide if its better for these to be properties or methods
  public IReadOnlyCollection<Track> NextUpTracks => this._queuedTracks
    .Where(qt => qt.Type == QueuedType.NextUp)
    .Select(qt => qt.Track)
    .ToList();

  public IReadOnlyCollection<Track> QueuedTracks => this._queuedTracks
    .Where(qt => qt.Type == QueuedType.Queued)
    .Select(qt => qt.Track)
    .ToList();

  public QueuedTracksRepository(MusicContext context) {
    this._context = context;
  }

  /// <inheritdoc cref="IQueuedTracksRepository.ChangeQueue"/>
  public void ChangeQueue(IReadOnlyCollection<Track> tracks) {
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
    this._queuedTracks.AddRange(newNextUps.Select(t => new QueuedTrack { Track = t, Type = QueuedType.NextUp }));
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