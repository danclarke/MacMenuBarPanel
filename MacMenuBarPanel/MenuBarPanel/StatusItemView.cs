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
using System.Drawing;

using MonoMac.AppKit;

namespace MacMenuBarPanel.MenuBarPanel
{
    public class StatusItemView : NSView
    {
        private readonly NSStatusItem _statusItem;
        private bool _mouseDown;

        public StatusItemView(NSStatusItem statusItem)
            : base(new RectangleF(0f, 0f, statusItem.Length, NSStatusBar.SystemStatusBar.Thickness))
        {
            _statusItem = statusItem;
            _statusItem.View = this;
        }

        public event EventHandler StatusItemClicked;

        private NSImage _image;
        public NSImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                NeedsDisplay = true;
            }
        }

        private NSImage _alternateImage;
        public NSImage AlternateImage
        {
            get { return _alternateImage; }
            set
            {
                _alternateImage = value;
                NeedsDisplay = true;
            }
        }

        public RectangleF GlobalRect
        {
            get
            {
                return new RectangleF(Window.ConvertBaseToScreen(Frame.Location), Frame.Size);
            }
        }

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                if (_isHighlighted == value)
                    return;

                _isHighlighted = value;
                NeedsDisplay = true;
            }
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            _statusItem.DrawStatusBarBackgroundInRectwithHighlight(dirtyRect, IsHighlighted);

            var icon = IsHighlighted ? AlternateImage : Image;

            if (icon == null)
                return;

            float iconX = (float)Math.Round((Bounds.Width - icon.Size.Width) / 2f);
            float iconY = (float)Math.Round((Bounds.Height - icon.Size.Height) / 2f);
            icon.Draw(new PointF(iconX, iconY), RectangleF.Empty, NSCompositingOperation.SourceOver, 1.0f);
        }

        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);

            _mouseDown = true;
        }

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);

            if (StatusItemClicked != null && _mouseDown)
                StatusItemClicked(this, EventArgs.Empty);

            _mouseDown = false;
        }
    }
}

