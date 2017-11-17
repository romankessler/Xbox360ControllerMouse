namespace Xbox360ControllerMouse
{
    using System.Runtime.InteropServices;

    public static class MouseClickHelper
    {
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;

        public const int MOUSEEVENTF_LEFTUP = 0x04;

        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;

        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        //This simulates a left mouse click
        public static void LeftMouseClickDown(int xpos, int ypos)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
        }

        //This simulates a left mouse click
        public static void LeftMouseClickUp(int xpos, int ypos)
        {
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        //This simulates a right mouse click
        public static void RightMouseClickDown(int xpos, int ypos)
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, xpos, ypos, 0, 0);
        }

        //This simulates a right mouse click
        public static void RightMouseClickUp(int xpos, int ypos)
        {
            mouse_event(MOUSEEVENTF_RIGHTUP, xpos, ypos, 0, 0);
        }
    }
}