# Orc.TrafficLights

This is our standard traffic light implmentation. To install it you simply reference the Dll and add the web.config sections
Add This to system.webServer/handlers
<add name="TrafficLights" path="TrafficLights.ashx" verb="*" type="Orc.TrafficLights.TrafficLightHandler, Orc.TrafficLights" preCondition="integratedMode,runtimeVersionv4.0" />

Add system.web/httpHandlers
<add verb="*" path="TrafficLights.ashx" type="Orc.TrafficLights.TrafficLightHandler, Orc.TrafficLights" />

## Custom tests

from your class project, you can implement the abstract class BaseTrafficLightTest, which you can code to test what you want. for example a test which checks if umbraco can communicate with the Database.

Note: theses tests CAN throw exceptions as they are caught when executed. Meaning you test doesnt have to account for any exceptions as any uncaught exceptions will fail the test.
```
 class UmbracoCanTalkToDbTest :BaseTrafficLightTest
    {
        public override void RunTest(ref Orc.TrafficLights.Models.TestResult result)
        {
            if (UmbracoContext.Current.Application.DatabaseContext.CanConnect)
            {
                result.Status = Orc.TrafficLights.Models.TestStatus.Pass;
            }
            else
            {
                result.Status = Orc.TrafficLights.Models.TestStatus.Failure;
            }
        }

        public override string TestName
        {
            get { return "Test if Umbraco can talk to the database"; }
        }
    }
    ```
    These tests are automatically found and wired in using reflection on the first hit of the TrafficLight.ashx page.
    
    ## Automation
    you can request TrafficLights.ashx?automatedCheck=yes and this will return a 500 or a 200 status code if any of the tests fail. This is for Server density monitoring where it looks at the status code
