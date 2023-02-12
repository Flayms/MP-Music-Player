using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Music_Player_Maui.Enums;
using Music_Player_Maui.Models;
using Music_Player_Maui.Services;
using TagLib;

namespace Music_Player_Maui.ViewModels;

public partial class GroupsViewModel : AViewModel {
  private readonly MusicService _musicService;
  private readonly MusicContext _context;

  [ObservableProperty]
  private GroupType _groupType;

  [ObservableProperty]
  private IReadOnlyCollection<SmallGroupViewModel> _groups;

  //todo: don't go over context here!
  public GroupsViewModel(MusicService musicService, MusicContext context) {
    this._musicService = musicService;
    this._context = context;
  }

  //todo: refac
  public void SetGroupType(GroupType groupType) {
    this.GroupType = groupType;
    var context = this._context;

    switch (groupType) {
      case GroupType.Artists:
        var artists = context.Artists
          .Include(a => a.Tracks)
          .ToList();

        var artistGroups = artists
          .Select(artist => new SmallGroupViewModel(artist.Name, artist.Tracks))
          .ToList();

        this.Groups = artistGroups;
        break;

      case GroupType.Genres:
        var genres = context.Genres
          .Include(a => a.Tracks)
          .ToList();

        var genreGroups = genres
          .Select(artist => new SmallGroupViewModel(artist.Name, artist.Tracks))
          .ToList();

        this.Groups = genreGroups;
        break;

      case GroupType.Albums:
        throw new NotImplementedException();
      case GroupType.Playlists:
        throw new NotImplementedException();
      case GroupType.Folders:
        throw new NotImplementedException();
      default:
        throw new ArgumentOutOfRangeException(nameof(groupType), groupType, null);
    }
  }
}