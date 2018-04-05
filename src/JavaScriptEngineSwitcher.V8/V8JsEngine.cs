﻿using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.ClearScript.V8;
using OriginalException = Microsoft.ClearScript.ScriptEngineException;
using OriginalInterruptedException = Microsoft.ClearScript.ScriptInterruptedException;
using OriginalUndefined = Microsoft.ClearScript.Undefined;

using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Core.Constants;
using JavaScriptEngineSwitcher.Core.Extensions;
using JavaScriptEngineSwitcher.Core.Helpers;
using JavaScriptEngineSwitcher.Core.Utilities;

using CoreStrings = JavaScriptEngineSwitcher.Core.Resources.Strings;
using WrapperCompilationException = JavaScriptEngineSwitcher.Core.JsCompilationException;
using WrapperEngineLoadException = JavaScriptEngineSwitcher.Core.JsEngineLoadException;
using WrapperException = JavaScriptEngineSwitcher.Core.JsException;
using WrapperFatalException = JavaScriptEngineSwitcher.Core.JsFatalException;
using WrapperInterruptedException = JavaScriptEngineSwitcher.Core.JsInterruptedException;
using WrapperRuntimeException = JavaScriptEngineSwitcher.Core.JsRuntimeException;
using WrapperScriptException = JavaScriptEngineSwitcher.Core.JsScriptException;

using JavaScriptEngineSwitcher.V8.Constants;
using JavaScriptEngineSwitcher.V8.Resources;

namespace JavaScriptEngineSwitcher.V8
{
	/// <summary>
	/// Adapter for the V8 JS engine (Microsoft ClearScript.V8)
	/// </summary>
	public sealed class V8JsEngine : JsEngineBase
	{
		/// <summary>
		/// Name of JS engine
		/// </summary>
		public const string EngineName = "V8JsEngine";

		/// <summary>
		/// Version of original JS engine
		/// </summary>
		private const string EngineVersion = "6.3.292.48";

		/// <summary>
		/// V8 JS engine
		/// </summary>
		private V8ScriptEngine _jsEngine;

		/// <summary>
		/// ClearScript <code>undefined</code> value
		/// </summary>
		private static OriginalUndefined _originalUndefinedValue;

		/// <summary>
		/// Regular expression for working with the error message with type
		/// </summary>
		private static readonly Regex _errorMessageWithTypeRegex =
			new Regex(@"^(?<type>" + CommonRegExps.JsFullNamePattern + @"):\s+(?<description>[\s\S]+?)$");

		/// <summary>
		/// Regular expression for working with the interface assembly load error message
		/// </summary>
		private static readonly Regex _interfaceAssemblyLoadErrorMessage =
			new Regex(@"^Cannot load V8 interface assembly. " +
				"Load failure information for (?<assemblyFileName>" + CommonRegExps.DocumentNamePattern + "):");

		/// <summary>
		/// Synchronizer of JS engine initialization
		/// </summary>
		private static readonly object _initializationSynchronizer = new object();

		/// <summary>
		/// Flag indicating whether the JS engine is initialized
		/// </summary>
		private static bool _initialized;


		/// <summary>
		/// Constructs an instance of adapter for the V8 JS engine (Microsoft ClearScript.V8)
		/// </summary>
		public V8JsEngine()
			: this(new V8Settings())
		{ }

		/// <summary>
		/// Constructs an instance of adapter for the V8 JS engine (Microsoft ClearScript.V8)
		/// </summary>
		/// <param name="settings">Settings of the V8 JS engine</param>
		public V8JsEngine(V8Settings settings)
		{
			Initialize();

			V8Settings v8Settings = settings ?? new V8Settings();

			var constraints = new V8RuntimeConstraints
			{
				MaxNewSpaceSize = v8Settings.MaxNewSpaceSize,
				MaxOldSpaceSize = v8Settings.MaxOldSpaceSize,
			};

			V8ScriptEngineFlags flags = V8ScriptEngineFlags.None;
			if (v8Settings.AwaitDebuggerAndPauseOnStart)
			{
				flags |= V8ScriptEngineFlags.AwaitDebuggerAndPauseOnStart;
			}
			if (v8Settings.EnableDebugging)
			{
				flags |= V8ScriptEngineFlags.EnableDebugging;
			}
			if (v8Settings.EnableRemoteDebugging)
			{
				flags |= V8ScriptEngineFlags.EnableRemoteDebugging;
			}
			if (v8Settings.DisableGlobalMembers)
			{
				flags |= V8ScriptEngineFlags.DisableGlobalMembers;
			}

			int debugPort = v8Settings.DebugPort;

			try
			{
				_jsEngine = new V8ScriptEngine(constraints, flags, debugPort);
			}
			catch (TypeLoadException e)
			{
				throw WrapTypeLoadException(e);
			}
			catch (Exception e)
			{
				throw JsErrorHelpers.WrapUnknownEngineLoadException(e, EngineName, EngineVersion);
			}

			_jsEngine.MaxRuntimeHeapSize = v8Settings.MaxHeapSize;
			_jsEngine.RuntimeHeapSizeSampleInterval = v8Settings.HeapSizeSampleInterval;
			_jsEngine.MaxRuntimeStackUsage = v8Settings.MaxStackUsage;
		}


