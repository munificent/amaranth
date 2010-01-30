using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public static class Sentence
    {
        public static string Format(string format, INoun subject, INoun obj)
        {
            // replace the subject
            string message = format.Replace("{subject}", subject.NounText);
            message = message.Replace("{pronoun}", subject.Pronoun);
            message = message.Replace("{possessive}", subject.Possessive);

            // replace the object
            if (obj != null)
            {
                message = message.Replace("{object}", obj.NounText);
                message = message.Replace("{object pronoun}", obj.Pronoun);
                message = message.Replace("{object possessive}", obj.Possessive);
            }

            // subject-verb agreement
            message = FixSubjectVerbAgreement(message, subject.Person);

            // sentence case the text
            message = message.Substring(0, 1).ToUpper() + message.Substring(1);

            return message;
        }

        public static string FixSubjectVerbAgreement(string text, Person person)
        {
            return sFormatPattern.Replace(text, (person == Person.Second) ?
                sReplaceSecondPerson : sReplaceThirdPerson);
        }

        /// <summary>
        /// This ugly regex matches strings containing bracketed verbs like "run[s]" and "[are|is]".
        /// It can handle up to four verbs in a single string. (Looping isn't used so a simple
        /// replace can be done.
        /// </summary>
        private static Regex sFormatPattern = new Regex(
            @"^(?<t1>[^\[]+)
              (\[((?<simple1>[^\]\|]*)|
                  ((?<before1>[^\]\|]*)\|(?<after1>[^\]\|]*))
                )\])?

              (?<t2>[^\[]+)?

              (\[((?<simple2>[^\]\|]*)|
                  ((?<before2>[^\]\|]*)\|(?<after2>[^\]\|]*))
                )\])?

              (?<t3>[^\[]+)?

              (\[((?<simple3>[^\]\|]*)|
                  ((?<before3>[^\]\|]*)\|(?<after3>[^\]\|]*))
                )\])?

              (?<t4>[^\[]+)?

              (\[((?<simple4>[^\]\|]*)|
                  ((?<before4>[^\]\|]*)\|(?<after4>[^\]\|]*))
                )\])?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static string sReplaceSecondPerson = "${t1}${before1}${t2}${before2}${t3}${before3}${t4}${before4}";
        private static string sReplaceThirdPerson = "${t1}${simple1}${after1}${t2}${simple2}${after2}${t3}${simple3}${after3}${t4}${simple4}${after4}";
    }
}