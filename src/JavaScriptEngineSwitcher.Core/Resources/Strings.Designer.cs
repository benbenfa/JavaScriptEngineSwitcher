//------------------------------------------------------------------------------
// <auto-generated>
//	 This code was generated by a tool.
//
//	 Changes to this file may cause incorrect behavior and will be lost if
//	 the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace JavaScriptEngineSwitcher.Core.Resources
{
	using System;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;

	/// <summary>
	/// A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	public class Strings
	{
		private static Lazy<ResourceManager> _resourceManager =
			new Lazy<ResourceManager>(() => new ResourceManager(
				"JavaScriptEngineSwitcher.Core.Resources.Strings",
#if NET40
				typeof(Strings).Assembly
#else
				typeof(Strings).GetTypeInfo().Assembly
#endif
			));

		private static CultureInfo _resourceCulture;

		/// <summary>
		/// Returns a cached ResourceManager instance used by this class
		/// </summary>
		public static ResourceManager ResourceManager
		{
			get
			{
				return _resourceManager.Value;
			}
		}

		/// <summary>
		/// Overrides a current thread's CurrentUICulture property for all
		/// resource lookups using this strongly typed resource class
		/// </summary>
		public static CultureInfo Culture
		{
			get
			{
				return _resourceCulture;
			}
			set
			{
				_resourceCulture = value;
			}
		}

		/// <summary>
		/// Looks up a localized string similar to "The parameter '{0}' must be a non-empty string."
		/// </summary>
		public static string Common_ArgumentIsEmpty
		{
			get { return GetString("Common_ArgumentIsEmpty"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The parameter '{0}' must be a non-nullable."
		/// </summary>
		public static string Common_ArgumentIsNull
		{
			get { return GetString("Common_ArgumentIsNull"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Cannot convert object of type `{0}` to type `{1}`."
		/// </summary>
		public static string Common_CannotConvertObjectToType
		{
			get { return GetString("Common_CannotConvertObjectToType"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Can not convert value '{0}' of enumeration type `{1}` to value of enumeration type `{2}`."
		/// </summary>
		public static string Common_EnumValueConversionFailed
		{
			get { return GetString("Common_EnumValueConversionFailed"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "File '{0}' not exist."
		/// </summary>
		public static string Common_FileNotExist
		{
			get { return GetString("Common_FileNotExist"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Value cannot be empty."
		/// </summary>
		public static string Common_ValueIsEmpty
		{
			get { return GetString("Common_ValueIsEmpty"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Value cannot be null."
		/// </summary>
		public static string Common_ValueIsNull
		{
			get { return GetString("Common_ValueIsNull"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Cannot convert null to a value type."
		/// </summary>
		public static string Common_ValueTypeCannotBeNull
		{
			get { return GetString("Common_ValueTypeCannotBeNull"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Name of default JavaScript engine not specified."
		/// </summary>
		public static string Configuration_DefaultJsEngineNameNotSpecified
		{
			get { return GetString("Configuration_DefaultJsEngineNameNotSpecified"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Could not find a factory, that creates an instance of the JavaScript engine with name `{0}`."
		/// </summary>
		public static string Configuration_JsEngineFactoryNotFound
		{
			get { return GetString("Configuration_JsEngineFactoryNotFound"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Category"
		/// </summary>
		public static string ErrorDetails_Category
		{
			get { return GetString("ErrorDetails_Category"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Column number"
		/// </summary>
		public static string ErrorDetails_ColumnNumber
		{
			get { return GetString("ErrorDetails_ColumnNumber"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Engine name"
		/// </summary>
		public static string ErrorDetails_EngineName
		{
			get { return GetString("ErrorDetails_EngineName"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Engine version"
		/// </summary>
		public static string ErrorDetails_EngineVersion
		{
			get { return GetString("ErrorDetails_EngineVersion"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Error code"
		/// </summary>
		public static string ErrorDetails_ErrorCode
		{
			get { return GetString("ErrorDetails_ErrorCode"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Line number"
		/// </summary>
		public static string ErrorDetails_LineNumber
		{
			get { return GetString("ErrorDetails_LineNumber"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Message"
		/// </summary>
		public static string ErrorDetails_Message
		{
			get { return GetString("ErrorDetails_Message"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Source fragment"
		/// </summary>
		public static string ErrorDetails_SourceFragment
		{
			get { return GetString("ErrorDetails_SourceFragment"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Resource with name '{0}' is null."
		/// </summary>
		public static string Resources_ResourceIsNull
		{
			get { return GetString("Resources_ResourceIsNull"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The embedded host object '{0}' has a type `{1}`, which is not supported."
		/// </summary>
		public static string Runtime_EmbeddedHostObjectTypeNotSupported
		{
			get { return GetString("Runtime_EmbeddedHostObjectTypeNotSupported"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The embedded host type `{0}` is not supported."
		/// </summary>
		public static string Runtime_EmbeddedHostTypeNotSupported
		{
			get { return GetString("Runtime_EmbeddedHostTypeNotSupported"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The function with the name '{0}' does not exist."
		/// </summary>
		public static string Runtime_FunctionNotExist
		{
			get { return GetString("Runtime_FunctionNotExist"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "One of the function parameters '{0}' has a type `{1}`, which is not supported."
		/// </summary>
		public static string Runtime_FunctionParameterTypeNotSupported
		{
			get { return GetString("Runtime_FunctionParameterTypeNotSupported"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The function name '{0}' has incorrect format."
		/// </summary>
		public static string Runtime_InvalidFunctionNameFormat
		{
			get { return GetString("Runtime_InvalidFunctionNameFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The script item name '{0}' has incorrect format."
		/// </summary>
		public static string Runtime_InvalidScriptItemNameFormat
		{
			get { return GetString("Runtime_InvalidScriptItemNameFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The variable name '{0}' has incorrect format."
		/// </summary>
		public static string Runtime_InvalidVariableNameFormat
		{
			get { return GetString("Runtime_InvalidVariableNameFormat"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "During loading of {0} error has occurred. See more details: {1}"
		/// </summary>
		public static string Runtime_JsEngineNotLoaded
		{
			get { return GetString("Runtime_JsEngineNotLoaded"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The type of return value `{0}` is not supported."
		/// </summary>
		public static string Runtime_ReturnValueTypeNotSupported
		{
			get { return GetString("Runtime_ReturnValueTypeNotSupported"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "Script execution was interrupted."
		/// </summary>
		public static string Runtime_ScriptInterrupted
		{
			get { return GetString("Runtime_ScriptInterrupted"); }
		}

		/// <summary>
		/// Looks up a localized string similar to "The variable '{0}' has a type `{1}`, which is not supported."
		/// </summary>
		public static string Runtime_VariableTypeNotSupported
		{
			get { return GetString("Runtime_VariableTypeNotSupported"); }
		}

			private static string GetString(string name)
			{
				string value = ResourceManager.GetString(name, _resourceCulture);

				return value;
			}
		}
	}