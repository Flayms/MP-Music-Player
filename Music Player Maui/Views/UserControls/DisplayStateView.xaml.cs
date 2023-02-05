using Music_Player_Maui.Enums;

namespace Music_Player_Maui.Views.UserControls;

//todo: not working atm probably bc of maui

/// <summary>
/// A Control that can switch between an empty, loading or content-displaying state.
/// </summary>
[ContentProperty("ControlContent")]
public partial class DisplayStateView : ContentView {

  public static readonly BindableProperty StateProperty
    = BindableProperty.Create(nameof(State), typeof(DisplayState), typeof(DisplayStateView), null, propertyChanged: _DisplayStateChanged);
  
  public static readonly BindableProperty ControlContentProperty
    = BindableProperty.Create(nameof(ControlContent), typeof(object), typeof(DisplayStateView), propertyChanged: _OnContentChanged);

  public DisplayState State {
    get => (DisplayState)this.GetValue(StateProperty);
    set {
      this.SetValue(StateProperty, value);
      this._UpdateDisplayState();
    }
  }

  public View ControlContent {
    get => (View)this.GetValue(ControlContentProperty);
    set => this.SetValue(ControlContentProperty, value);
  }

  public DisplayStateView() {
    this.InitializeComponent();
  }

  private static void _OnContentChanged(BindableObject bindable, object oldValue, object newValue) { 
    var @this = (DisplayStateView)bindable;
    var oldView = (View)oldValue;

    if (oldView != null)
      @this.StackLayout.Children.Remove(oldView);

    @this.StackLayout.Children.Add((View)newValue);
  }

  private static void _DisplayStateChanged(BindableObject bindable, object oldValue, object newValue) {
    var @this = (DisplayStateView)bindable;
    @this._UpdateDisplayState();
  }

  /// <summary>
  /// switches between showing items, or loading or no displayed items
  /// </summary>
  private void _UpdateDisplayState() {
    this._ShowLoading(false);
    this.lblEmpty.IsVisible = false;
 
    if (this.ControlContent != null)
      this.ControlContent.IsVisible = false;
 
 
    switch (this.State) {
      case DisplayState.Loading:
        this._ShowLoading();
        break;
      case DisplayState.Empty:
        this.lblEmpty.IsVisible = true;
        break;
      case DisplayState.DisplayingContent:
        if (this.ControlContent != null)
          this.ControlContent.IsVisible = true;
        break;
    }
  }

  private void _ShowLoading(bool isLoading = true) {
    this.activityIndicator.IsRunning = isLoading;
    this.activityIndicator.IsVisible = isLoading;
  }

}