		/// <summary>
		/// Initializes a JS engine
		/// </summary>
		private static void Initialize()
		{
			if (_initialized)
			{
				return;
			}

			lock (_initializationSynchronizer)
			{
				if (_initialized)
				{
					return;
				}

				AssemblyResolver.Initialize();

				try
				{
					LoadUndefinedValue();
				}
				catch (InvalidOperationException e)
				{
					string message = string.Format(CoreStrings.Engine_JsEngineNotLoaded, EngineName) + " " +
						e.Message;

					throw new WrapperEngineLoadException(message, EngineName, EngineVersion, e);
				}

				_initialized = true;
			}
		}

		/// <summary>
		/// Loads a ClearScript <code>undefined</code> value
		/// </summary>
		private static void LoadUndefinedValue()
		{
			FieldInfo undefinedValueFieldInfo = typeof(OriginalUndefined).GetField("Value",
				BindingFlags.NonPublic | BindingFlags.Static);
			OriginalUndefined originalUndefinedValue = null;

			if (undefinedValueFieldInfo != null)
			{
				originalUndefinedValue = undefinedValueFieldInfo.GetValue(null) as OriginalUndefined;
			}

			if (originalUndefinedValue != null)
			{
				_originalUndefinedValue = originalUndefinedValue;
			}
			else
			{
				throw new InvalidOperationException(Strings.Engines_ClearScriptUndefinedValueNotLoaded);
			}
		}

		#region Mapping

		/// <summary>
		/// Makes a mapping of value from the host type to a script type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private static object MapToScriptType(object value)
		{
			if (value is Undefined)
			{
				return _originalUndefinedValue;
			}

			return value;
		}

		/// <summary>
		/// Makes a mapping of value from the script type to a host type
		/// </summary>
		/// <param name="value">The source value</param>
		/// <returns>The mapped value</returns>
		private static object MapToHostType(object value)
		{
			if (value is OriginalUndefined)
			{
				return Undefined.Value;
			}

			return value;
		}

		private static WrapperException WrapScriptEngineException(OriginalException originalException)
		{
			WrapperException wrapperException;
			string message = originalException.Message;
			string messageWithErrorLocation = originalException.ErrorDetails;
			string description = message;
			string type = string.Empty;
			string documentName = string.Empty;
			int lineNumber = 0;
			int columnNumber = 0;
			string callStack = string.Empty;
			string sourceFragment = string.Empty;

			if (originalException.IsFatal)
			{
				if (message == "The V8 runtime has exceeded its memory limit")
				{
					wrapperException = new WrapperRuntimeException(message, EngineName, EngineVersion,
						originalException);
				}
				else
				{
					wrapperException = new WrapperFatalException(message, EngineName, EngineVersion,
						originalException);
				}
			}
			else
			{
				Match messageWithTypeMatch = _errorMessageWithTypeRegex.Match(message);
				if (messageWithTypeMatch.Success)
				{
					GroupCollection messageWithTypeGroups = messageWithTypeMatch.Groups;
					type = messageWithTypeGroups["type"].Value;
					description = messageWithTypeGroups["description"].Value;
					var errorLocationItems = new ErrorLocationItem[0];

					if (message.Length < messageWithErrorLocation.Length)
					{
						string errorLocation = messageWithErrorLocation
							.TrimStart(message)
							.TrimStart(new char[] { '\n', '\r' })
							;

						errorLocationItems = JsErrorHelpers.ParseErrorLocation(errorLocation);
						if (errorLocationItems.Length > 0)
						{
							ErrorLocationItem firstErrorLocationItem = errorLocationItems[0];

							documentName = firstErrorLocationItem.DocumentName;
							lineNumber = firstErrorLocationItem.LineNumber;
							columnNumber = firstErrorLocationItem.ColumnNumber;
							string sourceLine = firstErrorLocationItem.SourceFragment;
							sourceFragment = JsErrorHelpers.GetSourceFragment(sourceLine, columnNumber);

							firstErrorLocationItem.SourceFragment = sourceFragment;
						}
					}

					WrapperScriptException wrapperScriptException;
					if (type == JsErrorType.Syntax)
					{
						message = JsErrorHelpers.GenerateErrorMessage(type, description, documentName,
							lineNumber, columnNumber, sourceFragment);

						wrapperScriptException = new WrapperCompilationException(message, EngineName, EngineVersion,
							originalException);
					}
					else
					{
						callStack = JsErrorHelpers.StringifyErrorLocationItems(errorLocationItems, true);
						string callStackWithSourceFragment = JsErrorHelpers.StringifyErrorLocationItems(
							errorLocationItems);
						message = JsErrorHelpers.GenerateErrorMessage(type, description,
							callStackWithSourceFragment);

						wrapperScriptException = new WrapperRuntimeException(message, EngineName, EngineVersion,
							originalException)
						{
							CallStack = callStack
						};
					}

					wrapperScriptException.Type = type;
					wrapperScriptException.DocumentName = documentName;
					wrapperScriptException.LineNumber = lineNumber;
					wrapperScriptException.ColumnNumber = columnNumber;
					wrapperScriptException.SourceFragment = sourceFragment;

					wrapperException = wrapperScriptException;
				}
				else
				{
					wrapperException = new WrapperException(message, EngineName, EngineVersion,
						originalException);
				}
			}

			wrapperException.Description = description;

			return wrapperException;
		}

