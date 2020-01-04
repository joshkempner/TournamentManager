using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace TournamentManager.Helpers
{
    public static class StringUtilities
    {
        public static bool IsValidEmailAddress(string emailAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailAddress)) return false;
                // ReSharper disable once ObjectCreationAsStatement
                new MailAddress(emailAddress); // Validates the email address. We don't use the output for anything.
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidUSZipCode(string zipCode)
        {
            const string usZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$";
            return Regex.Match(zipCode, usZipRegEx).Success;
        }

        public static bool IsValidStateAbbreviation(string state)
        {
            const string states = "|AL|AK|AS|AZ|AR|CA|CO|CT|DE|DC|FM|FL|GA|GU|HI|ID|IL|IN|IA|KS|KY|LA|ME|MH|MD|MA|MI|MP|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|OH|OK|OR|PW|PA|PR|RI|SC|SD|TN|TX|UT|VT|VI|VA|WA|WV|WI|WY|AA|AE|AP|";
            return state.Length == 2 && !state.Contains("|") && states.IndexOf(state, StringComparison.Ordinal) > 0;
        }

        public static string GetPostalAbbreviation(string state)
        {
            return States[state];
        }

        public static readonly Dictionary<string, string> States = new Dictionary<string, string>
        {
            { "Alabama", "AL" },
            { "Alaska", "AK" },
            { "Arizona", "AZ" },
            { "Arkansas", "AR" },
            { "California", "CA" },
            { "Colorado", "CO" },
            { "Connecticut", "CT" },
            { "Delaware", "DE" },
            { "Washington, D.C.", "DC" },
            { "Florida", "FL" },
            { "Georgia", "GA" },
            { "Hawaii", "HI" },
            { "Idaho", "ID" },
            { "Illinois", "IL" },
            { "Indiana", "IN" },
            { "Iowa", "IA" },
            { "Kansas", "KS" },
            { "Kentucky", "KY" },
            { "Louisiana", "LA" },
            { "Maine", "ME" },
            { "Maryland", "MD" },
            { "Massachusetts", "MA" },
            { "Michigan", "MI" },
            { "Minnesota", "MN" },
            { "Mississippi", "MS" },
            { "Missouri", "MO" },
            { "Montana", "MT" },
            { "Nebraska", "NE" },
            { "Nevada", "NV" },
            { "New Hampshire", "NH" },
            { "New Jersey", "NJ" },
            { "New Mexico", "NM" },
            { "New York", "NY" },
            { "North Carolina", "NC" },
            { "North Dakota", "ND" },
            { "Ohio", "OH" },
            { "Oklahoma", "OK" },
            { "Oregon", "OR" },
            { "Rhode Island", "RI" },
            { "South Carolina", "SC" },
            { "South Dakota", "SD" },
            { "Tennessee", "TN" },
            { "Texas", "TX" },
            { "Utah", "UT" },
            { "Vermont", "VT" },
            { "Virginia", "VA" },
            { "Washington", "WA" },
            { "West Virginia", "WV" },
            { "Wisconsin", "WI" },
            { "Wyoming", "WY" },
            { "Puerto Rico", "PR" },
            { "American Samoa", "AS" },
            { "Guam", "GU" },
            { "Marshall Islands", "MH" },
            { "Micronesia", "FM" },
            { "Northern Mariana Islands", "MP" },
            { "Palau", "PW" },
            { "U.S. Virgin Islands", "VI" },
            { "U.S. Armed Forces - Americas", "AA" },
            { "U.S. Armed Forces - Europe", "AE" },
            { "U.S. Armed Forces - Pacific", "AP" }
        };
    }
}
