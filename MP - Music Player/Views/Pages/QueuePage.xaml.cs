using Music_Player_Maui.ViewModels;

namespace Music_Player_Maui.Views.Pages;

public partial class QueuePage : ContentPage {
  public QueuePage(QueueViewModel viewModel) {
    this.InitializeComponent();
    this.BindingContext = viewModel;
  }
}
