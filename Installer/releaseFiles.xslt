<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:wix="http://schemas.microsoft.com/wix/2006/wi" version="1.0">
    <xsl:output method="xml" indent="yes" />
    <xsl:template match="@*|*">
        <xsl:copy>
            <xsl:apply-templates select="@*|*" />
        </xsl:copy>
    </xsl:template>
    <!-- Persistent file Id for main executable -->
    <xsl:template match="wix:File[@Source = 'SourceDir\PasteIntoFile.exe']">
        <xsl:copy>
            <xsl:attribute name="Id">
                <xsl:value-of select="&quot;mainExecutable&quot;" />
            </xsl:attribute>
            <xsl:apply-templates select="@*[not(name()='Id')]" />
            <xsl:apply-templates select="*" />
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
