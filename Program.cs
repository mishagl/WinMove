using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

if (args.Length == 0)
{
    Console.Error.WriteLine("usage: WinMove <app_name>");
    Environment.Exit(-1);
}

// P/Invoke declarations.
[DllImport("user32.dll", SetLastError = true)]
static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

[DllImport("user32.dll")]
static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);

const uint SWP_NOSIZE = 0x0001;
const uint SWP_NOZORDER = 0x0004;
const int TRUE = 1;

var procName = args[0];

var matchingProcs = Process.GetProcessesByName(procName);

var process = matchingProcs.FirstOrDefault(w => w.MainWindowHandle != IntPtr.Zero);
IntPtr hWnd = process?.MainWindowHandle ?? IntPtr.Zero;
// If found, position it.
if (hWnd != IntPtr.Zero)
{
    if (GetWindowRect(hWnd, out var rectangle) == TRUE 
        && (Math.Abs(rectangle.Width) > 200 || Math.Abs(rectangle.Height) > 200))
    {
        Console.WriteLine($"`{process!.MainWindowTitle}` is being moved to 0,0. Size W: {Math.Abs(rectangle.Width)}, H: {Math.Abs(rectangle.Height)}");
        // Move the window to (0,0) without changing its size
        // in the Z order.
        SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE);
    }
    else
    {
        Console.WriteLine($"{process!.MainWindowTitle} is being moved to 0,0 and resized");
        // Move the window to (0,0) and change its size
        // in the Z order.
        SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 200, 200, SWP_NOZORDER);
    }
}
else
{
    Console.Error.WriteLine($"App {args[0]} not found");
}