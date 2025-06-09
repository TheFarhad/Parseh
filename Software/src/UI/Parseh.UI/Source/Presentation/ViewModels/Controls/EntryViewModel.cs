namespace Parseh.UI.ViewModels;

public sealed class EntryViewModel : VM
{
    #region Fields

    string _original = Empty;

    #endregion

    #region Properties

    public string Label { get => Get(); set => Set(value); }
    public string Text { get => Get(); set => Set(value); }
    public bool IsEditing { get => Get(); set => Set(value); }
    public bool IsWorking { get => Get(); set => Set(value); }

    #endregion

    #region Commands

    public IRelayCommand EditCommand { get; private set; }
    public IRelayCommand CancelCommand { get; private set; }
    public IRelayCommand SaveCommand { get; private set; }

    #endregion

    public EntryViewModel() => Init();

    #region Private Functionality

    void Init()
    {
        IsEditing = false;
        IsWorking = false;
        EditCommand = new Command(Edit);
        CancelCommand = new Command(Cancel);
        SaveCommand = new Command(Save);
    }

    #endregion

    #region Command Methods

    void Edit()
    {
        IsEditing = true;
        _original = Text;
    }

    void Cancel()
    {
        Text = _original;
        _original = Empty;
        IsEditing = false;
    }

    void Save()
    {
        IsWorking = true; // TODO: for async operation and create pogressbar while operation complete.
    }

    #endregion
}

internal sealed class EntryDesignModel : DesignModel<EntryDesignModel, EntryViewModel>
{
    public EntryDesignModel()
    {
        Model.Label = "Name";
        Model.Text = "Text";
        Model.IsEditing = false;
    }
}
