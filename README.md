## Information

exomia/configuration supports loading of ini, xml configuration files & command line configurations

![](https://img.shields.io/github/issues-pr/exomia/configuration.svg)
![](https://img.shields.io/github/issues/exomia/configuration.svg)
![](https://img.shields.io/github/last-commit/exomia/configuration.svg)
![](https://img.shields.io/github/contributors/exomia/configuration.svg)
![](https://img.shields.io/github/commit-activity/y/exomia/configuration.svg)
![](https://img.shields.io/github/languages/top/exomia/configuration.svg)
![](https://img.shields.io/github/languages/count/exomia/configuration.svg)
![](https://img.shields.io/github/license/exomia/configuration.svg)

## Installing

```shell
[Package Manager]
PM> Install-Package Exomia.Configuration
```

## Features

- ini, xml & argv support
- merging of IConfigSource together
- expanding of keys
- comments
- ...

## Example

#### Ini-Files / XML-Files
```csharp
using Exomia.Configuration;
using Exomia.Configuration.Ini;

IConfigSource source = new IniConfigSource() { SaveFileName = "test.ini" };
//IConfigSource source = new XmlConfigSource() { SaveFileName = "test.xml" };

IConfig cfg1 = source.Add("section1", "this is my first section");
cfg1.Set("arg1", 345, "number");
cfg1.Set("arg2", "text text...", "text");
cfg1.Set("arg3", 987.455f, "floating point number");

IConfig cfg2 = source.Add("section2", "this is my second section");
cfg2.Set("base_path", "C:\\Temp", "base path");
cfg2.SetExpanded("path1", "{0}\\file1.txt", "file1", "base_path");
cfg2.SetExpanded("path2", "{0}\\file2.txt", "file2", "base_path");
cfg2.Set("dir", "test", "dir");
cfg2.SetExpanded("path3", "{0}\\{1}\\file{2}.txt", "file3", "base_path", "dir", "section1.arg1");

source.Save();
```

test.ini
```ini
[section1] ;this is my first section
arg1 = 345 ;number
arg2 = text text... ;text
arg3 = 987.455 ;floating point number

[section2] ;this is my second section
base_path = C:\Temp ;base path
path1 = ${base_path}\file1.txt ;file1
path2 = ${base_path}\file2.txt ;file2
dir = test ;dir
path3 = ${base_path}\${dir}\file${section1.arg1}.txt ;file3
```

test.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<config>
	<section name="section1" comment="this is my first section">
		<item key="arg1" value="345" comment="number" />
		<item key="arg2" value="text text..." comment="text" />
		<item key="arg3" value="987.455" comment="floating point number" />
	</section>
	<section name="section2" comment="this is my second section">
		<item key="base_path" value="C:\Temp" comment="base path" />
		<item key="path1" value="${base_path}\file1.txt" comment="file1" />
		<item key="path2" value="${base_path}\file2.txt" comment="file2" />
		<item key="dir" value="test" comment="dir" />
		<item key="path3" value="${base_path}\${dir}\file${section1.arg1}.txt" comment="file3" />
	</section>
</config>
```

```csharp
using Exomia.Configuration;
using Exomia.Configuration.Ini;

IConfigSource source2 = IniParser.Parse("test.ini");
//IConfigSource source2 = XmlParser.Parse("test.xml");

if (!source2.TryGet("section1", out IConfig cfg3))
{
    //DEFAULTS
	//cfg3 = source2.Add("section1");
    //cfg3 = source2.Add("section1", "this is my first section");
    //cfg3.Set("arg1", 345, "number");
    //cfg3.Set("arg2", "text text...", "text");
    //cfg3.Set("arg3", 987.455f, "floating point number");
}

Console.WriteLine(cfg3.Get<int>("arg1"));		//345
Console.WriteLine(cfg3.Get<string>("arg2"));	//text text...
Console.WriteLine(cfg3.Get<float>("arg3"));		//987,455

if (!source2.TryGet("section2", out IConfig cfg4))
{
    //DEFAULTS
	//cfg4 = source2.Add("section2");
    //cfg4.Set("base_path", "C:\\Temp", "base path");
    //cfg4.SetExpanded("path1", "{0}\\file1.txt", "file1", "base_path");
    //cfg4.SetExpanded("path2", "{0}\\file2.txt", "file2", "base_path");
    //cfg4.Set("dir", "test", "dir");
    //cfg4.SetExpanded("path3", "{0}\\{1}\\file{2}.txt", "file3", "base_path", "dir", "section1.arg1");
}

Console.WriteLine(cfg4.Get<string>("base_path"));		//C:\Temp
Console.WriteLine(cfg4.GetExpanded<string>("path1"));	//C:\Temp\file1.txt
Console.WriteLine(cfg4.GetExpanded<string>("path2"));	//C:\Temp\file2.txt
Console.WriteLine(cfg4.Get<string>("dir"));				//test
Console.WriteLine(cfg4.GetExpanded<string>("path3"));	//C:\Temp\test\file1.txt
```

#### Argv (work in progress)
```csharp
//test.exe -p="path to sth" number=453
IConfigSource source3 = ArgvParser.Parse(args, 0, "argv", "argv section");
IConfig cfga = source3.Get("argv");
Console.WriteLine(cfga.Get<string>("-p"));   	//path to sth
Console.WriteLine(cfga.Get<string>("number"));	//453
```

---
## Social

[![Discord](https://img.shields.io/discord/427640639732187136.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.com/invite/ZFJXe6f)
[![Twitch](https://img.shields.io/twitch/status/exomia.svg?label=&logo=twitch&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://www.twitch.tv/exomia/about)