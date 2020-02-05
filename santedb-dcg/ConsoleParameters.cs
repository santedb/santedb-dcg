/*
 * Portions Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 * Portions Copyright 2019-2019 SanteSuite Contributors (See NOTICE)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: Justin Fyfe
 * Date: 2019-8-8
 */
using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Dcg
{
    /// <summary>
    /// Represents console parameters for this particular instance
    /// </summary>
    public class ConsoleParameters
    {
        // <summary>
        /// When true, parameters should be shown
        /// </summary>
        [Description("Shows help and exits")]
        [Parameter("?")]
        [Parameter("help")]
        public bool ShowHelp { get; set; }

        /// <summary>
        /// When true console mode should be enabled
        /// </summary>
        [Description("Instructs the host process to run in console mode")]
        [Parameter("c")]
        [Parameter("console")]
        public bool ConsoleMode { get; set; }

        /// <summary>
        /// Gets or sets the name of the instance that is running
        /// </summary>
        [Description("Identifies the name of the instance to start")]
        [Parameter("n")]
        [Parameter("name")]
        public string InstanceName { get; set; }

        /// <summary>
        /// Installs the service
        /// </summary>
        [Description("Installs the service")]
        [Parameter("i")]
        [Parameter("install")]
        public bool Install { get; set; }

        /// <summary>
        /// Installs the service
        /// </summary>
        [Description("Uninstalls the service")]
        [Parameter("u")]
        [Parameter("uninstall")]
        public bool Uninstall { get; set; }

        /// <summary>
        /// Restarts the service
        /// </summary>
        [Description("Restart the specified service")]
        [Parameter("restart")]
        public bool Restart { get; set; }

        /// <summary>
        /// Reset the service installation
        /// </summary>
        [Description("Resets the configuration of this WWW instance to default")]
        [Parameter("r")]
        [Parameter("reset")]
        public bool Reset { get; set; }

        /// <summary>
        /// Set the application name
        /// </summary>
        [Description("Sets the identity of the application (for OAUTH) for this instance")]
        [Parameter("appname")]
        public String ApplicationName { get; set; }

        /// <summary>
        /// The application secret
        /// </summary>
        [Description("Sets the secret of the application (for OAUTH) for this instance")]
        [Parameter("appsecret")]
        public String ApplicationSecret { get; set; }

        /// <summary>
        /// Start in noninteractive method
        /// </summary>
        [Description("Do not quit when the ENTER key is pressed")]
        [Parameter("daemon")]
        public bool Forever { get; internal set; }

        /// <summary>
        /// Use an initial configuration skeleton
        /// </summary>
        [Description("Use an initial configuration skeleton for this deployment")]
        [Parameter("skel")]
        public string Skel { get; set; }
    }
}
