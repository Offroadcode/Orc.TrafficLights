using Orc.TrafficLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orc.TrafficLights.Tests
{
    public abstract class BaseTrafficLightTest
    {
        public abstract void RunTest(ref TestResult result);
        public abstract string TestName { get; }
    }
}
