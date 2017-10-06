using System;
using System.Diagnostics.CodeAnalysis;

// Licence: Everyone is free to use the code contained in this file in any way.

namespace Softperson
{
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotCaptureVariablesAttribute : Attribute
	{
	}

	[DoNotPrune]
	[DoNotObfuscate]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotCaptureAttribute : Attribute
	{
	}

	[AttributeUsage(
		AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Enum |
		AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Module |
		AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotObfuscateAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotObfuscateTypeAttribute : Attribute
	{
	}

	[AttributeUsage(
		AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Delegate |
		AttributeTargets.Enum | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Interface |
		AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Parameter | AttributeTargets.Property |
		AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotPruneAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotPruneTypeAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotSealTypeAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	[ExcludeFromCodeCoverage]
	public sealed class ReportExceptionAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
	[ExcludeFromCodeCoverage]
	public class ReportUsageAttribute : Attribute
	{
		public ReportUsageAttribute()
		{
		}

		public ReportUsageAttribute(string featureName)
		{
		}
	}

	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class ObfuscateControlFlowAttribute : Attribute
	{
	}

	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotObfuscateControlFlowAttribute : Attribute
	{
	}

	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Interface |
		AttributeTargets.Method | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class ObfuscateToAttribute : Attribute
	{
		public ObfuscateToAttribute(string newName)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class ObfuscateNamespaceToAttribute : Attribute
	{
		public ObfuscateNamespaceToAttribute(string newName)
		{
		}
	}

	[AttributeUsage(
		AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor |
		AttributeTargets.Module | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotEncodeStringsAttribute : Attribute
	{
	}

	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class EncodeStringsAttribute : Attribute
	{
	}

	[AttributeUsage(
		AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method |
		AttributeTargets.Module | AttributeTargets.Struct)]
	[ExcludeFromCodeCoverage]
	public sealed class ExcludeFromMemberRefsProxyAttribute : Attribute
	{
	}

	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum |
		AttributeTargets.Delegate)]
	[ExcludeFromCodeCoverage]
	public sealed class StayPublicAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotMoveAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class)]
	[ExcludeFromCodeCoverage]
	public sealed class DoNotMoveMethodsAttribute : Attribute
	{
	}
}