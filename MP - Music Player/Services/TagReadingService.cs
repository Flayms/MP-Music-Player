using MP_Music_Player.Models;
using MP_Music_PLayer.Models;
using TagLib;
using Track = MP_Music_Player.Models.Track;

namespace MP_Music_Player.Services;

public class TagReadingService {

  //todo: fix this

  //todo: make object for method params
  // ReSharper disable once SuggestBaseTypeForParameter

  /// <summary>
  /// Reads all tags of the track and then adds them to the given lists if they don't contain them yet.
  /// </summary>
  /// <param name="file">The <see cref="FileInfo"/> of the track.</param>
  /// <param name="existingArtists">A list with all artists already read. This tracks artists get added to this.</param>
  /// <param name="existingGenres">A list with all genres already read. This tracks genres get added to this.</param>
  /// <param name="existingAlbums">A list with all albums already read. This tracks album gets added to this.</param>
  /// <param name="track">The newly parsed track from the given file.</param>
  /// <returns><c>True</c> if the file was readable.</returns>
  public bool TryReadTags(FileInfo file,
    ref List<Artist> existingArtists, ref List<Genre> existingGenres, ref List<Album> existingAlbums,
    out Track track) {

    try {
      var tFile = TagLib.File.Create(file.FullName);
      var fileTags = tFile.Tag;

      var title = fileTags.Title;

      track = new Track {
        Title = string.IsNullOrEmpty(title) ? file.Name : title.Trim(),
        Path = file.FullName,
        Duration = tFile.Properties.Duration,
      };

      _ReadArtists(ref existingArtists, track, fileTags);
      _ReadGenres(ref existingGenres, track, fileTags);
      _ReadAlbum(ref existingAlbums, track, fileTags);

      return true;
    } catch {
      track = null!;
      return false;
    }
  }

  private static void _ReadArtists(ref List<Artist> existingArtists, Track track, Tag fileTags) {
    var artistNames = _GetArtists(fileTags);
    var artists = new List<Artist>();

    foreach (var artistName in artistNames) {
      var artist = existingArtists.FirstOrDefault(a => a.Name.Equals(artistName, StringComparison.InvariantCultureIgnoreCase));

      if (artist == null) {
        artist = new Artist { Name = artistName };
        existingArtists.Add(artist);
      }

      artists.Add(artist);
    }

    track.Artists = artists;
  }

  private static void _ReadGenres(ref List<Genre> existingGenres, Track track, Tag fileTags) {
    var genreNames = _GetGenres(fileTags.Genres);
    var genres = new List<Genre>();

    foreach (var genreName in genreNames) {
      var genre = existingGenres.FirstOrDefault(g => g.Name.Equals(genreName, StringComparison.InvariantCultureIgnoreCase));

      if (genre == null) {
        genre = new Genre { Name = genreName };
        existingGenres.Add(genre);
      }

      genres.Add(genre);
    }

    track.Genres = genres;
  }

  private static void _ReadAlbum(ref List<Album> existingAlbums, Track track, Tag fileTags) {
    var albumName = fileTags.Album;

    var album = existingAlbums.FirstOrDefault(a => a.Name.Equals(albumName, StringComparison.InvariantCultureIgnoreCase));

    if (album == null && albumName != null) {
      album = new Album { Name = albumName };
      existingAlbums.Add(album);
    }

    track.Album = album;
  }

  private static string[] _GetArtists(Tag fileTags) {
    //artists

    //first try to read already separated performers
    var artists = fileTags.Performers;
    if (artists.Length != 0) {
      var allArtists = new List<string>();

      foreach (var artist in artists)
        allArtists.AddRange(_SplitArtists(artist));

      return allArtists.Distinct(StringComparer.InvariantCulture).ToArray();
    }

    //otherwise self split them
    var joinedArtists = fileTags.JoinedPerformers;
    if (joinedArtists != null)
      artists = _SplitArtists(joinedArtists).ToArray();

    return artists;
  }

  private static IEnumerable<string> _SplitArtists(string joinedArtists) {
    return joinedArtists
      .Split(Artist.SEPARATOR)
      .Select(a => a.Trim())
      .Where(a => a != string.Empty)
      .Distinct(StringComparer.InvariantCulture);
  }

  //todo: shouldn't split genres by default
  private static string[] _GetGenres(string[] tGenres) {
    var newList = tGenres.Length > 0
      ? tGenres
        .SelectMany(g => g.Split(Genre.SEPARATOR),
          (_, singleGenre) => singleGenre.Trim())
      : tGenres;

    return newList
      .Select(g => g.Trim())
      .Where(g => g != string.Empty)
      .Distinct(StringComparer.InvariantCulture)
      .ToArray();
  }

}