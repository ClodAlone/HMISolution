<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="UFUAWebEditor._Default" %>

<%@ Register assembly="DevExpress.Web.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>



<%@ Register assembly="DevExpress.Web.ASPxGauges.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.Linear" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.Circular" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.State" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxGauges.v21.2, Version=21.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxGauges.Gauges.Digital" tagprefix="dx" %>



<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to UF UA Server Configuration Editor
    </h2>
    <dx:ASPxCallbackPanel ID="ASPxCallbackPanel1" runat="server" ClientInstanceName="TimerCallbackPanel"
        Width="100%" OnCallback="OnTimerCallback" ShowLoadingPanel="false">

        <PanelCollection>
            <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                <table style="width: 100%">

                <td style="width: 33%;" align="center" valign="middle">
                    <dx:ASPxButton ID="processButton" runat="server"
                    AutoPostBack="false"
                    Text="Start Server">
                    <ClientSideEvents
                        Click="function( s, e ) { cbFileProcess.PerformCallback(); }" />
                </dx:ASPxButton>
                </td>
                <td style="width: 33%;" align="center" valign="middle">
                    <dx:ASPxCallback ID="cbFileProcess" runat="server"
                    ClientInstanceName="cbFileProcess" OnCallback="cbFileProcess_Callback">
                </dx:ASPxCallback>       
                </td>
                <td style="width: 33%;" align="center" valign="middle">             
                    <dx:ASPxGaugeControl ID="ASPxGaugeControl2" runat="server" BackColor="White" 
                    ClientIDMode="AutoID" Height="350px" Value="15" Width="350px">
                    <Gauges>
                        <dx:CircularGauge Bounds="0, 0, 350, 350" Name="SysInfo_DashBoard">
                            <scales>
                                <dx:ArcScaleComponent AppearanceTickmarkText-Font="Tahoma, 5pt" 
                                    AppearanceTickmarkText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                    Center="125, 75" EndAngle="60" MajorTickmark-FormatString="{0:F0}" 
                                    MajorTickmark-ShapeOffset="-2" MajorTickmark-ShapeScale="0.4, 0.6" 
                                    MajorTickmark-ShapeType="Circular_Style1_4" MajorTickmark-TextOffset="-7" 
                                    MajorTickmark-TextOrientation="LeftToRight" MaxValue="100" MinorTickCount="4" 
                                    MinorTickmark-ShapeScale="0.3, 0.5" MinorTickmark-ShapeType="Circular_Style1_3" 
                                    Name="cpuTotal" RadiusX="45" RadiusY="45" StartAngle="-240" Value="15" 
                                    ZOrder="-50">
                                    <labels>
                                        <dx:ScaleLabelWeb AppearanceText-Font="Tahoma, 5pt" 
                                            AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                            FormatString="{0} {1:F0}%" Name="Label0" Position="125, 116" Size="100, 12" 
                                            Text="Total:" />
                                        <dx:ScaleLabelWeb AppearanceText-Font="Tahoma, 5pt" 
                                            AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:DimGray&quot;/&gt;" 
                                            FormatString="{0}" Name="Label1" Position="125, 95" Size="50, 12" 
                                            Text="Kernel" />
                                        <dx:ScaleLabelWeb AppearanceText-Font="Tahoma, 5pt" 
                                            AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:DarkGray&quot;/&gt;" 
                                            FormatString="{0}" Name="Label2" Position="125, 88" Size="50, 12" Text="User" />
                                    </labels>
                                </dx:ArcScaleComponent>
                                <dx:ArcScaleComponent AppearanceTickmarkText-Font="Tahoma, 6pt" 
                                    AppearanceTickmarkText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Silver&quot;/&gt;" 
                                    Center="125, 75" EndAngle="60" MajorTickmark-FormatString="{0:F0}" 
                                    MajorTickmark-ShapeOffset="-2" MajorTickmark-ShapeScale="0.6, 0.8" 
                                    MajorTickmark-ShapeType="Circular_Style10_4" MajorTickmark-ShowText="False" 
                                    MajorTickmark-ShowTick="False" MajorTickmark-TextOffset="-10" 
                                    MajorTickmark-TextOrientation="LeftToRight" MaxValue="100" MinorTickCount="4" 
                                    MinorTickmark-ShapeScale="0.6, 1" MinorTickmark-ShapeType="Circular_Style10_3" 
                                    MinorTickmark-ShowTick="False" Name="cpuUser" RadiusX="50" RadiusY="50" 
                                    StartAngle="-240" Value="15" ZOrder="1001">
                                </dx:ArcScaleComponent>
                                <dx:ArcScaleComponent AppearanceTickmarkText-Font="Tahoma, 6pt" 
                                    AppearanceTickmarkText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Silver&quot;/&gt;" 
                                    Center="125, 75" EndAngle="60" MajorTickmark-FormatString="{0:F0}" 
                                    MajorTickmark-ShapeOffset="-2" MajorTickmark-ShapeScale="0.6, 0.8" 
                                    MajorTickmark-ShapeType="Circular_Style10_4" MajorTickmark-ShowText="False" 
                                    MajorTickmark-ShowTick="False" MajorTickmark-TextOffset="-10" 
                                    MajorTickmark-TextOrientation="LeftToRight" MaxValue="100" MinorTickCount="4" 
                                    MinorTickmark-ShapeScale="0.6, 1" MinorTickmark-ShapeType="Circular_Style10_3" 
                                    MinorTickmark-ShowTick="False" Name="cpuKernel" RadiusX="50" RadiusY="50" 
                                    StartAngle="-240" Value="3" ZOrder="1002">
                                </dx:ArcScaleComponent>
                                <dx:ArcScaleComponent AppearanceTickmarkText-Font="Tahoma, 4pt" 
                                    AppearanceTickmarkText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                    Center="125, 175" EndAngle="10" MajorTickCount="7" 
                                    MajorTickmark-FormatString="{0:F0}" MajorTickmark-ShapeOffset="-1" 
                                    MajorTickmark-ShapeScale="0.25, 0.4" 
                                    MajorTickmark-ShapeType="Circular_Style1_4" MajorTickmark-TextOffset="-7" 
                                    MajorTickmark-TextOrientation="LeftToRight" MaxValue="1300" MinorTickCount="4" 
                                    MinorTickmark-ShapeScale="0.3, 0.5" MinorTickmark-ShapeType="Circular_Style1_3" 
                                    MinValue="100" Name="osThreads" RadiusX="35" RadiusY="35" StartAngle="-190" 
                                    Value="500">
                                    <labels>
                                        <dx:ScaleLabelWeb AppearanceText-Font="Tahoma, 4pt" 
                                            AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                            FormatString="{0}{1:F0}" Name="Label1" Position="125, 160" Size="50, 12" 
                                            Text="Threads:" />
                                    </labels>
                                </dx:ArcScaleComponent>
                                <dx:ArcScaleComponent AppearanceTickmarkText-Font="Tahoma, 3pt" 
                                    AppearanceTickmarkText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                    Center="125, 175" EndAngle="150" MajorTickCount="6" 
                                    MajorTickmark-FormatString="{0:F0}" MajorTickmark-ShapeOffset="-1" 
                                    MajorTickmark-ShapeScale="0.3, 0.4" MajorTickmark-ShapeType="Circular_Style1_2" 
                                    MajorTickmark-TextOffset="-7" MajorTickmark-TextOrientation="LeftToRight" 
                                    MaxValue="150" MinorTickCount="4" MinorTickmark-ShapeScale="0.3, 0.5" 
                                    MinorTickmark-ShapeType="Circular_Style1_1" Name="osProcesses" RadiusX="35" 
                                    RadiusY="35" StartAngle="30" Value="50">
                                    <labels>
                                        <dx:ScaleLabelWeb AppearanceText-Font="Tahoma, 4pt" 
                                            AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                            FormatString="{0}{1:F0}" Name="Label1" Position="125, 191" Size="50, 12" 
                                            Text="Processes:" />
                                    </labels>
                                </dx:ArcScaleComponent>
                                <dx:ArcScaleComponent AppearanceTickmarkText-Font="Tahoma, 3pt" 
                                    AppearanceTickmarkText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                    Center="46.5, 135" EndAngle="60" MajorTickmark-FormatString="{0:F0}" 
                                    MajorTickmark-ShapeOffset="-2" MajorTickmark-ShapeScale="0.2, 0.3" 
                                    MajorTickmark-ShapeType="Circular_Style1_4" MajorTickmark-TextOffset="-7" 
                                    MajorTickmark-TextOrientation="LeftToRight" MaxValue="100" MinorTickCount="4" 
                                    MinorTickmark-ShapeScale="0.15, 0.25" 
                                    MinorTickmark-ShapeType="Circular_Style1_3" Name="memoryTotal" RadiusX="35" 
                                    RadiusY="35" StartAngle="-240" Value="15">
                                    <labels>
                                        <dx:ScaleLabelWeb AppearanceText-Font="Tahoma, 4pt" 
                                            AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                            FormatString="{0} {2:P1}" Name="Label0" Position="50, 150" Size="100, 12" 
                                            Text="Free:" />
                                        <dx:ScaleLabelWeb AppearanceText-Font="Tahoma, 4pt" 
                                            AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:#C00000&quot;/&gt;" 
                                            FormatString="{0}" Name="Label1" Position="50, 125" Text="Memory, MB" />
                                    </labels>
                                </dx:ArcScaleComponent>
                                <dx:ArcScaleComponent AppearanceTickmarkText-Font="Tahoma, 3pt" 
                                    AppearanceTickmarkText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                    Center="203.5, 135" EndAngle="60" MajorTickmark-FormatString="{0:F0}" 
                                    MajorTickmark-ShapeOffset="-2" MajorTickmark-ShapeScale="0.2, 0.3" 
                                    MajorTickmark-ShapeType="Circular_Style1_4" MajorTickmark-TextOffset="-7" 
                                    MajorTickmark-TextOrientation="LeftToRight" MaxValue="100" MinorTickCount="4" 
                                    MinorTickmark-ShapeScale="0.15, 0.25" 
                                    MinorTickmark-ShapeType="Circular_Style1_3" Name="hddTotal" RadiusX="35" 
                                    RadiusY="35" StartAngle="-240" Value="15">
                                    <labels>
                                        <dx:ScaleLabelWeb AppearanceText-Font="Tahoma, 4pt" 
                                            AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Black&quot;/&gt;" 
                                            FormatString="{0} {2:P1}" Name="Label0" Position="205, 150" Size="100, 12" 
                                            Text="Free:" />
                                        <dx:ScaleLabelWeb AppearanceText-Font="Tahoma, 4pt" 
                                            AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:#C00000&quot;/&gt;" 
                                            FormatString="{0}" Name="Label1" Position="205, 125" Text="HDD, GB" />
                                    </labels>
                                </dx:ArcScaleComponent>
                            </scales>
                            <backgroundlayers>
                                <dx:ArcScaleBackgroundLayerComponent Name="bgCPU" ScaleID="cpuTotal" 
                                    ShapeType="CircularFull_Style1" Size="112, 112" ZOrder="-5" />
                                <dx:ArcScaleBackgroundLayerComponent Name="bgOS" ScaleID="osThreads" 
                                    ShapeType="CircularFull_Style1" Size="87, 87" ZOrder="1050" />
                                <dx:ArcScaleBackgroundLayerComponent Name="bgMemory" ScaleID="memoryTotal" 
                                    ShapeType="CircularFull_Style1" Size="82, 82" ZOrder="1000" />
                                <dx:ArcScaleBackgroundLayerComponent Name="bgHDD" ScaleID="hddTotal" 
                                    ShapeType="CircularFull_Style1" Size="82, 82" ZOrder="1000" />
                                <dx:ArcScaleBackgroundLayerComponent Name="bgAll" ScaleCenterPos="0.5, 0.285" 
                                    ScaleID="cpuTotal" ShapeType="CircularFull_SysInfoBack" Size="250, 212" 
                                    ZOrder="5000" />
                            </backgroundlayers>
                            <needles>
                                <dx:ArcScaleNeedleComponent Name="needleCpu" ScaleID="cpuTotal" 
                                    ShapeType="CircularFull_Style1" StartOffset="-2" ZOrder="-50" />
                                <dx:ArcScaleNeedleComponent Name="needleRAM" ScaleID="memoryTotal" 
                                    ShapeType="CircularFull_Style1" StartOffset="-2.75" ZOrder="-50" />
                                <dx:ArcScaleNeedleComponent Name="needleHDD" ScaleID="hddTotal" 
                                    ShapeType="CircularFull_Style1" StartOffset="-1.5" ZOrder="-50" />
                            </needles>
                            <rangebars>
                                <dx:ArcScaleRangeBarComponent AppearanceRangeBar-ContentBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Silver&quot;/&gt;" 
                                    EndOffset="16" Name="rangeUser" ScaleID="cpuUser" StartOffset="40" 
                                    ZOrder="-10" />
                                <dx:ArcScaleRangeBarComponent AppearanceRangeBar-ContentBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Gray&quot;/&gt;" 
                                    EndOffset="16" Name="rangeKernel" ScaleID="cpuKernel" StartOffset="40" 
                                    ZOrder="-11" />
                                <dx:ArcScaleRangeBarComponent AnchorValue="100" 
                                    AppearanceRangeBar-ContentBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Silver&quot;/&gt;" 
                                    EndOffset="-3" Name="rangeThreads" ScaleID="osThreads" StartOffset="33" 
                                    ZOrder="1" />
                                <dx:ArcScaleRangeBarComponent AppearanceRangeBar-ContentBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:Silver&quot;/&gt;" 
                                    EndOffset="-2" Name="rangeProcesses" ScaleID="osProcesses" StartOffset="33" 
                                    ZOrder="1" />
                            </rangebars>
                            <labels>
                                <dx:LabelComponent AppearanceText-Font="Tahoma, 4pt" 
                                    AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:#C00000&quot;/&gt;" 
                                    Name="processorName" Position="125, 62" Size="75, 25" Text="CPU Info" 
                                    ZOrder="-45" />
                                <dx:LabelComponent AppearanceText-Font="Tahoma, 4pt" 
                                    AppearanceText-TextBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:#C00000&quot;/&gt;" 
                                    Name="osName" Position="125, 175" Size="55, 25" Text="OS Info" ZOrder="-2" />
                            </labels>
                        </dx:CircularGauge>
                    </Gauges>
                </dx:ASPxGaugeControl>
                </td>
                <td align="right" valign="middle" class="ControlActionSpacing">
                    <dx:ASPxGaugeControl ID="ASPxGaugeControl1" runat="server" Height="250px" 
                    Width="250px" Value="00,000">
                    <Gauges>
                        <dx:DigitalGauge AppearanceOff-ContentBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:#00FFFFFF&quot;/&gt;" 
                            AppearanceOn-ContentBrush="&lt;BrushObject Type=&quot;Solid&quot; Data=&quot;Color:WhiteSmoke&quot;/&gt;" 
                            Bounds="0, 0, 250, 250" DigitCount="5" ID="DigitalGauge" Padding="20, 20, 20, 20" 
                            Text="00,000">
                            <backgroundlayers>
                                <dx:DigitalBackgroundLayerComponent BottomRight="259.8125, 99.9625" 
                                    Name="digitalBackgroundLayerComponent6" ShapeType="Style6" TopLeft="20, 0" 
                                    ZOrder="1000" />
                            </backgroundlayers>
                        </dx:DigitalGauge>
                    </Gauges>
                </dx:ASPxGaugeControl>
                </td>
                </table>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    <dx:ASPxTimer ID="ASPxTimer1" runat="server" Interval="2000">
        <ClientSideEvents Tick="function(s, e) {
                    TimerCallbackPanel.PerformCallback(&quot;OnTimer&quot;);
             }">
        </ClientSideEvents>
    </dx:ASPxTimer>
</asp:Content>
