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

using MonoMac.AppKit;

namespace MacMenuBarPanel.MenuBarPanel
{
    public class StatusPanelController : IDisposable
    {
        public const float StatusItemViewWidth = 24.0f;

        private readonly NSStatusItem _statusItem;
        private readonly StatusItemView _statusItemView;

        private PanelController _panelController;
        private bool _disposed = false;

        public StatusPanelController(PanelController panelController)
        {
            _panelController = panelController;
            _statusItem = NSStatusBar.SystemStatusBar.CreateStatusItem(StatusItemViewWidth);
            _statusItemView = new StatusItemView(_statusItem)
            {
                Image = NSImage.ImageNamed("Status"),
                AlternateImage = NSImage.ImageNamed("StatusHighlighted")
            };
            _statusItemView.StatusItemClicked += HandleStatusItemClicked;
            _panelController.StatusItemView = _statusItemView;
        }

        protected NSStatusItem StatusItem { get { return _statusItem; } }

        /// <summary>
        /// Gets or sets the panel controller.
        /// </summary>
        /// <value>
        /// The panel controller.
        /// </value>
        public virtual PanelController PanelController
        {
            get { return _panelController; }
            set
            {
                if (_panelController == value)
                    return;

                _panelController.ClosePanel();

                _panelController = value;
                _panelController.StatusItemView = _statusItemView;
                _panelController.OpenPanel();
            }
        }

        protected virtual void HandleStatusItemClicked (object sender, EventArgs e)
        {
            _statusItemView.IsHighlighted = !_statusItemView.IsHighlighted;

            if (_statusItemView.IsHighlighted)
                PanelController.OpenPanel();
            else
                PanelController.ClosePanel();
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Clear managed resourced here
            }

            if (_statusItem != null)
            {
                NSStatusBar.SystemStatusBar.RemoveStatusItem(_statusItem);
                _statusItem = null;
            }

            _disposed = true;
        }

        ~StatusPanelController()
        {
            Dispose(false);
        }

        #endregion

    }
}

