//
//   Project:           WriteableBitmapEx - WriteableBitmap extensions
//   Description:       Collection of interchange extension methods for the WriteableBitmap class.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2012-05-05 20:52:49 +0200 (Sa, 05 Mai 2012) $
//   Changed in:        $Revision: 90091 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/branches/WBX_1.0_BitmapContext/Source/WriteableBitmapEx/WriteableBitmapConvertExtensions.cs $
//   Id:                $Id: WriteableBitmapConvertExtensions.cs 90091 2012-05-05 18:52:49Z unknown $
//
//
//   Copyright © 2009-2012 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//

#pragma once

#include "pch.h"
#include "BlankPage.g.h"

namespace WriteableBitmapExSimpleCircleSampleCppWinMD
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class BlankPage sealed
	{
	public:
		BlankPage();

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
	};
}
