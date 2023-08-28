// Copyright(c) F3H Consulting. All Rights Reserved.


using System.Reflection;

[assembly: AssemblyProduct("F3H.ProfileShark")]
[assembly: AssemblyTitle("F3H.ProfileShark")]
[assembly: AssemblyCompany("F3H Consulting")]
[assembly: AssemblyCopyright("Copyright © F3H Consulting 2023")]
[assembly: AssemblyTrademark("F3H Consulting")]

[assembly: AssemblyDescription("CGV ProfileShark")]
[assembly: AssemblyMetadata("RepositoryUrl", "https://github.com/fafa3711/ProfileShark")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif