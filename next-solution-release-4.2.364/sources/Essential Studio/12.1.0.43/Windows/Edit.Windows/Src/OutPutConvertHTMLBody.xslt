<?xml version='1.0' encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="utf-8" indent="no" />
	<xsl:template match="root">
		<xsl:text>&#10;</xsl:text>
		<xsl:comment>StartFragment </xsl:comment>
		<xsl:text>&#10;</xsl:text>
		<pre>
			<xsl:text>&#10;</xsl:text>
			<xsl:for-each select="lines/line">
				<xsl:for-each select="lexem">
					<xsl:variable name="fname" select="@format" />
					<xsl:variable name="id" select="/root/formats/format[@name=$fname]/@ID" />
					<b class="n{$id}">
						<xsl:value-of select="@text" />
					</b>
				</xsl:for-each>
				<xsl:text>&#10;</xsl:text>
			</xsl:for-each>
		</pre>
		<xsl:text>&#10;</xsl:text>
		<xsl:comment>EndFragment </xsl:comment>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>
</xsl:stylesheet>