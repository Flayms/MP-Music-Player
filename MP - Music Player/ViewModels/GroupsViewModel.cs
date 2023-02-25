using CommunityToolkit.Mvvm.ComponentModel;
using MP_Music_Player.Enums;
using MP_Music_Player.Models;

namespace MP_Music_Player.ViewModels;

public partial class GroupsViewModel : AViewModel {
  private readonly MusicContext _context;

  [ObservableProperty]
  private GroupType _groupType;

  [ObservableProperty]
  private IReadOnlyCollection<SmallGroupViewModel>? _groups;

  //todo: don't go over context here!
  public GroupsViewModel(MusicContext context) {
    this._context = context;
  }

  //todo: refac
  public void SetGroupType(GroupType groupType) {
    this.GroupType = groupType;
    var context = this._context;

    switch (groupType) {
      case GroupType.Artists:
        var artistGroups = context.Artists
          .OrderBy(g => g.Name)
          .Select(artist => new SmallGroupViewModel(artist.Name, artist.Tracks))
          .ToArray();

        this.Groups = artistGroups;
        break;

      case GroupType.Genres:
        var genreGroups = context.Genres
          .OrderBy(g => g.Name)
          .Select(artist => new SmallGroupViewModel(artist.Name, artist.Tracks))
          .ToArray();

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