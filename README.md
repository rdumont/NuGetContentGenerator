NuGet Content Generator
=======================

An MSBuild task to insert replacement tokens in C# files.

What does it do?
----------------

This tool helps you in creating NuGet packages by automating the process of converting *.cs into *.cs.pp files.
In the process, it creates the desired replacement tokens that can be used by NuGet when adding content files.

### Here is an example

Given the following directory structure:

```
MySolution:
- MyProject:
  - content:
    - SomeClassToPackage.cs
  - MyProject.csproj
- MySolution.sln
```

And that you have installed NuGet Content Generator in the `MyProject` project, when you build it, a new file called `SomeClassToPackage.cs.pp` will be created at the same level of `SomeClassToPackage.cs`, resulting in the following structure:

```
MySolution:
- MyProject:
  - content:
    - SomeClassToPackage.cs
    - SomeClassToPackage.cs.pp
  - MyProject.csproj
- MySolution.sln
```

> **Important**: the new *.pp files will NOT be added to the project, as they will only be created on your file system.

### Content transformations

NuGet supports some replacement tokens that are evaluated at package installation time. The most common is `rootnamespace`, although many others are available. More on this can be found in [the NuGet documentation](http://docs.nuget.org/docs/creating-packages/configuration-file-and-source-code-transformations#Specifying_Source_Code_Transformations "the NuGet documentation").

Let's say that you have the following `CategoryInfo.cs` file:

```csharp
/** @pp
 * rootnamespace: MySolution.SomeNamespace.Content
 */
namespace MySolution.SomeNamespace.Content.Models {
    public struct CategoryInfo {
        public string categoryid;
        public string description;
        public string htmlUrl;
        public string rssUrl;
        public string title;
    }
}
```

After the transformation, the symbols defined in the `@pp` comment section will be inserted where specified, resulting in the following `CategoryInfo.cs.pp` file:

```csharp
namespace $rootnamespace$.Models {
    public struct CategoryInfo {
        public string categoryid;
        public string description;
        public string htmlUrl;
        public string rssUrl;
        public string title;
    }
}
```

You can define as many replacements as you would like, one in each line. The only restriction is that the comment section for defining these replacements **must be** the first thing in the file.
