<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main"
	xmlns:v="urn:schemas-microsoft-com:vml"
	xmlns:w10="urn:schemas-microsoft-com:office:word"
	xmlns:o="urn:schemas-microsoft-com:office:office"
	xmlns:ve="http://schemas.openxmlformats.org/markup-compatibility/2006"
	xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
	xmlns:m="http://schemas.openxmlformats.org/officeDocument/2006/math"
	xmlns:wne="http://schemas.microsoft.com/office/word/2006/wordml"
	xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
	xmlns:pic="http://schemas.openxmlformats.org/drawingml/2006/picture"
	xmlns:wp="http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing">

	<xsl:template match="item | textboxes[@type='TextBox']" mode="textBox">
		<w:pict>
			<v:shape>
				<xsl:attribute name="type">#_x0000_t202</xsl:attribute>
				<xsl:choose>
					<xsl:when test="local-name() = 'item'">
						<xsl:apply-templates select="textbox-format" mode="textBoxFormat"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:apply-templates select="../../item" mode="textBoxFormat" />
					</xsl:otherwise>
				</xsl:choose>
				<v:textbox>
					<w:txbxContent>
						<xsl:apply-templates select="body/paragraphs/item" />
					</w:txbxContent>
				</v:textbox>
			</v:shape>
		</w:pict>
	</xsl:template>
	<xsl:template match="textboxes">
		<xsl:apply-templates select="textboxes[@type='TextBox']" mode="textBox" />
	</xsl:template>
	<xsl:template match="textbox-format | item[@type='ShapeObject']" mode="textBoxFormat">
		<xsl:variable name="horiz">
			<xsl:if test="count(@HorizontalOrigin)>0">
				<xsl:choose>
					<xsl:when test="@HorizontalOrigin = 'Margin'">margin</xsl:when>
					<xsl:when test="@HorizontalOrigin = 'Page'">page</xsl:when>
					<xsl:when test="@HorizontalOrigin = 'Character'">char</xsl:when>
				</xsl:choose>
			</xsl:if>
		</xsl:variable>
		<xsl:variable name="vert">
			<xsl:if test="count(@VerticalOrigin)>0">
				<xsl:choose>
					<xsl:when test="@VerticalOrigin = 'Margin'">margin</xsl:when>
					<xsl:when test="@VerticalOrigin = 'Page'">page</xsl:when>
					<xsl:when test="@VerticalOrigin = 'Line'">line</xsl:when>
				</xsl:choose>
			</xsl:if>
		</xsl:variable>

		<xsl:attribute name="style">
			<xsl:text>position:absolute;margin-left:</xsl:text>
			<xsl:value-of select="@HorizontalPosition" />
			<xsl:text>pt;margin-top:</xsl:text>
			<xsl:value-of select="@VerticalPosition" />
			<xsl:text>pt;width:</xsl:text>
			<xsl:value-of select="@Width" />
			<xsl:text>pt;height:</xsl:text>
			<xsl:value-of select="@Height" />
			<xsl:text>pt;</xsl:text>
			<xsl:if test="@IsBelowText = 'true'">
				<xsl:text>z-index:-251658752;</xsl:text>
			</xsl:if>
			<xsl:if test="($horiz) != ''">
				<xsl:text>mso-position-horizontal-relative:</xsl:text>
				<xsl:value-of select="$horiz" />
			</xsl:if>
			<xsl:if test="($vert) != ''">
				<xsl:text>;mso-position-vertical-relative:</xsl:text>
				<xsl:value-of select="$vert" />
			</xsl:if>
		</xsl:attribute>
		<xsl:if test="count(@FillColor)>0">
			<xsl:attribute name="fillcolor">
				#<xsl:value-of select="substring(@FillColor,4)" />
			</xsl:attribute>
		</xsl:if>
		<xsl:if test="count(@LineColor)>0">
			<xsl:attribute name="strokecolor">
				#<xsl:value-of select="substring(@LineColor,4)" />
			</xsl:attribute>
		</xsl:if>
		<xsl:if test="count(@LineWidth)>0">
			<xsl:attribute name="strokeweight">
				<xsl:value-of select="@LineWidth" />pt
			</xsl:attribute>
		</xsl:if>
		<xsl:if test="@NoLine = 'true'">
			<xsl:attribute name="stroked">f</xsl:attribute>
		</xsl:if>
		<xsl:if test="count(@LineDashing)>0">
			<v:stroke>
				<xsl:attribute name="dashstyle">
					<xsl:choose>
						<xsl:when test="@LineDashing = 'Dot'">1 1</xsl:when>
						<xsl:when test="@LineDashing = 'DashGEL'">dash</xsl:when>
						<xsl:when test="@LineDashing = 'DashDotGEL'">dashDot</xsl:when>
						<xsl:when test="@LineDashing = 'LongDashGEL'">longDash</xsl:when>
						<xsl:when test="@LineDashing = 'LongDashDotGEL'">longDashDot</xsl:when>
						<xsl:when test="@LineDashing = 'LongDashDotDotGEL'">longDashDotDot</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</v:stroke>
		</xsl:if>
		<xsl:if test="count(@HorizontalOrigin)>0 or count(@VerticalOrigin)>0
					or count(@WrappingStyle)=0 or @WrappingStyle != 'InFrontOfText'">
			<w10:wrap>
				<xsl:if test="($horiz) != ''">
					<xsl:attribute name="anchorx">
						<xsl:value-of select="$horiz" />
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="($vert) != ''">
					<xsl:attribute name="anchory">
						<xsl:value-of select="$vert" />
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="count(@WrappingStyle)=0">
					<xsl:attribute name="type">square</xsl:attribute>
				</xsl:if>
				<xsl:if test="@WrappingStyle = 'Tight'">
					<xsl:attribute name="type">tight</xsl:attribute>
				</xsl:if>
			</w10:wrap>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
