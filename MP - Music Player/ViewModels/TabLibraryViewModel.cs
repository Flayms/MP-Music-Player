using CommunityToolkit.Mvvm.Input;
using Music_Player_Maui.Views.Pages;

namespace Music_Player_Maui.ViewModels; 

public partial class TabLibraryViewModel : AViewModel {


  public TabLibraryViewModel() { }


  [RelayCommand]
  public async Task NavigateToSearch() {
    //Shell.Current.CurrentPage.Title = "Search";
    await Shell.Current.GoToAsync($"///{nameof(SearchPage)}");
  }

}