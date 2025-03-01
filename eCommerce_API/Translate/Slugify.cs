using System.Text.RegularExpressions;

namespace eCommerce_API.Translate
{
    public class Slugify
    {
        public static string Slug(string phrase)
        {
            // Remove all accents and make the string lower case.  
            //string output = phrase.RemoveAccents().ToLower();
            string output = null;

            // Remove all special characters from the string.  
            output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");

            // Remove all additional spaces in favour of just one.  
            output = Regex.Replace(output, @"\s+", " ").Trim();

            // Replace all spaces with the hyphen.  
            output = Regex.Replace(output, @"\s", "-");

            // Return the slug.  
            return output;
        }

    }
}
