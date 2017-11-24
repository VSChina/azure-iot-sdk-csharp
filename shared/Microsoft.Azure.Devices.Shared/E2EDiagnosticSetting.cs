// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Shared
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// E2EDiagnosticSetting Representation
    /// </summary>
    [JsonConverter(typeof(E2EDiagnosticSettingJsonConverter))]
    public class E2EDiagnosticSetting
    {
        /// <summary>
        /// Creates an instance of <see cref="E2EDiagnosticSetting"/>
        /// </summary>
        /// <param name="samplingRate">Sampling Rate</param>
        public E2EDiagnosticSetting(int samplingRate = 0)
        {
            SamplingRate = samplingRate;
        }

        private int _samplingRate;

        /// <summary>
        /// Gets and sets the <see cref="E2EDiagnosticSetting"/> Sampling Rate.
        /// </summary>
        public int SamplingRate
        {
            get
            {
                return _samplingRate;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException("Sampling rate should between [0,100]");
                }
                _samplingRate = value;
            }
        }
    }
}
