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

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MacMenuBarPanel.MenuBarPanel
{
    [Register("BackgroundView")]
    public class BackgroundView : NSView
    {
        public const float ArrowWidth = 12.0f;
        public const float ArrowHeight = 8.0f;

        protected const float FillOpacity = 0.9f;
        protected const float StrokeOpacity = 1.0f;
        protected const float LineThickness = 1.0f;
        protected const float CornerRadius = 6.0f;

        #region Constructors

        public BackgroundView(NSCoder coder) : base(coder) {}
        public BackgroundView(IntPtr handle) : base(handle) {}
        public BackgroundView(RectangleF frameRect) : base(frameRect) {}

        #endregion

        private int _arrowX;
        /// <summary>
        /// X coordinate of where the 'arrow' should be within the view
        /// </summary>
        /// <value>
        /// Arrow X coord
        /// </value>
        public virtual int ArrowX
        {
            get { return _arrowX; }
            set
            {
                _arrowX = value;
                NeedsDisplay = true;
            }
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            var contentRect = new RectangleF(Bounds.X + LineThickness, Bounds.Y + LineThickness, Bounds.Width - (LineThickness * 2), Bounds.Height - (LineThickness * 2));

            // Mac coords are reversed vs. .Net / MonoTouch coords, so we just reverse the top/bottom coords to compensate
            var top = contentRect.Bottom;
            var bottom = contentRect.Top;

            var left = contentRect.Left;
            var right = contentRect.Right;
            var path = new NSBezierPath();

            // Draw the 'arrow' at the top
            path.MoveTo(new PointF(ArrowX, top));
            path.LineTo(new PointF(ArrowX + ArrowWidth / 2, top - ArrowHeight));
            path.LineTo(new PointF(right - CornerRadius, top - ArrowHeight));

            // Right right
            var topRightCorner = new PointF(right, top - ArrowHeight);
            path.CurveTo(new PointF(right, top - ArrowHeight - CornerRadius), topRightCorner, topRightCorner);

            // Right line
            path.LineTo(new PointF(right, bottom + CornerRadius));

            // Bottom right
            var bottomRightCorner = new PointF(right, bottom);
            path.CurveTo(new PointF(right - CornerRadius, bottom), bottomRightCorner, bottomRightCorner);

            // Bottom line
            path.LineTo(new PointF(left + CornerRadius, bottom));

            // Bottom left
            var bottomLeftCorner = new PointF(left, bottom);
            path.CurveTo(new PointF(left, bottom + CornerRadius), bottomLeftCorner, bottomLeftCorner);

            // Left line
            path.LineTo(new PointF(left, top - ArrowHeight - CornerRadius));

            // Top left
            var topLeftCorner = new PointF(left, top - ArrowHeight);
            path.CurveTo(new PointF(left + CornerRadius, top - ArrowHeight), topLeftCorner, topLeftCorner);

            // Line up to start of 'arrow' & finish
            path.LineTo(new PointF(ArrowX - ArrowWidth / 2, top - ArrowHeight));
            path.ClosePath();

            // Fill the path with a semi-transparent white
            NSColor.FromDeviceWhite(1.0f, FillOpacity).SetFill();
            path.Fill();

            NSGraphicsContext.GlobalSaveGraphicsState();

            // Clip all rendering of controls within view to within the path outline we specified earlier
            var clip = NSBezierPath.FromRect(Bounds);
            clip.AppendPath(path);
            clip.AddClip();

            // Draw the border
            path.LineWidth = LineThickness * 2;
            NSColor.White.SetStroke();
            path.Stroke();

            NSGraphicsContext.GlobalRestoreGraphicsState();
        }
    }
}

