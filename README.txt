CMS Store
---------

Verwendete Technologien/Tools:
	* Umbraco for mono 4.7.2 beta mit Anpassungen (von https://github.com/m57j75/umbraco-mono)
	* mono 3.0.3 (trunk) (== .NET 4.5)
	* Ubuntu Precise 12.04 LTS (Entwicklungs- und Deployment-OS)
	* Apache web server 2.2.22
	* MySql 14.14 Distrib 5.5.28 (Debian-Linux)
	* MonoDevelop 3.1.0 (latest)
	* Linode zum Hosten

Hervoragende (und einzige) Quelle für Umbraco auf mono:
	* http://our.umbraco.org/forum/core/general/32923-Umbraco-472-on-Linux?p=0

Anmerkungen zur Umsetzung:
	Um Umbraco auf mono zu betreiben, muss eine mono-Version >= 2.11 verwendet werden. Konkret
	wurde eine eigens kompilierte tip-of-master Version vom 13. od. 14.12.2012 verwendet.
	Zusätzlich mussten einige weitere mono-bezogene Pakete kompiliert werden (alles trunk-
	Versionen vom 13. od. 14.12.2012): gio-sharp, gtk-sharp, libgdiplus, mod_mono, mono-addins,
	mono-tools, xsp.
	Umbraco-mono wurde von https://github.com/m57j75/umbraco-mono in der Version des tags
	"4.7.2-mono-beta-1" bezogen, der momentan stabilsten Version davon. Ein paar kleinere
	Anpassungen des Source Codes waren nötig.
	Umbraco-mono funktioniert gut für Übungszwecke, ist aber nicht für den Produktiveinsatz
	geeignet. Hin-und-wieder treten Fehler auf; manche Funktionen sind nicht verfügbar. Am
	schwerwiegendsten ist, dass es nicht möglich ist, Erweiterungs-Packages zu installieren.
