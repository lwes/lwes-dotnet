﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3082
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Org.Lwes.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Org.Lwes.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Already initialized.
        /// </summary>
        internal static string Error_AlreadyInitialized {
            get {
                return ResourceManager.GetString("Error_AlreadyInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attribute not defined: {0}.
        /// </summary>
        internal static string Error_AttributeNotDefined {
            get {
                return ResourceManager.GetString("Error_AttributeNotDefined", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Buffer does not have enough space to complete the operation.
        /// </summary>
        internal static string Error_BufferOutOfSpace {
            get {
                return ResourceManager.GetString("Error_BufferOutOfSpace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Emitter enters a shutdown state; operation no longer valid.
        /// </summary>
        internal static string Error_EmitterHasEnteredShutdownState {
            get {
                return ResourceManager.GetString("Error_EmitterHasEnteredShutdownState", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Empty string not allowed.
        /// </summary>
        internal static string Error_EmptyStringNotAllowed {
            get {
                return ResourceManager.GetString("Error_EmptyStringNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An event sink threw an uncaught exception. Registration key and data are attached..
        /// </summary>
        internal static string Error_EventSinkThrewException {
            get {
                return ResourceManager.GetString("Error_EventSinkThrewException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The configured IoCAdapter threw the following exception.
        /// </summary>
        internal static string Error_IocAdapterFailure {
            get {
                return ResourceManager.GetString("Error_IocAdapterFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not yet initialized.
        /// </summary>
        internal static string Error_NotYetInitialized {
            get {
                return ResourceManager.GetString("Error_NotYetInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unexpected error occured while disposing of a journaler; error to follow..
        /// </summary>
        internal static string Error_UnexpectedErrorDisposingJournaler {
            get {
                return ResourceManager.GetString("Error_UnexpectedErrorDisposingJournaler", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encountered an unrecognized encoding: {0}.
        /// </summary>
        internal static string Error_UnrecognizedEncoding {
            get {
                return ResourceManager.GetString("Error_UnrecognizedEncoding", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A duplicate event template has been encountered while processing ESF files; another ESF file probably contains an event by the same name:
        ///event={0}
        ///file={1}.
        /// </summary>
        internal static string Warning_DuplicateEventTemplateFromESF {
            get {
                return ResourceManager.GetString("Warning_DuplicateEventTemplateFromESF", resourceCulture);
            }
        }
    }
}
