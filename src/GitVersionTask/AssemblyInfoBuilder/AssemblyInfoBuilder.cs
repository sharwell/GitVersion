﻿using System;
using System.Collections.Generic;
using System.Linq;
using GitVersion;

public class AssemblyInfoBuilder
{
    public string GetAssemblyInfoText(VersionVariables vars, string assemblyName)
    {
        var v = vars.ToList();

        var assemblyInfo = string.Format(
@"using System;
using System.Reflection;

[assembly: AssemblyVersion(""{0}"")]
[assembly: AssemblyFileVersion(""{1}"")]
[assembly: AssemblyInformationalVersion(""{2}"")]
[assembly: {6}.ReleaseDate(""{3}"")]
[assembly: {6}.GitVersionInformation()]

namespace {6}
{{
    [System.Runtime.CompilerServices.CompilerGenerated]
    [AttributeUsage(AttributeTargets.Assembly)]
    sealed class ReleaseDateAttribute : System.Attribute
    {{
        public string Date {{ get; private set; }}

        public ReleaseDateAttribute(string date)
        {{
            Date = date;
        }}
    }}

    [System.Runtime.CompilerServices.CompilerGenerated]
    static class GitVersionInformation
    {{
{4}
    }}

    [System.Runtime.CompilerServices.CompilerGenerated]
    [AttributeUsage(AttributeTargets.Assembly)]
    sealed class GitVersionInformationAttribute : System.Attribute
    {{
{5}
    }}
}}
",
        vars.AssemblySemVer,
        vars.MajorMinorPatch + ".0",
        vars.InformationalVersion,
        vars.CommitDate,
        GenerateStaticVariableMembers(v),
        GenerateAttributeVariableMembers(v),
        assemblyName);

        return assemblyInfo;
    }

    static string GenerateStaticVariableMembers(IList<KeyValuePair<string, string>> vars)
    {
        return GenerateMembers(vars, "        public static string {0} = \"{1}\";");
    }


    static string GenerateAttributeVariableMembers(IList<KeyValuePair<string, string>> vars)
    {
        return GenerateMembers(vars, "        public string {0} {{ get {{ return \"{1}\"; }} }}");
    }

    static string GenerateMembers(IList<KeyValuePair<string, string>> vars, string memberFormat)
    {
        return string.Join(Environment.NewLine, vars.Select(v => string.Format(memberFormat, v.Key, v.Value)));
    }
}