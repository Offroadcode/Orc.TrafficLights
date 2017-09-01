using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orc.TrafficLights.Models
{
    public class TestResult
    {
        public TestStatus Status { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }
    public enum TestStatus{
        Unknown,
        Exception,
        Failure,
        Warning,
        Pass
    }   
}
