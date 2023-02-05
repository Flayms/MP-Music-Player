using Music_Player_Maui.Models;
using TagLib;

namespace Music_Player_Maui.Services;

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
      var artistsNames = _GetArtists(fileTags);
      var artists = new List<Artist>();

      foreach (var artistsName in artistsNames) {
        var artist = existingArtists.FirstOrDefault(a => a.Name == artistsName);

        if (artist == null) {
          artist = new Artist { Name = artistsName };
          existingArtists.Add(artist);
        }

        artists.Add(artist);
      }

      track.Artists = artists;

      //todo: same here

      var genreNames = _GetGenres(fileTags.Genres);
      var genres = new List<Genre>();

      foreach (var genreName in genreNames) {
        var genre = existingGenres.FirstOrDefault(g => g.Name == genreName);

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
    var artists = fileTags.Performers; //todo: check if performers or composers have values as well
    if (artists.Length != 0)
      return artists;

    var joinedArtists = fileTags.JoinedPerformers;
    if (joinedArtists != null)
      artists = joinedArtists.Split('&').Select(a => a.Trim()).ToArray();

    return artists;
  }

  //todo: shouldn't split genres by default
  private static string[] _GetGenres(string[] tGenres) => tGenres.Length > 0
    ? tGenres
      .SelectMany(g => g.Split(Genre.SEPARATOR),
        (_, singleGenre) => singleGenre.Trim())
    .ToArray()
    : tGenres;
}