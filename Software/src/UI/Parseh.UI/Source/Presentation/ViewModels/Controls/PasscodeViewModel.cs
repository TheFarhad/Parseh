using System.Security;

namespace Parseh.UI.ViewModels;

public sealed class PasscodeViewModel : VM
{
    #region Fields

    #endregion

    #region Properties

    public string Label { get => Get(); set => Set(value); }
    public SecureString Password { get => Get(); set => Set(value); }
    public SecureString New { get => Get(); set => Set(value); }
    public SecureString Confirm { get => Get(); set => Set(value); }
    public string PasscodeHint { get => Get(); private set => Set(value); }
    public string NewHint { get => Get(); private set => Set(value); }
    public string ConfirmHint { get => Get(); private set => Set(value); }
    public string Fake { get => Get(); private set => Set(value); }
    public bool IsEditing { get => Get(); set => Set(value); }
    public bool IsWorking { get => Get(); set => Set(value); }

    #endregion

    #region Commands

    public IRelayCommand EditCommand { get; private set; } = default!;
    public IRelayCommand CancelCommand { get; private set; } = default!;
    public IRelayCommand SaveCommand { get; private set; } = default!;

    #endregion

    public PasscodeViewModel() => Init();

    #region Private Functionality

    void Init()
    {
        InitProperties();
        InitCommands();
    }

    void InitProperties()
    {
        Fake = "**********";
        IsEditing = false;
        IsWorking = false;

        PasscodeHint = "Current Passcode";
        NewHint = "New Passcode";
        ConfirmHint = "Confirm Passcode";
    }

    void InitCommands()
    {
        EditCommand = new Command(Edit);
        CancelCommand = new Command(Cancel);
        SaveCommand = new Command(Save);
    }

    #endregion

    #region Command Methods

    void Edit()
    {
        IsEditing = true;

        New = new SecureString();
        Confirm = new SecureString();
    }

    void Cancel()
    {
        IsEditing = false;
    }

    void Save()
    {
        IsEditing = false;
        IsWorking = true;

        // TODO: make  sure that current password is correct [check with back-end server]
        // TODO: new password and confirm password should be equal

        var testSavedPassword = "123";

        if (Password.Unsecure() != testSavedPassword)
        {
            //NetIoC.Default.Notifier.MessageboxNotifyAsync(new MessageboxViewModel
            //{

            //});

            return;
        }

        var newString = New.Unsecure();
        var confirmString = Confirm.Unsecure();

        if (!String.Equals(newString, confirmString, StringComparison.OrdinalIgnoreCase))
        {
            //NetIoC.Default.Notifier.MessageboxNotifyAsync(new MessageboxViewModel
            //{

            //});

            return;
        }

        // TODO: check regex for new password
        if (false)
        {
            //NetIoC.Default.Notifier.MessageboxNotifyAsync(new MessageboxViewModel
            //{

            //});

            //return;
        }

        Password = new SecureString();
        foreach (var item in New.Unsecure())
        {
            Password.AppendChar(item);
        }
    }

    #endregion
}

internal sealed class PasscodeDesignModel : DesignModel<PasscodeDesignModel, PasscodeViewModel>
{
    public PasscodeDesignModel()
    {
        Model.Label = "Passcode";
    }
}
