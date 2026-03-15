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

	<xsl:template match="paragraphs/item">
		<xsl:param name="lastSection">true</xsl:param>
		<xsl:choose>
			<xsl:when test="@type = 'Table'">
				<w:tbl>
					<w:tblPr>
						<xsl:apply-templates select="rows/row[1]/table-format" />
						<w:tblW w:type="dxa">
							<xsl:attribute name="w:w">
								<xsl:value-of select="tblW/@w" />
							</xsl:attribute>
						</w:tblW>
					</w:tblPr>
					<xsl:apply-templates select="tblGrid" />
					<xsl:apply-templates select="rows" />
				</w:tbl>
			</xsl:when>
			<xsl:otherwise>
				<w:p>
					<w:pPr>
						<!--<xsl:if test="@BreakBefore = 'True'">
							<w:br w:type="page" />
						</xsl:if>-->
						<xsl:if test="$lastSection = 'false' and position() = last()">
							<w:sectPr >
								<w:type>
									<xsl:attribute name="w:val">
										<xsl:choose>
											<xsl:when test="../../../@BreakCode = 'NewColumn'">nextColumn</xsl:when>
											<xsl:when test="../../../@BreakCode = 'NewPage'">nextPage</xsl:when>
											<xsl:when test="../../../@BreakCode = 'EvenPage'">evenPage</xsl:when>
											<xsl:when test="../../../@BreakCode = 'Oddpage'">oddPage</xsl:when>
											<xsl:otherwise>continuous</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
								</w:type>
								<xsl:apply-templates select="../../../page-setup" />
								<xsl:apply-templates select="../../../headers-footers"/>
								<xsl:apply-templates select="../../../columns"/>
								<xsl:if test="count(items/item[@IsEndnoteAttr = 'true'])>0">
									<w:endnotePr>
										<w:numFmt w:val="decimal" />
									</w:endnotePr>
								</xsl:if>
								<xsl:if test="count(items/item[@type = 'Footnote' and @IsEndnoteAttr = ''])>0">
									<w:footnotePr>
										<w:numFmt w:val="decimal" />
									</w:footnotePr>
								</xsl:if>
							</w:sectPr>
						</xsl:if>
						<xsl:apply-templates select="paragraph-format"/>
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
			<xsl:when test="@type = 'Picture'">
				<xsl:if test="count(preceding-sibling::item[@BookmarkName = '_PictureBullets'])=0">
					<xsl:apply-templates select="." mode="drawing" />
				</xsl:if>
			</xsl:when>
			<xsl:when test="@type = 'ShapeObject'">
				<xsl:apply-templates select="textboxes" />
			</xsl:when>
			<xsl:when test="@type = 'InlineShapeObject'">

			</xsl:when>
			<xsl:when test="@type = 'BookmarkStart'">
				<w:bookmarkStart>
					<xsl:attribute name="w:name">
						<xsl:value-of select="@BookmarkName" />
					</xsl:attribute>
					<xsl:attribute name="w:id">
						<xsl:value-of select="@id" />
					</xsl:attribute>
				</w:bookmarkStart>
			</xsl:when>
			<xsl:when test="@type = 'BookmarkEnd'">
				<w:bookmarkEnd>
					<xsl:attribute name="w:id">
						<xsl:value-of select="@id" />
					</xsl:attribute>
				</w:bookmarkEnd>
			</xsl:when>
			<xsl:when test="@type = 'Field'">
				<xsl:variable name="parent">
					<xsl:value-of select="@ParentFieldId" />
				</xsl:variable>
				<xsl:if test="$parent = '' or preceding::item[@id = $parent][@FieldType != 'FieldHyperlink' or @type = 'TOC']">	
					<xsl:apply-templates select="." mode="field" />
				</xsl:if>
			</xsl:when>
			<xsl:when test="@type = 'TOC'">
				<xsl:apply-templates select="toc-field" mode="field" />
			</xsl:when>
			<xsl:when test="@type = 'FieldMark'">
				<xsl:variable name="markId">
					<xsl:value-of select="@id" />
				</xsl:variable>
				<xsl:variable name="parent">
					<xsl:value-of select="preceding::item[@id = $markId]/@ParentFieldId" />
				</xsl:variable>
				<xsl:if test="$parent = '' or preceding::item[@id = $parent][@FieldType != 'FieldHyperlink' or @type = 'TOC']">	
					<xsl:apply-templates select="." mode="fieldMark" />
				</xsl:if>
			</xsl:when>
			<xsl:when test="@type = 'MergeField'">

			</xsl:when>
			<xsl:when test="@type = 'DropDownFormField'">
				<w:r>
					<xsl:apply-templates select="character-format" />
					<w:fldChar>
						<xsl:call-template name="Field">
							<xsl:with-param name="fieldCharType">begin</xsl:with-param>
						</xsl:call-template>
						<w:ffData>
							<xsl:apply-templates select="." mode="FormFieldData" />
							<w:ddList>
								<w:default>
									<xsl:attribute name="w:val">
										<xsl:value-of select="@DefaultDrowDownValue" />
									</xsl:attribute>
								</w:default>
								<xsl:apply-templates select="dropdown-items" />
							</w:ddList>
						</w:ffData>
					</w:fldChar>
				</w:r>
				<w:r>
					<xsl:apply-templates select="character-format" />
					<w:instrText>
						<xsl:call-template name="FieldType">
							<xsl:with-param name="type">
								<xsl:value-of select="@FieldType" />
							</xsl:with-param>
						</xsl:call-template>
					</w:instrText>
				</w:r>
			</xsl:when>
			<xsl:when test="@type = 'TextFormField'">
				<w:r>
					<xsl:apply-templates select="text-range/character-format" />
					<w:fldChar>
						<xsl:call-template name="Field">
							<xsl:with-param name="fieldCharType">begin</xsl:with-param>
						</xsl:call-template>
						<w:ffData>
							<xsl:apply-templates select="." mode="FormFieldData" />
						</w:ffData>
					</w:fldChar>
				</w:r>
				<w:r>
					<xsl:apply-templates select="text-range/character-format" />
					<w:instrText>
						<xsl:call-template name="FieldType">
							<xsl:with-param name="type">
								<xsl:value-of select="@FieldType" />
							</xsl:with-param>
						</xsl:call-template>
					</w:instrText>
				</w:r>
			</xsl:when>
			<xsl:when test="@type = 'CheckBox'">
				<w:r>
					<xsl:apply-templates select="character-format" />
					<w:fldChar>
						<xsl:call-template name="Field">
							<xsl:with-param name="fieldCharType">begin</xsl:with-param>
						</xsl:call-template>
						<w:ffData>
							<xsl:apply-templates select="." mode="FormFieldData" />
						</w:ffData>
					</w:fldChar>
				</w:r>
				<w:r>
					<xsl:apply-templates select="character-format" />
					<w:instrText>
						<xsl:call-template name="FieldType">
							<xsl:with-param name="type">
								<xsl:value-of select="@FieldType" />
							</xsl:with-param>
						</xsl:call-template>
					</w:instrText>
				</w:r>
			</xsl:when>
			<xsl:when test="@type = 'SeqField'">

			</xsl:when>
			<xsl:when test="@type = 'EmbedField'">

			</xsl:when>
			<xsl:when test="@type = 'CommentAnchorStart'">
				<w:commentRangeStart>
					<xsl:attribute name="w:id">
						<xsl:value-of select="@id" />
					</xsl:attribute>
				</w:commentRangeStart>
			</xsl:when>
			<xsl:when test="@type = 'CommentAnchorEnd'">
				<w:commentRangeEnd>
					<xsl:attribute name="w:id">
						<xsl:value-of select="@id" />
					</xsl:attribute>
				</w:commentRangeEnd>
			</xsl:when>
			<xsl:when test="@type = 'Comment'">
				<w:r>
					<w:commentReference>
						<xsl:attribute name="w:id">
							<xsl:value-of select="@id" />
						</xsl:attribute>
					</w:commentReference>
				</w:r>
			</xsl:when>
			<xsl:when test="@type = 'TextBox'">
				<xsl:apply-templates select="." mode="textBox" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="fieldId">
					<xsl:value-of select="@ParentFieldId" />
				</xsl:variable>
				<xsl:variable name="parentId">
					<xsl:value-of select="preceding::item[@id = $fieldId]/@ParentFieldId" />
				</xsl:variable>
				<xsl:choose>
					<xsl:when test="($parentId != '') and (preceding::item[@id = $fieldId][@FieldType = 'FieldHyperlink'] or preceding::item[@id = $parentId][@FieldType = 'FieldHyperlink'])"></xsl:when>
					<xsl:when test="contains(character-format/internal-data, 'AgiB')">
						<xsl:apply-templates select="." mode="field" />
					</xsl:when>
					<xsl:otherwise>
						<w:r>
							<xsl:apply-templates select="character-format" />
							<xsl:choose>
								<xsl:when test="text = '' and ../../paragraph-format[count(Tabs/Tab[@Leader != 'NoLeader']) > 0]">
									<w:tab />
								</xsl:when>
								<xsl:otherwise>
									<xsl:choose>
										<xsl:when test="@type = 'Symbol'">
											<w:sym>
												<xsl:attribute name="w:char">
													<xsl:value-of select="@CharCode" />
												</xsl:attribute>
												<xsl:attribute name="w:font">
													<xsl:value-of select="@FontName" />
												</xsl:attribute>
											</w:sym>
										</xsl:when>
										<xsl:when test="@type = 'Break'">
											<w:br>
												<xsl:attribute name="w:type">
													<xsl:choose>
														<xsl:when test="@BreakType = 'PageBreak'">page</xsl:when>
														<xsl:when test="@BreakType = 'ColumnBreak'">column</xsl:when>
													</xsl:choose>
												</xsl:attribute>
											</w:br>
										</xsl:when>
										<xsl:when test="@type = 'Footnote'">
											<xsl:variable name="footEndnote">
												<xsl:choose>
													<xsl:when test="@IsEndnoteAttr = 'true'">
														<xsl:text>w:endnoteReference</xsl:text>
													</xsl:when>
													<xsl:otherwise>
														<xsl:text>w:footnoteReference</xsl:text>
													</xsl:otherwise>
												</xsl:choose>
											</xsl:variable>
											<xsl:apply-templates select="marker-character-format" />
											<xsl:element name="{$footEndnote}">
												<xsl:attribute name="w:customMarkFollows">
													<xsl:if test="@AutoNumbered = 'true'">
														<xsl:text>0</xsl:text>
													</xsl:if>
													<xsl:if test="@AutoNumbered = 'false'">
														<xsl:text>1</xsl:text>
													</xsl:if>
												</xsl:attribute>
												<xsl:attribute name="w:id">
													<xsl:value-of select="@id" />
												</xsl:attribute>
											</xsl:element>
											<xsl:if test="@CustomMarker != '' and @CustomMarker != '('">
												<w:t>
													<xsl:value-of select="@CustomMarker" />
												</w:t>
											</xsl:if>
											<xsl:if test="@SymbolCode != ''">
												<w:sym>
													<xsl:attribute name="w:char">
														<xsl:value-of select="@SymbolCode" />
													</xsl:attribute>
													<xsl:attribute name="w:font">
														<xsl:value-of select="@SymbolFontName" />
													</xsl:attribute>
												</w:sym>
											</xsl:if>
										</xsl:when>
									</xsl:choose>
									<xsl:apply-templates select="text" />
								</xsl:otherwise>
							</xsl:choose>
						</w:r>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Text -->
	<xsl:template match="text">
		<xsl:choose>
			<xsl:when test="count(ancestor::item[@type = 'Footnote' and @AutoNumbered = 'true' ])>0 and (../character-format/@CharStyleName='Endnote Reference' or ../character-format/@CharStyleName = 'Footnote Reference')">
				<xsl:if test="../character-format/@CharStyleName = 'Endnote Reference'">
					<w:endnoteRef />
				</xsl:if>
				<xsl:if test="../character-format/@CharStyleName = 'Footnote Reference'">
					<w:footnoteRef />
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test=". != ''">
					<w:t xml:space="preserve" ><xsl:value-of select="translate(.,'&#xB;','')"/></w:t>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>