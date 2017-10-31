using Orc.TrafficLights.Models;
using Orc.TrafficLights.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orc.TrafficLights
{
    public static class TestManager
    {
        private static List<TestResult> results = null;

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
            results = new List<TestResult>();

            var tasks = new List<Task>();
            foreach (var test in Tests)
            {
                // Create an async task for the test
                var task = Task.Factory.StartNew(() => 
                {
                    RunTest(test);
                });

                // Add to our list of tasks
                tasks.Add(task);
            }

            // Wait for all tasks to complete before we return the results
            Task.WaitAll(tasks.ToArray());

            return results;
        }

        public static TestResult RunTest(BaseTrafficLightTest test)
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
            return model;
        }

        //public static async Task<TestResult> RunTestAsync(BaseTrafficLightTest test)
        //{
        //    var model = new TestResult();
        //    model.Name = test.TestName;
        //    model.Status = TestStatus.Unknown;

        //    // wait for the test to complete running before we proceed with the rest of the logic
        //    // Can't use await await Task.Run(() => as its .Net 4.5 
        //    // await Task.Factory.StartNew(() => is for .Net 4.0

        //    await Task.Run(() =>
        //    {
        //        try
        //        {
        //            test.RunTest(ref model);
        //        }
        //        catch (Exception e)
        //        {
        //            model.Status = TestStatus.Exception;
        //            model.Message = e.Message;
        //        }
        //    });

        //    results.Add(model);
        //    return model;
        //}

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
