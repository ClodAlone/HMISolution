using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thriple.Controls
{
	/// <summary>
	/// Contains the available destinations for a rotation of ContentControl3D.
	/// A value of this enumeration can be passed as a parameter to the RotateCommand.
	/// </summary>
	public enum RotationDestination
	{
		/// <summary>
		/// The rotation will bring the back side of the 3D surface into view.
		/// If the back side is already in view, the rotation will not occur.
		/// </summary>
		BackSide,

		/// <summary>
		/// The rotation will bring the front side of the 3D surface into view.
		/// If the front side is already in view, the rotation will not occur.
		/// </summary>
		FrontSide
	}
}