		private static WrapperInterruptedException WrapScriptInterruptedException(
			OriginalInterruptedException originalInterruptedException)
		{
			string message = CoreStrings.Runtime_ScriptInterrupted;
			string description = message;

			var wrapperInterruptedException = new WrapperInterruptedException(message, EngineName, EngineVersion,
				originalInterruptedException)
			{
				Description = description
			}
			;

			return wrapperInterruptedException;
		}

		private static WrapperEngineLoadException WrapTypeLoadException(
			TypeLoadException originalTypeLoadException)
		{
			string originalMessage = originalTypeLoadException.Message;
			string jsEngineNotLoadedPart = string.Format(CoreStrings.Engine_JsEngineNotLoaded, EngineName);
			string message;

			Match errorMessageMatch = _interfaceAssemblyLoadErrorMessage.Match(originalMessage);
			if (errorMessageMatch.Success)
			{
				string assemblyFileName = errorMessageMatch.Groups["assemblyFileName"].Value;

				StringBuilder messageBuilder = StringBuilderPool.GetBuilder();
				messageBuilder.Append(jsEngineNotLoadedPart);
				messageBuilder.Append(" ");

				messageBuilder.AppendFormat(CoreStrings.Engine_AssemblyNotFound, assemblyFileName);
				messageBuilder.Append(" ");

				if (assemblyFileName == DllName.V8Base64Bit || assemblyFileName == DllName.V8Base32Bit)
				{
					messageBuilder.AppendFormat(CoreStrings.Engine_NuGetPackageInstallationRequired,
						assemblyFileName == DllName.V8Base64Bit ?
							"JavaScriptEngineSwitcher.V8.Native.win-x64"
							:
							"JavaScriptEngineSwitcher.V8.Native.win-x86"
						);
					messageBuilder.Append(" ");
					messageBuilder.Append(Strings.Engine_VcRedist2015InstallationRequired);
				}
				else
				{
					messageBuilder.AppendFormat(CoreStrings.Common_SeeOriginalErrorMessage, originalMessage);
				}

				message = messageBuilder.ToString();
				StringBuilderPool.ReleaseBuilder(messageBuilder);
			}
			else
			{
				message = jsEngineNotLoadedPart + " " +
					string.Format(CoreStrings.Common_SeeOriginalErrorMessage, originalMessage);
			}

			return new WrapperEngineLoadException(message, EngineName, EngineVersion, originalTypeLoadException);
		}

		#endregion

		#region JsEngineBase overrides

		protected override object InnerEvaluate(string expression)
		{
			return InnerEvaluate(expression, null);
		}

		protected override object InnerEvaluate(string expression, string documentName)
		{
			object result;

			try
			{
				result = _jsEngine.Evaluate(documentName, false, expression);
			}
			catch (OriginalException e)
			{
				throw WrapScriptEngineException(e);
			}
			catch (OriginalInterruptedException e)
			{
				throw WrapScriptInterruptedException(e);
			}

			result = MapToHostType(result);

			return result;
		}

		protected override T InnerEvaluate<T>(string expression)
		{
			return InnerEvaluate<T>(expression, null);
		}

