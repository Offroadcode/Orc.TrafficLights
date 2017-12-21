using System;
using System.Text;
using System.Web;
using System.Linq;
namespace Orc.TrafficLights
{
    public class TrafficLightHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var output = TestManager.RunTests();

            if (context.Request.QueryString.AllKeys.Any(x => x == "automatedCheck"))
            {
                // Read the time range we don't throw errors im between
                // This is for sites running maintanence at certain times, so we can avoid false positive alerts
                var startTimeString = context.Request.QueryString["disabledStartTime"];
                var endTimeString = context.Request.QueryString["disabledEndTime"];

                TimeSpan startTime;
                TimeSpan endTime;
                if (TimeSpan.TryParse(startTimeString, out startTime) && TimeSpan.TryParse(endTimeString, out endTime))
                {
                    // Don't show an error page when its thrown between the specified time range
                    if (!(DateTime.UtcNow.TimeOfDay > startTime && DateTime.UtcNow.TimeOfDay < endTime))
                    {
                        if (output.Any(x => x.Status == Models.TestStatus.Failure || x.Status == Models.TestStatus.Exception))
                        {
                            context.Response.StatusCode = 500;
                            context.Response.Write("Error");
                        }
                    }
                }
                else
                {
                    if (output.Any(x => x.Status == Models.TestStatus.Failure || x.Status == Models.TestStatus.Exception))
                    {
                        context.Response.StatusCode = 500;
                        context.Response.Write("Error");
                    }
                }
            }
            else
            {

                context.Response.Write("<html><body><h1>Traffic Lights</h1>");
                context.Response.Write("<table>");
                context.Response.Write("<tr>");
                context.Response.Write("<th>Test</th>");
                context.Response.Write("<th>Message</th>");
                context.Response.Write("<th>Result</th>");
                context.Response.Write("</tr>");
                foreach (var item in output)
                {
                    var bgColor = "#00FF00";
                    if (item.Status == Models.TestStatus.Failure)
                    {
                        bgColor = "#FF0000";
                    }
                    else if (item.Status == Models.TestStatus.Warning)
                    {
                        bgColor = "#FFFF00";
                    }
                    else if (item.Status == Models.TestStatus.Exception)
                    {
                        bgColor = "#0000FF";
                    }
                    context.Response.Write("<tr>");
                    context.Response.Write("<td>" + item.Name + "</td>");
                    context.Response.Write("<td>" + item.Message + "</td>");
                    context.Response.Write("<td bgcolor=\"" + bgColor + "\">" + item.Status + "</td>");
                    context.Response.Write("</tr>");
                }
                context.Response.Write("</table>");
                context.Response.Write("</body></html>");
            }
        }

        #endregion
    }
}
