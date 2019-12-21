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
using SanteDB.DisconnectedClient.UI;
using System;

namespace SanteDB.Dcg
{
    /// <summary>
    /// Represents a console based dialog provider
    /// </summary>
    internal class ConsoleDialogProvider : IDialogProvider
    {
        public ConsoleDialogProvider()
        {
        }

        /// <summary>
        /// Alert has been raised
        /// </summary>
        public void Alert(string text)
        {
            Console.WriteLine("ALERT >>>> {0}", text);
        }

        /// <summary>
        /// Confirmation dialog
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool Confirm(string text, string title)
        {
            Console.WriteLine("CONFIRM >>>> {0}", text);
            return true;
        }
    }
}