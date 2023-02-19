using Music_Player_Maui.Enums;
using Music_Player_Maui.Models;
using Music_Player_Maui.ViewModels;
using Music_Player_Maui.Views.Pages;

namespace Music_Player_Maui.Services;

public class TrackOptionsService {
  private readonly TrackQueue _queue;

  //todo: BiDictionary
  public static readonly IReadOnlyDictionary<TrackOption, string> OptionTexts = new Dictionary<TrackOption, string> {
    {TrackOption.PlayNext, "Play next"},
    {TrackOption.AddToQueue, "Add to queue" },
    {TrackOption.AddToEndOfQueue, "Add to end of queue"},
    {TrackOption.RemoveFromQueue, "Remove from queue"},
    {TrackOption.AddToPlaylist, "Add to playlist" },
    {TrackOption.RemoveFromPlaylist, "Remove from playlist" },
    {TrackOption.GoToArtist, "Go to artist"},
    {TrackOption.GoToAlbum, "Go to album"},
    {TrackOption.Details, "Details"}
  };

  /// <summary>
  /// The options that are always visible
  /// </summary>
  public static readonly TrackOption[] BasicOptions = {
    TrackOption.PlayNext,
    TrackOption.AddToQueue,
    TrackOption.AddToEndOfQueue,
    TrackOption.AddToPlaylist,
    TrackOption.GoToArtist
  };

  public TrackOptionsService(TrackQueue queue) {
    this._queue = queue;
  }

  public async Task StartBasicOptionsMenuAsync(Track track)
    => await this._StartOptionsAsync(track, BasicOptions);

  private async Task _StartOptionsAsync(Track track, params TrackOption[] options) {
    var dic = OptionTexts;
    const string cancelString = "Cancel";
    var texts = new string[options.Length];

    for (var i = 0; i < texts.Length; ++i)
      texts[i] = dic[options[i]];

    var selectText = await Application.Current!.MainPage!.DisplayActionSheet(track.ToString(), cancelString, null, texts);

    if (selectText is null or cancelString)
      return;

    //todo: handle this with BiDic instead
    this._ExecuteOption(dic.First(p => p.Value == selectText).Key, track);
  }

  private void _ExecuteOption(TrackOption option, Track track) {
    var queue = this._queue;

    switch (option) {
      case TrackOption.PlayNext:
        queue.AddNext(track);
        break;

      case TrackOption.AddToQueue:
        queue.AddToQueue(track);
        break;

      case TrackOption.AddToEndOfQueue:
        queue.AddToEndOfQueue(track);
        break;

      case TrackOption.RemoveFromQueue:
        queue.Remove(track);
        break;

      case TrackOption.AddToPlaylist:
        throw new NotImplementedException();
        //playlist.DisplayAddMenuAsync(track);
        //break;

      case TrackOption.RemoveFromPlaylist:
        throw new NotImplementedException();

      case TrackOption.GoToArtist: //todo: error handling when no artist available
        var artist = track.Artists.FirstOrDefault();

        var model = ServiceHelper.GetService<TrackListViewModel>();
        model.TrackViewModels = artist.Tracks.Select(t => new SmallTrackViewModel(t)).ToList();
        model.Title = artist.Name;

        Application.Current!.MainPage!.Navigation.PushAsync(new TrackListPage(model));
        break;

      case TrackOption.GoToAlbum:
        throw new NotImplementedException();

      case TrackOption.Details:
        throw new NotImplementedException();

      default:
        throw new ArgumentOutOfRangeException();
    }
  }
}