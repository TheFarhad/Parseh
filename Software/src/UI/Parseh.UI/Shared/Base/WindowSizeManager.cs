namespace Parseh.UI;

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
//using System.Drawing; for Point
using System.Windows.Media;

public enum WindowDockPosition
{
    Undocked = 0,
    Left = 1,
    Right = 2,
    TopBottom = 3,
    TopLeft = 4,
    TopRight = 5,
    BottomLeft = 6,
    BottomRight = 7,
}

public class WindowSizeManager
{
    #region Private Members

    private Window _window;

    private Rect _screenSize = new Rect();

    /// <summary>
    /// How close to the edge the window has to be to be detected as at the edge of the screen
    /// </summary>
    private int _edgeTolerance = 1;

    /// <summary>
    /// The transform matrix used to convert WPF sizes to screen pixels
    /// </summary>
    private DpiScale? _monitorDpi;

    /// <summary>
    /// The last screen the window was on
    /// </summary>
    private IntPtr _lastScreen;

    private WindowDockPosition mLastDock = WindowDockPosition.Undocked;

    /// <summary>
    /// A flag indicating if the window is currently being moved/dragged
    /// </summary>
    private bool _beingMoved = false;

    #endregion

    #region DLL Imports

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

    [DllImport("user32.dll")]
    static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorOptions dwFlags);

    #endregion

    #region Public Events

    /// <summary>
    /// Called when the window dock position changes
    /// </summary>
    public event Action<WindowDockPosition> WindowDockChanged = (dock) => { };

    /// <summary>
    /// Called when the window starts being moved/dragged
    /// </summary>
    public event Action WindowStartedMove = () => { };

    /// <summary>
    /// Called when the window has been moved/dragged and then finished
    /// </summary>
    public event Action WindowFinishedMove = () => { };

    #endregion

    #region Public Properties

    /// <summary>
    /// The size and position of the current monitor the window is on
    /// </summary>
    public Rectangle CurrentMonitorSize { get; set; } = new Rectangle();

    /// <summary>
    /// The margin around the window for the current window to compensate for any non-usable area
    /// such as the task bar
    /// </summary>
    public Thickness CurrentMonitorMargin { get; private set; } = new Thickness();

    /// <summary>
    /// The size and position of the current screen in relation to the multi-screen desktop
    /// For example a second monitor on the right will have a Left position of
    /// the X resolution of the screens on the left
    /// </summary>
    public Rect CurrentScreenSize => _screenSize;

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="window">The window to monitor and correctly maximize</param>
    /// <param name="adjustSize">The callback for the host to adjust the maximum available size if needed</param>
    public WindowSizeManager(Window window)
    {
        _window = window;

        // Listen out for source initialized to setup
        _window.SourceInitialized += Window_SourceInitialized!;

        // Monitor for edge docking
        _window.SizeChanged += Window_SizeChanged;
        _window.LocationChanged += Window_LocationChanged!;
    }

    #endregion

    #region Initialize

    /// <summary>
    /// Initialize and hook into the windows message pump
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_SourceInitialized(object sender, System.EventArgs e)
    {
        // Get the handle of this window
        var handle = (new WindowInteropHelper(_window)).Handle;
        var handleSource = HwndSource.FromHwnd(handle);

        // If not found, end
        if (handleSource == null)
            return;

        // Hook into it's Windows messages
        handleSource.AddHook(WindowProc);
    }

    #endregion

    #region Edge Docking

    /// <summary>
    /// Monitor for moving of the window and constantly check for docked positions
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_LocationChanged(object sender, EventArgs e)
    {
        Window_SizeChanged(null, null);
    }

    /// <summary>
    /// Monitors for size changes and detects if the window has been docked (Aero snap) to an edge
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Make sure our monitor info is up-to-date
        WmGetMinMaxInfo(IntPtr.Zero, IntPtr.Zero);

        // Get the monitor transform for the current position
        _monitorDpi = VisualTreeHelper.GetDpi(_window);

        // Cannot calculate size until we know monitor scale
        if (_monitorDpi == null)
            return;

        // Get window rectangle
        var top = _window.Top;
        var left = _window.Left;
        var bottom = top + _window.Height;
        var right = left + _window.Width;

        // Get window position/size in device pixels
        var windowTopLeft = new Point(left * _monitorDpi.Value.DpiScaleX, top * _monitorDpi.Value.DpiScaleX);
        var windowBottomRight = new Point(right * _monitorDpi.Value.DpiScaleX, bottom * _monitorDpi.Value.DpiScaleX);

        // Check for edges docked
        var edgedTop = windowTopLeft.Y <= (_screenSize.Top + _edgeTolerance) && windowTopLeft.Y >= (_screenSize.Top - _edgeTolerance);
        var edgedLeft = windowTopLeft.X <= (_screenSize.Left + _edgeTolerance) && windowTopLeft.X >= (_screenSize.Left - _edgeTolerance);
        var edgedBottom = windowBottomRight.Y >= (_screenSize.Bottom - _edgeTolerance) && windowBottomRight.Y <= (_screenSize.Bottom + _edgeTolerance);
        var edgedRight = windowBottomRight.X >= (_screenSize.Right - _edgeTolerance) && windowBottomRight.X <= (_screenSize.Right + _edgeTolerance);

        // Get docked position
        var dock = WindowDockPosition.Undocked;

        // Left docking
        if (edgedTop && edgedBottom && edgedLeft)
            dock = WindowDockPosition.Left;
        // Right docking
        else if (edgedTop && edgedBottom && edgedRight)
            dock = WindowDockPosition.Right;
        // Top/bottom
        else if (edgedTop && edgedBottom)
            dock = WindowDockPosition.TopBottom;
        // Top-left
        else if (edgedTop && edgedLeft)
            dock = WindowDockPosition.TopLeft;
        // Top-right
        else if (edgedTop && edgedRight)
            dock = WindowDockPosition.TopRight;
        // Bottom-left
        else if (edgedBottom && edgedLeft)
            dock = WindowDockPosition.BottomLeft;
        // Bottom-right
        else if (edgedBottom && edgedRight)
            dock = WindowDockPosition.BottomRight;

        // None
        else
            dock = WindowDockPosition.Undocked;

        // If dock has changed
        if (dock != mLastDock)
            // Inform listeners
            WindowDockChanged(dock);

        // Save last dock position
        mLastDock = dock;
    }

    #endregion

    #region Windows Message Pump

    /// <summary>
    /// Listens out for all windows messages for this window
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <param name="handled"></param>
    /// <returns></returns>
    private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            // Handle the GetMinMaxInfo of the Window
            case 0x0024: // WM_GETMINMAXINFO
                WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
                break;

            // Once the window starts being moved
            case 0x0231: // WM_ENTERSIZEMOVE
                _beingMoved = true;
                WindowStartedMove();
                break;

            // Once the window has finished being moved
            case 0x0232: // WM_EXITSIZEMOVE
                _beingMoved = false;
                WindowFinishedMove();
                break;
        }

        return (IntPtr)0;
    }

    #endregion

    private void WmGetMinMaxInfo(nint hwnd, nint lParam)
    {
        // گرفتن موقعیت موس برای تشخیص مانیتور فعلی
        GetCursorPos(out var mousePos);

        // گرفتن هندل مانیتور فعلی
        var currentMonitor = _beingMoved
            ? MonitorFromPoint(mousePos, MonitorOptions.MONITOR_DEFAULTTONULL)
            : MonitorFromWindow(hwnd, MonitorOptions.MONITOR_DEFAULTTONULL);

        // گرفتن اطلاعات مانیتور ای که پنجره در آن قرار دارد نه صرفا مانیتور اصلی سیتم
        // چون ممکن است چند مانیتور به سیستم وصل باشند
        var monitorInfo = new MONITORINFO();
        if (!GetMonitorInfo(currentMonitor, monitorInfo))
            return;

        // گرفتن اطلاعات مانیتور اصلی برای مقایسه
        //var lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
        //var lPrimaryScreenInfo = new MONITORINFO();
        //if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
        //    return;

        //var primaryX = lPrimaryScreenInfo.RCWork.Left - lPrimaryScreenInfo.RCMonitor.Left;
        //var primaryY = lPrimaryScreenInfo.RCWork.Top - lPrimaryScreenInfo.RCMonitor.Top;
        //var primaryWidth = lPrimaryScreenInfo.RCWork.Right - lPrimaryScreenInfo.RCWork.Left;
        //var primaryHeight = lPrimaryScreenInfo.RCWork.Bottom - lPrimaryScreenInfo.RCWork.Top;
        //var primaryRatio = primaryWidth / (float)primaryHeight;

        // گرفتن DPI فعلی
        _monitorDpi = VisualTreeHelper.GetDpi(_window);
        if (_monitorDpi is null)
            return;

        var monitorDpiValue = _monitorDpi.Value;
        var monitorRCWork = monitorInfo.RCWork;
        var monitorRCMonitor = monitorInfo.RCMonitor;

        // تنظیم اندازه‌ی قابل استفاده (RCWork) به جای اندازه‌ی کامل مانیتور (RCMonitor)
        if (lParam != nint.Zero)
        {
            var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            // Set to current monitor size
            mmi.PointMaxPosition.X = monitorRCWork.Left;
            mmi.PointMaxPosition.Y = monitorRCWork.Top;
            mmi.PointMaxSize.X = monitorRCWork.Right - monitorRCWork.Left;
            mmi.PointMaxSize.Y = monitorRCWork.Bottom - monitorRCWork.Top;

            // تنظیم حداقل اندازه‌ی پنجره با در نظر گرفتن DPI
            var minSize = new Point
            {
                X = _window.MinWidth * monitorDpiValue.DpiScaleX,
                Y = _window.MinHeight * monitorDpiValue.DpiScaleY
            };
            mmi.PointMinTrackSize.X = (int)minSize.X;
            mmi.PointMinTrackSize.Y = (int)minSize.Y;

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        // به‌روزرسانی اطلاعات مانیتور و حاشیه‌ها
        CurrentMonitorSize = new Rectangle
        {
            Left = monitorRCWork.Left,
            Top = monitorRCWork.Top,
            Right = monitorRCWork.Right,
            Bottom = monitorRCWork.Bottom
        };

        var leftMargin = monitorRCWork.Left - monitorRCMonitor.Left;
        var topMargin = monitorRCWork.Top - monitorRCMonitor.Top;
        var rightMargin = monitorRCMonitor.Right - monitorRCWork.Right;
        var bottomMargin = monitorRCMonitor.Bottom - monitorRCWork.Bottom;
        CurrentMonitorMargin = new Thickness
        {
            Left = leftMargin / monitorDpiValue.DpiScaleX,
            Top = topMargin / monitorDpiValue.DpiScaleY,
            Right = rightMargin / monitorDpiValue.DpiScaleX,
            Bottom = bottomMargin / monitorDpiValue.DpiScaleY
        };

        // Store new size
        _screenSize = new Rect
        {
            X = monitorRCWork.Left,
            Y = monitorRCWork.Top,
            Width = monitorRCWork.Right - monitorRCWork.Left,
            Height = monitorRCWork.Bottom - monitorRCWork.Top
        };
    }

    /// <summary>
    /// Gets the current cursor position in screen coordinates relative to an entire multi-desktop position
    /// </summary>
    /// <returns></returns>
    public Point GetCursorPosition()
    {
        // Get mouse position
        GetCursorPos(out var lMousePosition);

        // Apply DPI scaling
        return new Point(lMousePosition.X / _monitorDpi.Value.DpiScaleX, lMousePosition.Y / _monitorDpi.Value.DpiScaleY);
    }
}

