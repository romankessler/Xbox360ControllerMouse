namespace Xbox360ControllerMouse
{
    using System.Runtime.InteropServices;

    public static class MouseScrollHelper
    {
        private const int MOUSEEVENTF_VWHEEL = 0x0800;

        private const int MOUSEEVENTF_HWHEEL = 0x1000;

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void ScrollHorizontal(int scrollValue)
        {
            // this will cause a horizontal scroll
            mouse_event(MOUSEEVENTF_HWHEEL, 0, 0, scrollValue, 0);
        }

        public static void ScrollVertical(int scrollValue)
        {
            // this will cause a vertical scroll
            mouse_event(MOUSEEVENTF_VWHEEL, 0, 0, scrollValue, 0);
        }
    }
}