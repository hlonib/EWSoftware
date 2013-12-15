﻿//===============================================================================================================
// System  : Sandcastle XslTransform Tool
// File    : XslTransform.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 12/11/2013
// Compiler: Microsoft Visual C#
//
// This file contains the class used to make XslTransform callable from MSBuild projects.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice and all copyright notices must remain intact in all applications, documentation, and source files.
//
// Date        Who  Comments
// ==============================================================================================================
// 12/11/2013  EFW  Created the code
//===============================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Microsoft.Ddue.Tools.CommandLine;

namespace Microsoft.Ddue.Tools.MSBuild
{
    public class XslTransform : Task, ICancelableTask
    {
        #region Task properties
        //=====================================================================

        /// <summary>
        /// This is used to pass in the working folder where the files are located
        /// </summary>
        /// <value>If not set, no working folder will be set for the build</value>
        public string WorkingFolder { get; set; }

        /// <summary>
        /// This is used to pass in the input filename
        /// </summary>
        [Required]
        public string InputFile { get; set; }

        /// <summary>
        /// This is used to pass in the output filename
        /// </summary>
        [Required]
        public string OutputFile { get; set; }

        /// <summary>
        /// This is used to pass in the list of transformations to run
        /// </summary>
        /// <remarks>Separate multiple transforms with a semi-colon.  Relative paths are assumed to refer to
        /// Sandcastle transformations and will be fully qualified with the root Sandcastle tools folder (the
        /// folder one level up from the folder of this assembly).  Absolute paths are assumed to be custom
        /// transforms and the path is not modified.</remarks>
        [Required]
        public string[] Transformations { get; set; }

        /// <summary>
        /// This is used to pass in any optional XSL transform arguments
        /// </summary>
        /// <value>The optional XSL transform arguments in the form "argName=argValue".  Separate multiple
        /// arguments with a semi-colon.</value>
        public string[] Arguments { get; set; }

        #endregion

        #region ICancelableTask Members
        //=====================================================================

        /// <summary>
        /// Cancel the transformation process
        /// </summary>
        /// <remarks>The transformation process is stopped after the current transform completes</remarks>
        public void Cancel()
        {
            XslTransformCore.Canceled = true;
        }
        #endregion

        #region Task execution
        //=====================================================================

        /// <summary>
        /// This executes the task
        /// </summary>
        /// <returns>True on success, false on failure</returns>
        public override bool Execute()
        {
            List<string> args = new List<string>();
            string currentDirectory = null, sandcastlePath = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), "..");
            bool success = false;

            // Log messages via MSBuild
            ConsoleApplication.Log = this.Log;
            ConsoleApplication.ToolName = "XslTransform";

            if(!File.Exists(this.InputFile))
            {
                ConsoleApplication.WriteMessage(LogLevel.Error, "An input file must be specified and it must exist");
                return false;
            }

            if(this.Transformations == null || this.Transformations.Length == 0)
            {
                ConsoleApplication.WriteMessage(LogLevel.Error, "At least one XSL transformation is required");
                return false;
            }

            try
            {
                // Switch to the working folder for the process so that relative paths are resolved properly
                if(!String.IsNullOrWhiteSpace(this.WorkingFolder))
                {
                    currentDirectory = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(Path.GetFullPath(this.WorkingFolder));
                }

                // If a transform path isn't rooted, assume it is in the Sandcastle tools folder
                foreach(string transform in this.Transformations)
                    if(!Path.IsPathRooted(transform))
                        args.Add("/xsl:" + Path.Combine(sandcastlePath, transform));
                    else
                        args.Add("/xsl:" + transform);

                if(this.Arguments != null)
                    foreach(string arg in this.Arguments)
                        args.Add("/arg:" + arg);

                args.Add(this.InputFile);
                args.Add("/out:" + this.OutputFile);

                success = (XslTransformCore.Main(args.ToArray()) == 0);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                ConsoleApplication.WriteMessage(LogLevel.Error, "An unexpected error occurred trying to " +
                    "execute the XslTransform MSBuild task: {0}", ex);
            }
            finally
            {
                if(currentDirectory != null)
                    Directory.SetCurrentDirectory(currentDirectory);
            }

            return success;
        }
        #endregion
    }
}
