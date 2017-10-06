using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Softperson.Tools
{
	public static class ProcessTools
	{
		[DllImport("ntdll.dll")]
		private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
			ref ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);

		/// <summary>
		///     Gets the parent process of the current process.
		/// </summary>
		/// <returns>An instance of the Process class.</returns>
		public static Process GetParentProcess()
		{
			return GetParentProcess(Process.GetCurrentProcess().Handle);
		}

		/// <summary>
		///     Gets the parent process of specified process.
		/// </summary>
		/// <param name="id">The process id.</param>
		/// <returns>An instance of the Process class.</returns>
		public static Process GetParentProcess(int id)
		{
			var process = Process.GetProcessById(id);
			return GetParentProcess(process.Handle);
		}

		/// <summary>
		///     Gets the parent process of a specified process.
		/// </summary>
		/// <param name="handle">The process handle.</param>
		/// <returns>An instance of the Process class.</returns>
		public static Process GetParentProcess(IntPtr handle)
		{
			var pbi = new ProcessBasicInformation();
			int returnLength;
			var status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
			if (status != 0)
				throw new Win32Exception(status);

			try
			{
				return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
			}
			catch (ArgumentException)
			{
				// not found
				return null;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct ProcessBasicInformation
		{
			// These members must match PROCESS_BASIC_INFORMATION
			internal readonly IntPtr Reserved1;
			internal readonly IntPtr PebBaseAddress;
			internal readonly IntPtr Reserved2_0;
			internal readonly IntPtr Reserved2_1;
			internal readonly IntPtr UniqueProcessId;
			internal IntPtr InheritedFromUniqueProcessId;
		}
	}
}