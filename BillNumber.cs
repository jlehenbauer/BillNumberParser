using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bill_Number_Parser
{
    public class BillNumber
    {
        private Chamber chamber;
        public Chamber BillChamber
        {
            get { return chamber; }
            set
            {
                if (value != Chamber.H && value != Chamber.S)
                {
                    throw new ArgumentException("Invalid chamber. Must be 'H' or 'S'.");
                }
                chamber = value;
            }
        }

        private BillType billType;
        public BillType BillType
        {
            get { return billType; }
            set
            {
                if (value != BillType.B && value != BillType.R && value != BillType.CR && value != BillType.JR)
                {
                    throw new ArgumentException("Invalid bill type. Must be 'B', 'R', 'CR', or 'JR'.");
                }
                billType = value;
            }
        }

        private int number;
        public int Number
        {
            get { return number; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Bill number must be a positive.");
                }
                else if (value > 99999)
                {
                    throw new ArgumentException("Bill number must be less than or equal to 99999.");
                }
                number = value;
            }
        }

        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            private set
            {
                isValid = value;
            }
        }

        // Allow this to be computed at any time based on the current state
        public bool IsValidBillNumber()
        {
            return (BillChamber == Chamber.H || BillChamber == Chamber.S) &&
                   (BillType == BillType.B || BillType == BillType.R || BillType == BillType.CR || BillType == BillType.JR) &&
                   (Number > 0 && Number <= 99999);
        }

        public BillNumber(Chamber chamber, BillType billType, int number)
        {
            BillChamber = chamber;
            BillType = billType;
            Number = number;
            isValid = true;
        }

        public BillNumber(string billNumberString)
        {
            // Remove whitespace and convert to uppercase for consistent parsing
            string billNumber = Regex.Replace(billNumberString, @"\s+", "").ToUpper();

            // If the input is null or empty after trimming whitespace, it's not a valid bill number.
            if (string.IsNullOrEmpty(billNumber))
            {
                isValid = false;
                throw new ArgumentException("Bill number cannot be null or empty.");
            }

            // Regex feels uninspired, but after removing whitespace and ensuring the same case,
            // it provides a clear and consistent way to both validate the format and extract the necessary components.
            string pattern = @"^([HS]+)(CR|JR|B|R)+(\d{1,5})$";
            MatchCollection matches = Regex.Matches(billNumber, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            // If the regex doesn't match exactly once, the format is invalid.
            if (matches.Count != 1)
            {
                isValid = false;
                throw new ArgumentException("Invalid bill number format. Must state chamber (H or S) followed by the typ of resolution (B/R/CR/JR) and a number (e.g., 'HB04', 'SR123', 'HJR 01374').");
            }

            // If the pattern matches, we can safely parse the elements of the bill number
            BillChamber = (Chamber)Enum.Parse(typeof(Chamber), matches[0].Groups[1].Value);
            BillType = (BillType)Enum.Parse(typeof(BillType), matches[0].Groups[2].Value);
            Number = int.Parse(matches[0].Groups[3].Value);

            isValid = true;
        }

        public string BillNumberLong()
        {
            return string.Concat(BillChamber.ToString().Substring(0, 1), BillType.ToString(), Number.ToString("D5"));
        }

        public string BillNumberShort()
        {
            return string.Concat(BillChamber.ToString().Substring(0, 1), BillType.ToString(), Number.ToString());
        }

        // This felt like a useful and natural extension of the Short and Long strings.
        public string BillFullName()
        {
            return string.Concat(BillChamber.GetType()
                                        .GetMember(BillChamber.ToString())
                                        .FirstOrDefault()?
                                        .GetCustomAttribute<DescriptionAttribute>()?.Description,
                                 " ",
                                 BillType
                                        .GetType()
                                        .GetMember(BillType.ToString())
                                        .FirstOrDefault()?
                                        .GetCustomAttribute<DescriptionAttribute>()?.Description,
                                 " ",
                                 Number);
        }

        // Return whichever string version is most commonly used.
        public override string ToString()
        {
            return BillNumberShort();
        }

        public static bool ValidateBillNumber(string billNumber)
        {
            try
            {
                BillNumber bn = new BillNumber(billNumber);
                return bn.IsValidBillNumber();
            }
            catch
            {
                return false;
            }
        }
    }

    // Would love to talk about decision making here. 
    // Single-character enums made it easy to parse and create bill numbers,
    // but aren't as readable as the full names. 
    // Description attributes are useful to still keep that information, but as can be seen above,
    // they require a bit of reflection to access. If used, I'd probably implement a small helper function
    // to retrieve the full names from the description more clearly.
    public enum Chamber
    {
        [Description("House")]
        H,
        [Description("Senate")]
        S
    }

    public enum BillType
    {
        [Description("Bill")]
        B,
        [Description("Resolution")]
        R,
        [Description("Concurrent Resolution")]
        CR,
        [Description("Joint Resolution")]
        JR
    }
}
