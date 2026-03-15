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

#include "pch.h"
#include "BlankPage.xaml.h"

using namespace WriteableBitmapExSimpleCircleSampleCppWinMD;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

BlankPage::BlankPage()
{
	InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void BlankPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	auto writeableBitmap = ref new WriteableBitmap(512, 512);
	WriteableBitmapExtensions::DrawLine(writeableBitmap, 10, 10, 50, 50, Windows::UI::Colors::CadetBlue);
	Viewport->Source = writeableBitmap;
}

