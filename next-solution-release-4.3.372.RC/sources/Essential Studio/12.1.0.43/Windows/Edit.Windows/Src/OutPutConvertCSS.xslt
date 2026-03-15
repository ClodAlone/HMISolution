<?xml version='1.0' encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="utf-8" indent="no" />
	<xsl:template match="root">
		<xsl:text>&#10;</xsl:text>
		<xsl:for-each select="formats/format">.n<xsl:value-of select="@ID" />{ color: <xsl:value-of select="@color" />;<xsl:if test="@bold!='True'">font-weight: normal;</xsl:if><xsl:if test="@italic='True'">font-style: italic;</xsl:if>font-family: <xsl:value-of select="@font" />;font-size: <xsl:value-of select="@fontSize" />pt;<xsl:if test="@underline!='None'">text-decoration: underline;</xsl:if>}&#xA;</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>