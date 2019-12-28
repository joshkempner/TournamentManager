using System;
using System.Text.RegularExpressions;

namespace TournamentManager.Helpers
{
    public static class StringUtilities
    {
        public static bool IsValidUSZipCode(string zipCode)
        {
            const string usZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$";
            return Regex.Match(zipCode, usZipRegEx).Success;
        }

        public static bool IsValidStateAbbreviation(string state)
        {
            const string states = "|AL|AK|AS|AZ|AR|CA|CO|CT|DE|DC|FM|FL|GA|GU|HI|ID|IL|IN|IA|KS|KY|LA|ME|MH|MD|MA|MI|MP|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|MP|OH|OK|OR|PW|PA|PR|RI|SC|SD|TN|TX|UT|VT|VI|VA|WA|WV|WI|WY|AA|AE|AP|";
            return state.Length == 2 && !state.Contains("|") && states.IndexOf(state, StringComparison.Ordinal) > 0;
        }
    }
}
