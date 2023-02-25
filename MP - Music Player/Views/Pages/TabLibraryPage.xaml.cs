using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

public partial class TabLibraryPage : Shell {
  public TabLibraryPage(TabLibraryViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}
