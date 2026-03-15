<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:w="http://schemas.microsoft.com/office/word/2003/wordml"
  xmlns:v="urn:schemas-microsoft-com:vml"
	xmlns:w10="urn:schemas-microsoft-com:office:word"
	xmlns:SL="http://schemas.microsoft.com/schemaLibrary/2003/core"
	xmlns:aml="http://schemas.microsoft.com/aml/2001/core"
	xmlns:wx="http://schemas.microsoft.com/office/word/2003/auxHint"
	xmlns:o="urn:schemas-microsoft-com:office:office"
	xmlns:dt="uuid:C2F41010-65B3-11d1-A29F-00AA00C14882"
  xmlns:fo="http://www.w3.org/1999/XSL/Format"
  xmlns:axsl="http://www.w3.org/1999/XSL/TransformAlias">

	<xsl:output method="xml" standalone="yes" />

	<xsl:template match="/w:wordDocument">
		<DLS>
			<xsl:call-template name="DEBUG"/>
			<xsl:attribute name="ProtectionType">
				<xsl:choose>
					<xsl:when test="/w:wordDocument/w:docPr/w:documentProtection/@w:edit = 'comments'">
						<xsl:if test="/w:wordDocument/w:docPr/w:documentProtection/@w:enforcement = 'on'">AllowOnlyComments</xsl:if>
					</xsl:when>
					<xsl:otherwise>NoProtection</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<xsl:apply-templates select="w:styles"/>
			<xsl:apply-templates select="w:docPr"/>
			<xsl:apply-templates select="w:body"/>
			<xsl:apply-templates select="o:DocumentProperties"/>
		</DLS>
	</xsl:template>

	<xsl:template match="w:docPr">
		<view-setup>
			<xsl:attribute name="ZoomPercent">
				<xsl:value-of select="w:zoom/@w:percent"/>
			</xsl:attribute>
			<xsl:if test="w:view/@w:val != ''">
				<xsl:if test="w:view/@w:val != 'print'">
					<xsl:attribute name="ViewType">
						<xsl:choose>
							<xsl:when test="w:view/@w:val = 'normal'">NormalLayout</xsl:when>
							<xsl:when test="w:view/@w:val = 'web'">WebLayout</xsl:when>
							<xsl:when test="w:view/@w:val = 'master-pages'">OutlineLayout</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</xsl:if>
			</xsl:if></view-setup>
	</xsl:template>
	<xsl:template match="w:body">
		<sections>
			<xsl:apply-templates select="wx:sect"/>
		</sections>
	</xsl:template>
	<xsl:template match="wx:sect">
		<xsl:variable name="pagebreak">
			<xsl:apply-templates select="w:p|w:tbl">
				<xsl:with-param name="prSect">attr</xsl:with-param>
			</xsl:apply-templates>
		</xsl:variable>
		<section>
			<xsl:attribute name="id">
				<xsl:number value="position()-1"/>
			</xsl:attribute>
			<xsl:attribute name="BreakCode">
				<xsl:choose>
					<xsl:when test="$pagebreak = 'NoBreak'">NoBreak</xsl:when>
					<xsl:otherwise>NewPage</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<body>
				<paragraphs>
					<xsl:apply-templates select="w:p|w:tbl">
						<xsl:with-param name="prSect">false</xsl:with-param>
					</xsl:apply-templates>
				</paragraphs>
			</body>
			<xsl:choose>
				<xsl:when test="count(w:sectPr) = 1">
					<xsl:apply-templates select="w:sectPr"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates select="w:p|w:tbl">
						<xsl:with-param name="prSect">true</xsl:with-param>
					</xsl:apply-templates>
				</xsl:otherwise>
			</xsl:choose>
		</section>
	</xsl:template>
	<xsl:template match="w:p|w:tbl">
		<xsl:param name="prSect"></xsl:param>
		<xsl:variable name="id">
			<xsl:number value="position()-1"/>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="$prSect = 'false'">
				<xsl:choose>
					<xsl:when test="w:pPr/w:sectPr != ''">
					</xsl:when>
					<xsl:otherwise>
						<item>
							<xsl:attribute name="id">
								<xsl:value-of select="$id"/>
							</xsl:attribute>
							<xsl:choose>
								<xsl:when test="count(w:tr) > 0">
									<xsl:attribute name="type">Table</xsl:attribute>
									<rows>
										<xsl:apply-templates select="w:tr"/>
									</rows>
								</xsl:when>
								<xsl:otherwise>
									<xsl:attribute name="type">Paragraph</xsl:attribute>
									<items>
										<xsl:apply-templates select="w:r|w:pict|aml:annotation"/>
									</items>
								</xsl:otherwise>
							</xsl:choose>
							<paragraph-format>
								<xsl:variable name="br">
									<xsl:value-of select="following-sibling::w:p[1]/w:pPr/w:br/@w:type"/>
								</xsl:variable>
								<xsl:if test="following-sibling::w:p[1]/w:pPr/w:br/@w:type = 'page'">
									<xsl:attribute name="PageBreakAfter">true</xsl:attribute>
								</xsl:if>
								<xsl:if test="count(following-sibling::w:p[1]/w:pPr/w:pageBreakBefore) > 0">
									<xsl:attribute name="PageBreakAfter">true</xsl:attribute>
								</xsl:if>
								<xsl:apply-templates select="w:pPr"/>
							</paragraph-format>
							<xsl:apply-templates select="w:pPr">
								<xsl:with-param name="style">true</xsl:with-param>
							</xsl:apply-templates>
						</item>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$prSect = 'true'">
				<xsl:apply-templates select="w:pPr">
					<xsl:with-param name="prSect">true</xsl:with-param>
				</xsl:apply-templates>
			</xsl:when>
			<xsl:when test="$prSect = 'attr'">
				<xsl:if test="w:pPr/w:sectPr/w:type/@w:val = 'continuous'">NoBreak</xsl:if>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="w:r|w:pict|aml:annotation">
		<item>
			<xsl:choose>
				<xsl:when test="count(v:shape) > 0">
					<xsl:attribute name="type">Picture</xsl:attribute>
					<xsl:variable name="width">
						<xsl:value-of select="translate(substring(substring-before(v:shape/@style,';'),7),',','.')"/>
					</xsl:variable>
					<xsl:variable name="height">
						<xsl:value-of select="translate(substring(substring-after(v:shape/@style,';'),8),',','.')"/>
					</xsl:variable>
					<xsl:variable name="widthscale">
						<xsl:value-of select="($width div (v:shape/@imgWidth))*100"/>
					</xsl:variable>
					<xsl:variable name="heightscale">
						<xsl:value-of select="($height div (v:shape/@imgHeight))*100"/>
					</xsl:variable>
					<xsl:variable name="metafile">
						<xsl:value-of select="substring(substring-before(substring-after(v:shape/v:imagedata/@src, '//'), '.png'), 3)"/>
					</xsl:variable>
					<xsl:attribute name="width">
						<xsl:value-of select="($width)"/>
					</xsl:attribute>
					<xsl:attribute name="height">
						<xsl:value-of select="($height)"/>
					</xsl:attribute>
					<xsl:attribute name="WidthScale">
						<xsl:value-of select="$widthscale"/>
					</xsl:attribute>
					<xsl:attribute name="HeightScale">
						<xsl:value-of select="$heightscale"/>
					</xsl:attribute>
					<xsl:attribute name="IsMetafile">
						<xsl:choose>
							<xsl:when test="$metafile = 'm'">true</xsl:when>
							<xsl:otherwise>false</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<image>
						<xsl:value-of select="w:binData"/>
					</image>
				</xsl:when>
				<xsl:when test="@w:type='Word.Bookmark.Start'">
					<xsl:attribute name="type">BookmarkStart</xsl:attribute>
					<xsl:attribute name="BookmarkName">
						<xsl:value-of select="@w:name"/>
					</xsl:attribute>
				</xsl:when>
				<xsl:when test="@w:type='Word.Bookmark.End'">
					<xsl:attribute name="type">BookmarkEnd</xsl:attribute>
					<xsl:attribute name="BookmarkName">
						<xsl:value-of select="@w:name"/>
					</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates select="w:rPr"/>
					<xsl:if test="w:t != ''">
						<xsl:apply-templates select="w:t"/>
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
		</item>
	</xsl:template>
	<xsl:template match="w:t">
		<xsl:if test=". != ''">
			<text><xsl:value-of select="."/></text>
		</xsl:if>
	</xsl:template>
	<xsl:template match="w:rPr">
		<character-format>
			<xsl:if test="w:sz/@w:val != ''">
				<xsl:attribute name="FontSize">
					<xsl:value-of select="(w:sz/@w:val)*0.5"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="w:sz-cs/@w:val != ''">
				<xsl:attribute name="FontSizeBidi">
					<xsl:value-of select="(w:sz-cs/@w:val)*0.5"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="w:rFonts/@w:ascii != ''">
				<xsl:attribute name="FontName">
					<xsl:value-of select="w:rFonts/@w:ascii"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:i) = 1">
				<xsl:attribute name="Italic">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:b) = 1">
				<xsl:attribute name="Bold">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:u) = 1">
				<xsl:attribute name="Underline">					
					<xsl:choose>
						<xsl:when test="w:u/@w:val = 'single'">1</xsl:when>
						<xsl:when test="w:u/@w:val = 'words'">2</xsl:when>
						<xsl:when test="w:u/@w:val = 'double'">3</xsl:when>
						<xsl:when test="w:u/@w:val = 'dotted'">4</xsl:when>
						<xsl:when test="w:u/@w:val = 'thick'">6</xsl:when>
						<xsl:when test="w:u/@w:val = 'dash'">7</xsl:when>
						<xsl:when test="w:u/@w:val = 'dot-dash'">9</xsl:when>
						<xsl:otherwise>0</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:color) = 1">
				<xsl:attribute name="TextColor">
					<xsl:text>#FF</xsl:text>
					<xsl:value-of select="w:color/@w:val"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:strike) = 1">
				<xsl:attribute name="Strike">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:shadow) = 1">
				<xsl:attribute name="Shadow">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:highlight) = 1">
				<xsl:attribute name="HighlightColor">
					<xsl:choose>
						<xsl:when test="w:highlight/@w:val = 'black'">#FF000000</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'blue'">#FF0000FF</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'cyan'">#FF00FFFF</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'green'">#FF008000</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'magenta'">#FFFF00FF</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'red'">#FFFF0000</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'light-gray'">#FFD3D3D3</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'yellow'">#FFFFFF00</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'dark-blue'">#FF00008B</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'dark-cyan'">#FF008B8B</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'dark-green'">#FF006400</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'dark-magenta'">#FF8B008B</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'dark-red'">#FF8B0000</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'dark-gray'">#FFA9A9A9</xsl:when>
						<xsl:when test="w:highlight/@w:val = 'dark-yellow'">#FFFFD700</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:vertAlign) = 1">
				<xsl:attribute name="SubSuperScript">
					<xsl:choose>
						<xsl:when test="w:vertAlign/@w:val='subscript'">SubScript</xsl:when>
						<xsl:when test="w:vertAlign/@w:val='superscript'">SuperScript</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:emboss) = 1">
				<xsl:attribute name="Emboss">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:imprint) = 1">
				<xsl:attribute name="Engrave">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:vanish) = 1">
				<xsl:attribute name="Hidden">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:dstrike) = 1">
				<xsl:attribute name="DoubleStrike">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:caps) = 1">
				<xsl:attribute name="AllCaps">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:smallCaps) = 1">
				<xsl:attribute name="SmallCaps">true</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:position) > 0">
				<xsl:attribute name="Position">
					<xsl:value-of select="(w:position/@w:val)*0.05"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:spacing) > 0">
				<xsl:attribute name="LineSpacing">
					<xsl:value-of select="(w:spacing/@w:val)*0.05"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:shd) = 1">
				<xsl:attribute name="TextBackgroundColor">
					<xsl:text>#FF</xsl:text>
					<xsl:value-of select="w:shd/@w:fill"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:bidi) = 1">
				<xsl:attribute name="Bidi">true</xsl:attribute>
			</xsl:if>
		</character-format>
	</xsl:template>
	<xsl:template match="w:styles">
		<styles>
			<xsl:apply-templates select="w:style"/>
		</styles>
	</xsl:template>
	<xsl:template match="w:style">
		<xsl:variable name="id">			
			<xsl:number count="w:style"/>
		</xsl:variable>
		<xsl:element name="style">
			<xsl:attribute name="id">
				<xsl:value-of select="($id)-1"/>
			</xsl:attribute>
			<xsl:attribute name="Name">
				<xsl:value-of select="@w:styleId"/>
			</xsl:attribute>
			<xsl:attribute name="StyleId">4094</xsl:attribute>
			<xsl:attribute name="type">ParagraphStyle</xsl:attribute>
			<xsl:if test ="count(w:basedOn) > 0">
				<xsl:element name="base">
					<xsl:attribute name="ref">
						<xsl:call-template name="styleid">
							<xsl:with-param name="name">
								<xsl:value-of select="w:basedOn/@w:val"/>
							</xsl:with-param>
							<xsl:with-param name="type">
								<xsl:value-of select="@w:type"/>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:attribute>
					<xsl:value-of select="w:basedOn/@w:val"/>
				</xsl:element>
			</xsl:if>
			<xsl:apply-templates select="w:rPr"/>
		</xsl:element>		
	</xsl:template>
	<xsl:template name="DEBUG">
		<!--DEBUG>
			<xsl:copy-of select="/w:wordDocument/w:styles/w:style[position() = 3]/preceding-sibling::*"/>
		</DEBUG-->
	</xsl:template>
	<xsl:template name="styleid">
		<xsl:param name="name"></xsl:param>
		<xsl:param name="type"></xsl:param>
		<xsl:value-of select="count(/w:wordDocument/w:styles/w:style[@w:styleId=$name and @w:type=$type]/preceding-sibling::*)" />
	</xsl:template>
	<xsl:template match="w:pPr">
		<xsl:param name="prSect">false</xsl:param>
		<xsl:param name="style">false</xsl:param>

		<xsl:if test="$prSect = 'false'">
			<xsl:choose>
				<xsl:when test="$style = 'true'">
					<xsl:if test="w:pStyle/@w:val != ''">
						<xsl:variable name="ref">
							<xsl:number value="w:pStyle/@w:val"/>
						</xsl:variable>
						<style>
							<xsl:attribute name="ref">
								<xsl:call-template name="styleid">
									<xsl:with-param name="name">
										<xsl:value-of select="$ref"/>
									</xsl:with-param>
									<xsl:with-param name="type">paragraph</xsl:with-param>
								</xsl:call-template>
							</xsl:attribute>
						</style>
					</xsl:if>
				</xsl:when>
				<xsl:when test="$style = 'false'">
					<xsl:if test="count(w:keepLines) > 0">
						<xsl:attribute name="Keep">true</xsl:attribute>
					</xsl:if>
					<xsl:if test="count(w:keepNext) > 0">
						<xsl:attribute name="KeepFollow">true</xsl:attribute>
					</xsl:if>
					<xsl:if test="count(w:jc) > 0">
						<xsl:attribute name="HrAlignment">
							<xsl:choose>
								<xsl:when test="w:jc/@w:val = 'center'">Center</xsl:when>
								<xsl:when test="w:jc/@w:val = 'right'">Right</xsl:when>
								<xsl:when test="w:jc/@w:val = 'both'">Justify</xsl:when>
							</xsl:choose>
						</xsl:attribute>
					</xsl:if>
					<xsl:if test="count(w:ind) > 0">
						<xsl:if test="w:ind/@w:left != ''">
							<xsl:attribute name="LeftIndent">
								<xsl:value-of select="(w:ind/@w:left)*0.05"/>
							</xsl:attribute>
						</xsl:if>
						<xsl:if test="w:ind/@w:right != ''">
							<xsl:attribute name="RightIndent">
								<xsl:value-of select="(w:ind/@w:right)*0.05"/>
							</xsl:attribute>
						</xsl:if>
						<xsl:if test="w:ind/@w:first-line != ''">
							<xsl:attribute name="FirstLineIndent">
								<xsl:value-of select="(w:ind/@w:first-line)*0.05"/>
							</xsl:attribute>
						</xsl:if>
					</xsl:if>
					<xsl:if test="count(w:spacing) > 0">
						<xsl:if test="w:spacing/@w:before != ''">
							<xsl:attribute name="BeforeSpacing">
								<xsl:value-of select="(w:spacing/@w:before)*0.05"/>
							</xsl:attribute>
						</xsl:if>
						<xsl:if test="w:spacing/@w:after != ''">
							<xsl:attribute name="AfterSpacing">
								<xsl:value-of select="(w:spacing/@w:after)*0.05"/>
							</xsl:attribute>
						</xsl:if>
					</xsl:if>
					<xsl:if test="count(w:shd) > 0">
						<xsl:attribute name="BackColor">
							<xsl:text>#FF</xsl:text>
							<xsl:value-of select="w:shd/@w:fill"/>
						</xsl:attribute>
					</xsl:if>
					<xsl:if test="count(w:bidi) > 0">
						<xsl:attribute name="Bidi">true</xsl:attribute>
					</xsl:if>
					<xsl:if test="count(w:pBdr) > 0">
						<xsl:apply-templates select="w:pBdr"/>
					</xsl:if>
				</xsl:when>
			</xsl:choose>
		</xsl:if>
		<xsl:if test="$prSect = 'true'">
			<xsl:apply-templates select="w:sectPr"/>
		</xsl:if>
	</xsl:template>
	<xsl:template match="w:cols">
		<columns>
			<xsl:apply-templates select="w:col"/>
		</columns>
	</xsl:template>
	<xsl:template match="w:col">
		<column>
			<xsl:if test="@w:w != ''">
				<xsl:attribute name="Width">
					<xsl:value-of select="(@w:w)*0.05"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@w:space != ''">
				<xsl:attribute name="Spacing">
					<xsl:value-of select="(@w:space)*0.05"/>
				</xsl:attribute>
			</xsl:if>
		</column>
	</xsl:template>
	<xsl:template match="w:sectPr">
		<page-setup>
			<xsl:attribute name="PageHeight">
				<xsl:value-of select="(w:pgSz/@w:h)*0.05"/>
			</xsl:attribute>
			<xsl:attribute name="PageWidth">
				<xsl:value-of select="(w:pgSz/@w:w)*0.05"/>
			</xsl:attribute>
			<xsl:if test="w:pgSz/@w:orient != ''">
				<xsl:attribute name="Orientation">
					<xsl:choose>
						<xsl:when test="w:pgSz/@w:orient = 'landscape'">Landscape</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</xsl:if>
			<xsl:attribute name="TopMargin">
				<xsl:value-of select="(w:pgMar/@w:top)*0.05"/>
			</xsl:attribute>
			<xsl:attribute name="LeftMargin">
				<xsl:value-of select="(w:pgMar/@w:left)*0.05"/>
			</xsl:attribute>
			<xsl:attribute name="BottomMargin">
				<xsl:value-of select="(w:pgMar/@w:bottom)*0.05"/>
			</xsl:attribute>
			<xsl:attribute name="RightMargin">
				<xsl:value-of select="(w:pgMar/@w:right)*0.05"/>
			</xsl:attribute>
			<xsl:if test="w:pgMar/@w:footer != ''">
				<xsl:attribute name="FooterDistance">
					<xsl:value-of select="(w:pgMar/@w:footer)*0.05"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="w:pgMar/@w:header != ''">
				<xsl:attribute name="HeaderDistance">
					<xsl:value-of select="(w:pgMar/@w:header)*0.05"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:titlePg) > 0">
				<xsl:attribute name="DifferentFirstPage">true</xsl:attribute>
			</xsl:if>
			<xsl:attribute name="DifferentOddEvenPage">true</xsl:attribute>
			<xsl:if test="count(w:lnNumType) > 0">
				<xsl:attribute name="PageSetupLineNumStartValue">
					<xsl:value-of select="(w:lnNumType/@w:start)+1"/>
				</xsl:attribute>
				<xsl:attribute name="PageSetupLineNumDistance">
					<xsl:value-of select="(w:lnNumType/@w:distance)*0.05"/>
				</xsl:attribute>
				<xsl:attribute name="PageSetupLineNumStep">1</xsl:attribute>
				<xsl:attribute name="PageSetupLineNumMode">RestartSection</xsl:attribute>
			</xsl:if>
		</page-setup>
		<headers-footers>
			<xsl:apply-templates select="w:hdr"/>
			<xsl:apply-templates select="w:ftr"/>
		</headers-footers>
		<xsl:apply-templates select="w:cols"/>
	</xsl:template>
	<xsl:template match="w:hdr">
		<xsl:choose>
			<xsl:when test="@w:type = 'first'">
				<first-page-header>
					<paragraphs>
						<xsl:apply-templates select="w:p">
							<xsl:with-param name="prSect">false</xsl:with-param>
						</xsl:apply-templates>
					</paragraphs>
				</first-page-header>
			</xsl:when>
			<xsl:when test="@w:type = 'even'">
				<even-header>
					<paragraphs>
						<xsl:apply-templates select="w:p">
							<xsl:with-param name="prSect">false</xsl:with-param>
						</xsl:apply-templates>
					</paragraphs>
				</even-header>
			</xsl:when>
			<xsl:when test="@w:type = 'odd'">
				<odd-header>
					<paragraphs>
						<xsl:apply-templates select="w:p">
							<xsl:with-param name="prSect">false</xsl:with-param>
						</xsl:apply-templates>
					</paragraphs>
				</odd-header>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="w:ftr">
		<xsl:choose>
			<xsl:when test="@w:type = 'first'">
				<first-page-footer>
					<paragraphs>
						<xsl:apply-templates select="w:p">
							<xsl:with-param name="prSect">false</xsl:with-param>
						</xsl:apply-templates>
					</paragraphs>
				</first-page-footer>
			</xsl:when>
			<xsl:when test="@w:type = 'even'">
				<even-footer>
					<paragraphs>
						<xsl:apply-templates select="w:p">
							<xsl:with-param name="prSect">false</xsl:with-param>
						</xsl:apply-templates>
					</paragraphs>
				</even-footer>
			</xsl:when>
			<xsl:when test="@w:type = 'odd'">
				<odd-footer>
					<paragraphs>
						<xsl:apply-templates select="w:p">
							<xsl:with-param name="prSect">false</xsl:with-param>
						</xsl:apply-templates>
					</paragraphs>
				</odd-footer>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	<!-- Tables -->
	<xsl:template match="w:tr">
		<row>
			<xsl:variable name="id">
				<xsl:number value="position()-1"/>
			</xsl:variable>
			<xsl:attribute name="id">
				<xsl:value-of select="$id"/>
			</xsl:attribute>
			<xsl:apply-templates select="w:trPr"/>
			<cells>
				<xsl:apply-templates select="w:tc"/>
			</cells>
			<xsl:apply-templates select="../w:tblPr"/>
		</row>
	</xsl:template>
	<xsl:template match="w:tc">
		<cell>
			<xsl:variable name="id">
				<xsl:number value="position()-1"/>
			</xsl:variable>
			<xsl:attribute name="id">
				<xsl:value-of select="$id"/>
			</xsl:attribute>
			<xsl:attribute name="Width">
				<xsl:value-of select="(w:tcPr/w:tcW/@w:w)*0.05"/>
			</xsl:attribute>
			<xsl:apply-templates select="w:tcPr"/>
			<paragraphs>
				<xsl:apply-templates select="w:p|w:tbl">
					<xsl:with-param name="prSect">false</xsl:with-param>
				</xsl:apply-templates>
			</paragraphs>
		</cell>
	</xsl:template>
	<xsl:template match="w:tcPr">
		<cell-format>
			<xsl:attribute name="VAlignment">
				<xsl:choose>
					<xsl:when test="w:vAlign/@w:val = 'bottom'">Bottom</xsl:when>
					<xsl:when test="w:vAlign/@w:val = 'center'">Middle</xsl:when>
					<xsl:otherwise>Top</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<xsl:apply-templates select="w:tcBorders"/>
		</cell-format>
	</xsl:template>
	<xsl:template match="w:tcBorders">
		<borders>
			<xsl:choose>
				<xsl:when test="count(w:bottom) > 0">
					<xsl:apply-templates select="w:bottom">
						<xsl:with-param name="multiplier">0.125</xsl:with-param>
					</xsl:apply-templates>
				</xsl:when>
				<xsl:otherwise>
					<Bottom Color="#00000000" LineWidth="0" BorderType="None" Shadow="false" />
				</xsl:otherwise>
			</xsl:choose>
			<xsl:choose>
				<xsl:when test="count(w:left) > 0">
					<xsl:apply-templates select="w:left">
						<xsl:with-param name="multiplier">0.125</xsl:with-param>
					</xsl:apply-templates>
				</xsl:when>
				<xsl:otherwise>
					<Left Color="#00000000" LineWidth="0" BorderType="None" Shadow="false" />
				</xsl:otherwise>
			</xsl:choose>
			<xsl:choose>
				<xsl:when test="count(w:top) > 0">
					<xsl:apply-templates select="w:top">
						<xsl:with-param name="multiplier">0.125</xsl:with-param>
					</xsl:apply-templates>
				</xsl:when>
				<xsl:otherwise>
					<Top Color="#00000000" LineWidth="0" BorderType="None" Shadow="false" />
				</xsl:otherwise>
			</xsl:choose>
			<xsl:choose>
				<xsl:when test="count(w:right) > 0">
					<xsl:apply-templates select="w:right">
						<xsl:with-param name="multiplier">0.125</xsl:with-param>
					</xsl:apply-templates>
				</xsl:when>
				<xsl:otherwise>
					<Right Color="#00000000" LineWidth="0" BorderType="None" Shadow="false" />
				</xsl:otherwise>
			</xsl:choose>
		</borders>
	</xsl:template>
	<xsl:template match="w:tblBorders">
		<borders>
			<xsl:apply-templates select="w:bottom">
				<xsl:with-param name="multiplier">0.125</xsl:with-param>
			</xsl:apply-templates>
			<xsl:apply-templates select="w:left">
				<xsl:with-param name="multiplier">0.125</xsl:with-param>
			</xsl:apply-templates>
			<xsl:apply-templates select="w:top">
				<xsl:with-param name="multiplier">0.125</xsl:with-param>
			</xsl:apply-templates>
			<xsl:apply-templates select="w:right">
				<xsl:with-param name="multiplier">0.125</xsl:with-param>
			</xsl:apply-templates>
			<xsl:apply-templates select="w:insideH">
				<xsl:with-param name="multiplier">0.125</xsl:with-param>
			</xsl:apply-templates>
			<xsl:apply-templates select="w:insideV">
				<xsl:with-param name="multiplier">0.125</xsl:with-param>
			</xsl:apply-templates>
		</borders>
	</xsl:template>
	<xsl:template match="w:tblPr">
		<table-format>
			<xsl:if test="count(w:tblInd) > 0">
				<xsl:attribute name="LeftOffset">
					<xsl:value-of select="(w:tblInd/@w:w)*0.05"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:tblCellSpacing) > 0">
				<xsl:attribute name="SpacingBetweenCells">
					<xsl:value-of select="(w:tblCellSpacing/@w:w)*0.05"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="count(w:tblCellMar) > 0">
				<xsl:element name="Paddings">
					<xsl:attribute name="Top">
						<xsl:choose>
							<xsl:when test="count(w:tblCellMar/w:top) > 0">
								<xsl:value-of select="(w:tblCellMar/w:top/@w:w)*0.05"/>
							</xsl:when>
							<xsl:otherwise>0</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<xsl:attribute name="Left">
						<xsl:choose>
							<xsl:when test="count(w:tblCellMar/w:left ) > 0">
								<xsl:value-of select="(w:tblCellMar/w:left/@w:w)*0.05"/>
							</xsl:when>
							<xsl:otherwise>0</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<xsl:attribute name="Bottom">
						<xsl:choose>
							<xsl:when test="count(w:tblCellMar/w:bottom) > 0">
								<xsl:value-of select="(w:tblCellMar/w:bottom/@w:w)*0.05"/>
							</xsl:when>
							<xsl:otherwise>0</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<xsl:attribute name="Right">
						<xsl:choose>
							<xsl:when test="count(w:tblCellMar/w:right) > 0">
								<xsl:value-of select="(w:tblCellMar/w:right/@w:w)*0.05"/>
							</xsl:when>
							<xsl:otherwise>0</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
				</xsl:element>
			</xsl:if>
			<xsl:apply-templates select="w:tblBorders"/>
		</table-format>
	</xsl:template>
	<xsl:template match="w:trPr">
		<xsl:if test="count(w:trHeight) > 0">
			<xsl:attribute name="RowHeight">
				<xsl:value-of select="(w:trHeight/@w:val)*0.05"/>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>
	<!-- Borders -->
	<xsl:template name="Border">
		<xsl:param name="name"></xsl:param>
		<xsl:param name="val"></xsl:param>
		<xsl:param name="sz"></xsl:param>
		<xsl:param name="space"></xsl:param>
		<xsl:param name="color"></xsl:param>
		<xsl:param name="shadow"></xsl:param>
		<xsl:if test="$val != ''">
			<xsl:element name="{$name}">
				<xsl:if test="$val != 'nil'">
					<!-- Color  -->
					<xsl:attribute name="Color">
						<xsl:text>#FF</xsl:text>
						<xsl:value-of select="$color"/>
					</xsl:attribute>
					<!-- LineWidth     -->
					<xsl:attribute name="LineWidth">
						<xsl:value-of select="$sz" />
					</xsl:attribute>
					<!-- w:space  -->
					<xsl:if test="$space != 'NaN'">
						<xsl:attribute name="Space">
							<xsl:value-of select="$space"/>
						</xsl:attribute>
					</xsl:if>
				</xsl:if>
				<!-- w:val    -->
				<xsl:attribute name="BorderType">
					<xsl:choose>
						<xsl:when test="$val = 'triple'"					  >Triple</xsl:when>
						<xsl:when test="$val = 'dash-small-gap'"              >DashSmallGap</xsl:when>
						<xsl:when test="$val = 'single'"                      >Single</xsl:when>
						<xsl:when test="$val = 'dotted'"                      >Dot</xsl:when>
						<xsl:when test="$val = 'dot-dash'"                    >DotDash</xsl:when>
						<xsl:when test="$val = 'dashed'"                      >DashLargeGap</xsl:when>
						<xsl:when test="$val = 'dot-dot-dash'"                >DotDotDash</xsl:when>
						<xsl:when test="$val = 'double'"                      >Double</xsl:when>
						<xsl:when test="$val = 'thick-thin-small-gap'"        >ThinThinSmallGap</xsl:when>
						<xsl:when test="$val = 'thin-thick-small-gap'"        >ThinThickSmallGap</xsl:when>
						<xsl:when test="$val = 'thin-thick-thin-small-gap'"   >ThinThickThinSmallGap</xsl:when>
						<xsl:when test="$val = 'thick-thin-medium-gap'"       >ThickThinMediumGap</xsl:when>
						<xsl:when test="$val = 'thin-thick-medium-gap'"       >ThinThickMediumGap</xsl:when>
						<xsl:when test="$val = 'thin-thick-thin-medium-gap'"  >ThickThickThinMediumGap</xsl:when>
						<xsl:when test="$val = 'thick-thin-large-gap'"        >ThickThinLargeGap</xsl:when>
						<xsl:when test="$val = 'thin-thick-large-gap'"        >ThinThickLargeGap</xsl:when>
						<xsl:when test="$val = 'thin-thick-thin-large-gap'"   >ThinThickThinLargeGap</xsl:when>
						<xsl:when test="$val = 'wave'"                        >Wave</xsl:when>
						<xsl:when test="$val = 'double-wave'"                 >DoubleWave</xsl:when>
						<xsl:when test="$val = 'dash-dot-stroked'"            >DashDotStroker</xsl:when>
						<xsl:when test="$val = 'three-d-engrave'"             >Engrave3D</xsl:when>
						<xsl:when test="$val = 'three-d-emboss'"              >Emboss3D</xsl:when>
						<xsl:when test="$val = 'outset'"                      >Outset</xsl:when>
						<xsl:when test="$val = 'inset'"                       >Inset</xsl:when>
						<xsl:when test="$val = 'nil'"                         >Cleared</xsl:when>
					</xsl:choose>
				</xsl:attribute>
				<xsl:if test="$val != 'nil'">
					<!-- w:shadow -->
					<xsl:attribute name="Shadow">
						<xsl:choose>
							<xsl:when test="$shadow = 'on'">true</xsl:when>
							<xsl:otherwise>false</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
				</xsl:if>
			</xsl:element>
		</xsl:if>
	</xsl:template>
	<xsl:template match="w:top">
		<xsl:param name="multiplier">0.05</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">Top</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@w:val"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@w:sz)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@w:space)*0.05"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@w:color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@w:shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="w:left">
		<xsl:param name="multiplier">0.05</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">Left</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@w:val"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@w:sz)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@w:space)*0.05"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@w:color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@w:shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="w:bottom">
		<xsl:param name="multiplier">0.05</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">Bottom</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@w:val"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@w:sz)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@w:space)*0.05"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@w:color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@w:shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="w:right">
		<xsl:param name="multiplier">0.05</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">Right</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@w:val"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@w:sz)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@w:space)*0.05"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@w:color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@w:shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="w:insideH">
		<xsl:param name="multiplier">0.05</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">Horizontal</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@w:val"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@w:sz)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@w:space)*0.05"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@w:color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@w:shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="w:insideV">
		<xsl:param name="multiplier">0.05</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">Vertical</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@w:val"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@w:sz)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@w:space)*0.05"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@w:color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@w:shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="w:pBdr">
		<borders>
			<xsl:apply-templates select="w:bottom">
				<xsl:with-param name="multiplier">0.05</xsl:with-param>
			</xsl:apply-templates>
			<xsl:apply-templates select="w:left">
				<xsl:with-param name="multiplier">0.05</xsl:with-param>
			</xsl:apply-templates>
			<xsl:apply-templates select="w:top">
				<xsl:with-param name="multiplier">0.05</xsl:with-param>
			</xsl:apply-templates>
			<xsl:apply-templates select="w:right">
				<xsl:with-param name="multiplier">0.05</xsl:with-param>
			</xsl:apply-templates>
		</borders>
	</xsl:template>
	<!-- Properties -->
	<xsl:template match="o:DocumentProperties">
		<builtin-properties>
			<xsl:attribute name="Title">
				<xsl:value-of select="o:Title"/>
			</xsl:attribute>
			<xsl:attribute name="Subject">
				<xsl:value-of select="o:Subject"/>
			</xsl:attribute>
			<xsl:attribute name="Author">
				<xsl:value-of select="o:Author"/>
			</xsl:attribute>
			<xsl:attribute name="Keywords">
				<xsl:value-of select="o:Keywords"/>
			</xsl:attribute>
			<xsl:attribute name="Comments">
				<xsl:value-of select="o:Description"/>
			</xsl:attribute>
			<xsl:attribute name="LastAuthor">
				<xsl:value-of select="o:LastAuthor"/>
			</xsl:attribute>
			<xsl:attribute name="RevisionNumber">
				<xsl:choose>
					<xsl:when test="o:Revision != ''">
						<xsl:value-of select="o:Revision"/>
					</xsl:when>
					<xsl:otherwise>1</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<!--<xsl:attribute name="EditTime">
        <xsl:value-of select="o:TotalTime"/>
      </xsl:attribute>-->
			<!--<xsl:attribute name="CreateDate">
        <xsl:value-of select="o:Created"/>
      </xsl:attribute>
      <xsl:attribute name="LastSaveDate">
        <xsl:value-of select="o:LastSaved"/>
      </xsl:attribute>-->
			<xsl:attribute name="Category">
				<xsl:value-of select="o:Category"/>
			</xsl:attribute>
			<xsl:attribute name="Manager">
				<xsl:value-of select="o:Manager"/>
			</xsl:attribute>
			<xsl:attribute name="Company">
				<xsl:value-of select="o:Company"/>
			</xsl:attribute>
		</builtin-properties>
	</xsl:template>
</xsl:stylesheet>

