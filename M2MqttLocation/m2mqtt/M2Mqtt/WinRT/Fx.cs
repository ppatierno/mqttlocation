﻿/*
M2Mqtt Project - MQTT Client Library for .Net and GnatMQ MQTT Broker for .NET
Copyright (c) 2014, Paolo Patierno, All rights reserved.

Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this 
file except in compliance with the License. You may obtain a copy of the License at 
http://www.apache.org/licenses/LICENSE-2.0

THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR 
CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR 
NON-INFRINGEMENT.

See the Apache Version 2.0 License for specific language governing permissions and 
limitations under the License.
*/

using System.Threading;
using System.Threading.Tasks;

namespace uPLibrary.Networking.M2Mqtt
{
    /// <summary>
    /// Support methods fos specific framework
    /// </summary>
    public class Fx
    {

        public delegate void ThreadStart();
        public static void StartThread(ThreadStart threadStart)
        {
            Task.Factory.StartNew(o => ((ThreadStart)o)(), threadStart);
        }

        public static void SleepThread(int millisecondsTimeout) { Task.Delay(millisecondsTimeout).RunSynchronously(); }
    }
}
