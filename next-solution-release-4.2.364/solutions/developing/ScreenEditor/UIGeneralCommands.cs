using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Utilities;

namespace ScreenManager
{
    /// <summary>
    /// Our commands class. This contains all commands we would like to use in our application.
    /// Each command has properties for name, text (using an _ for mnemonics), gestures (like Ctrl+N),
    /// and gesture display text. Fill out strings in the application resources for each of these values
    /// and expose a get property for each command in use.
    /// 
    /// The gesture strings are a ; delimted collection of items parseable by the KeyGestureConverter
    /// class. The gesture display strings are what is displayed to the user in a menu item (for example)
    /// for a particular gesture. Odds are that the gesture display strings will be duplicates
    /// of the gesture strings, but this is how Microsoft does it under the hood.
    /// </summary>
    public static class UIGeneralCommands
    {
        private static GeneralCommand _AddCurrentCamera = new GeneralCommand(
          Properties.UICommandResource.AddCurrentCameraName,
          Properties.UICommandResource.AddCurrentCameraText,
          Properties.UICommandResource.AddCurrentCameraGestures,
          Properties.UICommandResource.AddCurrentCameraGesturesDisplayText,
          Properties.UICommandResource.AddCurrentCameraTooltip,
          Properties.UICommandResource.AddCurrentCameraDescription,
          typeof(UIGeneralCommands));

        public static GeneralCommand AddCurrentCamera
        {
            get { return _AddCurrentCamera; }
        }

        private static GeneralCommand _RemoveCurrentCamera = new GeneralCommand(
         Properties.UICommandResource.RemoveCurrentCameraName,
         Properties.UICommandResource.RemoveCurrentCameraText,
         Properties.UICommandResource.RemoveCurrentCameraGestures,
         Properties.UICommandResource.RemoveCurrentCameraGesturesDisplayText,
         Properties.UICommandResource.RemoveCurrentCameraTooltip,
         Properties.UICommandResource.RemoveCurrentCameraDescription,
         typeof(UIGeneralCommands));

        public static GeneralCommand RemoveCurrentCamera
        {
            get { return _RemoveCurrentCamera; }
        }

        private static GeneralCommand _EnableManipulation = new GeneralCommand(
          Properties.UICommandResource.EnableManipulationName,
          Properties.UICommandResource.EnableManipulationText,
          Properties.UICommandResource.EnableManipulationGestures,
          Properties.UICommandResource.EnableManipulationGesturesDisplayText,
          Properties.UICommandResource.EnableManipulationTooltip,
          Properties.UICommandResource.EnableManipulationDescription,
          typeof(UIGeneralCommands));

        public static GeneralCommand EnableManipulation
        {
            get { return _EnableManipulation; }
        }

        private static GeneralCommand _Enable3DCamera = new GeneralCommand(
          Properties.UICommandResource.Enable3DCameraName,
          Properties.UICommandResource.Enable3DCameraText,
          Properties.UICommandResource.Enable3DCameraGestures,
          Properties.UICommandResource.Enable3DCameraGesturesDisplayText,
          Properties.UICommandResource.Enable3DCameraTooltip,
          Properties.UICommandResource.Enable3DCameraDescription,
          typeof(UIGeneralCommands));

        public static GeneralCommand Enable3DCamera
        {
            get { return _Enable3DCamera; }
        }

        private static GeneralCommand _Edit3DCameraPositions = new GeneralCommand(
         Properties.UICommandResource.Edit3DCameraPositionsName,
         Properties.UICommandResource.Edit3DCameraPositionsText,
         Properties.UICommandResource.Edit3DCameraPositionsGestures,
         Properties.UICommandResource.Edit3DCameraPositionsGesturesDisplayText,
         Properties.UICommandResource.Edit3DCameraPositionsTooltip,
         Properties.UICommandResource.Edit3DCameraPositionsDescription,
         typeof(UIGeneralCommands));

        public static GeneralCommand Edit3DCameraPositions
        {
            get { return _Edit3DCameraPositions; }
        }

        private static GeneralCommand _Enable3D = new GeneralCommand(
          Properties.UICommandResource.Enable3DName,
          Properties.UICommandResource.Enable3DText,
          Properties.UICommandResource.Enable3DGestures,
          Properties.UICommandResource.Enable3DGesturesDisplayText,
          Properties.UICommandResource.Enable3DTooltip,
          Properties.UICommandResource.Enable3DDescription,
          typeof(UIGeneralCommands));

        public static GeneralCommand Enable3D
        {
            get { return _Enable3D; }
        }

        private static GeneralCommand _EnCacheMode = new GeneralCommand(
          Properties.UICommandResource.EnCacheModeName,
          Properties.UICommandResource.EnCacheModeText,
          Properties.UICommandResource.EnCacheModeGestures,
          Properties.UICommandResource.EnCacheModeGesturesDisplayText,
          Properties.UICommandResource.EnCacheModeTooltip,
          Properties.UICommandResource.EnCacheModeDescription,
          typeof(UIGeneralCommands));

        public static GeneralCommand EnCacheMode
        {
            get { return _EnCacheMode; }
        }

        private static GeneralCommand _ResetTransormOriginSettings = new GeneralCommand(
          Properties.UICommandResource.ResetTransormOriginSettingsName,
          Properties.UICommandResource.ResetTransormOriginSettingsText,
          Properties.UICommandResource.ResetTransormOriginSettingsGestures,
          Properties.UICommandResource.ResetTransormOriginSettingsGesturesDisplayText,
          Properties.UICommandResource.ResetTransormOriginSettingsTooltip,
          Properties.UICommandResource.ResetTransormOriginSettingsDescription,
          typeof(UIGeneralCommands));

        public static GeneralCommand ResetTransormOriginSettings
        {
            get { return _ResetTransormOriginSettings; }
        }


        private static GeneralCommand _ShowHideRotationThumb = new GeneralCommand(
           Properties.UICommandResource.ShowHideRotationThumbName,
           Properties.UICommandResource.ShowHideRotationThumbText,
           Properties.UICommandResource.ShowHideRotationThumbGestures,
           Properties.UICommandResource.ShowHideRotationThumbGesturesDisplayText,
           Properties.UICommandResource.ShowHideRotationThumbTooltip,
           Properties.UICommandResource.ShowHideRotationThumbDescription,
           typeof(UIGeneralCommands));

        public static GeneralCommand ShowHideRotationThumb
        {
            get { return _ShowHideRotationThumb; }
        }

        private static GeneralCommand _LockPosition = new GeneralCommand(
           Properties.UICommandResource.LockPositionName,
           Properties.UICommandResource.LockPositionText,
           Properties.UICommandResource.LockPositionGestures,
           Properties.UICommandResource.LockPositionGesturesDisplayText,
           Properties.UICommandResource.LockPositionTooltip,
           Properties.UICommandResource.LockPositionDescription,
           typeof(UIGeneralCommands));

