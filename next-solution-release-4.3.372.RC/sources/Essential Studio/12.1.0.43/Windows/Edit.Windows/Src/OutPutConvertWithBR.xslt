<?xml version='1.0' encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
			xmlns:msxsl="urn:schemas-microsoft-com:xslt"
      xmlns:ToHTML="http://syncfusion.com/schemas" >
	<xsl:output method="html" encoding="utf-8" indent="no" />
	<msxsl:script language="JavaScript" implements-prefix="ToHTML">
  function ReplaceSpaceToSpecSymbols( strText )
  {	
		var re1 = / /, re2 = /\t/;
		var amp = "\u00a0"; 
		strText = strText.replace( re1, amp );
		strText = strText.replace( re2, amp + amp + amp + amp + amp + amp );
		return strText;
  }
	</msxsl:script>
	<xsl:template match="root">
  <html xmlns="http://www.w3.org/1999/xhtml" xmlns:xhtml2rtf="http://www.lutecia.info/download/xmlns/xhtml2rtf">
			<xsl:text>&#10;</xsl:text>
    <head>
				<xsl:text>&#10;</xsl:text>
    <style>
					<xsl:text>&#10;</xsl:text>
					<xsl:for-each select="formats/format">.n<xsl:value-of select="@ID" />{ color: <xsl:value-of select="@color" />;<xsl:if test="@bold!='True'">font-weight: normal;</xsl:if><xsl:if test="@italic='True'">font-style: italic;</xsl:if>font-family: <xsl:value-of select="@font" />;font-size: <xsl:value-of select="@fontSize" />pt;<xsl:if test="@underline!='None'">text-decoration: underline;</xsl:if>}&#xA;</xsl:for-each>
    </style>
				<xsl:text>&#10;</xsl:text>
    </head>
			<xsl:text>&#10;</xsl:text>
			<body>
				<xsl:text>&#10;</xsl:text>
				<xsl:comment>StartFragment </xsl:comment>
					<xsl:text>&#10;</xsl:text>
					<xsl:for-each select="lines/line">
						<xsl:for-each select="lexem">
							<xsl:variable name="fname" select="@format" />
							<xsl:variable name="id" select="/root/formats/format[@name=$fname]/@ID" />
							<b class="n{$id}">
								<xsl:value-of select="ToHTML:ReplaceSpaceToSpecSymbols(string(@text))" />
							</b>
						</xsl:for-each>
						<xsl:text>&#10;</xsl:text>
						<br/>
					</xsl:for-each>
				<xsl:text>&#10;</xsl:text>
				<xsl:comment>EndFragment </xsl:comment>
				<xsl:text>&#10;</xsl:text>
			</body>
			<xsl:text>&#10;</xsl:text>
  </html>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>
	
</xsl:stylesheet>