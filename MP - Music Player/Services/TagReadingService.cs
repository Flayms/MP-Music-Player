using MP_Music_Player.Models;
using TagLib;

namespace MP_Music_Player.Services;

public class TagReadingService {

  //todo: fix this

  // ReSharper disable once SuggestBaseTypeForParameter
  public bool TryReadTags(FileInfo file, ref List<Artist> existingArtists, ref List<Genre> existingGenres, out Track track) {

    try {
      var tFile = TagLib.File.Create(file.FullName);
      var fileTags = tFile.Tag;

      var title = fileTags.Title;

      track = new Track {
        Title = string.IsNullOrEmpty(title) ? file.Name : title.Trim(),
        Path = file.FullName,
        Duration = tFile.Properties.Duration,
        Album = fileTags.Album
      };

      //todo: put in other method
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

      //todo: same here
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

      return true;
    } catch {
      track = null!;
      return false;
    }
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