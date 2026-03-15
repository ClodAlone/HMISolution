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

	<!-- Constants -->
	<xsl:variable name="twentiethOfPoint">20</xsl:variable>
	
	<xsl:template match="character-format | marker-character-format">
		<w:rPr>
			<xsl:apply-templates select="text-border"/>
			<xsl:if test="@FontSize != ''">
				<w:sz>
					<xsl:attribute name="w:val">
						<xsl:value-of select="(@FontSize)*2"/>
					</xsl:attribute>
				</w:sz>
			</xsl:if>
			<xsl:if test="@FontSizeBidi != ''">
				<w:szCs>
					<xsl:attribute name="w:val">
						<xsl:value-of select="(@FontSizeBidi)*2"/>
					</xsl:attribute>
				</w:szCs>
			</xsl:if>
			<xsl:if test="@FontName != ''">
				<w:rFonts>
					<xsl:attribute name="w:ascii">
						<xsl:value-of select="@FontName"/>
					</xsl:attribute>
					<xsl:attribute name="w:hAnsi">
						<xsl:value-of select="@FontName"/>
					</xsl:attribute>
					<xsl:attribute name="w:cs">
						<xsl:value-of select="@FontName"/>
					</xsl:attribute>
				</w:rFonts>
			</xsl:if>
			<xsl:if test="@Italic = 'true'">
				<w:i />
			</xsl:if>
			<xsl:if test="@ItalicComplex != ''">
				<w:iCs />
			</xsl:if>
			<xsl:if test="@Bold != ''">
				<w:b>
					<xsl:attribute name="w:val">
						<xsl:value-of select="@Bold" />
					</xsl:attribute>
				</w:b>
			</xsl:if>
			<xsl:if test="@BoldComplex != ''">
				<w:bCs />
			</xsl:if>
			<xsl:if test="@Underline != ''">
				<w:u>
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="@Underline = '1'">single</xsl:when>
							<xsl:when test="@Underline = '2'">words</xsl:when>
							<xsl:when test="@Underline = '3'">double</xsl:when>
							<xsl:when test="@Underline = '4'">dotted</xsl:when>
							<xsl:when test="@Underline = '5'">dottedHeavy</xsl:when>
							<xsl:when test="@Underline = '6'">thick</xsl:when>
							<xsl:when test="@Underline = '7'">dash</xsl:when>
							<xsl:when test="@Underline = '9'">dotDash</xsl:when>
							<xsl:when test="@Underline = '10'">dotDotDash</xsl:when>
							<xsl:when test="@Underline = '11'">wave</xsl:when>
							<xsl:when test="@Underline = '39'">dashLong</xsl:when>
							<xsl:when test="@Underline = '20'">dottedHeavy</xsl:when>
							<xsl:when test="@Underline = '23'">dashedHeavy</xsl:when>
							<xsl:when test="@Underline = '55'">dashLongHeavy</xsl:when>
							<xsl:when test="@Underline = '25'">dashDotHeavy</xsl:when>
							<xsl:when test="@Underline = '26'">dashDotDotHeavy</xsl:when>
							<xsl:when test="@Underline = '27'">wavyHeavy</xsl:when>
							<xsl:when test="@Underline = '43'">wavyDouble</xsl:when>
							<xsl:otherwise>
								<w:u w:val="none" />
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
				</w:u>
			</xsl:if>
			<xsl:if test="@TextColor != ''">
				<w:color>
					<xsl:attribute name="w:val">
						<xsl:value-of select="substring(@TextColor, 4)"/>
					</xsl:attribute>
				</w:color>
			</xsl:if>
			<xsl:if test="@Strike = 'true'">
				<w:strike />
			</xsl:if>
			<xsl:if test="@Shadow = 'true'">
				<w:shadow />
			</xsl:if>
			<xsl:if test="@HighlightColor != ''">
				<w:highlight>
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="substring(@HighlightColor, 4) = '000000'">black</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = '0000FF'">blue</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = '00FFFF'">cyan</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = '008000'">green</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = 'FF00FF'">magenta</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = 'FF0000'">red</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = 'D3D3D3'">lightGray</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = 'FFFF00'">yellow</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = '00008B'">darkBlue</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = '008B8B'">darkCyan</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = '006400'">darkGreen</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = '8B008B'">darkMagenta</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = '8B0000'">darkRed</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = '808080'">darkGray</xsl:when>
							<xsl:when test="substring(@HighlightColor, 4) = 'FFD700'">darkYellow</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</w:highlight>
			</xsl:if>
			<xsl:if test="@SubSuperScript != ''">
				<xsl:choose>
					<xsl:when test="@SubSuperScript = 'SubScript'">
						<w:vertAlign w:val="subscript" />
					</xsl:when>
					<xsl:when test="@SubSuperScript = 'SuperScript'">
						<w:vertAlign w:val="superscript" />
					</xsl:when>
				</xsl:choose>
			</xsl:if>
			<xsl:if test="@Emboss = 'true'">
				<w:emboss />
			</xsl:if>
			<xsl:if test="@Engrave = 'true'">
				<w:imprint />
			</xsl:if>
			<xsl:if test="@Hidden = 'true'">
				<w:vanish />
			</xsl:if>
			<xsl:if test="@DoubleStrike = 'true'">
				<w:dstrike />
			</xsl:if>
			<xsl:if test="@AllCaps = 'true'">
				<w:caps />
			</xsl:if>
			<xsl:if test="@SmallCaps = 'true'">
				<w:smallCaps />
			</xsl:if>
			<xsl:if test="@Position != ''">
				<w:position>
					<xsl:attribute name="w:val">
						<xsl:value-of select="(@Position)*$twentiethOfPoint"/>
					</xsl:attribute>
				</w:position>
			</xsl:if>
			<xsl:if test="@LineSpacing != ''">
				<w:spacing>
					<xsl:attribute name="w:val">
						<xsl:value-of select="(@LineSpacing)*$twentiethOfPoint"/>
					</xsl:attribute>
				</w:spacing>
			</xsl:if>
			<xsl:if test="@TextBackgroundColor != ''">
				<w:shd w:val="clear" w:color="auto" >
					<xsl:attribute name="w:fill">
						<xsl:value-of select="substring(@TextBackgroundColor,4)"/>
					</xsl:attribute>
				</w:shd>
			</xsl:if>
			<xsl:if test="@NoProof != ''">
				<w:noProof />
			</xsl:if>
			<xsl:if test="@Outline != ''">
				<w:outline />
			</xsl:if>
			<xsl:if test="@Bidi = 'true'">
				<w:rtl />
			</xsl:if>
			<xsl:if test="@CharStyleName != ''">
				<w:rStyle>
					<xsl:attribute name="w:val">
						<xsl:value-of select="translate(@CharStyleName,' ','')"/>
					</xsl:attribute>
				</w:rStyle>
			</xsl:if>
			<w:lang w:val="en-US" />
		</w:rPr>
	</xsl:template>
	<xsl:template match="paragraph-format">
		<xsl:if test="../list-format/@Name != ''">
			<xsl:apply-templates select="../list-format" />
		</xsl:if>
		<xsl:if test="count(borders) > 0">
			<w:pBdr>
				<xsl:apply-templates select="borders"/>
			</w:pBdr>
		</xsl:if>
		<xsl:if test="../style/@ref != ''">			
			<w:pStyle>
				<xsl:attribute name="w:val">
					<xsl:value-of select="../style/@ref" />
				</xsl:attribute>
			</w:pStyle>
		</xsl:if>		
		<xsl:apply-templates select="Tabs"/>
		<xsl:apply-templates select="../character-format"/>
		<xsl:if test="@PageBreakBefore = 'true'">
			<w:pageBreakBefore />
		</xsl:if>
		<xsl:if test="@Keep = 'true'">
			<w:keepLines />
		</xsl:if>
		<xsl:if test="@KeepFollow = 'true'">
			<w:keepNext />
		</xsl:if>
		<!-- Paragraph alignment -->
		<xsl:if test="count(@Bidi)=0">
			<xsl:choose>
				<xsl:when test="@HrAlignment = 'Left'">
					<w:jc w:val="left" />
				</xsl:when>
				<xsl:when test="@HrAlignment = 'Center'">
					<w:jc w:val="center" />
				</xsl:when>
				<xsl:when test="@HrAlignment = 'Right'">
					<w:jc w:val="right" />
				</xsl:when>
				<xsl:when test="@HrAlignment = 'Justify'">
					<w:jc w:val="both" />
				</xsl:when>
			</xsl:choose>
		</xsl:if>
		<!-- Paragraph indentation -->
		<xsl:if test="(@LeftIndent != '') or (@RightIndent != '') or (@FirstLineIndent != '')">
			<w:ind>
				<xsl:if test="@LeftIndent != ''">
					<xsl:attribute name="w:left">
						<xsl:value-of select="(@LeftIndent)*$twentiethOfPoint"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="@RightIndent != ''">
					<xsl:attribute name="w:right">
						<xsl:value-of select="(@RightIndent)*$twentiethOfPoint"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="@FirstLineIndent != ''">
					<xsl:attribute name="w:firstLine">
						<xsl:value-of select="(@FirstLineIndent)*$twentiethOfPoint"/>
					</xsl:attribute>
				</xsl:if>
			</w:ind>
		</xsl:if>
		<!-- Spacing -->
		<xsl:if test="(@BeforeSpacing != '') or (@AfterSpacing != '') 
				or (@LineSpacing != '') or (@LineSpacingRule != '')">
			<w:spacing>
				<xsl:if test="@BeforeSpacing != ''">
					<xsl:attribute name="w:before">
						<xsl:value-of select="(@BeforeSpacing)*$twentiethOfPoint"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="@AfterSpacing != ''">
					<xsl:attribute name="w:after">
						<xsl:value-of select="(@AfterSpacing)*$twentiethOfPoint"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="@LineSpacing  != ''">
					<xsl:attribute name="w:line">
						<xsl:value-of select="(@LineSpacing)*$twentiethOfPoint"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="@LineSpacingRule != ''">
					<xsl:attribute name="w:lineRule">
						<xsl:choose>
							<xsl:when test="@LineSpacingRule = 'AtLeast'">atLeast</xsl:when>
							<xsl:when test="@LineSpacingRule = 'Exactly'">exact</xsl:when>
							<xsl:otherwise>auto</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
				</xsl:if>
			</w:spacing>
		</xsl:if>
		<xsl:if test="(@BackColor != '') or (@ForeColor != '') or (@Texture != '')">
			<w:shd>
				<xsl:attribute name="w:fill">
					<xsl:choose>
						<xsl:when test="@BackColor != ''">
							<xsl:value-of select="substring(@BackColor, 4)"/>
						</xsl:when>
						<xsl:otherwise>auto</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:attribute name="w:color">
					<xsl:choose>
						<xsl:when test="@ForeColor != ''">
							<xsl:value-of select="substring(@ForeColor, 4)" />
						</xsl:when>
						<xsl:otherwise>auto</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:attribute name="w:val">
					<xsl:call-template name="TextureStyle">
						<xsl:with-param name="texture">
							<xsl:value-of select="@Texture" />
						</xsl:with-param>
					</xsl:call-template>
				</xsl:attribute>
			</w:shd>
		</xsl:if>
		<xsl:if test="@Bidi = 'true'">
			<w:bidi />
		</xsl:if>
		<xsl:if test="@WidowControl = 'false'">
			<w:widowControl>
				<xsl:attribute name="w:val">false</xsl:attribute>
			</w:widowControl>
		</xsl:if>
	</xsl:template>
	<xsl:template match="list-format">
		<w:numPr>
			<w:ilvl>
				<xsl:attribute name="w:val">
					<xsl:value-of select="@LevelNumber" />
				</xsl:attribute>
			</w:ilvl>
			<w:numId>
				<xsl:attribute name="w:val">
					<xsl:value-of select="@id" />
				</xsl:attribute>
			</w:numId>
		</w:numPr>
	</xsl:template>
	<xsl:template match="page-setup">
		<w:pgSz>
			<xsl:attribute name="w:w">
				<xsl:value-of select="(@PageWidth)*$twentiethOfPoint"/>
			</xsl:attribute>
			<xsl:attribute name="w:h">
				<xsl:value-of select="(@PageHeight)*$twentiethOfPoint"/>
			</xsl:attribute>
			<xsl:if test="@Orientation != ''">
				<xsl:attribute name="w:orient">
					<xsl:choose>
						<xsl:when test="@Orientation = 'Landscape'">landscape</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</xsl:if>
		</w:pgSz>
		<w:pgMar>
			<xsl:attribute name="w:top">
				<xsl:value-of select="(@TopMargin)*$twentiethOfPoint"/>
			</xsl:attribute>
			<xsl:attribute name="w:right">
				<xsl:value-of select="(@RightMargin)*$twentiethOfPoint"/>
			</xsl:attribute>
			<xsl:attribute name="w:bottom">
				<xsl:value-of select="(@BottomMargin)*$twentiethOfPoint"/>
			</xsl:attribute>
			<xsl:attribute name="w:left">
				<xsl:value-of select="(@LeftMargin)*$twentiethOfPoint"/>
			</xsl:attribute>
			<xsl:if test="@FooterDistance != ''">
				<xsl:attribute name="w:footer">
					<xsl:value-of select="(@FooterDistance)*$twentiethOfPoint"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@HeaderDistance != ''">
				<xsl:attribute name="w:header">
					<xsl:value-of select="(@HeaderDistance)*$twentiethOfPoint"/>
				</xsl:attribute>
			</xsl:if>
		</w:pgMar>
		<w:pgBorders>
			<xsl:if test="@PageSetupBorderOffsetFrom != ''">
				<xsl:attribute name="w:offsetFrom">
					<xsl:choose>
						<xsl:when test="@PageSetupBorderOffsetFrom = 'Text'">text</xsl:when>
						<xsl:when test="@PageSetupBorderOffsetFrom = 'PageEdge'">page</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</xsl:if>
			<xsl:apply-templates select="borders" />
		</w:pgBorders>
		<xsl:if test="@DifferentFirstPage = 'true'">
			<w:titlePg />
		</xsl:if>
		<xsl:if test="@PageSetupLineNumStep != ''">
			<w:lnNumType>
				<xsl:if test="@PageSetupLineNumStartValue != ''">
					<xsl:attribute name="w:start">
						<xsl:value-of select="(@PageSetupLineNumStartValue)-1"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:attribute name="w:countBy">
					<xsl:value-of select="@PageSetupLineNumStep" />
				</xsl:attribute>
				<xsl:if test="@PageSetupLineNumDistance != ''">
					<xsl:attribute name="w:distance">
						<xsl:value-of select="(@PageSetupLineNumDistance)*$twentiethOfPoint"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:attribute name="w:restart">
					<xsl:choose>
						<xsl:when test="@PageSetupLineNumMode = 'RestartPage'">newPage</xsl:when>
						<xsl:when test="@PageSetupLineNumMode = 'RestartSection'">newSection</xsl:when>
						<xsl:when test="@PageSetupLineNumMode = 'Continuous'">continuous</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</w:lnNumType>
		</xsl:if>
		<xsl:if test="@Alignment != ''">
			<w:vAlign>
				<xsl:attribute name="w:val">
					<xsl:choose>
						<xsl:when test="@Alignment = 'Top'">top</xsl:when>
						<xsl:when test="@Alignment = 'Middle'">center</xsl:when>
						<xsl:when test="@Alignment = 'Justified'">both</xsl:when>
						<xsl:when test="@Alignment = 'Bottom'">bottom</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</w:vAlign>
		</xsl:if>
	</xsl:template>
	<!-- Tabs -->
	<xsl:template match="Tabs">
		<w:tabs>
			<xsl:apply-templates select="Tab"/>
		</w:tabs>
	</xsl:template>
	<xsl:template match="Tab">
		<xsl:if test="@Leader != 'NoLeader'">
			<w:tab>
				<xsl:if test="@Position != ''">
					<xsl:attribute name="w:pos">
						<xsl:value-of select="(@Position)*$twentiethOfPoint"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="@Justification != ''">
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="@Justification = 'Left'">left</xsl:when>
							<xsl:when test="@Justification = 'Centered'">center</xsl:when>
							<xsl:when test="@Justification = 'Right'">right</xsl:when>
							<xsl:when test="@Justification = 'Decimal'">decimal</xsl:when>
							<xsl:when test="@Justification = 'Bar'">bar</xsl:when>
							<xsl:when test="@Justification = 'List'">num</xsl:when>
							<xsl:otherwise>clear</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="@Leader != ''">
					<xsl:attribute name="w:leader">
						<xsl:choose>
							<xsl:when test="@Leader = 'Dotted'">dot</xsl:when>
							<xsl:when test="@Leader = 'Hyphenated'">hyphen</xsl:when>
							<xsl:when test="@Leader = 'Single'">underscore</xsl:when>
							<xsl:when test="@Leader = 'Heavy'">heavy</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</xsl:if>
			</w:tab>
		</xsl:if>
	</xsl:template>
	<!-- Texture -->
	<xsl:template name="TextureStyle">
		<xsl:param name="texture"></xsl:param>

		<xsl:choose>
			<xsl:when test="$texture = 'Texture10Percent'">pct10</xsl:when>
			<xsl:when test="$texture = 'Texture12Pt5Percent'">pct12</xsl:when>
			<xsl:when test="$texture = 'Texture15Percent'">pct15</xsl:when>
			<xsl:when test="$texture = 'Texture17Pt5Percent'">pct15</xsl:when>
			<xsl:when test="$texture = 'Texture20Percent'">pct20</xsl:when>
			<xsl:when test="$texture = 'Texture25Percent'">pct25</xsl:when>
			<xsl:when test="$texture = 'Texture27Pt5Percent'">pct25</xsl:when>
			<xsl:when test="$texture = 'Texture2Pt5Percent'">pct5</xsl:when>
			<xsl:when test="$texture = 'Texture30Percent'">pct30</xsl:when>
			<xsl:when test="$texture = 'Texture32Pt5Percent'">pct30</xsl:when>
			<xsl:when test="$texture = 'Texture35Percent'">pct35</xsl:when>
			<xsl:when test="$texture = 'Texture37Pt5Percent'">pct37</xsl:when>
			<xsl:when test="$texture = 'Texture40Percent'">pct40</xsl:when>
			<xsl:when test="$texture = 'Texture42Pt5Percent'">pct40</xsl:when>
			<xsl:when test="$texture = 'Texture45Percent'">pct45</xsl:when>
			<xsl:when test="$texture = 'Texture47Pt5Percent'">pct45</xsl:when>
			<xsl:when test="$texture = 'Texture50Percent'">pct50</xsl:when>
			<xsl:when test="$texture = 'Texture52Pt5Percent'">pct50</xsl:when>
			<xsl:when test="$texture = 'Texture55Percent'">pct55</xsl:when>
			<xsl:when test="$texture = 'Texture57Pt5Percent'">pct55</xsl:when>
			<xsl:when test="$texture = 'Texture5Percent'">pct5</xsl:when>
			<xsl:when test="$texture = 'Texture60Percent'">pct60</xsl:when>
			<xsl:when test="$texture = 'Texture62Pt5Percent'">pct62</xsl:when>
			<xsl:when test="$texture = 'Texture65Percent'">pct65</xsl:when>
			<xsl:when test="$texture = 'Texture67Pt5Percent'">pct65</xsl:when>
			<xsl:when test="$texture = 'Texture70Percent'">pct70</xsl:when>
			<xsl:when test="$texture = 'Texture72Pt5Percent'">pct70</xsl:when>
			<xsl:when test="$texture = 'Texture75Percent'">pct75</xsl:when>
			<xsl:when test="$texture = 'Texture77Pt5Percent'">pct75</xsl:when>
			<xsl:when test="$texture = 'Texture7Pt5Percent'">pct5</xsl:when>
			<xsl:when test="$texture = 'Texture80Percent'">pct80</xsl:when>
			<xsl:when test="$texture = 'Texture82Pt5Percent'">pct80</xsl:when>
			<xsl:when test="$texture = 'Texture85Percent'">pct85</xsl:when>
			<xsl:when test="$texture = 'Texture87Pt5Percent'">pct87</xsl:when>
			<xsl:when test="$texture = 'Texture90Percent'">pct90</xsl:when>
			<xsl:when test="$texture = 'Texture92Pt5Percent'">pct90</xsl:when>
			<xsl:when test="$texture = 'Texture95Percent'">pct95</xsl:when>
			<xsl:when test="$texture = 'Texture97Pt5Percent'">pct95</xsl:when>
			<xsl:when test="$texture = 'TextureCross'">thinHorzCross</xsl:when>
			<xsl:when test="$texture = 'TextureDarkCross'">horzCross</xsl:when>
			<xsl:when test="$texture = 'TextureDarkDiagonalCross'">thinDiagCross</xsl:when>
			<xsl:when test="$texture = 'TextureDarkDiagonalDown'">diagStripe</xsl:when>
			<xsl:when test="$texture = 'TextureDarkDiagonalUp'">diagStripe</xsl:when>
			<xsl:when test="$texture = 'TextureDarkHorizontal'">horzStripe</xsl:when>
			<xsl:when test="$texture = 'TextureDarkVertical'">vertStripe</xsl:when>
			<xsl:when test="$texture = 'TextureDiagonalCross'">diagCross</xsl:when>
			<xsl:when test="$texture = 'TextureDiagonalDown'">thinDiagStripe</xsl:when>
			<xsl:when test="$texture = 'TextureDiagonalUp'">thinDiagStripe</xsl:when>
			<xsl:when test="$texture = 'TextureHorizontal'">thinHorzStripe</xsl:when>			
			<xsl:when test="$texture = 'TextureSolid'">solid</xsl:when>
			<xsl:when test="$texture = 'TextureVertical'">thinVertStripe</xsl:when>
			<xsl:otherwise>clear</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
