using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

/// <summary>
/// A Page to search for specific tracks.
/// </summary>
public partial class SearchPage : ContentPage {

  public SearchPage(SearchViewModel viewModel) {
    this.BindingContext = viewModel;
    this.InitializeComponent();
  }
}
