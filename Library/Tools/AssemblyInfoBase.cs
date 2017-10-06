using JetBrains.Annotations;

namespace Softperson
{
	[PublicAPI]
	public static class ProgramInfoBase
	{
		public const string PublicKeyToken = "16c973c253aec6d5";

		public const string PublicKey =
			"00240000048000009400000006020000002400005253413100040000010001005ff1323611d1ac" +
			"aae855d5b0a4d3c85d56cc208b8a7b1e8ffb586388b5a47118afedfd42e810157b731dc1d90bc6" +
			"e77fa41cde2d2d73ef8874e4fdbb641081aac8da0ec18dd6cce665c3cd5da37c8075efaf114eca" +
			"7f2a39ae1d73fc11dcb97e81afbebea9abcfbd93643d1d2048ce2708a3f429c3bede1690e9584d" +
			"d373b6de";

		[PublicAPI] public const string PublicKeyTokenInsert = ", PublicKeyToken=<" + PublicKeyToken + ">";

		public const string PublicKeyInsert = ", PublicKey=" + PublicKey;

		public const string UI = "SoftPerson.UI" + PublicKeyInsert;
		public const string Core = "SoftPerson.Core" + PublicKeyInsert;
		public const string Test = "SoftPerson.Test" + PublicKeyInsert;
		public const string Nlp = "SoftPerson.NLP" + PublicKeyInsert;

		public const string Copyright = "Copyright © 2010, SoftPerson, LLC.";
		public const string Company = "SoftPerson, LLC";
		public const string Trademark = "";
		public const string Product = "";
		public const string Culture = "";

		public const string LibraryVersion = "1.0.0.0";
		public const string ProductVersion = "1.0.0.0";
	}
}