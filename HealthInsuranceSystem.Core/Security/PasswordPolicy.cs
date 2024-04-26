using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HealthInsuranceSystem.Core.Security
{

    public class PasswordPolicy
    {
        public int MinimumLength { get; set; } = 1;

        public int MaximumLength { get; set; } = 32;

        public bool SymbolsRequired { get; set; }

        public bool NumericCharactersRequired { get; set; }

        public bool LowercaseCharactersRequired { get; set; }

        public bool UppercaseCharactersRequired { get; set; }

        public List<string> FormatRequirements()
        {
            return new[]
            {
                MinimumLength > 0 ? $"Be at least {MinimumLength} character(s) long" : null,
                MaximumLength > 0 ? $"Be at most {MaximumLength} character(s) long" : null,
                LowercaseCharactersRequired ? $"Contain a lowercase character" : null,
                UppercaseCharactersRequired ? $"Contain an uppercase character" : null,
                NumericCharactersRequired ? $"Contain a numeric character" : null,
                SymbolsRequired ? $"Contain a symbol: !\"#$%&'()*+,-./:;<=>?@[\\]^_`{{|}}~" : null
            }
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();
        }

        public bool IsValid(string password)
        {
            if (password.Length < MinimumLength)
                return false;

            if (MaximumLength > 0 && password.Length > MaximumLength)
                return false;

            var symbols = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

            return
                (!LowercaseCharactersRequired || Regex.IsMatch(password, @"[a-z]+")) &&
                (!UppercaseCharactersRequired || Regex.IsMatch(password, @"[A-Z]+")) &&
                (!NumericCharactersRequired || Regex.IsMatch(password, @"\d+")) &&
                (!SymbolsRequired || symbols.Any(password.Contains));
        }
    }
}
