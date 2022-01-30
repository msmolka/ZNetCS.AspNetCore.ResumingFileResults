# ZNetCS.AspNetCore.ResumingFileResults

[![NuGet](https://img.shields.io/nuget/v/ZNetCS.AspNetCore.ResumingFileResults.svg)](https://www.nuget.org/packages/ZNetCS.AspNetCore.ResumingFileResults)
[![Build](https://github.com/msmolka/ZNetCS.AspNetCore.ResumingFileResults/workflows/build/badge.svg)](https://github.com/msmolka/ZNetCS.AspNetCore.ResumingFileResults/actions)

A small package to allow using resume during transfering data over MVC application in ASP.NET Core.

It allows provide `ETag` header as well as `Last-Modified` one. It also supports following precondition headers: `If-Match`, `If-None-Match`, `If-Modified-Since`
, `If-Unmodified-Since`, `If-Range`.

## ASP.NET Core 2.0

As from version 2.0 resuming is supported out of box inside ASP.NET Core. So all code related to resuming was removed. I left only part for `Content-Disposition` inline.
Now all code relies on base .NET classes. Also support for multipart request is removed. To support that I would have to copy a lot of original code, because currently
there is no way to simple override some part of base classes.

## Installing

Install using the [ZNetCS.AspNetCore.ResumingFileResults NuGet package](https://www.nuget.org/packages/ZNetCS.AspNetCore.ResumingFileResults)

```
PM> Install-Package ZNetCS.AspNetCore.ResumingFileResults
```

## Usage

When you install the package, it should be added to your `.csproj`. Alternatively, you can add it directly by adding:

```xml
<ItemGroup>
    <PackageReference Include="ZNetCS.AspNetCore.ResumingFileResults" Version="6.0.0" />
</ItemGroup>
```

### .NET 6
```c#
// Add services to the container.
builder.Services.AddResumingFileResult();
```


### .NET 5 and Below

In order to use the ResumingFileResults, you must configure the services in the `ConfigureServices` call of `Startup`:


```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddResumingFileResult();
}
```

### Use inside controller

Then in your controller you can use it in similar way as build in `FileResult` helpers.

```c#
using ZNetCS.AspNetCore.ResumingFileResults.Extensions;
```

```
...
```

```c#
public IActionResult FileContents()
{
    string webRoot = this.hostingEnvironment.WebRootPath;
    var contents = System.IO.File.ReadAllBytes(Path.Combine(webRoot, "TestFile.txt"));

    return this.ResumingFile(contents, "text/plain", "TestFile.txt");
}

...

public IActionResult FileStream()
{
    string webRoot = this.hostingEnvironment.WebRootPath;
    FileStream stream = System.IO.File.OpenRead(Path.Combine(webRoot, "TestFile.txt"));
    return this.ResumingFile(stream, "text/plain", "TestFile.txt");
}

...
       
public IActionResult PhysicalFile()
{
    string webRoot = this.hostingEnvironment.WebRootPath;
    return this.ResumingPhysicalFile(Path.Combine(webRoot, "TestFile.txt"), "text/plain", "TestFile.txt");
}

...
    
public IActionResult VirtualFile()
{
    return this.ResumingFile("TestFile.txt", "text/plain", "TestFile.txt");
}
```

Above examples will serve your data as `Content-Disposition: attachment`. When fileName is not provided then data is served as `Content-Disposition: inline`. Additionaly
it is possible provide `ETag` and `LastModified` headers.

```c#
public IActionResult File()
{
    return new ResumingVirtualFileResult("TestFile.txt", "text/plain", "\"MyEtagHeader\"") 
    { 
        FileDownloadName = "TestFile.txt", 
        LastModified = DateTimeOffset.Now 
    };
}
```


