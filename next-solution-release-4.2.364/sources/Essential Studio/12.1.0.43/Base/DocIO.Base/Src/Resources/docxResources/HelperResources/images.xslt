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

	<xsl:template match="item | image" mode="drawing">

		<xsl:variable name="imageSizeConst">
			<xsl:choose>
				<xsl:when test="@IsMetafile = 'true'">16910</xsl:when>
				<xsl:otherwise>12689</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<w:drawing>
			<xsl:choose>
				<xsl:when test="count(@WrappingStyle)>0 and (count(preceding::item[1]/@FieldMarkType)=0 or preceding::item[1][@FieldMarkType != 'FieldSeparator'])">
					<wp:anchor>
						<xsl:attribute name="distT">0</xsl:attribute>
						<xsl:attribute name="distB">0</xsl:attribute>
						<xsl:attribute name="distL">0</xsl:attribute>
						<xsl:attribute name="distR">0</xsl:attribute>
						<xsl:attribute name="simplePos">0</xsl:attribute>
						<xsl:attribute name="relativeHeight">0</xsl:attribute>
						<xsl:attribute name="behindDoc">
							<xsl:value-of select="@IsBelowText" />
						</xsl:attribute>
						<xsl:attribute name="locked">0</xsl:attribute>
						<xsl:attribute name="layoutInCell">1</xsl:attribute>
						<xsl:attribute name="allowOverlap">1</xsl:attribute>

						<wp:simplePos x="0" y="0" />
						<wp:positionH>
							<xsl:attribute name="relativeFrom">
								<xsl:call-template name="imagePosition">
									<xsl:with-param name="from">
										<xsl:value-of select="@HorizontalOrigin" />
									</xsl:with-param>
								</xsl:call-template>
							</xsl:attribute>
							<wp:posOffset>
								<xsl:value-of select="round((@HorizontalPosition)*12700)" />
							</wp:posOffset>
						</wp:positionH>
						<wp:positionV>
							<xsl:attribute name="relativeFrom">
								<xsl:call-template name="imagePosition">
									<xsl:with-param name="from">
										<xsl:value-of select="@VerticalOrigin" />
									</xsl:with-param>
								</xsl:call-template>
							</xsl:attribute>
							<wp:posOffset>
								<xsl:value-of select="round((@VerticalPosition)*12700)" />
							</wp:posOffset>
						</wp:positionV>
						<wp:extent>
							<xsl:attribute name="cx">
								<xsl:value-of select="ceiling(((@width)*((@WidthScale) div 100))*($imageSizeConst))" />
							</xsl:attribute>
							<xsl:attribute name="cy">
								<xsl:value-of select="ceiling(((@height)*((@HeightScale) div 100))*($imageSizeConst))" />
							</xsl:attribute>
						</wp:extent>
						<!--<wp:effectExtent l="19050" t="0" r="0" b="0" />-->
						<xsl:choose>
							<xsl:when test="@WrappingStyle = 'Square'">
								<wp:wrapSquare>
									<xsl:attribute name="wrapText">
										<xsl:choose>
											<xsl:when test="@WrappingType = 'Both'">bothSides</xsl:when>
											<xsl:when test="@WrappingType = 'Left'">left</xsl:when>
											<xsl:when test="@WrappingType = 'Right'">right</xsl:when>
											<xsl:when test="@WrappingType = 'Largest'">largest</xsl:when>
										</xsl:choose>
									</xsl:attribute>
								</wp:wrapSquare>
							</xsl:when>
							<xsl:when test="@WrappingStyle = 'Tight'">
								<!--<wp:wrapTight>
									<xsl:attribute name="wrapText">
										<xsl:choose>
											<xsl:when test="@WrappingType = 'Both'">bothSides</xsl:when>
											<xsl:when test="@WrappingType = 'Left'">left</xsl:when>
											<xsl:when test="@WrappingType = 'Right'">right</xsl:when>
											<xsl:when test="@WrappingType = 'Largest'">largest</xsl:when>
										</xsl:choose>
									</xsl:attribute>
									<wp:wrapPolygon edited="0">
										<wp:start x="0" y="0" />
										<wp:lineTo x="0" y="100" />
										<wp:lineTo x="100" y="100" />
										<wp:lineTo x="100" y="0" />
										<wp:lineTo x="0" y="0" />
									</wp:wrapPolygon>
								</wp:wrapTight>-->
								<wp:wrapSquare>
									<xsl:attribute name="wrapText">
										<xsl:choose>
											<xsl:when test="@WrappingType = 'Both'">bothSides</xsl:when>
											<xsl:when test="@WrappingType = 'Left'">left</xsl:when>
											<xsl:when test="@WrappingType = 'Right'">right</xsl:when>
											<xsl:when test="@WrappingType = 'Largest'">largest</xsl:when>
										</xsl:choose>
									</xsl:attribute>
								</wp:wrapSquare>
							</xsl:when>
							<xsl:when test="@WrappingStyle = 'Through'">
								<!--<wp:wrapThrough>
									<xsl:attribute name="wrapText">
										<xsl:choose>
											<xsl:when test="@WrappingType = 'Both'">bothSides</xsl:when>
											<xsl:when test="@WrappingType = 'Left'">left</xsl:when>
											<xsl:when test="@WrappingType = 'Right'">right</xsl:when>
											<xsl:when test="@WrappingType = 'Largest'">largest</xsl:when>
										</xsl:choose>
									</xsl:attribute>
								</wp:wrapThrough>-->
								<wp:wrapSquare>
									<xsl:attribute name="wrapText">
										<xsl:choose>
											<xsl:when test="@WrappingType = 'Both'">bothSides</xsl:when>
											<xsl:when test="@WrappingType = 'Left'">left</xsl:when>
											<xsl:when test="@WrappingType = 'Right'">right</xsl:when>
											<xsl:when test="@WrappingType = 'Largest'">largest</xsl:when>
										</xsl:choose>
									</xsl:attribute>
								</wp:wrapSquare>
							</xsl:when>
							<xsl:when test="@WrappingStyle = 'TopAndBottom'">
								<wp:wrapTopAndBottom />
							</xsl:when>
							<xsl:otherwise>
								<wp:wrapNone />
							</xsl:otherwise>
						</xsl:choose>
						<xsl:apply-templates select="." mode="graphic">
							<xsl:with-param name="multiplier">
								<xsl:value-of select="$imageSizeConst" />
							</xsl:with-param>
						</xsl:apply-templates>
					</wp:anchor>
				</xsl:when>
				<xsl:otherwise>
					<wp:inline>
						<wp:extent>
							<xsl:attribute name="cx">
								<xsl:value-of select="ceiling(((@width)*((@WidthScale) div 100))*($imageSizeConst))" />
							</xsl:attribute>
							<xsl:attribute name="cy">
								<xsl:value-of select="ceiling(((@height)*((@HeightScale) div 100))*($imageSizeConst))" />
							</xsl:attribute>
						</wp:extent>
						<xsl:apply-templates select="." mode="graphic">
							<xsl:with-param name="multiplier">
								<xsl:value-of select="$imageSizeConst" />
							</xsl:with-param>
						</xsl:apply-templates>
					</wp:inline>
				</xsl:otherwise>
			</xsl:choose>
		</w:drawing>
		</xsl:template>
	<xsl:template match="item | image" mode="graphic">
		<xsl:param name="multiplier">12700</xsl:param>

		<wp:docPr>
			<xsl:attribute name="id">
				<xsl:value-of select="@id" />
			</xsl:attribute>
			<xsl:attribute name="name"></xsl:attribute>
			<xsl:attribute name="descr"></xsl:attribute>
		</wp:docPr>
		<a:graphic xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main">
			<a:graphicData uri="http://schemas.openxmlformats.org/drawingml/2006/picture">
				<pic:pic xmlns:pic="http://schemas.openxmlformats.org/drawingml/2006/picture">
					<pic:nvPicPr>
						<pic:cNvPr>
							<xsl:attribute name="id">
								<xsl:value-of select="@id" />
							</xsl:attribute>
							<xsl:attribute name="name"></xsl:attribute>
							<xsl:attribute name="descr"></xsl:attribute>
						</pic:cNvPr>
						<pic:cNvPicPr>
							<a:picLocks noChangeAspect="1" noChangeArrowheads="1" />
						</pic:cNvPicPr>
					</pic:nvPicPr>
					<pic:blipFill>
						<a:blip>
							<xsl:attribute name="r:embed">
								<xsl:value-of select="@id" />
							</xsl:attribute>
							<xsl:if test="count(@watermark)>0">
								<xsl:attribute name="watermark">true</xsl:attribute>
							</xsl:if>
						</a:blip>
						<a:stretch>
							<a:fillRect />
						</a:stretch>
					</pic:blipFill>
					<pic:spPr bwMode="auto">
						<a:xfrm>
							<a:off>
								<xsl:attribute name="x">0</xsl:attribute>
								<xsl:attribute name="y">0</xsl:attribute>
							</a:off>
							<a:ext>
								<xsl:attribute name="cx">
									<xsl:value-of select="ceiling(((@width)*((@WidthScale) div 100))*($multiplier))" />
								</xsl:attribute>
								<xsl:attribute name="cy">
									<xsl:value-of select="ceiling(((@height)*((@HeightScale) div 100))*($multiplier))" />
								</xsl:attribute>
							</a:ext>
						</a:xfrm>
						<a:prstGeom prst="rect">
							<a:avLst />
						</a:prstGeom>
					</pic:spPr>
				</pic:pic>
			</a:graphicData>
		</a:graphic>
	</xsl:template>
	<xsl:template name="imagePosition">
		<xsl:param name="from"></xsl:param>

		<xsl:choose>
			<xsl:when test="$from = 'Column'">column</xsl:when>
			<xsl:when test="$from = 'Character'">character</xsl:when>
			<xsl:when test="$from = 'Page'">page</xsl:when>
			<xsl:when test="$from = 'Margin'">margin</xsl:when>
			<xsl:when test="$from = 'Paragraph'">paragraph</xsl:when>
			<xsl:when test="$from = 'Line'">line</xsl:when>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
