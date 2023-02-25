using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

public partial class SearchPage : ContentPage {

  public SearchPage(SearchViewModel viewModel) {
    this.BindingContext = viewModel;
    this.InitializeComponent();
  }
}
