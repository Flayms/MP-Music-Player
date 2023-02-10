using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.Pages;

public partial class SearchPage : ContentPage {

  public SearchPage(SearchViewModel viewModel) {
    this.BindingContext = viewModel;
    this.InitializeComponent();
  }
}
