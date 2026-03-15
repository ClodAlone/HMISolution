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

	<xsl:template match="/DLS">
		<xsl:processing-instruction name="mso-application">
			progid="Word.Document"
		</xsl:processing-instruction>
		<w:wordDocument			
			w:macrosPresent="no" 
			w:embeddedObjPresent="no" 
			w:ocxPresent="no" 
			xml:space="preserve">
	
			<xsl:apply-templates select="builtin-properties" />      
			<xsl:apply-templates select="styles" />
			<xsl:apply-templates select="liststyles"/>
			<xsl:apply-templates select="sections" />
			<xsl:apply-templates select="view-setup"/>
		</w:wordDocument>
	</xsl:template>

	<xsl:template match="styles">
		<w:styles>
			<xsl:apply-templates select="style"/>
			<w:style w:type="table" w:default="on" w:styleId="TableNormal">
				<w:name w:val="Normal Table" />
				<wx:uiName wx:val="Table Normal" />
				<w:semiHidden />
				<w:rPr>
					<wx:font wx:val="Times New Roman" />
				</w:rPr>
				<w:tblPr>
					<w:tblInd w:w="0" w:type="dxa" />
					<w:tblCellMar>
						<w:top w:w="0" w:type="dxa" />
						<w:left w:w="108" w:type="dxa" />
						<w:bottom w:w="0" w:type="dxa" />
						<w:right w:w="108" w:type="dxa" />
					</w:tblCellMar>
				</w:tblPr>
			</w:style>
		</w:styles>
	</xsl:template>
	<xsl:template match="liststyles">
		<w:lists>
			<xsl:apply-templates select="style"/>
		</w:lists>
	</xsl:template>
	<xsl:template match="style">
		<xsl:if test="@type='ParagraphStyle'">
			<xsl:variable name="name">
				<xsl:value-of select="translate(@Name,' ','')" />
			</xsl:variable>
			<xsl:element name="w:style">
				<xsl:attribute name="w:type">paragraph</xsl:attribute>
				<xsl:if test="@Name = 'Normal'">
					<xsl:attribute name="w:default">on</xsl:attribute>
				</xsl:if>
				<xsl:attribute name="w:styleId">
					<xsl:value-of select="$name" />
				</xsl:attribute>
				<xsl:if test="base/@ref != ''">
					<xsl:variable name="ref">
						<xsl:value-of select="base/@ref"/>
					</xsl:variable>
					<xsl:element name="w:basedOn">
						<xsl:attribute name="w:val">
							<xsl:value-of select="translate(/DLS/styles/style[@id=$ref]/@Name,' ','')"/>
						</xsl:attribute>
					</xsl:element>
				</xsl:if>				
				<xsl:element name="w:name">
					<xsl:attribute name="w:val">
						<xsl:value-of select="$name"/>
					</xsl:attribute>
				</xsl:element>
				<w:pPr>
					<w:pStyle>
						<xsl:attribute name="w:val">
							<xsl:value-of select="$name"/>
						</xsl:attribute>
					</w:pStyle>
					<xsl:apply-templates select="paragraph-format" />
					<xsl:if test="character-format != ''">
						<xsl:apply-templates select="character-format" />
					</xsl:if>
				</w:pPr>
				<xsl:apply-templates select="character-format" />
			</xsl:element>
		</xsl:if>
		<!--xsl:if test="@type = 'OtherStyle'">
			<w:listDef>
				<xsl:attribute name="id">
					<xsl:value-of select="@id"/>
				</xsl:attribute>
				<w:plt w:val="HybridMultilevel" />
				<xsl:apply-templates select="levels"/>
			</w:listDef>
		</xsl:if-->
	</xsl:template>
	<!--xsl:template match="levels">
		<xsl:apply-templates select="level"/>
	</xsl:template>
	<xsl:template match="level">
		<w:lvl>
			<xsl:attribute name="w:ilvl">
				<xsl:value-of select="@id"/>
			</xsl:attribute>
			<w:start>
				<xsl:attribute name="w:val">
					<xsl:value-of select="@StartAt"/>
				</xsl:attribute>
			</w:start>
			<w:pPr>
				<w:ind>
					<xsl:attribute name="w:left">
						<xsl:value-of select="paragraph-format/@LeftIndent"/>
					</xsl:attribute>
				</w:ind>
			</w:pPr>
		</w:lvl>
	</xsl:template-->
	<xsl:template match="sections" >
		<w:body>
			<xsl:apply-templates select="section" />
		</w:body>
	</xsl:template>
	<xsl:template match="section">
		<wx:sect>
			<xsl:apply-templates select="body/paragraphs/item" />
			<xsl:if test="@PropInEndPar = 'False'">
				<w:sectPr >
					<xsl:apply-templates select="headers-footers"/>
					<xsl:apply-templates select="page-setup" />
					<xsl:apply-templates select="columns"/>
				</w:sectPr>
			</xsl:if>
		</wx:sect>
	</xsl:template>	
	<xsl:template match="paragraphs/item">
		<xsl:choose>
			<xsl:when test="@type = 'Table'">				
				<w:tbl>
					<w:tblPr>
						<xsl:apply-templates select="rows/row[1]/table-format" />
						<xsl:apply-templates select="tblGrid" />
					</w:tblPr>
					<xsl:apply-templates select="rows" />
				</w:tbl>
			</xsl:when>
			<xsl:otherwise>
				<w:p>
					<w:pPr>
						<xsl:if test="@BreakBefore = 'True'">
							<w:br w:type="page" />
						</xsl:if>
						<xsl:apply-templates select="paragraph-format" />
						<xsl:if test="character-format != ''">
							<xsl:apply-templates select="character-format" />
						</xsl:if>
						<xsl:if test="@SctPrp = 'True'">
							<w:sectPr >
								<xsl:if test="@Continue = 'True'">
									<w:type w:val="continuous" />
								</xsl:if>
								<xsl:apply-templates select="headers-footers"/>
								<xsl:apply-templates select="page-setup" />
								<xsl:apply-templates select="columns"/>
							</w:sectPr>
						</xsl:if>
						<xsl:if test="style/@ref != ''">
							<w:pStyle>
								<xsl:variable name="ref">
									<xsl:value-of select="style/@ref"/>
								</xsl:variable>
								<xsl:attribute name="w:val">
									<xsl:value-of select="translate(/DLS/styles/style[@id=$ref]/@Name,' ','')"/>
								</xsl:attribute>
							</w:pStyle>
						</xsl:if>
					</w:pPr>
					<xsl:apply-templates select="items" />
				</w:p>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="items">
		<xsl:apply-templates select="item" />
	</xsl:template>
	<xsl:template match="item">
		<xsl:choose>
			<xsl:when test="@type = 'BookmarkStart'">
				<aml:annotation w:type="Word.Bookmark.Start" >
					<xsl:attribute name="aml:id">
						<xsl:value-of select="@bookmarkID"/>
					</xsl:attribute>
					<xsl:attribute name="w:name">
						<xsl:value-of select="@BookmarkName"/>
					</xsl:attribute>
				</aml:annotation>
			</xsl:when>
			<xsl:when test="@type = 'BookmarkEnd'">
				<aml:annotation w:type="Word.Bookmark.End">
					<xsl:attribute name="aml:id">
						<xsl:value-of select="@bookmarkID"/>
					</xsl:attribute>
				</aml:annotation>
			</xsl:when>			
			<xsl:when test="@type = 'Picture'">
				<w:pict>
					<w:binData>
						<xsl:attribute name="w:name">
							<xsl:value-of select="@Name"/>
						</xsl:attribute>
						<xsl:value-of select="image"/>
					</w:binData>
					<v:shape>
						<xsl:attribute name="style">
							<xsl:value-of select="@style"/>
						</xsl:attribute>
						<v:imagedata>
							<xsl:attribute name="src">
								<xsl:value-of select="@Name"/>
							</xsl:attribute>
						</v:imagedata>
					</v:shape>
				</w:pict>
			</xsl:when>
			<xsl:otherwise>
				<w:r>
					<xsl:apply-templates select="character-format" />
					<w:t>
						<xsl:if test="text = ''">
							<xsl:text> </xsl:text>
						</xsl:if>
						<xsl:value-of select="text"/>
					</w:t>
				</w:r>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="rows">
		<xsl:apply-templates select="row" />
	</xsl:template>
	<xsl:template match="row">
		<w:tr>
			<xsl:if test="@RowHeight != ''">
				<w:trPr>
					<w:trHeight>
						<xsl:attribute name="w:val">
							<xsl:value-of select="(@RowHeight)*20"/>
						</xsl:attribute>
					</w:trHeight>
				</w:trPr>
			</xsl:if>
			<xsl:apply-templates select="cells" />
		</w:tr>
	</xsl:template>
	<xsl:template match="cells">
		<xsl:apply-templates select="cell"/>
	</xsl:template>
	<xsl:template match="cell">
		<w:tc>
			<w:tcPr>
				<xsl:if test="@Width != ''">
					<w:tcW w:type="dxa">
						<xsl:attribute name="w:w">
							<xsl:value-of select="(@Width)*20"/>
						</xsl:attribute>
					</w:tcW>
					<xsl:apply-templates select="cell-format"/>
				</xsl:if>
				<xsl:if test="@collcount != '' ">
					<w:gridSpan>
						<xsl:attribute name="w:val">
							<xsl:value-of select="@collcount"/>
						</xsl:attribute>
					</w:gridSpan>
				</xsl:if>
			</w:tcPr>
			<xsl:apply-templates select="paragraphs" />
		</w:tc>
	</xsl:template>
	<xsl:template match="columns">
		<xsl:element name="w:cols">
			<xsl:attribute name="w:num">
				<xsl:value-of select="count(column)" />
			</xsl:attribute>
			<xsl:if test="count(column) > 0">
				<xsl:attribute name="w:equalWidth">off</xsl:attribute>
				<xsl:apply-templates select="column"/>
			</xsl:if>
		</xsl:element>
	</xsl:template>
	<xsl:template match="column">
		<xsl:element name="w:col">
			<xsl:attribute name="w:w">
				<xsl:value-of select="(@Width)*20"/>
			</xsl:attribute>
			<xsl:attribute name="w:space">
				<xsl:value-of select="(@Spacing)*20"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>
	<!-- Borders -->
	<xsl:template name="Border">
		<xsl:param name="name"></xsl:param>
		<xsl:param name="val">20</xsl:param>
		<xsl:param name="sz"></xsl:param>
		<xsl:param name="space"></xsl:param>
		<xsl:param name="color"></xsl:param>
		<xsl:param name="shadow"></xsl:param>

		<xsl:if test="$val != ''">
			<xsl:if test="$val != 'None'">
				<xsl:element name="{$name}">
					<!-- w:val    -->
					<xsl:attribute name="w:val">
						<xsl:choose>
							<xsl:when test="$val = 'Triple'"                 >triple</xsl:when>
							<xsl:when test="$val = 'DashSmallGap'"           >dash-small-gap</xsl:when>
							<xsl:when test="$val = 'Single'"                 >single</xsl:when>
							<xsl:when test="$val = 'Dot'"                    >dotted</xsl:when>
							<xsl:when test="$val = 'DotDash'"                >dot-dash</xsl:when>
							<xsl:when test="$val = 'DashLargeGap'"           >dashed</xsl:when>
							<xsl:when test="$val = 'DotDotDash'"             >dot-dot-dash</xsl:when>
							<xsl:when test="$val = 'Double'"                 >double</xsl:when>
							<xsl:when test="$val = 'ThinThinSmallGap'"       >thick-thin-small-gap</xsl:when>
							<xsl:when test="$val = 'ThinThickSmallGap'"      >thin-thick-small-gap</xsl:when>
							<xsl:when test="$val = 'ThinThickThinSmallGap'"  >thin-thick-thin-small-gap</xsl:when>
							<xsl:when test="$val = 'ThickThinMediumGap'"     >thick-thin-medium-gap</xsl:when>
							<xsl:when test="$val = 'ThinThickMediumGap'"     >thin-thick-medium-gap</xsl:when>
							<xsl:when test="$val = 'ThickThickThinMediumGap'">thin-thick-thin-medium-gap</xsl:when>
							<xsl:when test="$val = 'ThickThinLargeGap'"      >thick-thin-large-gap</xsl:when>
							<xsl:when test="$val = 'ThinThickLargeGap'"      >thin-thick-large-gap</xsl:when>
							<xsl:when test="$val = 'ThinThickThinLargeGap'"  >thin-thick-thin-large-gap</xsl:when>
							<xsl:when test="$val = 'Wave'"                   >wave</xsl:when>
							<xsl:when test="$val = 'DoubleWave'"             >double-wave</xsl:when>
							<xsl:when test="$val = 'DashDotStroker'"         >dash-dot-stroked</xsl:when>
							<xsl:when test="$val = 'Engrave3D'"              >three-d-engrave</xsl:when>
							<xsl:when test="$val = 'Emboss3D'"               >three-d-emboss</xsl:when>
							<xsl:when test="$val = 'Outset'"                     >outset</xsl:when>
							<xsl:when test="$val = 'Inset'"                     >inset</xsl:when>
							<xsl:when test="$val = 'Cleared'"                    >nil</xsl:when>
						</xsl:choose>
					</xsl:attribute>
					<xsl:if test="$val != Cleared">
						<!-- w:sz     -->
						<xsl:attribute name="w:sz">
							<xsl:value-of select="$sz" />
						</xsl:attribute>
						<!-- w:space  -->
						<xsl:if test="$space != 'NaN'">
							<xsl:attribute name="w:space">
								<xsl:value-of select="$space"/>
							</xsl:attribute>
						</xsl:if>
						<!-- w:color  -->
						<xsl:attribute name="w:color">
							<xsl:value-of select="substring(($color),4)"/>
						</xsl:attribute>
						<!-- w:shadow -->
						<xsl:if test="$shadow = 'true'">
							<xsl:attribute name="w:shadow">on</xsl:attribute>
						</xsl:if>
					</xsl:if>
				</xsl:element>
			</xsl:if>
		</xsl:if>
	</xsl:template>
	<xsl:template match="Top">
		<xsl:param name="multiplier">20</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:top</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*20"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@Color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@Shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="Left">
		<xsl:param name="multiplier">20</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:left</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*20"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@Color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@Shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="Right">
		<xsl:param name="multiplier">20</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:right</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*20"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@Color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@Shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="Bottom">
		<xsl:param name="multiplier">20</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:bottom</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*20"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@Color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@Shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="Horizontal">
		<xsl:param name="multiplier">20</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:insideH</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*20"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@Color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@Shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="Vertical">
		<xsl:param name="multiplier">20</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:insideV</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*20"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@Color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@Shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="borders">
		<xsl:param name="multiplier">20</xsl:param>

		<xsl:apply-templates select="Top">
			<xsl:with-param name="multiplier">
				<xsl:value-of select="$multiplier"/>
			</xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Left">
			<xsl:with-param name="multiplier">
				<xsl:value-of select="$multiplier"/>
			</xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Bottom">
			<xsl:with-param name="multiplier">
				<xsl:value-of select="$multiplier"/>
			</xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Right">
			<xsl:with-param name="multiplier">
				<xsl:value-of select="$multiplier"/>
			</xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Horizontal">
			<xsl:with-param name="multiplier">8</xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Vertical">
			<xsl:with-param name="multiplier">8</xsl:with-param>
		</xsl:apply-templates>
	</xsl:template>
	<!-- Properties -->
	<xsl:template match="view-setup">
		<w:docPr>
			<w:zoom>
				<xsl:attribute name="w:percent">
					<xsl:choose>
						<xsl:when test="@ZoomPercent != ''">
							<xsl:value-of select="@ZoomPercent" />
						</xsl:when>
						<xsl:otherwise>100</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
			</w:zoom>
			<w:view>
				<xsl:attribute name="w:val">
					<xsl:choose>
						<xsl:when test="@ViewType = 'NormalLayout'">normal</xsl:when>
						<xsl:when test="@ViewType = 'WebLayout'">web</xsl:when>
						<xsl:when test="@ViewType = 'OutlineLayout'">master-pages</xsl:when>
						<xsl:otherwise>print</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
			</w:view>
			<w:evenAndOddHeaders />
			<xsl:if test="/DLS/@ProtectionType != ''">
				<xsl:choose>
					<xsl:when test="/DLS/@ProtectionType = 'AllowOnlyComments'">
						<w:documentProtection w:edit="comments" w:enforcement="on" />
					</xsl:when>
				</xsl:choose>
			</xsl:if>
		</w:docPr>
	</xsl:template>
	<xsl:template match="builtin-properties">
		<o:DocumentProperties>
			<o:Title>
				<xsl:value-of select="@Title"/>
			</o:Title>
			<o:Subject>
				<xsl:value-of select="@Subject"/>
			</o:Subject>
			<o:Author>
				<xsl:value-of select="@Author"/>
			</o:Author>
			<o:Keywords>
				<xsl:value-of select="@Keywords"/>
			</o:Keywords>
			<o:Description>
				<xsl:value-of select="@Comments"/>
			</o:Description>
			<o:LastAuthor>
				<xsl:value-of select="@LastAuthor"/>
			</o:LastAuthor>
			<o:Revision>
				<xsl:value-of select="@RevisionNumber"/>
			</o:Revision>
			<o:TotalTime>
				<xsl:value-of select="@EditTime"/>
			</o:TotalTime>
			<o:Created>
				<xsl:value-of select="@CreateDate"/>
			</o:Created>
			<o:LastSaved>
				<xsl:value-of select="@LastSaveDate"/>
			</o:LastSaved>
			<o:Category>
				<xsl:value-of select="@Category"/>
			</o:Category>
			<o:Manager>
				<xsl:value-of select="@Manager"/>
			</o:Manager>
			<o:Company>
				<xsl:value-of select="@Company"/>
			</o:Company>
		</o:DocumentProperties>
	</xsl:template>
	<xsl:template match="page-setup">
		<xsl:element name="w:pgSz" >
			<xsl:attribute name="w:w">
				<xsl:value-of select="(@PageWidth)*20"/>
			</xsl:attribute>
			<xsl:attribute name="w:h">
				<xsl:value-of select="(@PageHeight)*20"/>
			</xsl:attribute>
			<xsl:if test="@Orientation != ''">
				<xsl:attribute name="w:orient">
					<xsl:choose>
						<xsl:when test="@Orientation = 'Landscape'">landscape</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</xsl:if>
		</xsl:element>
		<xsl:element name="w:pgMar">
			<xsl:attribute name="w:top">
				<xsl:value-of select="(@TopMargin)*20"/>
			</xsl:attribute>
			<xsl:attribute name="w:right">
				<xsl:value-of select="(@RightMargin)*20"/>
			</xsl:attribute>
			<xsl:attribute name="w:bottom">
				<xsl:value-of select="(@BottomMargin)*20"/>
			</xsl:attribute>
			<xsl:attribute name="w:left">
				<xsl:value-of select="(@LeftMargin)*20"/>
			</xsl:attribute>
			<xsl:if test="@FooterDistance != ''">
				<xsl:attribute name="w:footer">
					<xsl:value-of select="(@FooterDistance)*20"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@HeaderDistance != ''">
				<xsl:attribute name="w:header">
					<xsl:value-of select="(@HeaderDistance)*20"/>
				</xsl:attribute>
			</xsl:if>
		</xsl:element>
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
				<xsl:attribute name="w:count-by">1</xsl:attribute>
				<xsl:if test="@PageSetupLineNumDistance != ''">
					<xsl:attribute name="w:distance">
						<xsl:value-of select="(@PageSetupLineNumDistance)*20"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:attribute name="w:restart">new-section</xsl:attribute>
			</w:lnNumType>
		</xsl:if>
	</xsl:template>
	<xsl:template match="character-format">
		<w:rPr>
			<xsl:if test="@FontSize != ''">
				<xsl:element name="w:sz">
					<xsl:attribute name="w:val">
						<xsl:value-of select="(@FontSize)*2"/>
					</xsl:attribute>
				</xsl:element>
			</xsl:if>
			<xsl:if test="@FontSizeBidi != ''">
				<xsl:element name="w:sz-cs">
					<xsl:attribute name="w:val">
						<xsl:value-of select="(@FontSizeBidi)*2"/>
					</xsl:attribute>
				</xsl:element>
			</xsl:if>
			<xsl:if test="@FontName != ''">
				<w:rFonts>
					<xsl:attribute name="w:ascii">
						<xsl:value-of select="@FontName"/>
					</xsl:attribute>
					<xsl:attribute name="w:h-ansi">
						<xsl:value-of select="@FontName"/>
					</xsl:attribute>
					<xsl:attribute name="w:cs">
						<xsl:value-of select="@FontName"/>
					</xsl:attribute>
				</w:rFonts>
				<wx:font>
					<xsl:attribute name="wx:val">
						<xsl:value-of select="@FontName"/>
					</xsl:attribute>
				</wx:font>
			</xsl:if>
			<xsl:if test="@Italic = 'true'">
				<w:i />
			</xsl:if>
			<xsl:if test="@Bold = 'true'">
				<w:b />
			</xsl:if>
			<xsl:if test="@Underline !=''">
				<xsl:choose>
					<xsl:when test="@Underline = '1'">
						<w:u w:val="single" />
					</xsl:when>
					<xsl:when test="@Underline = '2'">
						<w:u w:val="words" />
					</xsl:when>
					<xsl:when test="@Underline = '3'">
						<w:u w:val="double" />
					</xsl:when>
					<xsl:when test="@Underline = '4'">
						<w:u w:val="dotted" />
					</xsl:when>					
					<xsl:when test="@Underline = '6'">
						<w:u w:val="thick" />
					</xsl:when>
					<xsl:when test="@Underline = '7'">
						<w:u w:val="dash" />
					</xsl:when>					
					<xsl:when test="@Underline = '9'">
						<w:u w:val="dot-dash" />
					</xsl:when>
					<xsl:otherwise>
						<w:u w:val="none" />
					</xsl:otherwise>
				</xsl:choose>				
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
				<xsl:choose>
					<xsl:when test="substring(@HighlightColor, 4) = '000000'">
						<w:highlight w:val="black" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = '0000FF'">
						<w:highlight w:val="blue" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = '00FFFF'">
						<w:highlight w:val="cyan" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = '008000'">
						<w:highlight w:val="green" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = 'FF00FF'">
						<w:highlight w:val="magenta" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = 'FF0000'">
						<w:highlight w:val="red" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = 'D3D3D3'">
						<w:highlight w:val="light-gray" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = 'FFFF00'">
						<w:highlight w:val="yellow" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = '00008B'">
						<w:highlight w:val="dark-blue" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = '008B8B'">
						<w:highlight w:val="dark-cyan" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = '006400'">
						<w:highlight w:val="dark-green" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = '8B008B'">
						<w:highlight w:val="dark-magenta" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = '8B0000'">
						<w:highlight w:val="dark-red" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = 'A9A9A9'">
						<w:highlight w:val="dark-gray" />
					</xsl:when>
					<xsl:when test="substring(@HighlightColor, 4) = 'FFD700'">
						<w:highlight w:val="dark-yellow" />
					</xsl:when>
				</xsl:choose>
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
				<xsl:element name="w:position">
					<xsl:attribute name="w:val">
						<xsl:value-of select="(@Position)*20"/>
					</xsl:attribute>
				</xsl:element>
			</xsl:if>
			<xsl:if test="@LineSpacing != ''">
				<xsl:element name="w:spacing">
					<xsl:attribute name="w:val">
						<xsl:value-of select="(@LineSpacing)*20"/>
					</xsl:attribute>
				</xsl:element>
			</xsl:if>
			<xsl:if test="@TextBackgroundColor != ''">
				<w:shd w:val="clear" w:color="auto" >
					<xsl:attribute name="w:fill">
						<xsl:value-of select="substring(@TextBackgroundColor,4)"/>
					</xsl:attribute>
				</w:shd>
			</xsl:if>
			<xsl:if test="@Bidi = 'true'">
				<w:bidi/>
			</xsl:if>
		</w:rPr>
	</xsl:template>
	<xsl:template match="paragraph-format">
		<xsl:if test="@PageBreakBefore = 'true'">
			<w:pageBreakBefore />
		</xsl:if>
		<xsl:if test="@Keep = 'true'">
			<w:keepLines />
		</xsl:if>
		<xsl:if test="@KeepFollow = 'true'">
			<w:keepNext />
		</xsl:if>
		<xsl:choose>
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
		<w:ind>
			<xsl:if test="@LeftIndent != ''">
				<xsl:attribute name="w:left">
					<xsl:value-of select="(@LeftIndent)*20"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@RightIndent != ''">
				<xsl:attribute name="w:right">
					<xsl:value-of select="(@RightIndent)*20"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="@FirstLineIndent != ''">
				<xsl:attribute name="w:first-line">
					<xsl:value-of select="(@FirstLineIndent)*20"/>
				</xsl:attribute>
			</xsl:if>

		</w:ind>
		<xsl:if test="(@BeforeSpacing != '') or (@AfterSpacing != '')">
			<w:spacing>
				<xsl:if test="@BeforeSpacing != ''">
					<xsl:attribute name="w:before">
						<xsl:value-of select="(@BeforeSpacing)*20"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:if test="@AfterSpacing != ''">
					<xsl:attribute name="w:after">
						<xsl:value-of select="(@AfterSpacing)*20"/>
					</xsl:attribute>
				</xsl:if>
			</w:spacing>
		</xsl:if>
		<xsl:if test="@BackColor != ''">
			<w:shd>
				<xsl:attribute name="w:fill">
					<xsl:value-of select="substring(@BackColor, 4)"/>
				</xsl:attribute>
			</w:shd>
		</xsl:if>
		<xsl:if test="@Bidi = 'true'">
			<w:bidi/>
		</xsl:if>
		<w:pBdr>
			<xsl:apply-templates select="borders"/>
		</w:pBdr>
	</xsl:template>
	<xsl:template match="table-format">
		<xsl:if test="@CellSpacing != ''">
			<w:tblCellSpacing>
				<xsl:attribute name="w:w">
					<xsl:value-of select="(@CellSpacing)*20"/>
				</xsl:attribute>
				<xsl:attribute name="w:type">dxa</xsl:attribute>
			</w:tblCellSpacing>
		</xsl:if>
		<xsl:if test="@LeftOffset != ''">
			<xsl:element name="w:tblInd">
				<xsl:attribute name="w:w">
					<xsl:choose>
						<xsl:when test="Paddings/@Left != ''">
							<xsl:value-of select="((@LeftOffset)+(Paddings/@Left))*20"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="(@LeftOffset)*20"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:attribute name="w:type">dxa</xsl:attribute>
			</xsl:element>
		</xsl:if>
		<w:tblStyle w:val="TableGrid"/>
		<w:tblBorders>
			<xsl:apply-templates select="borders">
				<xsl:with-param name="multiplier">8</xsl:with-param>
			</xsl:apply-templates>			
		</w:tblBorders>
		<xsl:apply-templates select="Paddings"/>
	</xsl:template>
	<xsl:template match="Paddings">
		<w:tblCellMar>
			<xsl:if test="@Top != ''">
				<w:top>
					<xsl:attribute name="w:w">
						<xsl:value-of select="(@Top)*20"/>
					</xsl:attribute>
					<xsl:attribute name="w:type">dxa</xsl:attribute>
				</w:top>
			</xsl:if>
			<xsl:if test="@Left != ''">
				<w:left>
					<xsl:attribute name="w:w">
						<xsl:value-of select="(@Left)*20"/>
					</xsl:attribute>
					<xsl:attribute name="w:type">dxa</xsl:attribute>
				</w:left>
			</xsl:if>
			<xsl:if test="@Bottom != ''">
				<w:bottom>
					<xsl:attribute name="w:w">
						<xsl:value-of select="(@Bottom)*20"/>
					</xsl:attribute>
					<xsl:attribute name="w:type">dxa</xsl:attribute>
				</w:bottom>
			</xsl:if>
			<xsl:if test="@Right != ''">
				<w:right>
					<xsl:attribute name="w:w">
						<xsl:value-of select="(@Right)*20"/>
					</xsl:attribute>
					<xsl:attribute name="w:type">dxa</xsl:attribute>
				</w:right>
			</xsl:if>
		</w:tblCellMar>
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
	<xsl:template match="cell-format">
		<w:tcBorders>
			<xsl:apply-templates select="borders">
				<xsl:with-param name="multiplier">8</xsl:with-param>
			</xsl:apply-templates>
		</w:tcBorders>
		<xsl:if test="@ShadingColor != ''">
			<w:shd w:color="auto">
				<xsl:attribute name="w:fill">
					<xsl:value-of select="substring(@ShadingColor,4)"/>
				</xsl:attribute>
			</w:shd>
		</xsl:if>
		<xsl:if test="@VAlignment != ''">
			<w:vAlign>
				<xsl:attribute name="w:val">
					<xsl:choose>
						<xsl:when test="@VAlignment = 'Bottom'">bottom</xsl:when>
						<xsl:when test="@VAlignment = 'Middle'">center</xsl:when>
						<xsl:when test="@VAlignment = 'Top'">top</xsl:when>
					</xsl:choose>
				</xsl:attribute>
			</w:vAlign>
		</xsl:if>
		<xsl:if test="@SamePaddingsAsTable = 'false'">
			<w:tcMar>
				<w:top>
					<xsl:attribute name="w:w">0</xsl:attribute>
					<xsl:attribute name="w:type">dxa</xsl:attribute>
				</w:top>
				<w:left>
					<xsl:attribute name="w:w">0</xsl:attribute>
					<xsl:attribute name="w:type">dxa</xsl:attribute>
				</w:left>
				<w:bottom>
					<xsl:attribute name="w:w">0</xsl:attribute>
					<xsl:attribute name="w:type">dxa</xsl:attribute>
				</w:bottom>
				<w:right>
					<xsl:attribute name="w:w">0</xsl:attribute>
					<xsl:attribute name="w:type">dxa</xsl:attribute>
				</w:right>
			</w:tcMar>
		</xsl:if>
	</xsl:template>
	<!-- Headers - Footers -->
	<xsl:template match="headers-footers">
		<xsl:apply-templates select="first-page-header"/>
		<xsl:apply-templates select="first-page-footer"/>
		<xsl:apply-templates select="even-footer"/>
		<xsl:apply-templates select="even-header"/>
		<xsl:apply-templates select="odd-footer"/>
		<xsl:apply-templates select="odd-header"/>
	</xsl:template>
	<xsl:template match="odd-footer">
		<xsl:variable name="temp">
			<xsl:value-of select="count(paragraphs)"/>
		</xsl:variable>
		<xsl:if test="$temp > 0">
			<w:ftr w:type="odd">
				<xsl:apply-templates select="paragraphs"/>
			</w:ftr>
		</xsl:if>
	</xsl:template>
	<xsl:template match="odd-header">
		<xsl:variable name="temp">
			<xsl:value-of select="count(paragraphs)"/>
		</xsl:variable>
		<xsl:if test="$temp > 0">
			<w:hdr w:type="odd">
				<xsl:apply-templates select="paragraphs"/>
			</w:hdr>
		</xsl:if>
	</xsl:template>
	<xsl:template match="even-footer">
		<xsl:variable name="temp">
			<xsl:value-of select="count(paragraphs)"/>
		</xsl:variable>
		<xsl:if test="$temp > 0">
			<w:ftr w:type="even">
				<xsl:apply-templates select="paragraphs"/>
			</w:ftr>
		</xsl:if>
	</xsl:template>
	<xsl:template match="even-header">
		<xsl:variable name="temp">
			<xsl:value-of select="count(paragraphs)"/>
		</xsl:variable>
		<xsl:if test="$temp > 0">
			<w:hdr w:type="even">
				<xsl:apply-templates select="paragraphs"/>
			</w:hdr>
		</xsl:if>
	</xsl:template>
	<xsl:template match="first-page-header">
		<xsl:variable name="temp">
			<xsl:value-of select="count(paragraphs)"/>
		</xsl:variable>
		<xsl:if test="$temp > 0">
			<w:hdr w:type="first">
				<xsl:apply-templates select="paragraphs"/>
			</w:hdr>
		</xsl:if>
	</xsl:template>
	<xsl:template match="first-page-footer">
		<xsl:variable name="temp">
			<xsl:value-of select="count(paragraphs)"/>
		</xsl:variable>
		<xsl:if test="$temp > 0">
			<w:ftr w:type="first">
				<xsl:apply-templates select="paragraphs"/>
			</w:ftr>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>