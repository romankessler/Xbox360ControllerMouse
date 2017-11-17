namespace Xbox360ControllerMouse
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Timers;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using SharpDX;
    using SharpDX.XInput;

    using Timer = System.Timers.Timer;

    class ControllerInput
    {
        private readonly Controller _controller;

        readonly Timer _keytimer = new Timer();

        readonly Timer _mousetimer = new Timer();

        readonly Timer _scrolltimer = new Timer();

        readonly Timer _timer = new Timer();

        readonly Timer _toggltimer = new Timer();

        private GamepadButtonFlags _buttons;

        private bool _desktopToggled;

        private bool _isMouseActivated;

        private byte _leftTrigger;

        private short _leftXAxis;

        private short _leftYAxis;

        private bool _mouseLeftDown;

        private bool _mouseRightDown;

        private byte _rightTrigger;

        private short _rightXAxis;

        private short _rightYAxis;

        public ControllerInput()
        {
            _timer.Interval = 100;
            _timer.Start();
            _timer.Elapsed += TimerOnElapsed;

            _mousetimer.Interval = 15;
            _mousetimer.Elapsed += MouseUpdate;

            _keytimer.Interval = 100;
            _keytimer.Elapsed += KeytimerOnElapsed;

            _scrolltimer.Interval = 10;
            _scrolltimer.Elapsed += ScrollUpdate;

            _toggltimer.Interval = 100;
            _toggltimer.Elapsed += ToggltimerOnElapsed;

            _controller = new Controller(UserIndex.One);
            Console.WriteLine(!_controller.IsConnected ? "Gamecontroller is not connected ... you know ;)" : "Controller 1 connected!");
        }

        public short LeftXAxis
        {
            get
            {
                return _leftXAxis;
            }
            set
            {
                if (value == _leftXAxis)
                    return;
                _leftXAxis = value;
                OnPropertyChanged();
            }
        }

        public short LeftYAxis
        {
            get
            {
                return _leftYAxis;
            }
            set
            {
                if (value == _leftYAxis)
                    return;
                _leftYAxis = value;
                OnPropertyChanged();
            }
        }

        public short RightXAxis
        {
            get
            {
                return _rightXAxis;
            }
            set
            {
                if (value == _rightXAxis)
                    return;
                _rightXAxis = value;
                OnPropertyChanged();
            }
        }

        public short RightYAxis
        {
            get
            {
                return _rightYAxis;
            }
            set
            {
                if (value == _rightYAxis)
                    return;
                _rightYAxis = value;
                OnPropertyChanged();
            }
        }

        public GamepadButtonFlags Buttons
        {
            get
            {
                return _buttons;
            }
            set
            {
                if (value == _buttons)
                    return;
                _buttons = value;
                OnPropertyChanged();
                OnInputChanged();
            }
        }

        public byte LeftTrigger
        {
            get
            {
                return _leftTrigger;
            }
            set
            {
                if (value == _leftTrigger)
                    return;
                _leftTrigger = value;
                OnPropertyChanged();
                OnInputChanged();
            }
        }

        public byte RightTrigger
        {
            get
            {
                return _rightTrigger;
            }
            set
            {
                if (value == _rightTrigger)
                    return;
                _rightTrigger = value;
                OnPropertyChanged();
                OnInputChanged();
            }
        }

        private void KeytimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            ArrowKeys();
            KeyUpdate();
        }

        private void KeyUpdate()
        {
            if (CheckActivationDelay())
            {
                return;
            }

            if (Buttons.HasFlag(GamepadButtonFlags.X))
            {
                SendKeys.SendWait("{ENTER}");
            }
            if (Buttons.HasFlag(GamepadButtonFlags.Y))
            {
                SendKeys.SendWait("{ESCAPE}");
            }

            if (Buttons.HasFlag(GamepadButtonFlags.Start))
            {
                var onscreekeyboardPath = "C:\\Windows\\System32\\osk.exe";
                if (_onscreenKeyboardProcess == null)
                {
                    _onscreenKeyboardProcess = Process.Start(onscreekeyboardPath);
                }
                else
                {
                    _onscreenKeyboardProcess.Kill();
                    _onscreenKeyboardProcess = null;
                }
                
            }

            if (Buttons.HasFlag(GamepadButtonFlags.LeftShoulder)
                && Buttons.HasFlag(GamepadButtonFlags.RightShoulder))
            {
                SendKeys.SendWait("%{F4}");
            }
        }

        private Process _onscreenKeyboardProcess;

        private bool CheckActivationDelay()
        {
            return _activationTime + TimeSpan.FromMilliseconds(2000) >= DateTime.Now;
        }

        private void ToggltimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (CheckActivationDelay())
            {
                return;
            }

            ToggleDesktop();
            ToggleTaskMenu();
        }

        private bool _taskmenutoggled;

        private void ToggleTaskMenu()
        {
            if (LeftTrigger > 250 && RightTrigger > 250)
            {
                if (!_taskmenutoggled)
                {
                    _taskmenutoggled = true;
                    KeyboardSendHelper.KeyDown(Keys.LWin);
                    KeyboardSendHelper.KeyDown(Keys.Tab);
                    KeyboardSendHelper.KeyUp(Keys.LWin);
                    KeyboardSendHelper.KeyUp(Keys.Tab);
                }
            }
            else
            {
                _taskmenutoggled = false;
            }
        }

        private void ScrollUpdate(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            ScrollPosition();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            DisplayControllerInformation();
        }

        void DisplayControllerInformation()
        {
            try
            {
                var state = _controller.GetState();
                LeftXAxis = state.Gamepad.LeftThumbX;
                LeftYAxis = state.Gamepad.LeftThumbY;

                RightXAxis = state.Gamepad.RightThumbX;
                RightYAxis = state.Gamepad.RightThumbY;

                Buttons = state.Gamepad.Buttons;

                LeftTrigger = state.Gamepad.LeftTrigger;
                RightTrigger = state.Gamepad.LeftTrigger;
            }
            catch (SharpDXException exception)
            {
                Console.WriteLine("No controller detected");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
        }

        private void OnInputChanged()
        {
            if (Buttons.HasFlag(GamepadButtonFlags.LeftShoulder)
                && Buttons.HasFlag(GamepadButtonFlags.RightShoulder)
                && LeftTrigger == 255
                && RightTrigger == 255)
            {
                ActivateDeactivateMouseMode();
            }
        }

        private void MoveMousePosition()
        {
            var cursorPosition = MousePositionHelper.GetCursorPosition();

            const int TOLERANCEX = 7200;
            const int TOLERANCEY = 7200;
            const int MULTIPLICATOR = 2000;

            var x = 0;
            var y = 0;

            if (LeftXAxis > TOLERANCEX
                || LeftXAxis < -TOLERANCEX)
            {
                x = LeftXAxis / MULTIPLICATOR;
            }

            if (LeftYAxis > TOLERANCEY
                || LeftYAxis < -TOLERANCEY)
            {
                y = LeftYAxis / MULTIPLICATOR;
            }

            Console.WriteLine($"{LeftXAxis} , {LeftYAxis}");

            MousePositionHelper.SetCursorPos(cursorPosition.X + x, cursorPosition.Y - y);
        }

        private void ScrollPosition()
        {
            const int TOLERANCEX = 4300;
            const int TOLERANCEY = 3600;
            const int MULTIPLICATOR = 2000;

            var x = 0;
            var y = 0;

            if (RightXAxis > TOLERANCEX
                || RightXAxis < -TOLERANCEX)
            {
                x = RightXAxis / MULTIPLICATOR;
            }

            if (RightYAxis > TOLERANCEY
                || RightYAxis < -TOLERANCEY)
            {
                y = RightYAxis / MULTIPLICATOR;
            }

            Console.WriteLine($"{LeftXAxis} , {LeftYAxis}");

            if (y != 0)
            {
                MouseScrollHelper.ScrollVertical(y);
            }

            if (x != 0)
            {
                MouseScrollHelper.ScrollHorizontal(x);
            }
        }

        private DateTime _activationTime;

        private void ActivateDeactivateMouseMode()
        {
            _isMouseActivated = !_isMouseActivated;

            const int BEEP_DURATION = 30;
            const int BEEP_FREQ_1 = 500;
            const int BEEP_FREQ_2 = 600;
            const int BEEP_FREQ_3 = 700;

            
            if (_isMouseActivated)
            {
                _activationTime = DateTime.Now;
                // activate
                Console.WriteLine("Mouse activated");
                Console.Beep(BEEP_FREQ_1, BEEP_DURATION);
                Console.Beep(BEEP_FREQ_2, BEEP_DURATION);
                Console.Beep(BEEP_FREQ_3, BEEP_DURATION);
                _mousetimer.Start();
                _keytimer.Start();
                _scrolltimer.Start();
                _toggltimer.Start();
            }
            else
            {                _activationTime = DateTime.Now;
                // deavtivate
                Console.WriteLine("Mouse deactivated");
                Console.Beep(BEEP_FREQ_1, BEEP_DURATION);
                Console.Beep(BEEP_FREQ_2, BEEP_DURATION);
                Console.Beep(BEEP_FREQ_1, BEEP_DURATION);
                _mousetimer.Stop();
                _keytimer.Stop();
                _scrolltimer.Stop();
                _toggltimer.Stop();
            }
        }

        private void MouseUpdate(object sender, ElapsedEventArgs e)
        {
            MoveMousePosition();

            LeftMouseClick();
            RightMouseClick();
        }

        private void ArrowKeys()
        {
            if (Buttons.HasFlag(GamepadButtonFlags.DPadUp))
            {
                SendKeys.SendWait("{UP}");
            }
            if (Buttons.HasFlag(GamepadButtonFlags.DPadDown))
            {
                SendKeys.SendWait("{DOWN}");
            }
            if (Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
            {
                SendKeys.SendWait("{LEFT}");
            }
            if (Buttons.HasFlag(GamepadButtonFlags.DPadRight))
            {
                SendKeys.SendWait("{RIGHT}");
            }
        }

        private void ToggleDesktop()
        {
            if (Buttons.HasFlag(GamepadButtonFlags.Back))
            {
                if (!_desktopToggled)
                {
                    DesktopHelper.ToggleDesktop();
                    _desktopToggled = true;
                }
            }
            else
            {
                _desktopToggled = false;
            }
        }

        private void RightMouseClick()
        {
            if (Buttons.HasFlag(GamepadButtonFlags.B))
            {
                if (!_mouseRightDown)
                {
                    var cursorPosition = MousePositionHelper.GetCursorPosition();
                    MouseClickHelper.RightMouseClickDown(cursorPosition.X, cursorPosition.Y);
                    _mouseRightDown = true;
                }
            }
            else
            {
                if (_mouseRightDown)
                {
                    var cursorPosition = MousePositionHelper.GetCursorPosition();
                    MouseClickHelper.RightMouseClickUp(cursorPosition.X, cursorPosition.Y);
                    _mouseRightDown = false;
                }
            }
        }

        private void LeftMouseClick()
        {
            if (Buttons.HasFlag(GamepadButtonFlags.A))
            {
                if (!_mouseLeftDown)
                {
                    var cursorPosition = MousePositionHelper.GetCursorPosition();
                    MouseClickHelper.LeftMouseClickDown(cursorPosition.X, cursorPosition.Y);
                    _mouseLeftDown = true;
                }
            }
            else
            {
                if (_mouseLeftDown)
                {
                    var cursorPosition = MousePositionHelper.GetCursorPosition();
                    MouseClickHelper.LeftMouseClickUp(cursorPosition.X, cursorPosition.Y);
                    _mouseLeftDown = false;
                }
            }
        }
    }
}