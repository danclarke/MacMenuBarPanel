/*
    Copyright (c) 2012, Dan Clarke
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

        Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
        Neither the name of Dan Clarke nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Diagnostics;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MacMenuBarPanel.MenuBarPanel
{
    public partial class PanelController : NSWindowController
    {
        private const float OpenDuration = 0.15f;
        private const float CloseDuration = 0.1f;
        private const float MenuAnimationDuration = 0.1f;

		#region Constructors
		
        // Called when created from unmanaged code
        public PanelController(IntPtr handle) : base(handle)
        {
            Initialize();
        }
		
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public PanelController(NSCoder coder) : base(coder)
        {
            Initialize();
        }
		
        // Call to load from a XIB/NIB file
        public PanelController(string windowNibName) : base(windowNibName)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public PanelController() : base("Panel")
        {
            Initialize();
        }
		
        // Shared initialization code
        private void Initialize()
        {
            // Additional validation for inheriting implementations
            if (!(base.Window is Panel))
                throw new InvalidOperationException("Window must inherit from Panel");
            if (BackgroundView == null)
                throw new InvalidOperationException("BackgroundView cannot be NULL");

            // Perform styling
            Window.AcceptsMouseMovedEvents = true;
            Window.Level = NSWindowLevel.PopUpMenu;
            Window.IsOpaque = false;
            Window.BackgroundColor = NSColor.Clear;

            // Hookup Events
            Window.WillClose += (sender, e) => ClosePanel();
            Window.DidResignKey += (sender, e) =>
            {
                if (Window.IsVisible)
                    ClosePanel();
            };
            Window.DidResize += HandleWindowDidResize;
        }
		
		#endregion

        /// <summary>
        /// The StatusItemView that toggles this panel
        /// </summary>
        public StatusItemView StatusItemView { get; set; }

        protected virtual void HandleWindowDidResize (object sender, EventArgs e)
        {
            var statusRect = StatusRectForWindow(Window);
            var panelRect = Window.Frame;

            float statusX = (float)Math.Round(statusRect.Right - (statusRect.Width / 2.0f));
            float panelX = statusX - panelRect.Left;

            BackgroundView.ArrowX = (int)panelX;
        }

        protected RectangleF StatusRectForWindow(NSWindow window)
        {
            RectangleF statusRect;

            if (StatusItemView != null)
            {
                statusRect = StatusItemView.GlobalRect;
            }
            else
            {
                var screenRect = NSScreen.Screens[0].Frame;
                var originX = (screenRect.Width - statusRect.Width) / 2.0f;
                var originY = screenRect.Height - statusRect.Height * 2.0f;
                statusRect = new RectangleF(originX, originY, StatusPanelController.StatusItemViewWidth, NSStatusBar.SystemStatusBar.Thickness);
            }

            return statusRect;
        }

        /// <summary>
        /// Gets whether the panel is currently 'open'
        /// </summary>
        /// <value>
        /// <c>true</c> if this panel is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen { get; protected set; }

        /// <summary>
        /// Opens the panel
        /// </summary>
        public virtual void OpenPanel()
        {
            if (IsOpen)
                return;

            var screenRect = NSScreen.Screens[0].Frame;
            var statusRect = StatusRectForWindow(Window);
           
            var panelRect = Window.Frame;
            panelRect.X = (float)Math.Round((statusRect.Right - (statusRect.Width)) - panelRect.Width / 2f);
            panelRect.Y = statusRect.Top - panelRect.Height;

            if (panelRect.Right > (screenRect.Right - BackgroundView.ArrowHeight))
                panelRect.X -= panelRect.Right - (screenRect.Right - BackgroundView.ArrowHeight);

            NSApplication.SharedApplication.ActivateIgnoringOtherApps(false);
            Window.AlphaValue = 0f;
            Window.SetFrame(statusRect, true);
            Window.MakeKeyAndOrderFront(this);

            var currentEvent = NSApplication.SharedApplication.CurrentEvent;
            float openDuration = OpenDuration;
            if (currentEvent.Type == NSEventType.LeftMouseUp)
            {
                var clearFlags = currentEvent.ModifierFlags & NSEventModifierMask.DeviceIndependentModifierFlagsMask;
                bool shiftPressed = clearFlags == NSEventModifierMask.ShiftKeyMask;
                bool shiftOptionPressed = clearFlags == (NSEventModifierMask.ShiftKeyMask | NSEventModifierMask.AlternateKeyMask);
                if (shiftPressed || shiftOptionPressed)
                {
                    openDuration *= 10f;

                    if (shiftOptionPressed)
                        Debug.WriteLine("Icon is at {0}\n\tMenu is on screen {1}\n\tWill be animated to {2}", statusRect, screenRect, panelRect);
                }
            }

            NSAnimationContext.BeginGrouping();
            NSAnimationContext.CurrentContext.Duration = openDuration;
            var animator = (NSWindow)Window.Animator; // Note the cast neccesary for MonoMac
            animator.SetFrame(panelRect, true);
            animator.AlphaValue = 1f;
            NSAnimationContext.EndGrouping();

            IsOpen = true;
            StatusItemView.IsHighlighted = true;
        }

        /// <summary>
        /// Closes the panel
        /// </summary>
        public virtual void ClosePanel()
        {
            if (!IsOpen)
                return;

            NSAnimationContext.BeginGrouping();
            NSAnimationContext.CurrentContext.Duration = CloseDuration;
            var animator = (NSWindow)Window.Animator; // Note the cast neccesary for MonoMac
            animator.AlphaValue = 0f;
            NSAnimationContext.EndGrouping();

            IsOpen = false;
            StatusItemView.IsHighlighted = false;
        }
		
        //strongly typed window accessor
        public new Panel Window
        {
            get
            {
                return (Panel)base.Window;
            }
        }
    }
}

