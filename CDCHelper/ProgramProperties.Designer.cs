﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CDCHelper {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class ProgramProperties : global::System.Configuration.ApplicationSettingsBase {
        
        private static ProgramProperties defaultInstance = ((ProgramProperties)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ProgramProperties())));
        
        public static ProgramProperties Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DEV={\\\\wmsinterface\\projects\\xmlfiles}|PRD={\\\\wmsinterface\\projects\\xmlfiles}")]
        public string XMLSearch {
            get {
                return ((string)(this["XMLSearch"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Production {
            get {
                return ((bool)(this["Production"]));
            }
        }
    }
}