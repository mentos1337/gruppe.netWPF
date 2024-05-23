using ClassLibrary.HarborFramework.DockingInfo;
using ClassLibrary.HarborFramework.ShipInfo;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

public class SpaceViewModel : INotifyPropertyChanged
{
    private static int spaceIdCounter = 1;

    public ObservableCollection<Space> Spaces { get; set; }
    public ObservableCollection<Ship> Ships { get; set; }
    public ObservableCollection<string> SpaceTypes { get; set; } = new ObservableCollection<string> { "WaitingStation", "LoadingSpace", "DockSpace" };
    public Ship SelectedShip { get; set; }
    public Ship SelectedShipInSpace { get; set; }
    public Space SelectedSpace { get; set; }
    public string SelectedSpaceType { get; set; }
    public int NumberOfCranes { get; set; }
    public ICommand CreateSpaceCommand { get; }
    public ICommand ScheduleArrivalCommand { get; }
    public ICommand ScheduleDepartureCommand { get; }

    public SpaceViewModel(ObservableCollection<Ship> ships)
    {
        Ships = ships;
        Spaces = new ObservableCollection<Space>();

        CreateSpaceCommand = new RelayCommand(CreateSpace);
        ScheduleArrivalCommand = new RelayCommand(ScheduleArrival, CanSchedule);
        ScheduleDepartureCommand = new RelayCommand(ScheduleDeparture, CanSchedule);
    }

    private void CreateSpace(object parameter)
    {
        if (!string.IsNullOrEmpty(SelectedSpaceType))
        {
            Space newSpace = null;

            switch (SelectedSpaceType)
            {
                case "WaitingStation":
                    newSpace = new WaitingStation();
                    break;
                case "LoadingSpace":
                    if (NumberOfCranes > 0)
                        newSpace = new LoadingSpace(NumberOfCranes);
                    break;
                case "DockSpace":
                    newSpace = new DockSpace(spaceIdCounter); // DockSpaceNumber is now auto-generated
                    break;
            }

            if (newSpace != null)
            {
                newSpace.SpaceId = spaceIdCounter++;
                Spaces.Add(newSpace);
                OnPropertyChanged(nameof(Spaces));
            }
        }
    }

    private bool CanSchedule(object parameter)
    {
        return SelectedShip != null && SelectedSpace != null;
    }

    private void ScheduleArrival(object parameter)
    {
        if (SelectedSpace != null && SelectedShip != null)
        {
            SelectedSpace.ScheduleShipArrival(SelectedShip, DateTime.Now);
            OnPropertyChanged(nameof(SelectedSpace.Ships));
        }
    }

    private void ScheduleDeparture(object parameter)
    {
        if (SelectedSpace != null && SelectedShip != null)
        {
            SelectedSpace.ScheduleShipDeparture(SelectedShip, DateTime.Now);
            OnPropertyChanged(nameof(SelectedSpace.Ships));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