#region DLL Helper Structures

public enum MonitorOptions : uint
{
    MONITOR_DEFAULTTONULL = 0x00000000,
    MONITOR_DEFAULTTOPRIMARY = 0x00000001,
    MONITOR_DEFAULTTONEAREST = 0x00000002
}


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class MONITORINFO
{
#pragma warning disable IDE1006 // Naming Styles
    public int CBSize = Marshal.SizeOf(typeof(MONITORINFO));
    public Rectangle RCMonitor = new Rectangle();
    public Rectangle RCWork = new Rectangle();
    public int DWFlags = 0;
#pragma warning restore IDE1006 // Naming Styles
}


[StructLayout(LayoutKind.Sequential)]
public struct Rectangle
{
#pragma warning disable IDE1006 // Naming Styles
    public int Left, Top, Right, Bottom;
#pragma warning restore IDE1006 // Naming Styles

    public Rectangle(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct MINMAXINFO
{
#pragma warning disable IDE1006 // Naming Styles
    public POINT PointReserved;
    public POINT PointMaxSize;
    public POINT PointMaxPosition;
    public POINT PointMinTrackSize;
    public POINT PointMaxTrackSize;
#pragma warning restore IDE1006 // Naming Styles
};

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    /// <summary>
    /// x coordinate of point.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    public int X;
#pragma warning restore IDE1006 // Naming Styles

    /// <summary>
    /// y coordinate of point.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    public int Y;
#pragma warning restore IDE1006 // Naming Styles

    /// <summary>
    /// Construct a point of coordinates (x,y).
    /// </summary>
    public POINT(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"{X} {Y}";
    }
}

#endregion
