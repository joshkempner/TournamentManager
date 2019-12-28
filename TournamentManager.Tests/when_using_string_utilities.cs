﻿using System;
using System.Collections.Generic;
using TournamentManager.Helpers;
using Xunit;

namespace TournamentManager.Tests
{
    public class when_using_string_utilities
    {
        [Fact]
        public void correct_zip_validates()
        {
            Assert.True(StringUtilities.IsValidUSZipCode("01234"));
        }

        [Fact]
        public void short_zip_fails_validation()
        {
            Assert.False(StringUtilities.IsValidUSZipCode("0123"));
        }

        [Fact]
        public void long_zip_fails_validation()
        {
            Assert.False(StringUtilities.IsValidUSZipCode("012345"));
        }

        [Fact]
        public void non_numeric_zip_fails_validation()
        {
            Assert.False(StringUtilities.IsValidUSZipCode("0123a"));
        }

        [Fact]
        public void state_abbreviations_must_be_upper_case()
        {
            Assert.False(StringUtilities.IsValidStateAbbreviation("az"));
        }

        [Fact]
        public void all_us_postal_abbreviations_validate_correctly()
        {
            var stateList = new List<string>
            {
                "AL", "AK", "AS", "AZ", "AR", "CA", "CO", "CT", "DE", "DC", "FM", "FL", "GA", "GU", "HI", "ID", "IL",
                "IN", "IA", "KS", "KY", "LA", "ME", "MH", "MD", "MA", "MI", "MP", "MN", "MS", "MO", "MT", "NE", "NV",
                "NH", "NJ", "NM", "NY", "NC", "ND", "MP", "OH", "OK", "OR", "PW", "PA", "PR", "RI", "SC", "SD", "TN",
                "TX", "UT", "VT", "VI", "VA", "WA", "WV", "WI", "WY", "AA", "AE", "AP"
            };
            foreach (var abbreviation in stateList)
            {
                Assert.True(StringUtilities.IsValidStateAbbreviation(abbreviation));
            }
        }
    }
}
