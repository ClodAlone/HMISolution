using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using System.IO;
using System.CodeDom.Compiler;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TextTemplating.Interfaces;



namespace AmazedSaint.Elastic.Templating
{
    /// <summary>
    ///The text template transformation engine is responsible for running 
    ///the transformation process.    
    /// </summary>
    public class DynamicTemplateHost :MarshalByRefObject, ITextTemplatingEngineHost
    {
        //The path and file name of the text template that is being processed.
        string templateFilePath;
        public string TemplateFile
        {
            get { return templateFilePath; }
            set { templateFilePath = value; }
        }


        //This will be the extension of the generated text output file.     
        string fileExtensionValue = ".cs";
        public string FileExtension
        {
            get { return fileExtensionValue; }
            set { fileExtensionValue = value; }
        }


        //This will be the encoding of the generated text output file.      
        private Encoding fileEncodingValue = Encoding.UTF8;

        public Encoding FileEncoding
        {
            get { return fileEncodingValue; }
            set { fileEncodingValue = value; }
        }



        //These are the errors that occur when the engine processes a template.       
        private CompilerErrorCollection errorsValue;
        public CompilerErrorCollection Errors
        {
            get { return errorsValue; }
        }


        public bool EnablePreProcess { get; set; }



        /// <summary>
        /// Standard assembly resources
        /// </summary>
        public virtual IList<string> StandardAssemblyReferences
        {
            get
            {
                return new string[]
                {
                    //If this host searches standard paths and the GAC,
                    //we can specify the assembly name like this.
                    //"System"
                    typeof(System.Uri).Assembly.Location,
                    typeof(System.Xml.Linq.XName).Assembly.Location,
                    typeof(Microsoft.CSharp.CSharpCodeProvider).Assembly.Location,
                    typeof (System.Dynamic.BinaryOperationBinder).Assembly.Location,
                    typeof (System.Xml.NameTable).Assembly.Location,
                    typeof (System.Xml.Serialization.SchemaImporter).Assembly.Location,
                     typeof (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location,
                };
            }
        }



        /// <summary>
        /// Standard Imports
        /// </summary>
        public virtual IList<string> StandardImports
        {
            get
            {
                return new string[]
                {
                    "System",
                    "System.Xml.Linq",
                    "System.Collections",
                    "System.Collections.Specialized",
                    "System.Dynamic",
                };
            }
        }



        /// <summary>
        ///The engine calls this method based on the optional include directive
        ///if the user has specified it in the text template.
        /// </summary>
        /// <param name="requestFileName"></param>
        /// <param name="content"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = System.String.Empty;
            location = System.String.Empty;

            //If the argument is the fully qualified path of an existing file,
            //then we are done.
            if (File.Exists(requestFileName))
            {
                content = File.ReadAllText(requestFileName);
                return true;
            }

            string candidate = Path.Combine(Path.GetDirectoryName(this.TemplateFile), requestFileName);
            if (File.Exists(candidate))
            {
                content = File.ReadAllText(candidate);
                return true;
            }

            //This can be customized to search specific paths for the file.
            else
            {
                return false;
            }
        }


        /// <summary>
        ///The engine calls this method to resolve assembly references used in
        ///the generated transformation class project, and for the optional 
        /// </summary>
        /// <param name="assemblyReference"></param>
        /// <returns></returns>
        public virtual string ResolveAssemblyReference(string assemblyReference)
        {
            if (File.Exists(assemblyReference))
            {
                return assemblyReference;
            }
            string candidate = Path.Combine(Path.GetDirectoryName(this.TemplateFile), assemblyReference);
            if (File.Exists(candidate))
            {
                return candidate;
            }
            return "";
        }


        /// <summary>
        /// Gets the host options
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object GetHostOption(string parameter)
        {
            return null;
        }




        /// <summary>
        /// Resolves the directive processor
        /// </summary>
        /// <param name="processorName"></param>
        /// <returns></returns>
        public Type ResolveDirectiveProcessor(string processorName)
        {
            //This host will not resolve any specific processors.         
            if (string.Compare(processorName, "XYZ", StringComparison.OrdinalIgnoreCase) == 0)
            {
                //return typeof();
            }

            //If the directive processor cannot be found, throw an error.
            throw new Exception("Directive Processor not found");
        }



        /// <summary>
        ///A directive processor can call this method if a file name does not 
        ///have a path.        
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ResolvePath(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("The file name cannot be null");
            }

            //If the argument is the fully qualified path of an existing file,
            //then we are done.
            if (File.Exists(fileName))
            {
                return fileName;
            }

            //Maybe the file is in the same folder as the text template that 
            //called the directive.
            string candidate = Path.Combine(Path.GetDirectoryName(this.TemplateFile), fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            //Look more places.
            //More code can go here...

            //If we cannot do better - return the original file name.
            return fileName;
        }


