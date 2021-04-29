using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Case.Business
{
	[XmlRoot(ElementName = "packages")]
	public class Packages
	{

		[XmlElement(ElementName = "package")]
		public List<string> Package { get; set; }
	}

	[XmlRoot(ElementName = "xml")]
	public class API3RequestDTO
	{
		[XmlElement(ElementName = "source")]
		public string Source { get; set; }

		[XmlElement(ElementName = "destination")]
		public string Destination { get; set; }

		[XmlElement(ElementName = "packages")]
		public Packages Packages { get; set; }
	}

	[XmlRoot(ElementName = "xml")]
	public class API3ResponseDTO {

		[XmlElement(ElementName = "quote")]
		public string Quote { get; set; }
	}
}
