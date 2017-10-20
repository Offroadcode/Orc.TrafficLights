using Orc.TrafficLights.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orc.TrafficLights.Tests
{
    public class FreeDiskSpaceTest : BaseTrafficLightTest
    {
        public override void RunTest(ref TestResult model)
        {
            model.Status = TestStatus.Pass;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady )
                {
                    double percentFree = 100 * (double)drive.TotalFreeSpace / drive.TotalSize;
                    double driveSpaceUsed = 100 - percentFree;
                    model.Message = model.Message + "Drive: " + drive.Name + " is at " + Math.Round(driveSpaceUsed )+ "%";
                    
                    //TODO change this to look at percentage instead.
                    if (driveSpaceUsed  > 90)
                    {
                        model.Status = TestStatus.Failure;
                    }
                    else if (driveSpaceUsed > 70)
                    {
                        if (model.Status != TestStatus.Failure)
                        {
                            model.Status = TestStatus.Warning;
                        }   
                    }
                }
            }
            
        }

        public override string TestName
        {
            get { return "Is there any available disk space on the server"; }
        }
    }
}
