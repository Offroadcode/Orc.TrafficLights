using Orc.TrafficLights.Models;
using Orc.TrafficLights.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orc.TrafficLights
{
    public static class TestManager
    {
        /// <summary>
        /// backing variable for the list of tests
        /// </summary>
        private static List<BaseTrafficLightTest> _tests = null;

        /// <summary>
        /// Accessor list of tests which will populate on first access
        /// </summary>
        private static List<BaseTrafficLightTest> Tests
        {
            get
            {
                if (_tests == null)
                {
                    _tests = new List<BaseTrafficLightTest>();
                    
                    //Get a type object for our base
                    Type ti = typeof(BaseTrafficLightTest);

                    //loop over all assembiles in our site
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        //For each of the types in this assembly
                        foreach (Type t in GetLoadableTypes(asm))
                        {
                            //if the current type is based on the BaseTrafficLightTest and it ISNT the abstract BaseTrafficLightTest itself
                            if (ti.IsAssignableFrom(t) && !t.IsAbstract)
                            {
                                //Create an instance of the test object and cast it to the base class
                                var instance = Activator.CreateInstance(t) as BaseTrafficLightTest;
                                _tests.Add(instance);
                            }
                        }
                    }
                }
                return _tests;
            }
        }

        public static List<TestResult> RunTests()
        {
            List<TestResult> results = new List<TestResult>();
            foreach (var test in Tests)
            {
                var model = new TestResult();
                model.Name = test.TestName;
                model.Status = TestStatus.Unknown;
                try
                {
                    test.RunTest(ref model);
                }
                catch (Exception e)
                {
                    model.Status = TestStatus.Exception;
                    model.Message = e.Message;
                }
                results.Add(model);
            }
            return results;
        }

        /// <summary>
        /// Safely Retrives all loadable types from the assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }

            return types;
        }
    }
}
