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

	<xsl:output method="xml" standalone="yes" />

	<xsl:template name="FieldType">
		<xsl:param name="type"></xsl:param>

		<xsl:choose>
			<xsl:when test="$type = 'FieldAdvance'">ADVANCE</xsl:when>
			<!--<xsl:when test="$type = 'FieldAsk'">ASK</xsl:when>-->
			<xsl:when test="$type = 'FieldAuthor'">AUTHOR</xsl:when>
			<xsl:when test="$type = 'FieldAutoNum'">AUTONUM</xsl:when>
			<xsl:when test="$type = 'FieldAutoNumLegal'">AUTONUMLGL</xsl:when>
			<xsl:when test="$type = 'FieldAutoNumOutline'">AUTONUMOUT</xsl:when>
			<xsl:when test="$type = 'FieldAutoText'">AUTOTEXT</xsl:when>
			<xsl:when test="$type = 'FieldAutoTextList'">AUTOTEXTLIST</xsl:when>
			<xsl:when test="$type = 'FieldBarCode'">BARCODE</xsl:when>
			<xsl:when test="$type = 'FieldComments'">COMMENTS</xsl:when>
			<!--<xsl:when test="$type = 'FieldCompare'">COMPARE</xsl:when>-->
			<xsl:when test="$type = 'FieldCreateDate'">CREATEDATE</xsl:when>
			<!--<xsl:when test="$type = 'FieldDatabase'">DATABASE</xsl:when>-->
			<xsl:when test="$type = 'FieldDate'">DATE</xsl:when>
			<xsl:when test="$type = 'FieldDocProperty'">DOCPROPERTY</xsl:when>
			<xsl:when test="$type = 'FieldDocVariable'">DOCVARIABLE</xsl:when>
			<xsl:when test="$type = 'FieldEditTime'">EDITTIME</xsl:when>
			<xsl:when test="$type = 'FieldFileName'">FILENAME</xsl:when>
			<xsl:when test="$type = 'FieldFileSize'">FILESIZE</xsl:when>
			<!--<xsl:when test="$type = 'FieldFillIn'">FILLIN</xsl:when>-->
			<xsl:when test="$type = 'FieldFormCheckBox'">FORMCHECKBOX</xsl:when>
			<xsl:when test="$type = 'FieldFormDropDown'">FORMDROPDOWN</xsl:when>
			<xsl:when test="$type = 'FieldFormTextInput'">FORMTEXT</xsl:when>
			<xsl:when test="$type = 'FieldGoToButton'">GOTOBUTTON</xsl:when>
			<xsl:when test="$type = 'FieldHyperlink'">HYPERLINK</xsl:when>
			<!--<xsl:when test="$type = 'FieldIf'">IF</xsl:when>-->
			<xsl:when test="$type = 'FieldIncludePicture'">INCLUDEPICTURE</xsl:when>
			<xsl:when test="$type = 'FieldIncludeText'">INCLUDETEXT</xsl:when>
			<xsl:when test="$type = 'FieldIndex'">INDEX</xsl:when>
			<xsl:when test="$type = 'FieldInfo'">INFO</xsl:when>
			<xsl:when test="$type = 'FieldKeyWord'">KEYWORDS</xsl:when>
			<xsl:when test="$type = 'FieldLastSavedBy'">LASTSAVEDBY</xsl:when>
			<xsl:when test="$type = 'FieldLink'">LINK</xsl:when>
			<xsl:when test="$type = 'FieldListNum'">LISTNUM</xsl:when>
			<xsl:when test="$type = 'FieldMacroButton'">MACROBUTTON</xsl:when>
			<!--<xsl:when test="$type = 'FieldMergeField'">MERGEFIELD</xsl:when>-->
			<!--<xsl:when test="$type = 'FieldMergeRec'">MERGEREC</xsl:when>-->
			<!--<xsl:when test="$type = 'FieldMergeSeq'">MERGESEQ</xsl:when>-->
			<!--<xsl:when test="$type = 'FieldNext'">NEXT</xsl:when>-->
			<!--<xsl:when test="$type = 'FieldNextIf'">NEXTIF</xsl:when>-->
			<xsl:when test="$type = 'FieldNoteRef'">NOTEREF</xsl:when>
			<xsl:when test="$type = 'FieldNumChars'">NUMCHARS</xsl:when>
			<xsl:when test="$type = 'FieldNumPages'">NUMPAGES</xsl:when>
			<xsl:when test="$type = 'FieldNumWords'">NUMWORDS</xsl:when>
			<xsl:when test="$type = 'FieldPage'">PAGE</xsl:when>
			<xsl:when test="$type = 'FieldPageRef'">PAGEREF</xsl:when>
			<xsl:when test="$type = 'FieldPrint'">PRINT</xsl:when>
			<xsl:when test="$type = 'FieldPrintDate'">PRINTDATE</xsl:when>
			<xsl:when test="$type = 'FieldPrivate'">PRIVATE</xsl:when>
			<xsl:when test="$type = 'FieldQuote'">QUOTE</xsl:when>
			<xsl:when test="$type = 'FieldRef'">REF</xsl:when>
			<xsl:when test="$type = 'FieldRevisionNum'">REVNUM</xsl:when>
			<xsl:when test="$type = 'FieldSaveDate'">SAVEDATE</xsl:when>
			<xsl:when test="$type = 'FieldSection'">SECTION</xsl:when>
			<xsl:when test="$type = 'FieldSectionPages'">SECTIONPAGES</xsl:when>
			<xsl:when test="$type = 'FieldSequence'">SEQ</xsl:when>
			<!--<xsl:when test="$type = 'FieldSet'">SET</xsl:when>-->
			<!--<xsl:when test="$type = 'FieldSkipIf'">SKIPIF</xsl:when>-->
			<xsl:when test="$type = 'FieldStyleRef'">STYLEREF</xsl:when>
			<xsl:when test="$type = 'FieldSubject'">SUBJECT</xsl:when>
			<xsl:when test="$type = 'FieldSymbol'">SYMBOL</xsl:when>
			<xsl:when test="$type = 'FieldTemplate'">TEMPLATE</xsl:when>
			<xsl:when test="$type = 'FieldTime'">TIME</xsl:when>
			<xsl:when test="$type = 'FieldTitle'">TITLE</xsl:when>
			<xsl:when test="$type = 'FieldTOA'">TOA</xsl:when>
			<xsl:when test="$type = 'FieldTOC'">TOC</xsl:when>
			<xsl:when test="$type = 'FieldUserAddress'">USERADDRESS</xsl:when>
			<xsl:when test="$type = 'FieldUserInitials'">USERINITIALS</xsl:when>
			<xsl:when test="$type = 'FieldUserName'">USERNAME</xsl:when>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="FieldTextFormat">
		<xsl:param name="format"></xsl:param>

		<xsl:choose>
			<xsl:when test="$format = 'Uppercase'">\* Upper</xsl:when>
			<xsl:when test="$format = 'Lowercase'">\* Lower</xsl:when>
			<xsl:when test="$format = 'FirstCapital'">\* FirstCap</xsl:when>
			<xsl:when test="$format = 'Titlecase'">\* Caps</xsl:when>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="Field">
		<xsl:param name="fieldCharType"></xsl:param>

		<xsl:attribute name="w:fldCharType">
			<xsl:value-of select="$fieldCharType" />
		</xsl:attribute>
	</xsl:template>
	<xsl:template match="item" mode="fieldText">
		<xsl:param name="id"></xsl:param>
		<w:r>
			<xsl:choose>
				<xsl:when test="count(@FieldType) > 0">
					<xsl:apply-templates select="." mode="field" />
				</xsl:when>
				<xsl:when test="count(@FieldMarkType) > 0">
					<xsl:apply-templates select="." mode="fieldMark" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates select="character-format" />
					<xsl:apply-templates select="text" />
				</xsl:otherwise>
			</xsl:choose>
		</w:r>
		<xsl:if test="following::item[1][@ParentFieldId >= $id or @id = $id]">
			<xsl:apply-templates select="following::item[1]" mode="fieldText">
				<xsl:with-param name="id">
					<xsl:value-of select="$id" />
				</xsl:with-param>
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>
	<xsl:template match="item | toc-field" mode="field">
		<xsl:if test="(@FieldType != 'FieldAsk'
							and @FieldType != 'FieldCompare'
							and @FieldType != 'FieldDatabase'
							and @FieldType != 'FieldFillIn'
							and @FieldType != 'FieldIf'
							and @FieldType != 'FieldMergeRec'
							and @FieldType != 'FieldMergeSeq'
							and @FieldType != 'FieldNext'
							and @FieldType != 'FieldNextIf'
							and @FieldType != 'FieldSet'
							and @FieldType != 'FieldSkipIf'
							and @FieldType != 'FieldShape')
					or (count(@FieldType) = 0)">
			<xsl:variable name="fieldId">
				<xsl:value-of select="@id" />
			</xsl:variable>
			<xsl:variable name="parentFieldId">
				<xsl:value-of select="@ParentFieldId" />
			</xsl:variable>
			<xsl:choose>
				<xsl:when test="@FieldType = 'FieldHyperlink'">
					<w:hyperlink w:history="1"  r:id="">
						<xsl:attribute name="target">
							<xsl:value-of select="translate(@FieldValue,'&quot;','')" />
						</xsl:attribute>
						<xsl:if test="count(@ParentFieldId)>0 and
									preceding::item[@id = $parentFieldId][@type = 'TOC']">
							<xsl:attribute name="w:anchor">
								<xsl:value-of select="translate(@FieldValue,'&quot;','')" />
							</xsl:attribute>
						</xsl:if>
						<xsl:apply-templates select="following-sibling::item[1]" mode="fieldText">
							<xsl:with-param name="id">
								<xsl:value-of select="$fieldId" />
							</xsl:with-param>
						</xsl:apply-templates>
					</w:hyperlink>
				</xsl:when>
				<xsl:when test="contains(character-format/internal-data, 'AgiB')">
					<xsl:if test="not(contains(preceding-sibling::item[not(@type)][1]/character-format/internal-data, 'AgiB'))">
					<w:r>
							<xsl:apply-templates select="character-format" />
							<w:fldChar w:fldCharType="begin" />
						</w:r>
					</xsl:if>
					<w:r>
						<xsl:apply-templates select="character-format" />
						<w:instrText xml:space="preserve"><xsl:value-of select="text" /></w:instrText>
					</w:r>
					<xsl:if test="not(contains(following::item[not(@type)][1]/character-format/internal-data, 'AgiB'))">
						
						<w:r>
							<xsl:apply-templates select="character-format" />
							<w:fldChar w:fldCharType="end" />
						</w:r>
					</xsl:if>
				</xsl:when>
				<xsl:otherwise>
					<w:r>
						<xsl:apply-templates select="character-format" />
						<w:fldChar>
							<xsl:call-template name="Field">
								<xsl:with-param name="fieldCharType">begin</xsl:with-param>
							</xsl:call-template>
						</w:fldChar>
					</w:r>
					<w:r>
						<xsl:apply-templates select="character-format" />
						<xsl:if test="@FieldType != ''">
							<xsl:variable name="fieldFormatting">
								<xsl:value-of select="@FieldFormatting" />
							</xsl:variable>
							<w:instrText>
								<xsl:call-template name="FieldType">
									<xsl:with-param name="type">
										<xsl:value-of select="@FieldType" />
									</xsl:with-param>
								</xsl:call-template>
								<xsl:text> </xsl:text>
								<xsl:call-template name="FieldTextFormat">
									<xsl:with-param name="format">
										<xsl:value-of select="@TextFormat" />
									</xsl:with-param>
								</xsl:call-template>
								<xsl:choose>
									<xsl:when test="contains(@FieldValue,'&#x1;')">
										<xsl:value-of select="substring-before(@FieldValue,'&#x1;')" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="@FieldValue" />
									</xsl:otherwise>
								</xsl:choose>
								<xsl:text> </xsl:text>
								<xsl:value-of select="$fieldFormatting" />
							</w:instrText>
						</xsl:if>
					</w:r>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>
	<xsl:template match="item" mode="fieldMark">
		<xsl:if test="(preceding::item[@FieldType][1][@FieldType != 'FieldAsk'
							and @FieldType != 'FieldCompare'
							and @FieldType != 'FieldDatabase'
							and @FieldType != 'FieldFillIn'
							and @FieldType != 'FieldIf'
							and @FieldType != 'FieldMergeRec'
							and @FieldType != 'FieldMergeSeq'
							and @FieldType != 'FieldNext'
							and @FieldType != 'FieldNextIf'
							and @FieldType != 'FieldSet'
							and @FieldType != 'FieldSkipIf'
							and @FieldType != 'FieldShape']) or
					(preceding::item[1]/@type = 'TOC')">
			<xsl:variable name="fieldId">
				<xsl:value-of select="@id" />
			</xsl:variable>
			<xsl:choose>
				<xsl:when test="(preceding::item[@id = $fieldId][@FieldType != 'FieldHyperlink' or @type = 'TOC'])">
					<w:r>
						<xsl:apply-templates select="character-format" />
						<w:fldChar>
							<xsl:call-template name="Field">
								<xsl:with-param name="fieldCharType">
									<xsl:choose>
										<xsl:when test="@FieldMarkType = 'FieldSeparator'">separate</xsl:when>
										<xsl:when test="@FieldMarkType = 'FieldEnd'">end</xsl:when>
									</xsl:choose>
								</xsl:with-param>
							</xsl:call-template>
						</w:fldChar>
					</w:r>
					<xsl:if test="@FieldMarkType = 'FieldSeparator' and preceding-sibling::item[@FieldType][@type = 'TextFormField']">
						<w:r>
							<xsl:apply-templates select="preceding-sibling::item/text-range/character-format" />
							<xsl:apply-templates select="preceding-sibling::item/text-range/text" />
						</w:r>
					</xsl:if>
				</xsl:when>
			</xsl:choose>
		</xsl:if>
	</xsl:template>
	<xsl:template match="item" mode="FormFieldData">
		<w:entryMacro>
			<xsl:attribute name="w:val">
				<xsl:value-of select="@MacroOnStart" />
			</xsl:attribute>
		</w:entryMacro>
		<w:exitMacro>
			<xsl:attribute name="w:val">
				<xsl:value-of select="@MacroOnEnd" />
			</xsl:attribute>
		</w:exitMacro>
		<xsl:if test="@Help != ''">
			<w:helpText>
				<xsl:attribute name="w:type">
					<xsl:choose>
						<xsl:when test="@HelpType = 'text'">text</xsl:when>
						<xsl:otherwise>autoText</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:attribute name="w:val">
					<xsl:value-of select="@Help" />
				</xsl:attribute>
			</w:helpText>
		</xsl:if>
		<xsl:if test="@Tooltip != ''">
			<w:statusText>
				<xsl:attribute name="w:type">
					<xsl:choose>
						<xsl:when test="@StatusType = 'text'">text</xsl:when>
						<xsl:otherwise>autoText</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:attribute name="w:val">
					<xsl:value-of select="@Tooltip" />
				</xsl:attribute>
			</w:statusText>
		</xsl:if>
		<xsl:if test="count(@Enabled) = 0">
			<w:enabled w:val="0" />
		</xsl:if>
		<xsl:if test="count(@CalculateOnExit) = 0">
			<w:calcOnExit />
		</xsl:if>
		<xsl:choose>
			<xsl:when test="@FieldType = 'FieldFormTextInput'">
				<w:textInput>
					<xsl:if test="@MaxLength > 0">
						<w:maxLength>
							<xsl:attribute name="w:val">
								<xsl:value-of select="@MaxLength" />
							</xsl:attribute>
						</w:maxLength>
					</xsl:if>
					<w:type>
						<xsl:attribute name="w:val">
							<xsl:choose>
								<xsl:when test="@TextType = 0">regular</xsl:when>
								<xsl:when test="@TextType = 1">number</xsl:when>
								<xsl:when test="@TextType = 2">date</xsl:when>
							</xsl:choose>
						</xsl:attribute>
					</w:type>
					<w:format>
						<xsl:attribute name="w:val">
							<xsl:value-of select="@TextFormat" />
						</xsl:attribute>
					</w:format>
					<w:default>
						<xsl:attribute name="w:val">
							<xsl:value-of select="@DefaultText" />
						</xsl:attribute>
					</w:default>
				</w:textInput>
			</xsl:when>
			<xsl:when test="@FieldType = 'FieldFormCheckBox'">
				<w:checkBox>
					<w:default>
						<xsl:attribute name="w:val">
							<xsl:value-of select="@DefaultCheckBoxValue" />
						</xsl:attribute>
					</w:default>
					<xsl:choose>
						<xsl:when test="@CheckBoxSizeType = 'Auto'">
							<w:sizeAuto />
						</xsl:when>
						<xsl:otherwise>
							<w:size>
								<xsl:attribute name="w:val">
									<xsl:value-of select="@CheckBoxSize*2" />
								</xsl:attribute>
							</w:size>
						</xsl:otherwise>
					</xsl:choose>
				</w:checkBox>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="dropdown-items">
		<xsl:for-each select="dropdown-items">
			<w:listEntry>
				<xsl:attribute name="w:val">
					<xsl:value-of select="@itemText" />
				</xsl:attribute>
			</w:listEntry>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
