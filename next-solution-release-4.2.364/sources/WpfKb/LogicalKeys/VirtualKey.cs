using WindowsInput;

namespace WpfKb.LogicalKeys
{
    public class VirtualKey : LogicalKeyBase
    {
        private VirtualKeyCode _keyCode;

        public virtual VirtualKeyCode KeyCode
        {
            get { return _keyCode; }
            set
            {
                if (value != _keyCode)
                {
                    _keyCode = value;
                    OnPropertyChanged("KeyCode");
                }
            }
        }

        bool needShiftDown;
        public bool NeedShiftDown
        {
            get { return needShiftDown; }
            set
            {
                if (value != needShiftDown)
                {
                    needShiftDown = value;
                    OnPropertyChanged("NeedShiftDown");
                }
            }
        }

        public VirtualKey(VirtualKeyCode keyCode, string displayName)
        {
            DisplayName = displayName;
            KeyCode = keyCode;
        }

        public VirtualKey(VirtualKeyCode keyCode)
        {
            KeyCode = keyCode;
        }

        public VirtualKey()
        {
        }

        public override void Press()
        {
            if (NeedShiftDown)
                InputSimulator.SimulateKeyDown(VirtualKeyCode.SHIFT);
            InputSimulator.SimulateKeyPress(_keyCode);
            base.Press();
            if (NeedShiftDown)
                InputSimulator.SimulateKeyUp(VirtualKeyCode.SHIFT);
        }
    }
}