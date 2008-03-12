// see http://www.codeproject.com/KB/recipes/command_line.aspx

/*
* Command Line Arguments Class
*/

using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace WinFormsDemo
{
    /// <summary>
    /// CommandLineArguments class
    /// </summary>
    public class CommandLineArguments
    {
        private StringDictionary namedParameters = new StringDictionary();
        private System.Collections.ArrayList unnamedParameters = new System.Collections.ArrayList();

        /// <summary>
        /// Creates a <see cref="CommandLineArguments"/> object to parse
        /// command lines.
        /// </summary>
        /// <param name="args">The command line to parse.</param>
        public CommandLineArguments(string[] args)
        {
            Regex splitter = new Regex(@"^-{1,2}|^/|=|:",
                 RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex remover = new Regex(@"^['""]?(.*?)['""]?$",
                 RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string parameter = null;
            string[] parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: -param1 value1 --param2 /param3:"Test-:-work"
            //               /param4=happy -param5 '--=nice=--'
            foreach (string str in args)
            {
                // Do we have a parameter (starting with -, /, or --)?
                if (str.StartsWith("-") || str.StartsWith("/"))
                {
                    // Look for new parameters (-,/ or --) and a possible
                    // enclosed value (=,
                    parts = splitter.Split(str, 3);
                    switch (parts.Length)
                    {
                        // Found a value (for the last parameter found
                        // (space separator))
                        case 1:
                            if (parameter != null)
                            {
                                if (!namedParameters.ContainsKey(parameter))
                                {
                                    parts[0] = remover.Replace(parts[0], "$1");
                                    namedParameters.Add( parameter, parts[0]);
                                }
                                parameter = null;
                            }
                            // else Error: no parameter waiting for a value
                            // (skipped)
                            break;
                        // Found just a parameter
                        case 2:
                            // The last parameter is still waiting. With no
                            // value, set it to true.
                            if (parameter != null)
                            {
                                if (!namedParameters.ContainsKey(parameter))
                                    namedParameters.Add(parameter, "true");
                            }
                            parameter = parts[1];
                            break;
                        // parameter with enclosed value
                        case 3:
                            // The last parameter is still waiting. With no
                            // value, set it to true.
                            if (parameter != null)
                            {
                                if (!namedParameters.ContainsKey(parameter))
                                    namedParameters.Add(parameter, "true");
                            }
                            parameter = parts[1];
                            // Remove possible enclosing characters (",')
                            if (!namedParameters.ContainsKey(parameter))
                            {
                                parts[2] = remover.Replace(parts[2], "$1");
                                namedParameters.Add(parameter, parts[2]);
                            }
                            parameter = null;
                            break;
                    }
                }
                else
                {
                    unnamedParameters.Add(str);
                }
            }
            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (!namedParameters.ContainsKey(parameter))
                    namedParameters.Add(parameter, "true");
            }
        }

        /// <summary>
        /// Retrieves the parameter with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the parameter. The name is case insensitive.
        /// </param>
        /// <returns>
        /// The parameter or <c>null</c> if it can not be found.
        /// </returns>
        public string this[string name]
        {
            get { return (namedParameters[name]); }
        }

        /// <summary>
        /// Checks is aparameter with the specified name exists.
        /// </summary>
        /// <param name="name">
        /// The name of the parameter. The name is case insensitive.
        /// </param>
        /// <returns>
        /// True if the param exists, otherwise false.
        /// </returns>
        public bool HasParam(string param)
        {
            return namedParameters.ContainsKey(param);
        }

        /// <summary>
        /// Retrieves an unnamed parameter (that did not start with '-'
        /// or '/').
        /// </summary>
        /// <param name="name">The index of the unnamed parameter.</param>
        /// <returns>The unnamed parameter or <c>null</c> if it does not
        /// exist.</returns>
        /// <remarks>
        /// Primarily used to retrieve filenames which extension has been
        /// associated to the application.
        /// </remarks>
        public string this[int index]
        {
            get
            {
                return (string)(index < unnamedParameters.Count ? unnamedParameters[index] :
                null);
            }
        }

        public int UnnamedParamCount
        {
            get { return unnamedParameters.Count; }
        }
    }

    public class WorkBookArguments : CommandLineArguments
    {
        static WorkBookArguments args = null;
        public static WorkBookArguments GlobalWbArgs
        {
            get
            {
                if (args == null)
                    args = new WorkBookArguments(Environment.GetCommandLineArgs());
                return args;
            }
        }
        public WorkBookArguments(string[] args) : base(args)
        {
        }

        public bool FloatingTools
        {
            get { return HasParam("floatingtools"); }
        }
    }
}
