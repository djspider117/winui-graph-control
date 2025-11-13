using CommunityToolkit.Mvvm.ComponentModel;
using DataGraphControl.Samples.Samples;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace DataGraphControl.Samples.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public const string DEFAULT_TITLE = "Data Graph Control WinUI Samples";

    [ObservableProperty]
    public partial Sample? SelectedSample { get; set; }
    partial void OnSelectedSampleChanged(Sample? value) => Title = SelectedSample?.Title ?? DEFAULT_TITLE;

    [ObservableProperty]
    public partial string Title { get; set; } = DEFAULT_TITLE;

    [ObservableProperty]
    public partial ObservableCollection<Sample> AvailableSamples { get; set; }

    public MainWindowViewModel()
    {
        AvailableSamples =
        [
            new("Unity Style", "An Unity3D style node graph", Symbol.OutlineStar, typeof(UnityStyleSample)),
            new("Nuke Style", "An Nuke/NukeX style node graph", Symbol.Comment, typeof(NukeStyleSample))
        ];
        SelectedSample = AvailableSamples[0];
    }
}
