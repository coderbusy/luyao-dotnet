<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="yes"/>
	<xsl:template match="/">
		<TestXmlModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
			<TitleList>
				<xsl:for-each select="catalog/cd">
					<string>
						<xsl:value-of select="title"/>
					</string>
				</xsl:for-each>
			</TitleList>
		</TestXmlModel>
	</xsl:template>
</xsl:stylesheet>