using MP_Music_Player.ViewModels;

namespace MP_Music_Player.Views.Pages;

/// <summary>
/// a self build shell with the common tabs. Currently not in use.
/// </summary>
public partial class TabLibraryPage : Shell {
  public TabLibraryPage(TabLibraryViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}