        public static GeneralCommand LockPosition
        {
            get { return _LockPosition; }
        }
        private static GeneralCommand _CreateGroup = new GeneralCommand(
            Properties.UICommandResource.CreateGroupName,
            Properties.UICommandResource.CreateGroupText,
            Properties.UICommandResource.CreateGroupGestures,
            Properties.UICommandResource.CreateGroupGesturesDisplayText,
            Properties.UICommandResource.CreateGroupTooltip,
            Properties.UICommandResource.CreateGroupDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateGroup
        {
            get { return _CreateGroup; }
        }

        private static GeneralCommand _Regroup = new GeneralCommand(
            Properties.UICommandResource.RegroupName,
            Properties.UICommandResource.RegroupText,
            Properties.UICommandResource.RegroupGestures,
            Properties.UICommandResource.RegroupGesturesDisplayText,
            Properties.UICommandResource.RegroupTooltip,
            Properties.UICommandResource.RegroupDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand Regroup
        {
            get { return _Regroup; }
        }

        private static GeneralCommand _CreateVerticalStackPanel = new GeneralCommand(
            Properties.UICommandResource.CreateVerticalStackPanelName,
            Properties.UICommandResource.CreateVerticalStackPanelText,
            Properties.UICommandResource.CreateVerticalStackPanelGestures,
            Properties.UICommandResource.CreateVerticalStackPanelGesturesDisplayText,
            Properties.UICommandResource.CreateVerticalStackPanelTooltip,
            Properties.UICommandResource.CreateVerticalStackPanelDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateVerticalStackPanel
        {
            get { return _CreateVerticalStackPanel; }
        }

        private static GeneralCommand _CreateHorizontalStackPanel = new GeneralCommand(
            Properties.UICommandResource.CreateHorizontalStackPanelName,
            Properties.UICommandResource.CreateHorizontalStackPanelText,
            Properties.UICommandResource.CreateHorizontalStackPanelGestures,
            Properties.UICommandResource.CreateHorizontalStackPanelGesturesDisplayText,
            Properties.UICommandResource.CreateHorizontalStackPanelTooltip,
            Properties.UICommandResource.CreateHorizontalStackPanelDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateHorizontalStackPanel
        {
            get { return _CreateHorizontalStackPanel; }
        }


        private static GeneralCommand _CreateVerticalWrapPanel = new GeneralCommand(
            Properties.UICommandResource.CreateVerticalWrapPanelName,
            Properties.UICommandResource.CreateVerticalWrapPanelText,
            Properties.UICommandResource.CreateVerticalWrapPanelGestures,
            Properties.UICommandResource.CreateVerticalWrapPanelGesturesDisplayText,
            Properties.UICommandResource.CreateVerticalWrapPanelTooltip,
            Properties.UICommandResource.CreateVerticalWrapPanelDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateVerticalWrapPanel
        {
            get { return _CreateVerticalWrapPanel; }
        }

        private static GeneralCommand _CreateHorizontalWrapPanel = new GeneralCommand(
            Properties.UICommandResource.CreateHorizontalWrapPanelName,
            Properties.UICommandResource.CreateHorizontalWrapPanelText,
            Properties.UICommandResource.CreateHorizontalWrapPanelGestures,
            Properties.UICommandResource.CreateHorizontalWrapPanelGesturesDisplayText,
            Properties.UICommandResource.CreateHorizontalWrapPanelTooltip,
            Properties.UICommandResource.CreateHorizontalWrapPanelDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateHorizontalWrapPanel
        {
            get { return _CreateHorizontalWrapPanel; }
        }


        private static GeneralCommand _CreateDockPanel = new GeneralCommand(
            Properties.UICommandResource.CreateDockPanelName,
            Properties.UICommandResource.CreateDockPanelText,
            Properties.UICommandResource.CreateDockPanelGestures,
            Properties.UICommandResource.CreateDockPanelGesturesDisplayText,
            Properties.UICommandResource.CreateDockPanelTooltip,
            Properties.UICommandResource.CreateDockPanelDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateDockPanel
        {
            get { return _CreateDockPanel; }
        }


        private static GeneralCommand _CreateGridPanel = new GeneralCommand(
            Properties.UICommandResource.CreateGridPanelName,
            Properties.UICommandResource.CreateGridPanelText,
            Properties.UICommandResource.CreateGridPanelGestures,
            Properties.UICommandResource.CreateGridPanelGesturesDisplayText,
            Properties.UICommandResource.CreateGridPanelTooltip,
            Properties.UICommandResource.CreateGridPanelDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateGridPanel
        {
            get { return _CreateGridPanel; }
        }


        private static GeneralCommand _CreateExpander = new GeneralCommand(
            Properties.UICommandResource.CreateExpanderName,
            Properties.UICommandResource.CreateExpanderText,
            Properties.UICommandResource.CreateExpanderGestures,
            Properties.UICommandResource.CreateExpanderGesturesDisplayText,
            Properties.UICommandResource.CreateExpanderTooltip,
            Properties.UICommandResource.CreateExpanderDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateExpander
        {
            get { return _CreateExpander; }
        }

        private static GeneralCommand _CreateGroupBox = new GeneralCommand(
            Properties.UICommandResource.CreateGroupBoxName,
            Properties.UICommandResource.CreateGroupBoxText,
            Properties.UICommandResource.CreateGroupBoxGestures,
            Properties.UICommandResource.CreateGroupBoxGesturesDisplayText,
            Properties.UICommandResource.CreateGroupBoxTooltip,
            Properties.UICommandResource.CreateGroupBoxDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateGroupBox
        {
            get { return _CreateGroupBox; }
        }


        private static GeneralCommand _CreateBorder = new GeneralCommand(
            Properties.UICommandResource.CreateBorderName,
            Properties.UICommandResource.CreateBorderText,
            Properties.UICommandResource.CreateBorderGestures,
            Properties.UICommandResource.CreateBorderGesturesDisplayText,
            Properties.UICommandResource.CreateBorderTooltip,
            Properties.UICommandResource.CreateBorderDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateBorder
        {
            get { return _CreateBorder; }
        }


        private static GeneralCommand _GeneratePowerTemplate = new GeneralCommand(
            Properties.UICommandResource.GeneratePowerTemplateName,
            Properties.UICommandResource.GeneratePowerTemplateText,
            Properties.UICommandResource.GeneratePowerTemplateGestures,
            Properties.UICommandResource.GeneratePowerTemplateGesturesDisplayText,
            Properties.UICommandResource.GeneratePowerTemplateTooltip,
            Properties.UICommandResource.GeneratePowerTemplateDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand GeneratePowerTemplate
        {
            get { return _GeneratePowerTemplate; }
        }


        private static GeneralCommand _ResolvePowerTemplate = new GeneralCommand(
            Properties.UICommandResource.ResolvePowerTemplateName,
            Properties.UICommandResource.ResolvePowerTemplateText,
            Properties.UICommandResource.ResolvePowerTemplateGestures,
            Properties.UICommandResource.ResolvePowerTemplateGesturesDisplayText,
            Properties.UICommandResource.ResolvePowerTemplateTooltip,
            Properties.UICommandResource.ResolvePowerTemplateDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ResolvePowerTemplate
        {
            get { return _ResolvePowerTemplate; }
        }







        private static GeneralCommand _ShadowEffect = new GeneralCommand(
            Properties.UICommandResource.ShadowEffectName,
            Properties.UICommandResource.ShadowEffectText,
            Properties.UICommandResource.ShadowEffectGestures,
            Properties.UICommandResource.ShadowEffectGesturesDisplayText,
            Properties.UICommandResource.ShadowEffectTooltip,
            Properties.UICommandResource.ShadowEffectDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ShadowEffect
        {
            get { return _ShadowEffect; }
        }

        private static GeneralCommand _OuterGlowEffect = new GeneralCommand(
            Properties.UICommandResource.OuterGlowEffectName,
            Properties.UICommandResource.OuterGlowEffectText,
            Properties.UICommandResource.OuterGlowEffectGestures,
            Properties.UICommandResource.OuterGlowEffectGesturesDisplayText,
            Properties.UICommandResource.OuterGlowEffectTooltip,
            Properties.UICommandResource.OuterGlowEffectDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand OuterGlowEffect
        {
            get { return _OuterGlowEffect; }
        }

        private static GeneralCommand _BlurEffect = new GeneralCommand(
            Properties.UICommandResource.BlurEffectName,
            Properties.UICommandResource.BlurEffectText,
            Properties.UICommandResource.BlurEffectGestures,
            Properties.UICommandResource.BlurEffectGesturesDisplayText,
            Properties.UICommandResource.BlurEffectTooltip,
            Properties.UICommandResource.BlurEffectDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand BlurEffect
        {
            get { return _BlurEffect; }
        }

        private static GeneralCommand _ReflectionEffect = new GeneralCommand(
            Properties.UICommandResource.ReflectionEffectName,
            Properties.UICommandResource.ReflectionEffectText,
            Properties.UICommandResource.ReflectionEffectGestures,
            Properties.UICommandResource.ReflectionEffectGesturesDisplayText,
            Properties.UICommandResource.ReflectionEffectTooltip,
            Properties.UICommandResource.ReflectionEffectDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ReflectionEffect
        {
            get { return _ReflectionEffect; }
        }


        private static GeneralCommand _UnGroupItem = new GeneralCommand(
            Properties.UICommandResource.UnGroupItemName,
            Properties.UICommandResource.UnGroupItemText,
            Properties.UICommandResource.UnGroupItemGestures,
            Properties.UICommandResource.UnGroupItemGesturesDisplayText,
            Properties.UICommandResource.UnGroupItemTooltip,
            Properties.UICommandResource.UnGroupItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand UnGroupItem
        {
            get { return _UnGroupItem; }
        }


        private static GeneralCommand _RunTest = new GeneralCommand(
            Properties.UICommandResource.RunTestItemName,
            Properties.UICommandResource.RunTestItemText,
            Properties.UICommandResource.RunTestItemGestures,
            Properties.UICommandResource.RunTestItemGesturesDisplayText,
            Properties.UICommandResource.RunTestItemTooltip,
            Properties.UICommandResource.RunTestItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand RunTest
        {
            get { return _RunTest; }
        }


        private static GeneralCommand _ChangeEditingMode = new GeneralCommand(
            Properties.UICommandResource.ChangeEditingModeItemName,
            Properties.UICommandResource.ChangeEditingModeItemText,
            Properties.UICommandResource.ChangeEditingModeItemGestures,
            Properties.UICommandResource.ChangeEditingModeItemGesturesDisplayText,
            Properties.UICommandResource.ChangeEditingModeItemTooltip,
            Properties.UICommandResource.ChangeEditingModeItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ChangeEditingMode
        {
            get { return _ChangeEditingMode; }
        }


        private static GeneralCommand _MoveFirst = new GeneralCommand(
            Properties.UICommandResource.MoveFirstItemName,
            Properties.UICommandResource.MoveFirstItemText,
            Properties.UICommandResource.MoveFirstItemGestures,
            Properties.UICommandResource.MoveFirstItemGesturesDisplayText,
            Properties.UICommandResource.MoveFirstItemTooltip,
            Properties.UICommandResource.MoveFirstItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveFirst
        {
            get { return _MoveFirst; }
        }

        private static GeneralCommand _MoveLast = new GeneralCommand(
            Properties.UICommandResource.MoveLastItemName,
            Properties.UICommandResource.MoveLastItemText,
            Properties.UICommandResource.MoveLastItemGestures,
            Properties.UICommandResource.MoveLastItemGesturesDisplayText,
            Properties.UICommandResource.MoveLastItemTooltip,
            Properties.UICommandResource.MoveLastItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveLast
        {
            get { return _MoveLast; }
        }

        private static GeneralCommand _MovePrev = new GeneralCommand(
            Properties.UICommandResource.MovePrevItemName,
            Properties.UICommandResource.MovePrevItemText,
            Properties.UICommandResource.MovePrevItemGestures,
            Properties.UICommandResource.MovePrevItemGesturesDisplayText,
            Properties.UICommandResource.MovePrevItemTooltip,
            Properties.UICommandResource.MovePrevItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MovePrev
        {
            get { return _MovePrev; }
        }

        private static GeneralCommand _MoveNext = new GeneralCommand(
            Properties.UICommandResource.MoveNextItemName,
            Properties.UICommandResource.MoveNextItemText,
            Properties.UICommandResource.MoveNextItemGestures,
            Properties.UICommandResource.MoveNextItemGesturesDisplayText,
            Properties.UICommandResource.MoveNextItemTooltip,
            Properties.UICommandResource.MoveNextItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveNext
        {
            get { return _MoveNext; }
        }

        private static GeneralCommand _AlignLeft = new GeneralCommand(
            Properties.UICommandResource.AlignLeftItemName,
            Properties.UICommandResource.AlignLeftItemText,
            Properties.UICommandResource.AlignLeftItemGestures,
            Properties.UICommandResource.AlignLeftItemGesturesDisplayText,
            Properties.UICommandResource.AlignLeftItemTooltip,
            Properties.UICommandResource.AlignLeftItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AlignLeft
        {
            get { return _AlignLeft; }
        }

        private static GeneralCommand _AlignCenterHorizontal = new GeneralCommand(
            Properties.UICommandResource.AlignCenterHorizontalItemName,
            Properties.UICommandResource.AlignCenterHorizontalItemText,
            Properties.UICommandResource.AlignCenterHorizontalItemGestures,
            Properties.UICommandResource.AlignCenterHorizontalItemGesturesDisplayText,
            Properties.UICommandResource.AlignCenterHorizontalItemTooltip,
            Properties.UICommandResource.AlignCenterHorizontalItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AlignCenterHorizontal
        {
            get { return _AlignCenterHorizontal; }
        }

        private static GeneralCommand _AlignCenterVertical = new GeneralCommand(
            Properties.UICommandResource.AlignCenterVerticalItemName,
            Properties.UICommandResource.AlignCenterVerticalItemText,
            Properties.UICommandResource.AlignCenterVerticalItemGestures,
            Properties.UICommandResource.AlignCenterVerticalItemGesturesDisplayText,
            Properties.UICommandResource.AlignCenterVerticalItemTooltip,
            Properties.UICommandResource.AlignCenterVerticalItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AlignCenterVertical
        {
            get { return _AlignCenterVertical; }
        }

        private static GeneralCommand _AlignTop = new GeneralCommand(
            Properties.UICommandResource.AlignTopItemName,
            Properties.UICommandResource.AlignTopItemText,
            Properties.UICommandResource.AlignTopItemGestures,
            Properties.UICommandResource.AlignTopItemGesturesDisplayText,
            Properties.UICommandResource.AlignTopItemTooltip,
            Properties.UICommandResource.AlignTopItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AlignTop
        {
            get { return _AlignTop; }
        }

        private static GeneralCommand _AlignRight = new GeneralCommand(
            Properties.UICommandResource.AlignRightItemName,
            Properties.UICommandResource.AlignRightItemText,
            Properties.UICommandResource.AlignRightItemGestures,
            Properties.UICommandResource.AlignRightItemGesturesDisplayText,
            Properties.UICommandResource.AlignRightItemTooltip,
            Properties.UICommandResource.AlignRightItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AlignRight
        {
            get { return _AlignRight; }
        }

        private static GeneralCommand _AlignBottom = new GeneralCommand(
            Properties.UICommandResource.AlignBottomItemName,
            Properties.UICommandResource.AlignBottomItemText,
            Properties.UICommandResource.AlignBottomItemGestures,
            Properties.UICommandResource.AlignBottomItemGesturesDisplayText,
            Properties.UICommandResource.AlignBottomItemTooltip,
            Properties.UICommandResource.AlignBottomItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AlignBottom
        {
            get { return _AlignBottom; }
        }

        private static GeneralCommand _SetSameWidth = new GeneralCommand(
            Properties.UICommandResource.SetSameWidthItemName,
            Properties.UICommandResource.SetSameWidthItemText,
            Properties.UICommandResource.SetSameWidthItemGestures,
            Properties.UICommandResource.SetSameWidthItemGesturesDisplayText,
            Properties.UICommandResource.SetSameWidthItemTooltip,
            Properties.UICommandResource.SetSameWidthItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SetSameWidth
        {
            get { return _SetSameWidth; }
        }

        private static GeneralCommand _SetSameHeight = new GeneralCommand(
            Properties.UICommandResource.SetSameHeightItemName,
            Properties.UICommandResource.SetSameHeightItemText,
            Properties.UICommandResource.SetSameHeightItemGestures,
            Properties.UICommandResource.SetSameHeightItemGesturesDisplayText,
            Properties.UICommandResource.SetSameHeightItemTooltip,
            Properties.UICommandResource.SetSameHeightItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SetSameHeight
        {
            get { return _SetSameHeight; }
        }

        private static GeneralCommand _SetSameBoth = new GeneralCommand(
            Properties.UICommandResource.SetSameBothItemName,
            Properties.UICommandResource.SetSameBothItemText,
            Properties.UICommandResource.SetSameBothItemGestures,
            Properties.UICommandResource.SetSameBothItemGesturesDisplayText,
            Properties.UICommandResource.SetSameBothItemTooltip,
            Properties.UICommandResource.SetSameBothItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SetSameBoth
        {
            get { return _SetSameBoth; }
        }

        private static GeneralCommand _DistributeSpace = new GeneralCommand(
            Properties.UICommandResource.DistributeSpaceItemName,
            Properties.UICommandResource.DistributeSpaceItemText,
            Properties.UICommandResource.DistributeSpaceItemGestures,
            Properties.UICommandResource.DistributeSpaceItemGesturesDisplayText,
            Properties.UICommandResource.DistributeSpaceItemTooltip,
            Properties.UICommandResource.DistributeSpaceItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand DistributeSpace
        {
            get { return _DistributeSpace; }
        }

        private static GeneralCommand _PropertyMapper = new GeneralCommand(
            Properties.UICommandResource.PropertyMapperName,
            Properties.UICommandResource.PropertyMapperText,
            Properties.UICommandResource.PropertyMapperGestures,
            Properties.UICommandResource.PropertyMapperGesturesDisplayText,
            Properties.UICommandResource.PropertyMapperTooltip,
            Properties.UICommandResource.PropertyMapperDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand PropertyMapper
        {
            get { return _PropertyMapper; }
        }

        private static GeneralCommand _DynamicPropertyMapper = new GeneralCommand(
            Properties.UICommandResource.DynamicPropertyMapperName,
            Properties.UICommandResource.DynamicPropertyMapperText,
            Properties.UICommandResource.DynamicPropertyMapperGestures,
            Properties.UICommandResource.DynamicPropertyMapperGesturesDisplayText,
            Properties.UICommandResource.DynamicPropertyMapperTooltip,
            Properties.UICommandResource.DynamicPropertyMapperDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand DynamicPropertyMapper
        {
            get { return _DynamicPropertyMapper; }
        }

        private static GeneralCommand _AddSymbolLibrary = new GeneralCommand(
            Properties.UICommandResource.AddSymbolLibraryItemName,
            Properties.UICommandResource.AddSymbolLibraryItemText,
            Properties.UICommandResource.AddSymbolLibraryItemGestures,
            Properties.UICommandResource.AddSymbolLibraryItemGesturesDisplayText,
            Properties.UICommandResource.AddSymbolLibraryItemTooltip,
            Properties.UICommandResource.AddSymbolLibraryItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddSymbolLibrary
        {
            get { return _AddSymbolLibrary; }
        }

        private static GeneralCommand _UpdateSymbolLibrary = new GeneralCommand(
            Properties.UICommandResource.UpdateSymbolLibraryItemName,
            Properties.UICommandResource.UpdateSymbolLibraryItemText,
            Properties.UICommandResource.UpdateSymbolLibraryItemGestures,
            Properties.UICommandResource.UpdateSymbolLibraryItemGesturesDisplayText,
            Properties.UICommandResource.UpdateSymbolLibraryItemTooltip,
            Properties.UICommandResource.UpdateSymbolLibraryItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand UpdateSymbolLibrary
        {
            get { return _UpdateSymbolLibrary; }
        }

        private static GeneralCommand _Escape = new GeneralCommand(
            Properties.UICommandResource.EscapeItemName,
            Properties.UICommandResource.EscapeItemText,
            Properties.UICommandResource.EscapeItemGestures,
            Properties.UICommandResource.EscapeItemGesturesDisplayText,
            Properties.UICommandResource.EscapeItemTooltip,
            Properties.UICommandResource.EscapeItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand Escape
        {
            get { return _Escape; }
        }


        private static GeneralCommand _ToggleGrid = new GeneralCommand(
            Properties.UICommandResource.ToggleGridName,
            Properties.UICommandResource.ToggleGridText,
            Properties.UICommandResource.ToggleGridGestures,
            Properties.UICommandResource.ToggleGridGesturesDisplayText,
            Properties.UICommandResource.ToggleGridTooltip,
            Properties.UICommandResource.ToggleGridDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ToggleGrid
        {
            get { return _ToggleGrid; }
        }

        private static GeneralCommand _SetGrid = new GeneralCommand(
            Properties.UICommandResource.SetGridName,
            Properties.UICommandResource.SetGridText,
            Properties.UICommandResource.SetGridGestures,
            Properties.UICommandResource.SetGridGesturesDisplayText,
            Properties.UICommandResource.SetGridTooltip,
            Properties.UICommandResource.SetGridDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SetGrid
        {
            get { return _SetGrid; }
        }

        private static GeneralCommand _SnapToGrid = new GeneralCommand(
            Properties.UICommandResource.SnapToGridName,
            Properties.UICommandResource.SnapToGridText,
            Properties.UICommandResource.SnapToGridGestures,
            Properties.UICommandResource.SnapToGridGesturesDisplayText,
            Properties.UICommandResource.SnapToGridTooltip,
            Properties.UICommandResource.SnapToGridDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SnapToGrid
        {
            get { return _SnapToGrid; }
        }


        private static GeneralCommand _SmartSnap = new GeneralCommand(
            Properties.UICommandResource.SmartSnapName,
            Properties.UICommandResource.SmartSnapText,
            Properties.UICommandResource.SmartSnapGestures,
            Properties.UICommandResource.SmartSnapGesturesDisplayText,
            Properties.UICommandResource.SmartSnapTooltip,
            Properties.UICommandResource.SmartSnapDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SmartSnap
        {
            get { return _SmartSnap; }
        }



        private static GeneralCommand _StrokeThickness = new GeneralCommand(
            Properties.UICommandResource.StrokeThicknessName,
            Properties.UICommandResource.StrokeThicknessText,
            Properties.UICommandResource.StrokeThicknessGestures,
            Properties.UICommandResource.StrokeThicknessGesturesDisplayText,
            Properties.UICommandResource.StrokeThicknessTooltip,
            Properties.UICommandResource.StrokeThicknessDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StrokeThickness
        {
            get { return _StrokeThickness; }
        }

        private static GeneralCommand _StrokeDashArray = new GeneralCommand(
            Properties.UICommandResource.StrokeDashArrayName,
            Properties.UICommandResource.StrokeDashArrayText,
            Properties.UICommandResource.StrokeDashArrayGestures,
            Properties.UICommandResource.StrokeDashArrayGesturesDisplayText,
            Properties.UICommandResource.StrokeDashArrayTooltip,
            Properties.UICommandResource.StrokeDashArrayDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StrokeDashArray
        {
            get { return _StrokeDashArray; }
        }



        private static GeneralCommand _BrushEditor = new GeneralCommand(
            Properties.UICommandResource.BrushEditorName,
            Properties.UICommandResource.BrushEditorText,
            Properties.UICommandResource.BrushEditorGestures,
            Properties.UICommandResource.BrushEditorGesturesDisplayText,
            Properties.UICommandResource.BrushEditorTooltip,
            Properties.UICommandResource.BrushEditorDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand BrushEditor
        {
            get { return _BrushEditor; }
        }


        private static GeneralCommand _PenEditor = new GeneralCommand(
            Properties.UICommandResource.PenEditorName,
            Properties.UICommandResource.PenEditorText,
            Properties.UICommandResource.PenEditorGestures,
            Properties.UICommandResource.PenEditorGesturesDisplayText,
            Properties.UICommandResource.PenEditorTooltip,
            Properties.UICommandResource.PenEditorDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand PenEditor
        {
            get { return _PenEditor; }
        }

        private static GeneralCommand _LineEditor = new GeneralCommand(
            Properties.UICommandResource.LineEditorName,
            Properties.UICommandResource.LineEditorText,
            Properties.UICommandResource.LineEditorGestures,
            Properties.UICommandResource.LineEditorGesturesDisplayText,
            Properties.UICommandResource.LineEditorTooltip,
            Properties.UICommandResource.LineEditorDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand LineEditor
        {
            get { return _LineEditor; }
        }


        private static GeneralCommand _StyleEditor = new GeneralCommand(
            Properties.UICommandResource.StyleEditorName,
            Properties.UICommandResource.StyleEditorText,
            Properties.UICommandResource.StyleEditorGestures,
            Properties.UICommandResource.StyleEditorGesturesDisplayText,
            Properties.UICommandResource.StyleEditorTooltip,
            Properties.UICommandResource.StyleEditorDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand StyleEditor
        {
            get { return _StyleEditor; }
        }


        private static GeneralCommand _Rotate = new GeneralCommand(
            Properties.UICommandResource.RotateName,
            Properties.UICommandResource.RotateText,
            Properties.UICommandResource.RotateGestures,
            Properties.UICommandResource.RotateGesturesDisplayText,
            Properties.UICommandResource.RotateTooltip,
            Properties.UICommandResource.RotateDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand Rotate
        {
            get { return _Rotate; }
        }


        private static GeneralCommand _CreateScreenTemplate = new GeneralCommand(
            Properties.UICommandResource.CreateScreenTemplateName,
            Properties.UICommandResource.CreateScreenTemplateText,
            Properties.UICommandResource.CreateScreenTemplateGestures,
            Properties.UICommandResource.CreateScreenTemplateGesturesDisplayText,
            Properties.UICommandResource.CreateScreenTemplateTooltip,
            Properties.UICommandResource.CreateScreenTemplateDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CreateScreenTemplate
        {
            get { return _CreateScreenTemplate; }
        }


        private static GeneralCommand _ShowInlineProperties = new GeneralCommand(
            Properties.UICommandResource.CommonPropertyEditorName,
            Properties.UICommandResource.CommonPropertyEditorText,
            Properties.UICommandResource.CommonPropertyEditorGestures,
            Properties.UICommandResource.CommonPropertyEditorGesturesDisplayText,
            Properties.UICommandResource.CommonPropertyEditorTooltip,
            Properties.UICommandResource.CommonPropertyEditorDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ShowInlineProperties
        {
            get { return _ShowInlineProperties; }
        }

        private static GeneralCommand _ShowInlineCommands = new GeneralCommand(
           Properties.UICommandResource.ShowInlineCommandsName,
           Properties.UICommandResource.ShowInlineCommandsText,
           Properties.UICommandResource.ShowInlineCommandsGestures,
           Properties.UICommandResource.ShowInlineCommandsGesturesDisplayText,
           Properties.UICommandResource.ShowInlineCommandsTooltip,
           Properties.UICommandResource.ShowInlineCommandsDescription,
           typeof(UIGeneralCommands));

        public static GeneralCommand ShowInlineCommands
        {
            get { return _ShowInlineCommands; }
        }

        private static GeneralCommand _ShowInlineAnimations = new GeneralCommand(
           Properties.UICommandResource.ShowInlineAnimationsName,
           Properties.UICommandResource.ShowInlineAnimationsText,
           Properties.UICommandResource.ShowInlineAnimationsGestures,
           Properties.UICommandResource.ShowInlineAnimationsGesturesDisplayText,
           Properties.UICommandResource.ShowInlineAnimationsTooltip,
           Properties.UICommandResource.ShowInlineAnimationsDescription,
           typeof(UIGeneralCommands));

        public static GeneralCommand ShowInlineAnimations
        {
            get { return _ShowInlineAnimations; }
        }

        private static GeneralCommand _EditDataContextItem = new GeneralCommand(
            Properties.UICommandResource.EditDataContextItemName,
            Properties.UICommandResource.EditDataContextItemText,
            Properties.UICommandResource.EditDataContextItemGestures,
            Properties.UICommandResource.EditDataContextItemGesturesDisplayText,
            Properties.UICommandResource.EditDataContextItemTooltip,
            Properties.UICommandResource.EditDataContextItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EditDataContextItem
        {
            get { return _EditDataContextItem; }
        }

        private static GeneralCommand _EditDataTypeItem = new GeneralCommand(
            Properties.UICommandResource.EditDataTypeItemName,
            Properties.UICommandResource.EditDataTypeItemText,
            Properties.UICommandResource.EditDataTypeItemGestures,
            Properties.UICommandResource.EditDataTypeItemGesturesDisplayText,
            Properties.UICommandResource.EditDataTypeItemTooltip,
            Properties.UICommandResource.EditDataTypeItemDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EditDataTypeItem
        {
            get { return _EditDataTypeItem; }
        }

        private static GeneralCommand _InsertObjectType = new GeneralCommand();

        public static GeneralCommand InsertObjectType
        {
            get { return _InsertObjectType; }
        }

        private static GeneralCommand _EnableUndoRedo = new GeneralCommand();

        public static GeneralCommand EnableUndoRedo
        {
            get { return _EnableUndoRedo; }
        }

        private static GeneralCommand _ZoomControl = new GeneralCommand(
            Properties.UICommandResource.MaximizeControlName,
            Properties.UICommandResource.MaximizeControlText,
            Properties.UICommandResource.MaximizeControlGestures,
            Properties.UICommandResource.MaximizeControlGesturesDisplayText,
            Properties.UICommandResource.MaximizeControlTooltip,
            Properties.UICommandResource.MaximizeControlDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ZoomControl
        {
            get { return _ZoomControl; }
        }

        private static GeneralCommand _IconControl = new GeneralCommand(
            Properties.UICommandResource.MinimizeControlName,
            Properties.UICommandResource.MinimizeControlText,
            Properties.UICommandResource.MinimizeControlGestures,
            Properties.UICommandResource.MinimizeControlGesturesDisplayText,
            Properties.UICommandResource.MinimizeControlTooltip,
            Properties.UICommandResource.MinimizeControlDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand IconControl
        {
            get { return _IconControl; }
        }

        private static GeneralCommand _EnableManipulationControl = new GeneralCommand(
            Properties.UICommandResource.EnableManipulationControlName,
            Properties.UICommandResource.EnableManipulationControlText,
            Properties.UICommandResource.EnableManipulationControlGestures,
            Properties.UICommandResource.EnableManipulationControlGesturesDisplayText,
            Properties.UICommandResource.EnableManipulationControlTooltip,
            Properties.UICommandResource.EnableManipulationControlDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EnableManipulationControl
        {
            get { return _EnableManipulationControl; }
        }

        private static GeneralCommand _ResetManipulationControl = new GeneralCommand(
            Properties.UICommandResource.ResetManipulationControlName,
            Properties.UICommandResource.ResetManipulationControlText,
            Properties.UICommandResource.ResetManipulationControlGestures,
            Properties.UICommandResource.ResetManipulationControlGesturesDisplayText,
            Properties.UICommandResource.ResetManipulationControlTooltip,
            Properties.UICommandResource.ResetManipulationControlDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ResetManipulationControl
        {
            get { return _ResetManipulationControl; }
        }



        private static GeneralCommand _CopyReference = new GeneralCommand(
            Properties.UICommandResource.CopyReferenceName,
            Properties.UICommandResource.CopyReferenceText,
            Properties.UICommandResource.CopyReferenceGestures,
            Properties.UICommandResource.CopyReferenceGesturesDisplayText,
            Properties.UICommandResource.CopyReferenceTooltip,
            Properties.UICommandResource.CopyReferenceDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CopyReference
        {
            get { return _CopyReference; }
        }

        private static GeneralCommand _CopyCommands = new GeneralCommand(
            Properties.UICommandResource.CopyCommandsName,
            Properties.UICommandResource.CopyCommandsText,
            Properties.UICommandResource.CopyCommandsGestures,
            Properties.UICommandResource.CopyCommandsGesturesDisplayText,
            Properties.UICommandResource.CopyCommandsTooltip,
            Properties.UICommandResource.CopyCommandsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CopyCommands
        {
            get { return _CopyCommands; }
        }

        private static GeneralCommand _CopyAnimations = new GeneralCommand(
            Properties.UICommandResource.CopyAnimationsName,
            Properties.UICommandResource.CopyAnimationsText,
            Properties.UICommandResource.CopyAnimationsGestures,
            Properties.UICommandResource.CopyAnimationsGesturesDisplayText,
            Properties.UICommandResource.CopyAnimationsTooltip,
            Properties.UICommandResource.CopyAnimationsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CopyAnimations
        {
            get { return _CopyAnimations; }
        }

        private static GeneralCommand _CopyFontSettingList = new GeneralCommand(
            Properties.UICommandResource.CopyFontSettingListName,
            Properties.UICommandResource.CopyFontSettingListText,
            Properties.UICommandResource.CopyFontSettingListGestures,
            Properties.UICommandResource.CopyFontSettingListGesturesDisplayText,
            Properties.UICommandResource.CopyFontSettingListTooltip,
            Properties.UICommandResource.CopyFontSettingListDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand CopyFontSettingList
        {
            get { return _CopyFontSettingList; }
        }

        private static GeneralCommand _PasteFontSettingList = new GeneralCommand(
            Properties.UICommandResource.PasteFontSettingListName,
            Properties.UICommandResource.PasteFontSettingListText,
            Properties.UICommandResource.PasteFontSettingListGestures,
            Properties.UICommandResource.PasteFontSettingListGesturesDisplayText,
            Properties.UICommandResource.PasteFontSettingListTooltip,
            Properties.UICommandResource.PasteFontSettingListDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand PasteFontSettingList
        {
            get { return _PasteFontSettingList; }
        }

        private static GeneralCommand _PasteReference = new GeneralCommand(
            Properties.UICommandResource.PasteReferenceName,
            Properties.UICommandResource.PasteReferenceText,
            Properties.UICommandResource.PasteReferenceGestures,
            Properties.UICommandResource.PasteReferenceGesturesDisplayText,
            Properties.UICommandResource.PasteReferenceTooltip,
            Properties.UICommandResource.PasteReferenceDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand PasteReference
        {
            get { return _PasteReference; }
        }

        private static GeneralCommand _PasteCommands = new GeneralCommand(
            Properties.UICommandResource.PasteCommandsName,
            Properties.UICommandResource.PasteCommandsText,
            Properties.UICommandResource.PasteCommandsGestures,
            Properties.UICommandResource.PasteCommandsGesturesDisplayText,
            Properties.UICommandResource.PasteCommandsTooltip,
            Properties.UICommandResource.PasteCommandsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand PasteCommands
        {
            get { return _PasteCommands; }
        }

        private static GeneralCommand _PasteAnimations = new GeneralCommand(
            Properties.UICommandResource.PasteAnimationsName,
            Properties.UICommandResource.PasteAnimationsText,
            Properties.UICommandResource.PasteAnimationsGestures,
            Properties.UICommandResource.PasteAnimationsGesturesDisplayText,
            Properties.UICommandResource.PasteAnimationsTooltip,
            Properties.UICommandResource.PasteAnimationsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand PasteAnimations
        {
            get { return _PasteAnimations; }
        }

        private static GeneralCommand _ExecuteDroppingCode = new GeneralCommand(
            Properties.UICommandResource.ExecuteDroppingCodeName,
            Properties.UICommandResource.ExecuteDroppingCodeText,
            Properties.UICommandResource.ExecuteDroppingCodeGestures,
            Properties.UICommandResource.ExecuteDroppingCodeGesturesDisplayText,
            Properties.UICommandResource.ExecuteDroppingCodeTooltip,
            Properties.UICommandResource.ExecuteDroppingCodeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ExecuteDroppingCode
        {
            get { return _ExecuteDroppingCode; }
        }

        private static GeneralCommand _RefreshRepositoryItems = new GeneralCommand(
            Properties.UICommandResource.RefreshRepositoryItemsName,
            Properties.UICommandResource.RefreshRepositoryItemsText,
            Properties.UICommandResource.RefreshRepositoryItemsGestures,
            Properties.UICommandResource.RefreshRepositoryItemsGesturesDisplayText,
            Properties.UICommandResource.RefreshRepositoryItemsTooltip,
            Properties.UICommandResource.RefreshRepositoryItemsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand RefreshRepositoryItems
        {
            get { return _RefreshRepositoryItems; }
        }

        private static GeneralCommand _MoveUp = new GeneralCommand(
            Properties.UICommandResource.MoveUpName,
            Properties.UICommandResource.MoveUpText,
            Properties.UICommandResource.MoveUpGestures,
            Properties.UICommandResource.MoveUpGesturesDisplayText,
            Properties.UICommandResource.MoveUpTooltip,
            Properties.UICommandResource.MoveUpDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveUp
        {
            get { return _MoveUp; }
        }

        private static GeneralCommand _MoveDown = new GeneralCommand(
            Properties.UICommandResource.MoveDownName,
            Properties.UICommandResource.MoveDownText,
            Properties.UICommandResource.MoveDownGestures,
            Properties.UICommandResource.MoveDownGesturesDisplayText,
            Properties.UICommandResource.MoveDownTooltip,
            Properties.UICommandResource.MoveDownDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveDown
        {
            get { return _MoveDown; }
        }

        private static GeneralCommand _MoveLeft = new GeneralCommand(
            Properties.UICommandResource.MoveLeftName,
            Properties.UICommandResource.MoveLeftText,
            Properties.UICommandResource.MoveLeftGestures,
            Properties.UICommandResource.MoveLeftGesturesDisplayText,
            Properties.UICommandResource.MoveLeftTooltip,
            Properties.UICommandResource.MoveLeftDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveLeft
        {
            get { return _MoveLeft; }
        }

        private static GeneralCommand _MoveRight = new GeneralCommand(
            Properties.UICommandResource.MoveRightName,
            Properties.UICommandResource.MoveRightText,
            Properties.UICommandResource.MoveRightGestures,
            Properties.UICommandResource.MoveRightGesturesDisplayText,
            Properties.UICommandResource.MoveRightTooltip,
            Properties.UICommandResource.MoveRightDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand MoveRight
        {
            get { return _MoveRight; }
        }

        private static GeneralCommand _LayoutEdit = new GeneralCommand(
            Properties.UICommandResource.LayoutEditName,
            Properties.UICommandResource.LayoutEditText,
            Properties.UICommandResource.LayoutEditGestures,
            Properties.UICommandResource.LayoutEditGesturesDisplayText,
            Properties.UICommandResource.LayoutEditTooltip,
            Properties.UICommandResource.LayoutEditDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand LayoutEdit
        {
            get { return _LayoutEdit; }
        }


        private static GeneralCommand _LoginUser = new GeneralCommand(
            Properties.UICommandResource.LoginUserName,
            Properties.UICommandResource.LoginUserText,
            Properties.UICommandResource.LoginUserGestures,
            Properties.UICommandResource.LoginUserGesturesDisplayText,
            Properties.UICommandResource.LoginUserTooltip,
            Properties.UICommandResource.LoginUserDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand LoginUser
        {
            get { return _LoginUser; }
        }

        private static GeneralCommand _LogoutUser = new GeneralCommand(
            Properties.UICommandResource.LogoutUserName,
            Properties.UICommandResource.LogoutUserText,
            Properties.UICommandResource.LogoutUserGestures,
            Properties.UICommandResource.LogoutUserGesturesDisplayText,
            Properties.UICommandResource.LogoutUserTooltip,
            Properties.UICommandResource.LogoutUserDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand LogoutUser
        {
            get { return _LogoutUser; }
        }

        private static GeneralCommand _ResetManipulation = new GeneralCommand(
            Properties.UICommandResource.ResetManipulationName,
            Properties.UICommandResource.ResetManipulationText,
            Properties.UICommandResource.ResetManipulationGestures,
            Properties.UICommandResource.ResetManipulationGesturesDisplayText,
            Properties.UICommandResource.ResetManipulationTooltip,
            Properties.UICommandResource.ResetManipulationDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ResetManipulation
        {
            get { return _ResetManipulation; }
        }

        private static GeneralCommand _FillMode = new GeneralCommand(
            Properties.UICommandResource.FillModeName,
            Properties.UICommandResource.FillModeText,
            Properties.UICommandResource.FillModeGestures,
            Properties.UICommandResource.FillModeGesturesDisplayText,
            Properties.UICommandResource.FillModeTooltip,
            Properties.UICommandResource.FillModeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand FillMode
        {
            get { return _FillMode; }
        }

        private static GeneralCommand _ZoomMode = new GeneralCommand(
            Properties.UICommandResource.ZoomModeName,
            Properties.UICommandResource.ZoomModeText,
            Properties.UICommandResource.ZoomModeGestures,
            Properties.UICommandResource.ZoomModeGesturesDisplayText,
            Properties.UICommandResource.ZoomModeTooltip,
            Properties.UICommandResource.ZoomModeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ZoomMode
        {
            get { return _ZoomMode; }
        }

        private static GeneralCommand _Back = new GeneralCommand(
            Properties.UICommandResource.BackName,
            Properties.UICommandResource.BackText,
            Properties.UICommandResource.BackGestures,
            Properties.UICommandResource.BackGesturesDisplayText,
            Properties.UICommandResource.BackTooltip,
            Properties.UICommandResource.BackDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand Back
        {
            get { return _Back; }
        }

        private static GeneralCommand _Home = new GeneralCommand(
            Properties.UICommandResource.HomeName,
            Properties.UICommandResource.HomeText,
            Properties.UICommandResource.HomeGestures,
            Properties.UICommandResource.HomeGesturesDisplayText,
            Properties.UICommandResource.HomeTooltip,
            Properties.UICommandResource.HomeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand Home
        {
            get { return _Home; }
        }

        private static GeneralCommand _AddStringId = new GeneralCommand(
            TranslatableMenu.Properties.Resources.AddStringIdName,
            TranslatableMenu.Properties.Resources.AddStringIdText,
            Properties.UICommandResource.AddStringIdGestures,
            Properties.UICommandResource.AddStringIdGesturesDisplayText,
            Properties.UICommandResource.AddStringIdTooltip,
            Properties.UICommandResource.AddStringIdDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddStringId
        {
            get { return _AddStringId; }
        }

        private static GeneralCommand _AddGeoLocation = new GeneralCommand(
            Properties.UICommandResource.EditGeoLocationName,
            Properties.UICommandResource.EditGeoLocationText,
            Properties.UICommandResource.EditGeoLocationGestures,
            Properties.UICommandResource.EditGeoLocationGesturesDisplayText,
            Properties.UICommandResource.EditGeoLocationTooltip,
            Properties.UICommandResource.EditGeoLocationDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddGeoLocation
        {
            get { return _AddGeoLocation; }
        }

        private static GeneralCommand _ImportXaml = new GeneralCommand(
            Properties.UICommandResource.ImportXamlName,
            Properties.UICommandResource.ImportXamlText,
            Properties.UICommandResource.ImportXamlGestures,
            Properties.UICommandResource.ImportXamlGesturesDisplayText,
            Properties.UICommandResource.ImportXamlTooltip,
            Properties.UICommandResource.ImportXamlDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ImportXaml
        {
            get { return _ImportXaml; }
        }

        private static GeneralCommand _ShowClientStatus = new GeneralCommand(
            Properties.UICommandResource.ShowClientStatusName,
            Properties.UICommandResource.ShowClientStatusText,
            Properties.UICommandResource.ShowClientStatusGestures,
            Properties.UICommandResource.ShowClientStatusGesturesDisplayText,
            Properties.UICommandResource.ShowClientStatusTooltip,
            Properties.UICommandResource.ShowClientStatusDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ShowClientStatus
        {
            get { return _ShowClientStatus; }
        }

        private static GeneralCommand _ShowLog = new GeneralCommand(
            Properties.UICommandResource.ShowLogName,
            Properties.UICommandResource.ShowLogText,
            Properties.UICommandResource.ShowLogGestures,
            Properties.UICommandResource.ShowLogGesturesDisplayText,
            Properties.UICommandResource.ShowLogTooltip,
            Properties.UICommandResource.ShowLogDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ShowLog
        {
            get { return _ShowLog; }
        }

        private static GeneralCommand _ShowCrossReference = new GeneralCommand(
            Properties.UICommandResource.ShowCrossReferenceName,
            Properties.UICommandResource.ShowCrossReferenceText,
            Properties.UICommandResource.ShowCrossReferenceGestures,
            Properties.UICommandResource.ShowCrossReferenceGesturesDisplayText,
            Properties.UICommandResource.ShowCrossReferenceTooltip,
            Properties.UICommandResource.ShowCrossReferenceDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ShowCrossReference
        {
            get { return _ShowCrossReference; }
        }

        private static GeneralCommand _ResetGadgets = new GeneralCommand(
            Properties.UICommandResource.ResetGadgetsName,
            Properties.UICommandResource.ResetGadgetsText,
            Properties.UICommandResource.ResetGadgetsGestures,
            Properties.UICommandResource.ResetGadgetsGesturesDisplayText,
            Properties.UICommandResource.ResetGadgetsTooltip,
            Properties.UICommandResource.ResetGadgetsDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ResetGadgets
        {
            get { return _ResetGadgets; }
        }

        private static GeneralCommand _SelectNext = new GeneralCommand(
            Properties.UICommandResource.SelectNextName,
            Properties.UICommandResource.SelectNextText,
            Properties.UICommandResource.SelectNextGestures,
            Properties.UICommandResource.SelectNextGesturesDisplayText,
            Properties.UICommandResource.SelectNextTooltip,
            Properties.UICommandResource.SelectNextDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SelectNext
        {
            get { return _SelectNext; }
        }

        private static GeneralCommand _SelectPrev = new GeneralCommand(
            Properties.UICommandResource.SelectPrevName,
            Properties.UICommandResource.SelectPrevText,
            Properties.UICommandResource.SelectPrevGestures,
            Properties.UICommandResource.SelectPrevGesturesDisplayText,
            Properties.UICommandResource.SelectPrevTooltip,
            Properties.UICommandResource.SelectPrevDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SelectPrev
        {
            get { return _SelectPrev; }
        }

        private static GeneralCommand _Rename = new GeneralCommand(
            Properties.UICommandResource.RenameName,
            Properties.UICommandResource.RenameText,
            Properties.UICommandResource.RenameGestures,
            Properties.UICommandResource.RenameGesturesDisplayText,
            Properties.UICommandResource.RenameTooltip,
            Properties.UICommandResource.RenameDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand Rename
        {
            get { return _Rename; }
        }

        private static GeneralCommand _SetZOrder = new GeneralCommand(
            Properties.UICommandResource.SetZOrderName,
            Properties.UICommandResource.SetZOrderText,
            Properties.UICommandResource.SetZOrderGestures,
            Properties.UICommandResource.SetZOrderGesturesDisplayText,
            Properties.UICommandResource.SetZOrderTooltip,
            Properties.UICommandResource.SetZOrderDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand SetZOrder
        {
            get { return _SetZOrder; }
        }

        private static GeneralCommand _EditExpression = new GeneralCommand(
            Properties.UICommandResource.EditExpressionName,
            Properties.UICommandResource.EditExpressionText,
            Properties.UICommandResource.EditExpressionGestures,
            Properties.UICommandResource.EditExpressionGesturesDisplayText,
            Properties.UICommandResource.EditExpressionTooltip,
            Properties.UICommandResource.EditExpressionDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EditExpression
        {
            get { return _EditExpression; }
        }

        private static GeneralCommand _ClearExpression = new GeneralCommand(
                    Properties.UICommandResource.ClearExpressionName,
                    Properties.UICommandResource.ClearExpressionText,
                    Properties.UICommandResource.ClearExpressionGestures,
                    Properties.UICommandResource.ClearExpressionGesturesDisplayText,
                    Properties.UICommandResource.ClearExpressionTooltip,
                    Properties.UICommandResource.ClearExpressionDescription,
                    typeof(UIGeneralCommands));

        public static GeneralCommand ClearExpression
        {
            get { return _ClearExpression; }
        }

        private static GeneralCommand _EditReverseExpression = new GeneralCommand(
            Properties.UICommandResource.EditReverseExpressionName,
            Properties.UICommandResource.EditReverseExpressionText,
            Properties.UICommandResource.EditReverseExpressionGestures,
            Properties.UICommandResource.EditReverseExpressionGesturesDisplayText,
            Properties.UICommandResource.EditReverseExpressionTooltip,
            Properties.UICommandResource.EditReverseExpressionDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand EditReverseExpression
        {
            get { return _EditReverseExpression; }
        }

        private static GeneralCommand _ClearReverseExpression = new GeneralCommand(
                    Properties.UICommandResource.ClearReverseExpressionName,
                    Properties.UICommandResource.ClearReverseExpressionText,
                    Properties.UICommandResource.ClearReverseExpressionGestures,
                    Properties.UICommandResource.ClearReverseExpressionGesturesDisplayText,
                    Properties.UICommandResource.ClearReverseExpressionTooltip,
                    Properties.UICommandResource.ClearReverseExpressionDescription,
                    typeof(UIGeneralCommands));

        public static GeneralCommand ClearReverseExpression
        {
            get { return _ClearReverseExpression; }
        }

        private static GeneralCommand _FlipVertical = new GeneralCommand(
            Properties.UICommandResource.FlipVerticalName,
            Properties.UICommandResource.FlipVerticalText,
            Properties.UICommandResource.FlipVerticalGestures,
            Properties.UICommandResource.FlipVerticalGesturesDisplayText,
            Properties.UICommandResource.FlipVerticalTooltip,
            Properties.UICommandResource.FlipVerticalDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand FlipVertical
        {
            get { return _FlipVertical; }
        }

        private static GeneralCommand _FlipHorizontal = new GeneralCommand(
            Properties.UICommandResource.FlipHorizontalName,
            Properties.UICommandResource.FlipHorizontalText,
            Properties.UICommandResource.FlipHorizontalGestures,
            Properties.UICommandResource.FlipHorizontalGesturesDisplayText,
            Properties.UICommandResource.FlipHorizontalTooltip,
            Properties.UICommandResource.FlipHorizontalDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand FlipHorizontal
        {
            get { return _FlipHorizontal; }
        }

        private static GeneralCommand _DynamicPropertyInspector = new GeneralCommand(
            Properties.UICommandResource.DynamicPropertyInspectorName,
            Properties.UICommandResource.DynamicPropertyInspectorText,
            Properties.UICommandResource.DynamicPropertyInspectorGestures,
            Properties.UICommandResource.DynamicPropertyInspectorGesturesDisplayText,
            Properties.UICommandResource.DynamicPropertyInspectorTooltip,
            Properties.UICommandResource.DynamicPropertyInspectorDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand DynamicPropertyInspector
        {
            get { return _DynamicPropertyInspector; }
        }

        private static GeneralCommand _ExploreObjectTree = new GeneralCommand(
            Properties.UICommandResource.ExploreObjectTreeName,
            Properties.UICommandResource.ExploreObjectTreeText,
            Properties.UICommandResource.ExploreObjectTreeGestures,
            Properties.UICommandResource.ExploreObjectTreeGesturesDisplayText,
            Properties.UICommandResource.ExploreObjectTreeTooltip,
            Properties.UICommandResource.ExploreObjectTreeDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ExploreObjectTree
        {
            get { return _ExploreObjectTree; }
        }

        private static GeneralCommand _ShowAdornerExpander = new GeneralCommand(
            Properties.UICommandResource.ShowAdornerExpanderName,
            Properties.UICommandResource.ShowAdornerExpanderText,
            Properties.UICommandResource.ShowAdornerExpanderGestures,
            Properties.UICommandResource.ShowAdornerExpanderGesturesDisplayText,
            Properties.UICommandResource.ShowAdornerExpanderTooltip,
            Properties.UICommandResource.ShowAdornerExpanderDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ShowAdornerExpander
        {
            get { return _ShowAdornerExpander; }
        }

        private static GeneralCommand _ShowWatchWindow = new GeneralCommand(
            Properties.UICommandResource.ShowWatchWindowName,
            Properties.UICommandResource.ShowWatchWindowText,
            Properties.UICommandResource.ShowWatchWindowGestures,
            Properties.UICommandResource.ShowWatchWindowGesturesDisplayText,
            Properties.UICommandResource.ShowWatchWindowTooltip,
            Properties.UICommandResource.ShowWatchWindowDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand ShowWatchWindow
        {
            get { return _ShowWatchWindow; }
        }

        private static GeneralCommand _EditVisibilityLevel = new GeneralCommand(
             Properties.UICommandResource.EditVisibilityLevelName,
             Properties.UICommandResource.EditVisibilityLevelText,
             Properties.UICommandResource.EditVisibilityLevelGestures,
             Properties.UICommandResource.EditVisibilityLevelGesturesDisplayText,
             Properties.UICommandResource.EditVisibilityLevelTooltip,
             Properties.UICommandResource.EditVisibilityLevelDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand EditVisibilityLevel
        {
            get { return _EditVisibilityLevel; }
        }

        private static GeneralCommand _ToolbarEditVisibilityLevel = new GeneralCommand(
             Properties.UICommandResource.ToolbarEditVisibilityLevelName,
             Properties.UICommandResource.ToolbarEditVisibilityLevelText,
             Properties.UICommandResource.ToolbarEditVisibilityLevelGestures,
             Properties.UICommandResource.ToolbarEditVisibilityLevelGesturesDisplayText,
             Properties.UICommandResource.ToolbarEditVisibilityLevelTooltip,
             Properties.UICommandResource.ToolbarEditVisibilityLevelDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand ToolbarEditVisibilityLevel
        {
            get { return _ToolbarEditVisibilityLevel; }
        }

        private static GeneralCommand _EditAlias = new GeneralCommand(
             Properties.UICommandResource.EditAliasName,
             Properties.UICommandResource.EditAliasText,
             Properties.UICommandResource.EditAliasGestures,
             Properties.UICommandResource.EditAliasGesturesDisplayText,
             Properties.UICommandResource.EditAliasTooltip,
             Properties.UICommandResource.EditAliasDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand EditAlias
        {
            get { return _EditAlias; }
        }

        private static GeneralCommand _ZoomIn = new GeneralCommand(
             Properties.UICommandResource.ZoomInName,
             Properties.UICommandResource.ZoomInText,
             Properties.UICommandResource.ZoomInGestures,
             Properties.UICommandResource.ZoomInGesturesDisplayText,
             Properties.UICommandResource.ZoomInTooltip,
             Properties.UICommandResource.ZoomInDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand ZoomIn
        {
            get { return _ZoomIn; }
        }

        private static GeneralCommand _ZoomOut = new GeneralCommand(
             Properties.UICommandResource.ZoomOutName,
             Properties.UICommandResource.ZoomOutText,
             Properties.UICommandResource.ZoomOutGestures,
             Properties.UICommandResource.ZoomOutGesturesDisplayText,
             Properties.UICommandResource.ZoomOutTooltip,
             Properties.UICommandResource.ZoomOutDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand ZoomOut
        {
            get { return _ZoomOut; }
        }

        private static GeneralCommand _ShowToolbar = new GeneralCommand(
             Properties.UICommandResource.ShowToolbarName,
             Properties.UICommandResource.ShowToolbarText,
             Properties.UICommandResource.ShowToolbarGestures,
             Properties.UICommandResource.ShowToolbarGesturesDisplayText,
             Properties.UICommandResource.ShowToolbarTooltip,
             Properties.UICommandResource.ShowToolbarDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand ShowToolbar
        {
            get { return _ShowToolbar; }
        }

        private static GeneralCommand _Next = new GeneralCommand(
             Properties.UICommandResource.NextName,
             Properties.UICommandResource.NextText,
             Properties.UICommandResource.NextGestures,
             Properties.UICommandResource.NextGesturesDisplayText,
             Properties.UICommandResource.NextTooltip,
             Properties.UICommandResource.NextDescription,
             typeof(UIGeneralCommands));

        public static GeneralCommand Next
        {
            get { return _Next; }
        }
    }
}