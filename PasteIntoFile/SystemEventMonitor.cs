using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace PasteIntoFile {
    /// <summary>
    /// Monitor and react to global events like hotkeys and clipboard updates
    /// </summary>
    public sealed class SystemEventMonitor : IDisposable {
        // Registers a hot key with Windows.
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Clipboard listener APIs
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private sealed class Window : NativeWindow, IDisposable {
            private static int WM_HOTKEY = 0x0312;
            private static int WM_CLIPBOARDUPDATE = 0x031D;

            public Window() {
                // create the handle for the window.
                CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m) {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY) {
                    // get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    KeyPressed?.Invoke(this, new KeyPressedEventArgs(modifier, key));
                }

                // check if we got a clipboard update
                if (m.Msg == WM_CLIPBOARDUPDATE) {
                    ClipboardChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;
            public event EventHandler ClipboardChanged;

            #region IDisposable Members

            public void Dispose() {
                DestroyHandle();
            }

            #endregion
        }

        private Window _window;
        private int _currentId;

        // clipboard monitoring state
        private volatile bool _clipboardMonitoring;

        public SystemEventMonitor() {
            _window = new Window();

            // register the event of the inner native window.
            _window.KeyPressed += delegate (object sender, KeyPressedEventArgs args) {
                KeyPressed?.Invoke(this, args);
            };

            // Map window clipboard events to the monitor, with safe access handling
            _window.ClipboardChanged += (s, e) => {
                if (!_clipboardMonitoring) return;

                // Ensure clipboard is readable before firing event
                for (var i = 0; i < 10; i++) {
                    try {
                        Clipboard.GetDataObject();
                        // Clipboard access works, fire event
                        ClipboardChanged?.Invoke(this, EventArgs.Empty);
                        break;
                    } catch {
                        Thread.Sleep(100);
                    }
                }

            };
        }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(ModifierKeys modifier, Keys key) {
            // increment the counter.
            _currentId += 1;

            // register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key)) {
                var error = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                throw new InvalidOperationException($"Registration of HotKey {modifier.ToString().Replace(", ", "+").ToUpper()}+{key} failed with error {error.NativeErrorCode}: {error.Message}");
            }
        }

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        /// <summary>
        /// Fired when the clipboard was changed and the new content is accessible
        /// </summary>
        public event EventHandler ClipboardChanged;

        public bool IsClipboardMonitoring => _clipboardMonitoring;

        /// <summary>
        /// Starts monitoring the clipboard for changes.
        /// When the clipboard content changes and is accessible, the ClipboardChanged event will be fired.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void StartClipboardMonitoring() {
            if (_clipboardMonitoring) return;
            if (_window == null) throw new InvalidOperationException("Internal window not created.");

            if (!AddClipboardFormatListener(_window.Handle)) {
                var err = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                throw new InvalidOperationException($"AddClipboardFormatListener failed: {err.NativeErrorCode} {err.Message}");
            }

            _clipboardMonitoring = true;
        }

        /// <summary>
        /// Stops monitoring the clipboard for changes.
        /// </summary>
        public void StopClipboardMonitoring() {
            if (!_clipboardMonitoring) return;
            try {
                RemoveClipboardFormatListener(_window.Handle);
            } catch {
                // ignored
            }
            _clipboardMonitoring = false;
        }

        /// <summary>
        /// Calls a callback while temporarily stopping clipboard monitoring.
        /// The callback may alter the clipboard without triggering the ClipboardChanged event.
        /// </summary>
        /// <param name="callback"></param>
        public void CallWithoutClipboardMonitoring(Action callback) {
            var wasMonitoring = _clipboardMonitoring;
            try {
                StopClipboardMonitoring();
            } catch {
                // ignored
            }
            try {
                callback?.Invoke();
            } finally {
                if (wasMonitoring) {
                    StartClipboardMonitoring();
                }
            }
        }

        #region IDisposable Members

        public void Dispose() {
            // stop clipboard monitoring
            StopClipboardMonitoring();

            // unregister all the registered hot keys.
            for (var i = _currentId; i > 0; i--) {
                UnregisterHotKey(_window.Handle, i);
            }

            // dispose the inner native window.
            _window.Dispose();
            _window = null;
        }

        #endregion
    }

    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs {
        private ModifierKeys _modifier;
        private Keys _key;

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key) {
            _modifier = modifier;
            _key = key;
        }

        public ModifierKeys Modifier => _modifier;

        public Keys Key => _key;
    }

    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
