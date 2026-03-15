<%@ Page language="c#" 
Inherits="PropertyGridWeb.WebForm1" 
enableViewState="true" CodeFile="default.aspx.cs" %>
<%@ Register TagPrefix="xacc" Namespace="Xacc" Assembly="xacc.propertygrid" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML lang="en">
  <HEAD runat="server">
    <title>ASP.NET PropertyGrid Demo</title>
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <xacc:propertygrid id="pg1" runat="server" ShowHelp="True"></xacc:propertygrid><xacc:propertygrid id="pg2" runat="server" ReadOnly="True" Width="350" SelectionColor="CadetBlue" BackgroundColor="NavajoWhite"
        FontFamily="Tahoma" FontSize="9pt" ForeColor="DimGray" HeaderForeColor="Brown" ItemBackgroundColor="WhiteSmoke"></xacc:propertygrid><IMG alt="" src="ms.pg.png">
      <div style="CLEAR: both">&nbsp;</div>
      <DIV style="CLEAR: both">&nbsp;</DIV>
      <DIV style="CLEAR: both">&nbsp;</DIV>
      <DIV style="CLEAR: both"><asp:button id="Button1" runat="server" Text="Object code" Width="120px" onclick="Button1_Click"></asp:button><asp:button id="Button2" runat="server" Text="Usage" Width="120px" onclick="Button2_Click"></asp:button></DIV>
      <div>Works in:
        <dl>
          <dd>
            <span>Internet Explorer 6</span>
            <IMG src="ie.png">
            <dd>
              <span>Mozilla Firefox 1.5</span>
              <IMG src="ff15.png">
              <dd>
                <span>Opera 8 &amp;&nbsp;9 
  beta</span>
                <IMG src="opera.gif">
              </dd>
        </dl>
        Conformance
        <dl>
          <dd>
            <span>XHTML 1.0 Strict (very 
  almost) valid</span><IMG src="valid-xhtml10.png">
            <dd>
              <span>W3C CSS valid</span>
              <IMG src="vcss.png">
            </dd>
        </dl>
        Technologies
        <dl>
          <dd>
            .NET 1.1
            <dd>
              ASP.NET
              <dd>
                AJAX (via stripped Anthem)
                <dd>
                  JavaScript
                  <dd>
                    DHTML
                    <dd>
                      CSS
                      <dd>
                        Common sense and intuition</dd></dl>
      </div>
    </form>
  </body>
</HTML>
