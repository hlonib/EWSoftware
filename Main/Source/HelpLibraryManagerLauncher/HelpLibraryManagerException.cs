﻿//=============================================================================
// System  : Help Library Manager Launcher
// File    : HelpLibraryManagerException.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 07/07/2010
// Note    : Copyright 2010, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains an exception class used to report problems with the
// Help Library Manager operation.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy
// of the license should be distributed with the code.  It can also be found
// at the project website: http://www.CodePlex.com/SandcastleStyles.   This
// notice, the author's name, and all copyright notices must remain intact in
// all applications, documentation, and source files.
//
// Version     Date     Who  Comments
// ============================================================================
// 1.0.0.0  07/03/2010  EFW  Created the code
//=============================================================================

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SandcastleBuilder.MicrosoftHelpViewer
{
    /// <summary>
    /// This exception class is thrown to report problems with the Help Library
    /// Manager operation.
    /// </summary>
    [Serializable]
    public class HelpLibraryManagerException : Exception
    {
        #region Error code constants
        //=====================================================================

        // Launcher error codes

        /// <summary>The operation was successful.</summary>
        public const int Success = 0;

        /// <summary>The Help Library Manager was not found.</summary>
        public const int HelpLibraryManagerNotFound = 1;

        /// <summary>The Help Library Manager is already running.</summary>
        public const int HelpLibraryManagerAlreadyRunning = 2;

        /// <summary>The local store is not initialized.</summary>
        public const int LocalStoreNotInitialized = 3;

        /// <summary>A required command line argument is missing.</summary>
        public const int MissingCommandLineArgument = 4;

        /// <summary>The catalog is not installed.</summary>
        public const int CatalogNotInstalled = 5;

        /// <summary>An unknown error occurred in the Help Library Manager launcher.</summary>
        public const int UnknownError = 6;

        // Help Library Manager error codes

        /// <summary>One or more command line arguments was missing or invalid.</summary>
        public const int InvalidCmdArgs = 100;

        /// <summary>The application configuration file for HLM was missing or invalid.</summary>
        public const int MissingOrInvalidAppConfig = 110;

        /// <summary>The help content store could not be locked for update.  This error typically
        /// occurs when the content is locked for update by another process.</summary>
        public const int FailOnMachineLock = 120;

        /// <summary>Files required to install content for a new product were not found.</summary>
        public const int MissingCatalogInfo = 130;

        /// <summary>Files required to install content for a new product were invalid.</summary>
        public const int InvalidCatalogInfo = 131;

        /// <summary>The path specified for the /content switch is invalid.</summary>
        public const int FailOnSettingLocalStore = 140;

        /// <summary>The local content store is in an invalid state.  This error occurs when the
        /// directory permissions do not allow writing, or a required file is missing from the
        /// directory.</summary>
        public const int InvalidHelpLocation = 150;

        /// <summary>The arguments passed to HLM did not result in content being installed or removed.
        /// This can occur when the content is already installed or has already been removed.</summary>
        public const int NoBooksToInstall = 200;

        /// <summary>The removal of content failed.  Detailed information can be found in the event log
        /// and in the installation log.</summary>
        public const int FailOnSilentUninstall = 400;

        /// <summary>The installation of content failed.  Detailed information can be found in the event
        /// log and in the installation log.</summary>
        public const int FailOnSilentInstall = 401;

        /// <summary>A non-admin user is trying to initialize the store using the /silent switch.</summary>
        public const int NonAdminSetsLocalStoreOnSilentMode = 402;

        /// <summary>An unknown error occurred.</summary>
        public const int Others = 999;

        #endregion

        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the error code
        /// </summary>
        public int ErrorCode { get; private set; }
        #endregion

        #region Constructors
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorCode">The error code associated with the exception</param>
        /// <overloads>There are three overloads for the constructor</overloads>
        public HelpLibraryManagerException(int errorCode) : base(ErrorMessageForCode(errorCode))
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorCode">The error code associated with the exception</param>
        /// <param name="additionalInfo">Additional information for the error message</param>
        public HelpLibraryManagerException(int errorCode, string additionalInfo) :
          base(ErrorMessageForCode(errorCode) + additionalInfo)
        {
            this.ErrorCode = errorCode;
        }

        /// <inheritdoc />
        protected HelpLibraryManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if(info != null)
                this.ErrorCode = info.GetInt32("ErrorCode");
        }
        #endregion

        #region Private helper methods
        //=====================================================================

        /// <summary>
        /// Return a descriptive error message for the specified error code
        /// </summary>
        /// <param name="code">The error code</param>
        /// <returns>A descriptive error message for the error code</returns>
        private static string ErrorMessageForCode(int code)
        {
            string hlmErrorMsg = String.Format(CultureInfo.InvariantCulture,
                "The Help Library Manager returned the exit code {0}: ", code);

            switch(code)
            {
                case Success:
                    return "No error occurred.  Why did you throw an exception?";

                case HelpLibraryManagerNotFound:
                    return "The Help Library Manager executable could not be found.  Is it installed?";

                case HelpLibraryManagerAlreadyRunning:
                    return "The Help Library Manager is already running.  In order to install or uninstall your " +
                        "help file, you must first close the running instance.";

                case LocalStoreNotInitialized:
                    return "The local help library store has not been initialized.  You need to install some help " +
                        "content first such as the Visual Studio help in order to install your help content.";

                case MissingCommandLineArgument:
                    return "Missing command line argument: ";

                case CatalogNotInstalled:
                    return "The specified help catalog is not installed  The new catalog could not be created " +
                        "because no /locale argument was supplied.  ";

                case InvalidCmdArgs:
                    return hlmErrorMsg + "One or more command line arguments was missing or invalid.";

                case MissingOrInvalidAppConfig:
                    return hlmErrorMsg + "The application configuration file for the Help Library Manager was " +
                        "missing or invalid.";

                case FailOnMachineLock:
                    return hlmErrorMsg + "The help content store could not be locked for update.  This error " +
                        "typically occurs when the content is locked for update by another process.";

                case MissingCatalogInfo:
                    return hlmErrorMsg + "Files required to install content for a new product were not found.";

                case InvalidCatalogInfo:
                    return hlmErrorMsg + "Files required to install content for a new product were invalid.";

                case FailOnSettingLocalStore:
                    return hlmErrorMsg + "The path specified for the /content switch is invalid.";

                case InvalidHelpLocation:
                    return hlmErrorMsg + "The local content store is in an invalid state.  This error occurs when the " +
                        "directory permissions do not allow writing, or a required file is missing from the directory.";

                case NoBooksToInstall:
                    return hlmErrorMsg + "The arguments passed to the Help Library Manager did not result in content " +
                        "being installed or removed.  This can occur when the content is already installed or has " +
                        "already been removed.";

                case FailOnSilentUninstall:
                    return hlmErrorMsg + "The removal of content failed.  Detailed information can be found in the " +
                        "event log and in the installation log.";

                case FailOnSilentInstall:
                    return hlmErrorMsg + "The installation of content failed.  Detailed information can be found in the " +
                        "event log and in the installation log.";

                case NonAdminSetsLocalStoreOnSilentMode:
                    return hlmErrorMsg + "A non-admin user is trying to initialize the store using the /silent switch.";

                case Others:
                    return hlmErrorMsg + "An nknown error occurred in the Help Library Manager.";

                default:
                    return "An unknown error occurred in the Help Library Manager launcher process.  ";
            }
        }
        #endregion

        #region Method overrides
        //=====================================================================

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if(info != null)
                info.AddValue("ErrorCode", this.ErrorCode);
        }
        #endregion
    }
}