        /// <summary>
        ///If a call to a directive in a text template does not provide a value
        ///for a required parameter, the directive processor can try to get it
        ///from the host by calling this method.
        //This method can be called 0, 1, or more times.        
        /// </summary>
        /// <param name="directiveId"></param>
        /// <param name="processorName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            if (directiveId == null)
            {
                throw new ArgumentNullException("The directiveId cannot be null");
            }
            if (processorName == null)
            {
                throw new ArgumentNullException("The processorName cannot be null");
            }
            if (parameterName == null)
            {
                throw new ArgumentNullException("The parameterName cannot be null");
            }

            //Code to provide "hard-coded" parameter values goes here.
            //This code depends on the directive processors this host will interact with.

            //If we cannot do better - return the empty string.
            return String.Empty;
        }



        /// <summary>
        ///The engine calls this method to change the extension of the 
        ///generated text output file based on the optional output directive 
        ///if the user specifies it in the text template.        
        /// </summary>
        /// <param name="extension"></param>
        public void SetFileExtension(string extension)
        {
            //The parameter extension has a '.' in front of it already.
            fileExtensionValue = extension;
        }


        /// <summary>
        /// Sets the output encoding
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="fromOutputDirective"></param>
        public void SetOutputEncoding(System.Text.Encoding encoding, bool fromOutputDirective)
        {
            fileEncodingValue = encoding;
        }


        /// <summary>
        /// Log the erros
        /// </summary>
        /// <param name="errors"></param>
        public void LogErrors(CompilerErrorCollection errors)
        {
            errorsValue = errors;
        }



        /// <summary>
        /// Returns the app domain of the host
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public AppDomain ProvideTemplatingAppDomain(string content)
        {
            return AppDomain.CreateDomain("Generation App Domain");
        }

        /// <summary>
        /// Process the input template
        /// </summary>
        /// <param name="templateFileName"></param>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        public CompilerErrorCollection ProcessTemplate(string templateFileName, string outputFileName)
        {

            if (!File.Exists(templateFileName))
            {
                throw new FileNotFoundException("The file cannot be found");
            }

            DynamicTemplateHost host = this;
            Engine engine = new Engine();

            host.TemplateFile = templateFileName;

            //Read the text template.
            string input = File.ReadAllText(templateFileName);

            //Transform the text template.
            string output = engine.ProcessTemplate(Preprocess(input), host);

            File.WriteAllText(outputFileName, output, host.FileEncoding);
            

            return host.Errors;
        }

        /// <summary>
        /// Process the input template
        /// </summary>
        /// <param name="templateFileName"></param>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        public CompilerErrorCollection ProcessTemplate(string templateFileName,out  string data)
        {

            if (!File.Exists(templateFileName))
            {
                throw new FileNotFoundException("The file cannot be found");
            }

            DynamicTemplateHost host = this;
            Engine engine = new Engine();

            host.TemplateFile = templateFileName;

            //Read the text template.
            string input = File.ReadAllText(templateFileName);

            //Transform the text template.
            if (EnablePreProcess)
                input = Preprocess(input);

            string output = engine.ProcessTemplate(input, host);

            data = output;

            return host.Errors;
        }


        public DynamicTemplateHost()
        {
            EnablePreProcess = true;
        }


        /// <summary>
        /// Enhanced reg-ex based pre processing
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Preprocess(string input)
        {

            Regex expressionInCode = new Regex(@"(\#\(.*?\s*?\))", RegexOptions.None);
            Regex expressionInTemp = new Regex(@"(\$\(.*?\s*?\))", RegexOptions.None);
            Regex expressionLoopStart = new Regex(@"\${1}[a-zA-Z0-9_ ]*\?\s*?\(.*?\)", RegexOptions.None);


            var matches = expressionInCode.Matches(input, 0);

            string data=input;

            //Process the matches
            foreach (var match in matches)
            {
                string formatted = match.ToString().Trim().Remove(0, 2);
                formatted = formatted.Remove(formatted.Length - 1);
                data = data.Replace(match.ToString(), "#><#=" + formatted + "#><#");
            }

            matches = expressionInTemp.Matches(data, 0);

            //Process the matches
            foreach (var match in matches)
            {
                string formatted = match.ToString().Trim().Remove(0, 2);
                formatted = formatted.Remove(formatted.Length - 1);
                data = data.Replace(match.ToString(), "<#=" + formatted + "#>");
            }

            matches = expressionLoopStart.Matches(data, 0);

            //Process the matches
            foreach (var match in matches)
            {
                string formatted = match.ToString().Trim().Remove(0, 1);
                formatted = formatted.Trim();
                string op = formatted.Remove(formatted.IndexOf("?")).Trim();
                string expression = formatted.Substring(formatted.IndexOf("?")+1).Trim();

                data = data.Replace(match.ToString(),"<# foreach(var " + op + " in " + expression + ") {#>");
                
                data = data.Replace("$" + op + " end" , "<# } #>");
            }


            return data;
        }




    }

}
