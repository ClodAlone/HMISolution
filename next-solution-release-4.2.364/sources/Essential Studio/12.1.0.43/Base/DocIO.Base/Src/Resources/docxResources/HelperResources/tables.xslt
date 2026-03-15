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
	
	<xsl:template match="table-format">
		<xsl:if test="@LeftOffset != ''">
			<xsl:element name="w:tblInd">
				<xsl:attribute name="w:w">
					<xsl:choose>
						<xsl:when test="Paddings/@Left != ''">
							<xsl:value-of select="((@LeftOffset)+(Paddings/@Left))*$twentiethOfPoint"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="(@LeftOffset)*$twentiethOfPoint"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:attribute name="w:type">dxa</xsl:attribute>
			</xsl:element>
		</xsl:if>		
		<w:tblBorders>
			<xsl:apply-templates select="borders">
				<xsl:with-param name="multiplier">8</xsl:with-param>
			</xsl:apply-templates>
		</w:tblBorders>
		<xsl:if test="@CellSpacing != ''">
			<w:tblCellSpacing>
				<xsl:attribute name="w:w">
					<xsl:value-of select="(@CellSpacing)*$twentiethOfPoint"/>
				</xsl:attribute>
				<xsl:attribute name="w:type">dxa</xsl:attribute>
			</w:tblCellSpacing>
		</xsl:if>
		<xsl:choose>
			<xsl:when test="@BidiTable = 'true'">
				<w:bidiVisual />
				<w:jc>
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="@HAlignment = 'Left' or count(@HAlignment) = 0">right</xsl:when>
							<xsl:when test="@HAlignment = 'Right'">left</xsl:when>
							<xsl:when test="@HAlignment = 'Center'">center</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</w:jc>
			</xsl:when>
			<xsl:otherwise>
				<w:jc>
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="@HAlignment = 'Left' or count(@HAlignment) = 0">left</xsl:when>
							<xsl:when test="@HAlignment = 'Right'">right</xsl:when>
							<xsl:when test="@HAlignment = 'Center'">center</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</w:jc>
			</xsl:otherwise>
		</xsl:choose>
		<w:tblStyle w:val="TableGrid"/>
		<xsl:if test="count(@IsAutoResized)=0">
			<w:tblLayout w:type="fixed" />
		</xsl:if>
		<w:tblCellMar>
			<xsl:apply-templates select="Paddings"/>
		</w:tblCellMar>
	</xsl:template>
	<xsl:template match="cell-format">
		<w:tcPr>
			<w:tcW w:type="dxa">
				<xsl:attribute name="w:w">
					<xsl:value-of select="(../@Width)*$twentiethOfPoint" />
				</xsl:attribute>
			</w:tcW>
			<w:tcBorders>
				<xsl:apply-templates select="borders">
					<xsl:with-param name="multiplier">8</xsl:with-param>
				</xsl:apply-templates>
			</w:tcBorders>
			<xsl:if test="@VAlignment != ''" >
				<w:vAlign>
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="@VAlignment = 'Top'">top</xsl:when>
							<xsl:when test="@VAlignment = 'Middle'">center</xsl:when>
							<xsl:when test="@VAlignment = 'Bottom'">bottom</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</w:vAlign>
			</xsl:if>
			<xsl:if test="@VMerge != ''">
				<w:vMerge>
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="@VMerge = 'Start'">restart</xsl:when>
							<xsl:when test="@VMerge = 'Continue'">continue</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</w:vMerge>
			</xsl:if>
			<xsl:if test="@HMerge != ''">
				<w:hMerge>
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="@VMerge = 'Start'">restart</xsl:when>
							<xsl:when test="@VMerge = 'Continue'">continue</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</w:hMerge>
			</xsl:if>
			<xsl:if test="(@ShadingColor != '') or (../@ForeColor != '')">
				<w:shd>
					<xsl:if test="@ShadingColor != ''">
						<xsl:attribute name="w:fill">
							<xsl:value-of select="substring(@ShadingColor, 4)" />
						</xsl:attribute>
					</xsl:if>
					<xsl:if test="../@ForeColor != ''">
						<xsl:attribute name="w:color">
							<xsl:value-of select="substring(../@ForeColor, 4)" />
						</xsl:attribute>
					</xsl:if>
					<xsl:attribute name="w:val">
						<xsl:call-template name="TextureStyle">
							<xsl:with-param name="texture">
								<xsl:value-of select="../@Texture" />
							</xsl:with-param>
						</xsl:call-template>
					</xsl:attribute>
				</w:shd>
			</xsl:if>
			<xsl:if test="@TextWrap = 'false'">
				<w:noWrap w:val="false" />
			</xsl:if>
			<xsl:if test="@FitText = 'true'">
				<w:tcFitText w:val ="true" />
			</xsl:if>
			<xsl:if test="@TextDirection != ''">
				<w:textDirection>
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="@TextDirection = 'Horizontal'">lrTb</xsl:when>
							<xsl:when test="@TextDirection = 'VerticalBottomToTop'">btLr</xsl:when>
							<xsl:when test="@TextDirection = 'VerticalTopToBottom'">tbRl</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</w:textDirection>
			</xsl:if>
			<xsl:if test="../@collcount != ''">
				<w:gridSpan>
					<xsl:attribute name="w:val">
						<xsl:value-of select="../@collcount" />
					</xsl:attribute>
				</w:gridSpan>
			</xsl:if>
			<xsl:if test="@SamePaddingsAsTable = 'false'">
				<w:tcMar>
					<xsl:apply-templates select="Paddings" />
				</w:tcMar>
			</xsl:if>
		</w:tcPr>
	</xsl:template>
	<xsl:template match="tblGrid">
		<w:tblGrid>
			<xsl:apply-templates select="gridCol" />
		</w:tblGrid>
	</xsl:template>
	<xsl:template match="gridCol">
		<w:gridCol>
			<xsl:attribute name="w:w">
				<xsl:value-of select="@w"/>
			</xsl:attribute>
		</w:gridCol>
	</xsl:template>
	<xsl:template match="Paddings">
		<xsl:if test="@Top != ''">
			<w:top>
				<xsl:attribute name="w:w">
					<xsl:value-of select="(@Top)*$twentiethOfPoint"/>
				</xsl:attribute>
				<xsl:attribute name="w:type">dxa</xsl:attribute>
			</w:top>
		</xsl:if>
		<xsl:if test="@Left != ''">
			<w:left>
				<xsl:attribute name="w:w">
					<xsl:value-of select="(@Left)*$twentiethOfPoint"/>
				</xsl:attribute>
				<xsl:attribute name="w:type">dxa</xsl:attribute>
			</w:left>
		</xsl:if>
		<xsl:if test="@Bottom != ''">
			<w:bottom>
				<xsl:attribute name="w:w">
					<xsl:value-of select="(@Bottom)*$twentiethOfPoint"/>
				</xsl:attribute>
				<xsl:attribute name="w:type">dxa</xsl:attribute>
			</w:bottom>
		</xsl:if>
		<xsl:if test="@Right != ''">
			<w:right>
				<xsl:attribute name="w:w">
					<xsl:value-of select="(@Right)*$twentiethOfPoint"/>
				</xsl:attribute>
				<xsl:attribute name="w:type">dxa</xsl:attribute>
			</w:right>
		</xsl:if>
	</xsl:template>
	<xsl:template match="rows">
		<xsl:apply-templates select="row" />
	</xsl:template>
	<xsl:template match="row">
		<w:tr>
			<w:trPr>
				<xsl:if test="@RowHeight != ''">
					<w:trHeight>
						<xsl:attribute name="w:val">
							<xsl:value-of select="(@RowHeight)*$twentiethOfPoint" />
						</xsl:attribute>
						<xsl:if test="@HeightType != ''">
							<xsl:attribute name="w:hRule">
								<xsl:choose>
									<xsl:when test="@HeightType = 'AtLeast'">atLeast</xsl:when>
									<xsl:when test="@HeightType = 'Exactly'">exact</xsl:when>
								</xsl:choose>
							</xsl:attribute>
						</xsl:if>
					</w:trHeight>
				</xsl:if>
				<xsl:if test="count(@IsHeader) > 0">
					<w:tblHeader />
				</xsl:if>
				<xsl:if test="(table-format/@LeftOffset) > (preceding::row[1]/table-format/@LeftOffset)">
					<xsl:variable name="offsetBefore">
						<xsl:value-of select="(table-format/@LeftOffset)*$twentiethOfPoint" />
					</xsl:variable>
					<w:wBefore>
						<xsl:attribute name="w:type">dxa</xsl:attribute>
						<xsl:attribute name="w:w">
							<xsl:value-of select="$offsetBefore" />
						</xsl:attribute>
					</w:wBefore>
					<w:gridBefore>
						<xsl:attribute name="w:val">
							<xsl:value-of select="table-format/@GridBefore" />
						</xsl:attribute>
					</w:gridBefore>
				</xsl:if>
			</w:trPr>
			<xsl:apply-templates select="character-format" />
			<xsl:apply-templates select="cells" />
		</w:tr>
	</xsl:template>
	<xsl:template match="cells">
		<xsl:apply-templates select="cell" />
	</xsl:template>
	<xsl:template match="cell">
		<w:tc>
			<xsl:apply-templates select="cell-format" />
			<xsl:choose>
				<xsl:when test="count(paragraphs) > 0">
					<xsl:apply-templates select="paragraphs/item" />
					<xsl:if test="count(paragraphs/item[position() = last() and @type='Table'])>0">
						<w:p />
					</xsl:if>
				</xsl:when>
				<xsl:otherwise>
					<w:p />
					<!--w:p>
						<w:pPr>
							<xsl:apply-templates select="character-format" />
						</w:pPr>
					</w:p>-->
				</xsl:otherwise>
			</xsl:choose>
		</w:tc>
	</xsl:template>
	
</xsl:stylesheet>