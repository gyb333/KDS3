﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本: 15.0.0.0
//  
//     对此文件的更改可能导致不正确的行为，如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Wilmar.Build.Core.Service.Default.ServiceTemplate
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using Wilmar.Model.Core.Definitions;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class ProjectServiceTemplate : ProjectServiceTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("using Microsoft.OData.Edm;\r\nusing System;\r\nusing System.Web.OData.Builder;\r\nusing" +
                    " System.Collections.Generic;\r\nusing Wilmar.Service.Common.ProjectBase;\r\nusing Wi" +
                    "lmar.");
            
            #line 12 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.Identity));
            
            #line default
            #line hidden
            this.Write(".Service.Models;\r\n\r\nnamespace Wilmar.");
            
            #line 14 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.Identity));
            
            #line default
            #line hidden
            this.Write(".Service\r\n{\r\n\tpublic class ");
            
            #line 16 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.Identity));
            
            #line default
            #line hidden
            this.Write("ProjectService : ProjectServiceBase\r\n\t{\r\n\t\tpublic override string Identity\r\n\t\t{\r\n" +
                    "\t\t\tget{ return  \"");
            
            #line 20 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Data.Identity.ToLower()));
            
            #line default
            #line hidden
            this.Write("\";}\r\n\t\t}\r\n\r\n\t\tpublic override void InitEdmModel(ODataConventionModelBuilder build" +
                    "er)\r\n\t\t{\r\n");
            
            #line 25 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
	foreach (var item in Data.Entitys) { 
            
            #line default
            #line hidden
            
            #line 26 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
 if(item.IsDimFast) { 
            
            #line default
            #line hidden
            this.Write("\t\t\tbuilder.EntitySet<");
            
            #line 27 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(item.ClassName));
            
            #line default
            #line hidden
            this.Write("DimProxy>(\"");
            
            #line 27 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(item.ClassName));
            
            #line default
            #line hidden
            this.Write("s\");\r\n");
            
            #line 28 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
	} else { 
            
            #line default
            #line hidden
            this.Write("\t\t\tbuilder.EntitySet<");
            
            #line 29 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(item.ClassName));
            
            #line default
            #line hidden
            this.Write(">(\"");
            
            #line 29 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(item.ClassName));
            
            #line default
            #line hidden
            this.Write("s\");\r\n");
            
            #line 30 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
	} 
            
            #line default
            #line hidden
            
            #line 31 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
	} 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 33 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
	int index = 1;
	foreach (var action in Data.Actions) { var field = "_aa" + (index++).ToString(); 
            
            #line default
            #line hidden
            this.Write("\t\t\tvar ");
            
            #line 35 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(field));
            
            #line default
            #line hidden
            this.Write(" = builder.EntityType<");
            
            #line 35 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(action.Entity.ClassName));
            
            #line default
            #line hidden
            this.Write(">()");
            
            #line 35 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(action.Source.IsCollectionInvoke ? ".Collection" : ""));
            
            #line default
            #line hidden
            this.Write(".Action(\"");
            
            #line 35 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(action.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n");
            
            #line 36 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
		foreach (var para in action.Parameters) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 37 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(field));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 37 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateParameterPrefix(para.Value)));
            
            #line default
            #line hidden
            this.Write("Parameter<");
            
            #line 37 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateElementType(para.Value)));
            
            #line default
            #line hidden
            this.Write(">(\"");
            
            #line 37 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(para.Key));
            
            #line default
            #line hidden
            this.Write("\");\r\n");
            
            #line 38 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
		} 
            
            #line default
            #line hidden
            
            #line 39 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
		if (action.ReturnType != null) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 40 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(field));
            
            #line default
            #line hidden
            this.Write(".Returns");
            
            #line 40 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateReturnsPrefix(action.ReturnType)));
            
            #line default
            #line hidden
            this.Write("<");
            
            #line 40 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateElementType(action.ReturnType)));
            
            #line default
            #line hidden
            this.Write(">(");
            
            #line 40 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateReturnsEntityName(action.ReturnType)));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 41 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
		} 
            
            #line default
            #line hidden
            
            #line 42 "D:\works\KDS3_NEW\KDS3\code\Wilmar.Platform\src\Wilmar.Build.Core\Service\Default\ServiceTemplate\ProjectServiceTemplate.tt"
	} 
            
            #line default
            #line hidden
            this.Write("\t\t}\r\n\t}\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class ProjectServiceTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
