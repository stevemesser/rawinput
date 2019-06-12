using System;
using RawInput_dll;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;


namespace Keyboard
{
    public partial class Keyboard : Form
    {
        private readonly RawInput _rawinput;
        
        const bool CaptureOnlyInForeground = true;
        // Todo: add checkbox to form when checked/uncheck create method to call that does the same as Keyboard ctor 

        public Keyboard()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _rawinput = new RawInput(Handle, CaptureOnlyInForeground);
           
            _rawinput.AddMessageFilter();   // Adding a message filter will cause keypresses to be handled
            Win32.DeviceAudit();            // Writes a file DeviceAudit.txt to the current directory

            _rawinput.KeyPressed += OnKeyPressed;   
        }

        private void OnKeyPressed(object sender, RawInputEventArg e)
        {
            lbHandle.Text = e.KeyPressEvent.DeviceHandle.ToString();
            lbType.Text = e.KeyPressEvent.DeviceType;
            lbName.Text = e.KeyPressEvent.DeviceName;
            lbDescription.Text = e.KeyPressEvent.Name;
            lbKey.Text = e.KeyPressEvent.VKey.ToString(CultureInfo.InvariantCulture);
            lbNumKeyboards.Text = _rawinput.NumberOfKeyboards.ToString(CultureInfo.InvariantCulture);
            lbVKey.Text = e.KeyPressEvent.VKeyName;
            lbSource.Text = e.KeyPressEvent.Source;
            lbKeyPressState.Text = e.KeyPressEvent.KeyPressState;
            lbMessage.Text = string.Format("0x{0:X4} ({0})", e.KeyPressEvent.Message);
           
            //switch (e.KeyPressEvent.Message)
            //{
            //    case Win32.WM_KEYDOWN:
            //        Debug.WriteLine(e.KeyPressEvent.KeyPressState);
            //        break;
            //     case Win32.WM_KEYUP:
            //        Debug.WriteLine(e.KeyPressEvent.KeyPressState);
            //        break;
            //}
        }

        private void Keyboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            _rawinput.KeyPressed -= OnKeyPressed;
        }

        private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (null == ex) return;

            // Log this error. Logging the exception doesn't correct the problem but at least now
            // you may have more insight as to why the exception is being thrown.
            Debug.WriteLine("Unhandled Exception: " + ex.Message);
            Debug.WriteLine("Unhandled Exception: " + ex);
            MessageBox.Show(ex.Message);
        }
    }
}
