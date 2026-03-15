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

	<xsl:template match="text-border">
		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:bdr</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$twentiethOfPoint"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*$twentiethOfPoint"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@Color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@Shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<xsl:template name="Border">
		<xsl:param name="name"></xsl:param>
		<xsl:param name="val">
			<xsl:value-of select="$twentiethOfPoint" />
		</xsl:param>
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
							<xsl:when test="$val = 'DashSmallGap'"           >dashSmallGap</xsl:when>
							<xsl:when test="$val = 'Single'"                 >single</xsl:when>
							<xsl:when test="$val = 'Dot'"                    >dotted</xsl:when>
							<xsl:when test="$val = 'DotDash'"                >dotDash</xsl:when>
							<xsl:when test="$val = 'DashLargeGap'"           >dashed</xsl:when>
							<xsl:when test="$val = 'DotDotDash'"             >dotDotDash</xsl:when>
							<xsl:when test="$val = 'Double'"                 >double</xsl:when>
							<xsl:when test="$val = 'ThinThinSmallGap'"       >thickThinSmallGap</xsl:when>
							<xsl:when test="$val = 'ThinThickSmallGap'"      >thinThickSmallGap</xsl:when>
							<xsl:when test="$val = 'ThinThickThinSmallGap'"  >thinThickThinSmallGap</xsl:when>
							<xsl:when test="$val = 'ThickThinMediumGap'"     >thickThinMediumGap</xsl:when>
							<xsl:when test="$val = 'ThinThickMediumGap'"     >thinThickMediumGap</xsl:when>
							<xsl:when test="$val = 'ThickThickThinMediumGap'">thinThickThinMediumGap</xsl:when>
							<xsl:when test="$val = 'ThickThinLargeGap'"      >thickThinLargeGap</xsl:when>
							<xsl:when test="$val = 'ThinThickLargeGap'"      >thinThickLargeGap</xsl:when>
							<xsl:when test="$val = 'ThinThickThinLargeGap'"  >thinThickThinLargeGap</xsl:when>
							<xsl:when test="$val = 'Thick'"					 >thick</xsl:when>
							<xsl:when test="$val = 'Wave'"                   >wave</xsl:when>
							<xsl:when test="$val = 'DoubleWave'"             >doubleWave</xsl:when>
							<xsl:when test="$val = 'DashDotStroker'"         >dashDotStroked</xsl:when>
							<xsl:when test="$val = 'Engrave3D'"              >threeDEngrave</xsl:when>
							<xsl:when test="$val = 'Emboss3D'"               >threeDEmboss</xsl:when>
							<xsl:when test="$val = 'Outset'"                 >outset</xsl:when>
							<xsl:when test="$val = 'Inset'"                  >inset</xsl:when>
							<xsl:when test="$val = 'Cleared'"                >nil</xsl:when>
						</xsl:choose>
					</xsl:attribute>
					<xsl:if test="$val != 255">
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
	<xsl:template match="borders">
		<xsl:param name="multiplier">
			<xsl:value-of select="$twentiethOfPoint" />
		</xsl:param>

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
	<xsl:template match="Top">
		<xsl:param name="multiplier">
			<xsl:value-of select="$twentiethOfPoint" />
		</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:top</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*$twentiethOfPoint"/>
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
		<xsl:param name="multiplier">
			<xsl:value-of select="$twentiethOfPoint" />
		</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:left</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*$twentiethOfPoint"/>
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
		<xsl:param name="multiplier">
			<xsl:value-of select="$twentiethOfPoint" />
		</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:right</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*$twentiethOfPoint"/>
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
		<xsl:param name="multiplier">
			<xsl:value-of select="$twentiethOfPoint" />
		</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:bottom</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*$twentiethOfPoint"/>
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
		<xsl:param name="multiplier">
			<xsl:value-of select="$twentiethOfPoint" />
		</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:insideH</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*$twentiethOfPoint"/>
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
		<xsl:param name="multiplier">
			<xsl:value-of select="$twentiethOfPoint" />
		</xsl:param>

		<xsl:call-template name="Border">
			<xsl:with-param name="name">w:insideV</xsl:with-param>
			<xsl:with-param name="val">
				<xsl:value-of select="@BorderType"/>
			</xsl:with-param>
			<xsl:with-param name="sz">
				<xsl:value-of select="(@LineWidth)*$multiplier"/>
			</xsl:with-param>
			<xsl:with-param name="space">
				<xsl:value-of select="(@Space)*$twentiethOfPoint"/>
			</xsl:with-param>
			<xsl:with-param name="color">
				<xsl:value-of select="@Color"/>
			</xsl:with-param>
			<xsl:with-param name="shadow">
				<xsl:value-of select="@Shadow"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	
</xsl:stylesheet>
	
