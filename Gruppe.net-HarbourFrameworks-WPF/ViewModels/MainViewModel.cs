using ClassLibrary.HarborFramework.DockingInfo;
using ClassLibrary.HarborFramework.ShipInfo;
using Gruppe.net_HarbourFrameworks_WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using WpfApp;
using Container = ClassLibrary.HarborFramework.ContainerYardInfo.Container;

public class MainViewModel : INotifyPropertyChanged
{
    private static int containerIdCounter = 1;

    public ObservableCollection<Ship> Ships { get; set; }
    public ObservableCollection<string> ContainerLengthTypes { get; set; } = new ObservableCollection<string> { "HalfLength", "FullLength" };

    private Ship selectedShip;
    public Ship SelectedShip
    {
        get => selectedShip;
        set
        {
            selectedShip = value;
            OnPropertyChanged(nameof(SelectedShip));
            OnPropertyChanged(nameof(SelectedShip.ListOfContainersOnShip));
        }
    }

    public Container SelectedContainer { get; set; }
    public string ShipName { get; set; }
    public string ShipType { get; set; }
    private string containerLengthType;
    public string ContainerLengthType
    {
        get => containerLengthType;
        set
        {
            if (value == "HalfLength" || value == "FullLength")
            {
                containerLengthType = value;
                OnPropertyChanged(nameof(ContainerLengthType));
            }
        }
    }

    public ICommand CreateShipCommand { get; }
    public ICommand AddContainerCommand { get; }
    public ICommand RemoveContainerCommand { get; }
    public ICommand OpenSpaceViewCommand { get; }

    public MainViewModel()
    {
        Ships = new ObservableCollection<Ship>();
        CreateShipCommand = new RelayCommand(CreateShip);
        AddContainerCommand = new RelayCommand(AddContainer, CanModifyShip);
        RemoveContainerCommand = new RelayCommand(RemoveContainer, CanModifyShip);
        OpenSpaceViewCommand = new RelayCommand(OpenSpaceView);
    }

    private void CreateShip(object parameter)
    {
        if (!string.IsNullOrEmpty(ShipName) && !string.IsNullOrEmpty(ShipType))
        {
            int newId = Ships.Count + 1;
            var newShip = new Ship(newId, ShipType) { Name = ShipName };
            Ships.Add(newShip);
            OnPropertyChanged(nameof(Ships));
        }
    }

    private bool CanModifyShip(object parameter)
    {
        return SelectedShip != null;
    }

    private void AddContainer(object parameter)
    {
        if (SelectedShip != null && !string.IsNullOrEmpty(ContainerLengthType))
        {
            var container = new Container(containerIdCounter++, ContainerLengthType);
            SelectedShip.AddContainerToShip(container);
            OnPropertyChanged(nameof(SelectedShip.ListOfContainersOnShip));
        }
    }

    private void RemoveContainer(object parameter)
    {
        if (SelectedShip != null && SelectedContainer != null)
        {
            SelectedShip.RemoveContainerFromShip(SelectedContainer.ContainerId);
            OnPropertyChanged(nameof(SelectedShip.ListOfContainersOnShip));
        }
    }

    private void OpenSpaceView(object parameter)
    {
        var spaceView = new SpaceView
        {
            DataContext = new SpaceViewModel(Ships)
        };
        spaceView.Show();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
