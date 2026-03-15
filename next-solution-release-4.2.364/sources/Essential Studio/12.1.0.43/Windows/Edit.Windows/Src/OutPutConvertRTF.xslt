<?xml version='1.0' encoding="ASCII" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ToRTF="converter"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt">
  <xsl:output method="text" encoding="ascii" indent="no" />
	<xsl:template match="root">{\rtf1\ansi\ansicpg1049\deff0\deflang1049
{\fonttbl
    <xsl:for-each select="formats/format">
      {\f<xsl:value-of select="@ID" /><xsl:text> </xsl:text><xsl:value-of select="@font" />;}
    </xsl:for-each>}
{\colortbl ;<xsl:for-each select="formats/format">\red<xsl:value-of select="@colorRed" />\green<xsl:value-of select="@colorGreen" />\blue<xsl:value-of select="@colorBlue" />;</xsl:for-each>}
<xsl:for-each select="lines/line"><xsl:for-each select="lexem"><xsl:variable name="fname" select="@format" />\cf<xsl:for-each select="/root/formats/format"><xsl:if test="@name=$fname"><xsl:value-of select="position()" /><xsl:choose><xsl:when test="@bold='True'">\b1</xsl:when><xsl:otherwise>\b0</xsl:otherwise></xsl:choose><xsl:choose><xsl:when test="@italic='True'">\i1</xsl:when><xsl:otherwise>\i0</xsl:otherwise></xsl:choose>
\f<xsl:value-of select="@ID" />\fs<xsl:value-of select="ceiling( @fontSize * '2' )" /></xsl:if></xsl:for-each><xsl:text> </xsl:text><xsl:value-of select="ToRTF:RTFEncode(., string(@text), 1)" /></xsl:for-each>\par
</xsl:for-each>}</xsl:template>
  <xsl:template match="@text">
    <xsl:value-of select="ToRTF:RTFEncode(., string(.), 1)" />
  </xsl:template>
  <msxsl:script language="JavaScript" implements-prefix="ToRTF">
    function CharCode(strText)
    {
    var strCharCodes = "###";
    var strSeparator = "";
    for (var intChar = 0; intChar &lt; strText.length; intChar++)
    {
    strCharCodes += strText.charCodeAt(intChar).toString() + strSeparator;
    strSeparator = ","
    }
    strCharCodes += "###";
    return(strCharCodes);
    }
    function RTFEncode(objXMLNodes, strText, intMyNormalizeSpaces)
    {
    // Encode text, character by character
    var blnAppendParagraphBreak = false;

    // Build an array of characters
    var arrChars = strText.split("");
    for (var intChar = 0; intChar &lt; arrChars.length; intChar++)
    {
    var strChar = arrChars[intChar];
    switch (strChar.valueOf())
    {
    case "\\":
    case "{":
    case "}":
    // Encode backslashes, left curly bracket, right curly bracket (prefix with a backslash)
    arrChars[intChar] = "\\" + strChar;
    break;

    case "&#160;":
    // Encode non-breacking space (backslash+tilda)
    arrChars[intChar] = "\\~";
    break;

    case "\n":
    if (intMyNormalizeSpaces == 2)
    {
    // Preformatted mode - use \line for all EOL characters
    arrChars[intChar] = "\\line ";
    // Check if next node is a paragraph - if yes, we will use a paragraph break INSTEAD of line break
    if (objXMLNodes != null &amp;&amp; objXMLNodes.length != 0)
    {
    var objXMLContext = objXMLNodes[0];
    var objNextNode = objXMLContext.selectSingleNode("following-sibling::node()[position() = 1]");
    if (objNextNode != null)
    {
    if (objNextNode.nodeName == "p")
    {
    blnAppendParagraphBreak = true;
    }
    }
    }
    }
    break;

    default:
    var intCharCode = strChar.charCodeAt(0);
    if (intCharCode &gt; 255)
    {
    // Non-ascii: encode as UNICODE (\u)
    arrChars[intChar] = "\\u" + intCharCode.toString() + "  ";
    }
    else
    {
    // TODO Handle control characters (ASCII code lesser than 32 - TAB, EOL, etc...)
    // No encoding
    }
    break;

    }
    }

    // Convert back array to string
    var strRTFEncoded = arrChars.join("");

    if (blnAppendParagraphBreak)
    {
    // Append a paragraph break - next node is a p tag, but we are not inside a p tag (bad!)
    strRTFEncoded += "\\par ";
    }

    return(strRTFEncoded);
    }
  </msxsl:script>
</xsl:stylesheet>
