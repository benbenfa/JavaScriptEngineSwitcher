﻿using System;

using JavaScriptEngineSwitcher.Core.Utilities;

namespace JavaScriptEngineSwitcher.ChakraCore
{
	/// <summary>
	/// Settings of the ChakraCore JS engine
	/// </summary>
	public sealed class ChakraCoreSettings
	{
		/// <summary>
		/// Gets or sets a flag for whether to disable any background work (such as garbage collection)
		/// on background threads
		/// </summary>
		public bool DisableBackgroundWork
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a flag for whether to disable calls of <code>eval</code> function
		/// </summary>
		public bool DisableEval
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a flag for whether to disable executable page allocation
		/// </summary>
		/// <remarks>
		/// <para>
		/// This also implies that Native Code generation will be turned off.
		/// </para>
		/// <para>
		/// Note that this will break JavaScript stack decoding in tools like WPA since they
		/// rely on allocation of unique thunks to interpret each function and allocation of
		/// those thunks will be disabled as well.
		/// </para>
		/// </remarks>
		public bool DisableExecutablePageAllocation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a flag for whether to disable native code generation
		/// </summary>
		public bool DisableNativeCodeGeneration
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a flag for whether to disable Failfast fatal error on OOM
		/// </summary>
		public bool DisableFatalOnOOM
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a flag for whether to enable all experimental features
		/// </summary>
		public bool EnableExperimentalFeatures
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a current memory limit for a runtime in bytes
		/// </summary>
		public UIntPtr MemoryLimit
		{
			get;
			set;
		}


		/// <summary>
		/// Constructs an instance of the ChakraCore settings
		/// </summary>
		public ChakraCoreSettings()
		{
			DisableBackgroundWork = false;
			DisableEval = false;
			DisableExecutablePageAllocation = false;
			DisableNativeCodeGeneration = false;
			DisableFatalOnOOM = false;
			EnableExperimentalFeatures = false;
			MemoryLimit = Utils.Is64BitProcess() ?
				new UIntPtr(ulong.MaxValue) : new UIntPtr(uint.MaxValue);
		}
	}
}