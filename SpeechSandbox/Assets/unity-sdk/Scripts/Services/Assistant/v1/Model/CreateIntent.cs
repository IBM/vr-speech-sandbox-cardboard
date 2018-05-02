/**
* Copyright 2018 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using FullSerializer;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// CreateIntent.
    /// </summary>
    [fsObject]
    public class CreateIntent
    {
        /// <summary>
        /// The name of the intent. This string must conform to the following restrictions:  - It can contain only Unicode alphanumeric, underscore, hyphen, and dot characters.  - It cannot begin with the reserved prefix `sys-`.  - It must be no longer than 128 characters.
        /// </summary>
        /// <value>The name of the intent. This string must conform to the following restrictions:  - It can contain only Unicode alphanumeric, underscore, hyphen, and dot characters.  - It cannot begin with the reserved prefix `sys-`.  - It must be no longer than 128 characters.</value>
        [fsProperty("intent")]
        public string Intent { get; set; }
        /// <summary>
        /// The description of the intent. This string cannot contain carriage return, newline, or tab characters, and it must be no longer than 128 characters.
        /// </summary>
        /// <value>The description of the intent. This string cannot contain carriage return, newline, or tab characters, and it must be no longer than 128 characters.</value>
        [fsProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// An array of user input examples for the intent.
        /// </summary>
        /// <value>An array of user input examples for the intent.</value>
        [fsProperty("examples")]
        public List<CreateExample> Examples { get; set; }
    }

}