		protected override T InnerEvaluate<T>(string expression, string documentName)
		{
			object result = InnerEvaluate(expression, documentName);

			return TypeConverter.ConvertToType<T>(result);
		}

		protected override void InnerExecute(string code)
		{
			InnerExecute(code, null);
		}

		protected override void InnerExecute(string code, string documentName)
		{
			try
			{
				_jsEngine.Execute(documentName, false, code);
			}
			catch (OriginalException e)
			{
				throw WrapScriptEngineException(e);
			}
			catch (OriginalInterruptedException e)
			{
				throw WrapScriptInterruptedException(e);
			}
		}

		protected override object InnerCallFunction(string functionName, params object[] args)
		{
			object result;
			int argumentCount = args.Length;
			var processedArgs = new object[argumentCount];

			if (argumentCount > 0)
			{
				for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
				{
					processedArgs[argumentIndex] = MapToScriptType(args[argumentIndex]);
				}
			}

			try
			{
				result = _jsEngine.Invoke(functionName, processedArgs);
			}
			catch (OriginalException e)
			{
				throw WrapScriptEngineException(e);
			}
			catch (OriginalInterruptedException e)
			{
				throw WrapScriptInterruptedException(e);
			}

			result = MapToHostType(result);

			return result;
		}

		protected override T InnerCallFunction<T>(string functionName, params object[] args)
		{
			object result = InnerCallFunction(functionName, args);

			return TypeConverter.ConvertToType<T>(result);
		}

		protected override bool InnerHasVariable(string variableName)
		{
			string expression = string.Format("(typeof {0} !== 'undefined');", variableName);
			var result = InnerEvaluate<bool>(expression);

			return result;
		}

		protected override object InnerGetVariableValue(string variableName)
		{
			object result;

			try
			{
				result = _jsEngine.Script[variableName];
			}
			catch (OriginalException e)
			{
				throw WrapScriptEngineException(e);
			}
			catch (OriginalInterruptedException e)
			{
				throw WrapScriptInterruptedException(e);
			}

			result = MapToHostType(result);

			return result;
		}

		protected override T InnerGetVariableValue<T>(string variableName)
		{
			object result = InnerGetVariableValue(variableName);

			return TypeConverter.ConvertToType<T>(result);
		}

		protected override void InnerSetVariableValue(string variableName, object value)
		{
			object processedValue = MapToScriptType(value);

			try
			{
				_jsEngine.Script[variableName] = processedValue;
			}
			catch (OriginalException e)
			{
				throw WrapScriptEngineException(e);
			}
			catch (OriginalInterruptedException e)
			{
				throw WrapScriptInterruptedException(e);
			}
		}

		protected override void InnerRemoveVariable(string variableName)
		{
			InnerSetVariableValue(variableName, Undefined.Value);
		}

		protected override void InnerEmbedHostObject(string itemName, object value)
		{
			object processedValue = MapToScriptType(value);

			try
			{
				_jsEngine.AddHostObject(itemName, processedValue);
			}
			catch (OriginalException e)
			{
				throw WrapScriptEngineException(e);
			}
			catch (OriginalInterruptedException e)
			{
				throw WrapScriptInterruptedException(e);
			}
		}

		protected override void InnerEmbedHostType(string itemName, Type type)
		{
			try
			{
				_jsEngine.AddHostType(itemName, type);
			}
			catch (OriginalException e)
			{
				throw WrapScriptEngineException(e);
			}
			catch (OriginalInterruptedException e)
			{
				throw WrapScriptInterruptedException(e);
			}
		}

		protected override void InnerInterrupt()
		{
			_jsEngine.Interrupt();
		}

		protected override void InnerCollectGarbage()
		{
			_jsEngine.CollectGarbage(true);
		}

		#region IJsEngine implementation

		/// <summary>
		/// Gets a name of JS engine
		/// </summary>
		public override string Name
		{
			get { return EngineName; }
		}

		/// <summary>
		/// Gets a version of original JS engine
		/// </summary>
		public override string Version
		{
			get { return EngineVersion; }
		}

		/// <summary>
		/// Gets a value that indicates if the JS engine supports script interruption
		/// </summary>
		public override bool SupportsScriptInterruption
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value that indicates if the JS engine supports garbage collection
		/// </summary>
		public override bool SupportsGarbageCollection
		{
			get { return true; }
		}

		#endregion

		#region IDisposable implementation

		public override void Dispose()
		{
			if (_disposedFlag.Set())
			{
				if (_jsEngine != null)
				{
					_jsEngine.Dispose();
					_jsEngine = null;
				}
			}
		}

		#endregion

		#endregion
	}
}