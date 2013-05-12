using System;
using System.Text;

namespace Entropy.SimpleStateMachine
{
    public static class UtilityExtensionMethods
    {
        public static string ExpandAtCaseTransitions(this string theString)
        {
            if (theString == null)
                return String.Empty;

            theString = theString.Trim();
            if (theString.Length < 3)
                return theString;

            var output = new StringBuilder(theString.Length + 3);
            output.Append(theString[0]);

            for (int i = 1; i < theString.Length; i++)
            {
                bool addSpace = false;

                if (Char.IsUpper(theString[i]))
                {
                    if (Char.IsLower(theString[i - 1]))
                        addSpace = true;
                    else if (i < theString.Length - 1 && Char.IsLower(theString[i + 1]))
                        addSpace = true;
                }

                if (addSpace)
                    output.Append(" ");
                output.Append(theString[i]);
            }
            return output.ToString();
        }
    }
}