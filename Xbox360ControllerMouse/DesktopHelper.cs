namespace Xbox360ControllerMouse
{
    using System;
    using System.Reflection;

    static class DesktopHelper
    {
        public static void ToggleDesktop()
        {
            var typeShell = Type.GetTypeFromProgID("Shell.Application");
            var objShell = Activator.CreateInstance(typeShell);
            typeShell.InvokeMember("ToggleDesktop", BindingFlags.InvokeMethod, null, objShell, null); // Call function MinimizeAll
        }
    }
}