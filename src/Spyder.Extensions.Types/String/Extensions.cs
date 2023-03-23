namespace Spyder.Extensions.Types.String
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Extension method for manipulating strings
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Is Null Or Empty String?
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>true/false</returns>
        public static bool IsNullOrEmpty(this string s)
            => string.IsNullOrEmpty(s);

        /// <summary>
        /// Is Null Or White Space String?
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>true/false</returns>
        public static bool IsNullOrWhiteSpace(this string s)
            => string.IsNullOrWhiteSpace(s);

        /// <summary>
        /// Examines the Char if it is digit (0-9)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDigit(this char c)
            => c >= '0' && c <= '9';

        /// <summary>
        /// Examines the String if it is Integer
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInteger(this string s)
        {
            if (s.IsNullOrEmpty())
                return false;

            return int.TryParse(s, out _);
        }

        /// <summary>
        /// Examines the String if it is Number
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNumber(this string s)
            => double.TryParse(s, out _);

        /// <summary>
        /// Is the string alphanumeric?
        /// </summary>
        /// <param name="s">text</param>
        /// <returns>true/false</returns>
        public static bool IsAlphaNumeric(this string s)
        {
            if (s.IsNullOrEmpty())
                return false;

            Regex regExp = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);

            return !regExp.IsMatch(s);
        }

        /// <summary>
        /// Is the URL string is valid?
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsValidUrl(this string s)
        {
            if (s.IsNullOrEmpty())
                return false;

            if (s.IsNullOrWhiteSpace())
                return false;

            return Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out _);
        }

        /// <summary>
        /// If the string is null convert it to empty
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string IfNullReturnEmptyString(this string? s)
            => s ?? string.Empty;

        /// <summary>
        /// Reverse the input string
        /// </summary>
        /// <param name="s">text</param>
        /// <returns>reversed text</returns>
        public static string Reverse(this string s)
        {
            if (s.IsNullOrEmpty())
            {
                return s;
            }

            char[] chars = s.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// Remove Extra Spaces from the text
        /// </summary>
        /// <param name="s">text</param>
        /// <returns>clean text</returns>
        public static string RemoveExtraSpaces(this string s)
        {
            if (s.IsNullOrEmpty())
            {
                return s;
            }

            string pattern = "\\s+";
            string replacement = " ";

            Regex rx = new Regex(pattern);

            return rx.Replace(s, replacement).Trim();
        }

        /// <summary>
        /// Replace the Tabs with Spaces and remove the extra spaces
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceTabsWithSpaces(this string s)
        {
            if (s.IsNullOrEmpty())
            {
                return s;
            }

            return s.Replace("\t", " ").RemoveExtraSpaces();
        }

        /// <summary>
        /// Replace the New Line Chars with Spaces and remove the extra spaces
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceNewLinesWithSpaces(this string s)
        {
            if (s.IsNullOrEmpty())
            {
                return s;
            }

            return s.Replace("\n", " ").RemoveExtraSpaces();
        }

        /// <summary>
        /// Clean the text from the tabs, New Line Chars, and Extra Spaces
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToCleanString(this string s)
        {
            if (s.IsNullOrEmpty())
            {
                return s;
            }

            return s.Replace("\n", " ")
                .Replace("\t", " ")
                .RemoveExtraSpaces();
        }

        /// <summary>
        /// Remove all characters rather than letters and numbers
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToAlphaNumericString(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            // Use regular expressions to replace characters
            // that are not letters or numbers with spaces.
            Regex regExp = new Regex("[^a-zA-Z0-9]");
            return regExp.Replace(s, string.Empty).ToCleanString();
        }

        /// <summary>
        /// Convert string to HTML format (Replacing \n with equivalent HTML Tag)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHtml(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            s = Regex.Replace(s, @"\n\r?", "<br/>");

            return s.ToHyperlink();
        }

        /// <summary>
        /// To N Number of Characters Abbreviation
        /// </summary>
        /// <param name="s"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string ToNcharAbbreviation(this string s,
            int n)
        {
            if (s.IsNullOrWhiteSpace())
            {
                return s;
            }

            int numberOfWords = s.GetTotalNumberOfWords();

            int abbreviationLength = numberOfWords >= n ? n : numberOfWords;

            string result = string.Empty;

            string[] words = s.ToCleanString().Split(' ');

            for (int i = 0; i < abbreviationLength; i++)
            {
                result += char.ToUpper(words[i][0]);
            }

            return result;
        }

        /// <summary>
        /// Return List of distinct words of a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static List<string> ToDistinctListOfWords(this string s)
        {
            if (s.IsNullOrWhiteSpace())
                return new List<string>();

            // Use regular expressions to replace characters
            // that are not letters or numbers with spaces.
            Regex regExp = new Regex("[^a-zA-Z0-9]");
            s = regExp.Replace(s, " ");

            // Split the text into words.
            string[] words = s.Split(
                new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            var wordQuery =
                (from string word in words
                 orderby word
                 select word).Distinct();

            return wordQuery.ToList();
        }

        /// <summary>
        /// Convert string to Hyperlink
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHyperlink(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (s.IsNullOrWhiteSpace())
                return string.Empty;

            Regex r = new Regex("(https?://[^ ]+)");

            return r.Replace(s, "<a href=\"$1\" target=\"_blank\">$1</a>");
        }

        /// <summary>
        /// To Abbreviation
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToAbbreviation(this string s)
        {
            if (s.IsNullOrWhiteSpace())
            {
                return s;
            }

            string result = string.Empty;

            string[] words = s.ToCleanString().Split(' ');

            return words.Aggregate(result, (current, word) => current + char.ToUpper(word[0]));
        }

        /// <summary>
        /// Returns Sentence Case of a text
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToSentenceCase(this string s, char separator)
        {
            if (s.IsNullOrEmpty())
                return s;

            s = s.Trim().ToCleanString();
            if (s.IsNullOrEmpty())
                return s;

            // Only 1 separator
            if (s.IndexOf(separator) < 0)
            {
                s = s.ToLower();
                s = s[0].ToString().ToUpper() + s.Substring(1);
                return s;
            }

            if (s.Trim().Last() == separator)
            {
                s = s.Trim().Remove(s.Length - 1, 1);
            }

            // More than 1 separator.
            string[] sentences = s.Split(separator);
            StringBuilder buffer = new StringBuilder();

            foreach (string sentence in sentences)
            {
                string currentSentence = sentence.ToLower().Trim();
                if (!string.IsNullOrWhiteSpace(currentSentence))
                {
                    currentSentence = $"{currentSentence[0].ToString().ToUpper()}{currentSentence.Substring(1)}";
                    buffer.Append(currentSentence + separator + ' ');
                }
            }

            s = buffer.ToString();
            return s.Trim();
        }

        /// <summary>
        /// Convert a string to hash (16 bytes)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string To16ByteSaltedHash(this string s)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(s, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Encode a string into Base64
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string ToBase64(this string plainText)
        {
            if (plainText.IsNullOrEmpty())
                return plainText;

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Extract Summary Text of long Text
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <param name="dotsToBeUsed"></param>
        /// <returns></returns>
        public static string ToSummarizedText(
            this string s,
            int length,
            bool dotsToBeUsed = false)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            return s.Length > length ?
                                dotsToBeUsed ? s[..length] + "..."
                                : s[..length]
                                : s;
        }

        /// <summary>
        /// Extract Summary Text of long Text (from the right)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <param name="dotsToBeUsed"></param>
        /// <returns></returns>
        public static string ToSummarizedTextRight(
            this string s,
            int length,
            bool dotsToBeUsed = false)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            return s.Length > length ?
                                dotsToBeUsed ? "..." + s.Substring(s.Length - length, length)
                                : s.Substring(s.Length - length, length)
                                : s;
        }

        /// <summary>
        /// Eliminate accent from a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToAccentEliminated(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;


            StringBuilder result = new StringBuilder(s.Normalize(NormalizationForm.FormD).Length);

            foreach (var character in s)
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                    result.Append(character);

            return result.ToString();
        }

        /// <summary>
        /// Convert the text to be support being CSV
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToCsvCompatible(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (s.Contains("\""))
                s = s.Replace("\"", "\"\"");

            if (s.Contains(",")
                || s.Contains(";")
                || s.Contains("\"")
                || s.Contains("\n")
                || s[0] == ' '
                || s[^1] == ' ')
            {
                s = $"\"{s}\"";
            }

            return s;
        }

        /// <summary>
        /// Convert the traditional Guid to UpperCase Canonical Guid (without hyphen)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToUpperCaseCanonicalGuid(this Guid guid)
        {
            return guid.ToString("N").ToUpper();
        }

        /// <summary>
        /// Returns double quoted string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToDoubleQuotedString(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (s.IsNullOrWhiteSpace())
                return s;

            return $"\"{s}\"";
        }

        /// <summary>
        /// Extract String Array from Delimited String
        /// </summary>
        /// <param name="s"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[] ToStringArrayFromDelimitedString(
            this string s,
            char delimiter)
        {
            if (s.IsNullOrEmpty())
                return Array.Empty<string>();

            if (s.IsNullOrWhiteSpace())
                return Array.Empty<string>();

            return s.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Generate a Url Friendly String
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToUrlFriendly(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            string result = Regex.Replace(s, @"[^-\w]+", string.Empty);

            return Regex.Replace(result.ToCleanString(), @"\s+", "-");
        }

        /// <summary>
        /// Convert the traditional Guid to Canonical Guid (without hyphen)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToCanonicalGuid(this Guid guid)
        {
            return guid.ToString("N");
        }

        /// <summary>
        /// Capitalize Each Word
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string CapitalizeEachWord(this string s)
        {
            if (s.IsNullOrEmpty())
            {
                return s;
            }

            var x = Thread.CurrentThread.CurrentCulture
                .TextInfo
                .ToTitleCase(s.ToLower());

            return x;
        }

        /// <summary>
        /// Finds the number of characters in a string
        /// - include/exclude extra spaces, tabs and new line characters
        /// </summary>
        /// <param name="s"></param>
        /// <param name="clean"></param>
        /// <returns></returns>
        public static int GetTotalNumberOfCharacters(
            this string s,
            bool clean = false)
        {
            if (s.IsNullOrEmpty())
                return 0;

            return clean != true ? s.Length : s.ToCleanString().Length;
        }

        /// <summary>
        /// Finds the number of words in a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int GetTotalNumberOfWords(this string s)
        {
            if (s.IsNullOrWhiteSpace())
                return 0;

            int result = 1;

            s = s.ToCleanString();

            for (int i = 0; i <= s.Length - 1; i++)
            {
                if (s[i] == ' ')
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Get The Longest Word in a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetFirstLongestWord(this string s)
        {
            if (s.IsNullOrWhiteSpace())
                return s;

            return s.ToCleanString()
                    .Split(' ')
                    .OrderByDescending(word => word.Length)
                    .First();
        }

        /// <summary>
        /// Finds the number of lines in a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int GetTotalNumberOfLines(this string s)
        {
            if (s.IsNullOrWhiteSpace())
                return 0;

            int result = 1;

            for (int i = 0; i <= s.Length - 1; i++)
            {
                if (s[i] == '\n')
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Get The Shortest Word in a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetFirstShortestWord(this string s)
        {
            if (s.IsNullOrWhiteSpace())
                return s;

            return s.ToCleanString()
                    .Split(' ')
                    .OrderBy(word => word.Length)
                    .First();
        }

        /// <summary>
        /// Counts the Occurrences of a pattern in a text,
        /// The default is Case Sensitive search
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public static int CountPatternOccurrences(
            this string s,
            string pattern,
            bool isCaseSensitive = true)
        {
            if (s.IsNullOrEmpty())
                return 0;

            if (pattern.IsNullOrEmpty())
                return 0;

            string tempS = s;
            string tempP = pattern;

            if (isCaseSensitive != true)
            {
                tempS = s.ToLower();
                tempP = pattern.ToLower();
            }

            int count = 0;
            int i = 0;
            while ((i = tempS.IndexOf(tempP, i, StringComparison.Ordinal)) != -1)
            {
                i += tempP.Length;
                count++;
            }
            return count;
        }

        /// <summary>
        /// Remove specific number of chars from at the begining of text
        /// </summary>
        /// <param name="s">the text</param>
        /// <param name="n">the number of characters</param>
        /// <returns>resulted text</returns>
        public static string RemoveNumberOfCharsAtBeginning(this string s, int n)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n));

            return s.Length < n ? string.Empty : s.Substring(n);
        }

        /// <summary>
        /// Remove All Line Breaks in a text
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveAllLineBreaks(this string s)
        {
            return s.IsNullOrEmpty() ? s : s.Replace("\n", string.Empty).Replace("\r", string.Empty);
        }

        /// <summary>
        /// Extract the file extension from a valid
        /// path string or returns empty string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetFileExtension(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            int pos = s.LastIndexOf('.');

            if (pos < 0)
                return string.Empty;

            return s.Substring(pos + 1).Trim();
        }

        /// <summary>
        /// Get Domain Part of a valid URL string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetUrlDomain(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (s.IsNullOrWhiteSpace())
                return string.Empty;

            var match = Regex.Match(s, @"^http[s]?[:/]+[^/]+");

            return match.Success ? match.Captures[0].Value : s;
        }

        /// <summary>
        /// Get Domain Part of a valid URL
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetUrlDomain(this Uri uri)
        {
            if (uri.ToString().IsNullOrEmpty())
                return uri.ToString();

            return uri.ToString().GetUrlDomain();
        }

        /// <summary>
        /// Return the number of Occurrences of a Char in a string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="targeted"></param>
        /// <returns></returns>
        public static int CharacterOccurrences(this string s, char targeted)
        {
            if (s.IsNullOrEmpty())
                return 0;

            if (s.IsNullOrWhiteSpace())
                return 0;

            return s.Count(f => f == targeted);
        }

        /// <summary>
        /// Return the number of Occurrences of a string in a text
        /// </summary>
        /// <param name="s"></param>
        /// <param name="targeted"></param>
        /// <returns></returns>
        public static int StringOccurrences(
            this string s,
            string targeted)
        {
            if (s.IsNullOrEmpty())
                return 0;

            if (s.IsNullOrWhiteSpace())
                return 0;

            return s.Split(targeted).Length - 1;
        }

        /// <summary>
        /// Add a specific text to the begining if missed
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AddToBeginningIfMissed(this string s, string value)
        {
            if (value.IsNullOrEmpty())
                throw new ArgumentException("the missed text cannot be empty!");

            if (!s.IfNullReturnEmptyString().StartsWith(value, StringComparison.CurrentCulture))
                s = value + s;

            return s;
        }

        /// <summary>
        /// Return a string of all digits (0-9) available in a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FindAllDigits(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            char[] chars = s.ToCharArray();

            string digits = "";

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i].IsDigit())
                {
                    digits += chars[i];
                }
            }

            return digits;
        }

        /// <summary>
        /// Return only the numbers of a string
        /// </summary>
        /// <param name="s">The input string</param>
        /// <returns>The Numeric Chars of a string</returns>
        public static string CleanNonNumericChars(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            var regExp = new Regex("[^0123456789.]");
            return regExp.Replace(s, string.Empty);
        }

        /// <summary>
        /// Return the number of digits (0-9) available in a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int FindNumberOfDigits(this string s)
        {
            if (s.IsNullOrEmpty())
                return 0;

            if (s.IsNullOrWhiteSpace())
                return 0;

            char[] chars = s.ToCharArray();

            int digits = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i].IsDigit())
                {
                    digits++;
                }
            }

            return digits;
        }

        /// <summary>
        /// Convert the slashes in a path with the right
        /// format depending on RuntimePlatform (Windows or else)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string CorrectPathSlashes(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? s.Replace("/", "\\") : s.Replace("\\", "/");
        }

        /// <summary>
        /// Align a string in the middle of a Space Block
        /// </summary>
        /// <param name="s"></param>
        /// <param name="blockLength"></param>
        /// <returns></returns>
        public static string CenterAligned(this string s, int blockLength)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (blockLength <= 0)
                return s;

            if (blockLength < s.Length)
                return s;

            string result = s.Length > blockLength ? s.Substring(0, blockLength) : s;

            int reminderBlockSpace = blockLength - result.Trim().Length;
            int sideSpace = reminderBlockSpace / 2;
            bool sideSpaceIsEven = reminderBlockSpace % 2 == 0;

            return new string(' ', sideSpace)
                + result
                + new string(' ', sideSpaceIsEven || sideSpace == 0 ? sideSpace : sideSpace - 1);
        }

        /// <summary>
        /// Replace non ASCII characters with custom char
        /// </summary>
        /// <param name="s">the input string</param>
        /// <param name="r">the replacement character</param>
        /// <returns></returns>
        public static string ReplaceNonAsciiCharsWith(this string s, char r)
        {
            return s.IsNullOrEmpty() ? s : Regex.Replace(s, @"[^\u0000-\u007F]", r.ToString());
        }

        /// <summary>
        /// Replace first occurrence of a string in a text
        /// </summary>
        /// <param name="s"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceFirstOccurrence(
            this string s,
            string search,
            string replace)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (search == null)
                throw new ArgumentNullException(nameof(search));

            if (replace == null)
                throw new ArgumentNullException(nameof(replace));

            int pos = s.IndexOf(search, StringComparison.Ordinal);

            if (pos < 0)
            {
                return s;
            }

            return s.Substring(0, pos) + replace + s.Substring(pos + search.Length);
        }

        /// <summary>
        /// Align a string in the right of a Space Block
        /// </summary>
        /// <param name="s"></param>
        /// <param name="blockLength"></param>
        /// <returns></returns>
        public static string RightAligned(this string s, int blockLength)
        {
            if (s.IsNullOrEmpty() && blockLength <= 0)
                return s;

            if (blockLength < s.Length)
                return s;

            string result = s.Length > blockLength ? s.Substring(0, blockLength) : s;

            int reminderBlockSpace = blockLength - result.Trim().Length;

            return new string(' ', reminderBlockSpace) + result;
        }

        /// <summary>
        /// Remove Non ASCII Chars from a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveNonAsciiChars(this string s)
        {
            return s.IsNullOrEmpty() ? s : Regex.Replace(s, @"[^\u0000-\u007F]", string.Empty);
        }

        /// <summary>
        /// Replace all Double Quotes in a string with Single
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceDoubleQuotesWithSingle(this string s)
        {
            return s.IsNullOrEmpty() ? s : s.Replace("\"", "'");
        }

        /// <summary>
        /// Return null if the string match the check string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="checkValue"></param>
        /// <returns></returns>
        public static string? NullIfEqualTo(
            this string s,
            string checkValue) => s == checkValue ? null : s;

        /// <summary>
        /// Add a specific text to the end if missed
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AddToEndIfMissed(this string s, string value)
        {
            if (value.IsNullOrEmpty())
                throw new ArgumentException("the missed text cannot be empty!");

            if (!s.IfNullReturnEmptyString()
                .EndsWith(value, StringComparison.CurrentCulture))
                s += value;

            return s;
        }

        /// <summary>
        /// Replace all Single Quotes in a string with Double
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceSingleQuotesWithDouble(this string s)
        {
            return s.IsNullOrEmpty() ? s : s.Replace("'", "\"");
        }

        /// <summary>
        /// Generate Lorem Ipsum Html Paragraph text
        /// </summary>
        /// <param name="minWords"></param>
        /// <param name="maxWords"></param>
        /// <param name="minSentences"></param>
        /// <param name="maxSentences"></param>
        /// <param name="numParagraphs"></param>
        /// <returns></returns>
        public static string GenerateLoremIpsumHtmlSafe(
            int minWords,
            int maxWords,
            int minSentences,
            int maxSentences,
            int numParagraphs)
        {
            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            Random rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;

            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new StringBuilder();

            for (int p = 0; p < numParagraphs; p++)
            {
                result.Append("<p>");
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result.Append(" "); }
                        result.Append(words[rand.Next(words.Length)]);
                    }
                    result.Append(". ");
                }
                result.Append("</p>");
            }

            return result.ToString();
        }

        /// <summary>
        /// Return the first possible not null value of a string array
        /// </summary>
        /// <param name="values">string array</param>
        /// <returns></returns>
        public static string Coalesce(params string[] values)
        {
            return values.FirstOrDefault(t => t != null) ?? string.Empty;
        }

        /// <summary>
        /// Return the first possible null or empty value of a string array
        /// </summary>
        /// <param name="values">string array</param>
        /// <returns></returns>
        public static string GetFirstNullOrEmpty(params string[] values)
        {
            return values.FirstOrDefault(value => value.IsNullOrEmpty()) ?? string.Empty;
        }

        /// <summary>
        /// Remove specific number of chars from at the end of text
        /// </summary>
        /// <param name="s">the text</param>
        /// <param name="n">the number of characters</param>
        /// <returns>resulted text</returns>
        public static string RemoveNumberOfCharsAtEnd(this string s, int n)
        {
            if (s.IsNullOrEmpty())
                return s;

            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n));

            return s.Length < n ? string.Empty : s.Remove(s.Length - n);
        }

        /// <summary>
        /// Generate Lorem Ipsum text
        /// </summary>
        /// <param name="minWords"></param>
        /// <param name="maxWords"></param>
        /// <param name="minSentences"></param>
        /// <param name="maxSentences"></param>
        /// <param name="numParagraphs"></param>
        /// <returns></returns>
        public static string GenerateLoremIpsumString(
            int minWords,
            int maxWords,
            int minSentences,
            int maxSentences,
            int numParagraphs)
        {
            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            Random rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;

            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new StringBuilder();

            for (int p = 0; p < numParagraphs; p++)
            {
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result.Append(" "); }
                        result.Append(words[rand.Next(words.Length)]);
                    }
                    result.Append(". ");
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Decode a Base64 data into plain string
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string DecodeBase64(this string base64EncodedData)
        {
            if (base64EncodedData.IsNullOrEmpty())
                return base64EncodedData;

            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}