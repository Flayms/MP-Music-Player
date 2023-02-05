using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.Pages;

public partial class TabLibraryPage : TabbedPage {
  public TabLibraryPage(TabLibraryViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}
