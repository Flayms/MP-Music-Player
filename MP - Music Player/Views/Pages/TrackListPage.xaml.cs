using MP_Music_Player.Enums;
using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

/// <summary>
/// Displays a list of tracks of a certain category (artist, album etc.) instead of all tracks like in <see cref="SongsPage"/>.
/// <remarks>All the categories are in the <see cref="GroupType"/> enum.</remarks>
/// </summary>
public partial class TrackListPage : ContentPage {
  public TrackListPage(TrackListViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}