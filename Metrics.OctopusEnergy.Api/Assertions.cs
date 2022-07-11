using System.Globalization;
using System.Text.RegularExpressions;
using Metrics.OctopusEnergy.Api.Properties;

namespace Metrics.OctopusEnergy.Api
{
    internal static class Assertions
    {
        public static void AssertValidGsp(string gsp)
        {
            if (!Regex.IsMatch(gsp, @"^_[A-N]$", RegexOptions.Singleline | RegexOptions.Compiled))
            {
                throw new GspException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidGsp, gsp));
            }
        }
    }
}