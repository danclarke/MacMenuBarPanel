// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MacMenuBarPanel.MenuBarPanel
{
	[Register ("PanelController")]
	partial class PanelController
	{
		[Outlet]
		MacMenuBarPanel.MenuBarPanel.BackgroundView BackgroundView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}
		}
	}

	[Register ("Panel")]
	partial class Panel
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
