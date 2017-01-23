# dynamics-crm-ci-clis

Command line tools for continuous integration when developing with Microsoft Dynamics CRM / Microsoft Dynamics 365.

## Overview

The WebResourceHelper can be used to upload / update / sync web resources from a folder to a Microsoft Dynamics CRM solution, through a command line interface, and is very useful when you wish to integrate continous integration into your Dynamics CRM Web Resource workflow.

The PluginWorkflowHelper is currently under development.

Running the helpers requires support for .NET framework 4.5.2.

## Download & Installation

The lastest helpers are published [in the github releases section](https://github.com/Softwire/dynamics-crm-ci-clis/releases) - the latest versions are:


* *WebResourceHelper* - v1.0.0 - [available here](https://github.com/Softwire/dynamics-crm-ci-clis/releases/tag/v1.0.0-webresource)
* *PluginWorkflowHelper* - not yet released

Download and extract the zip file into a folder.

# Features & Documentation

## WebResourceHelper

This tool can be used to syncing an entire folder with the webresources in a solution on CRM. It can also be used to run a publish all command against a CRM organisation.

It is a much improved / automatic version of the Web Resource Uploader that bundles with the CRM SDK. It is particularly useful as part of a continuous integration set-up. You can, for example, use it with the output of grunt/gulp build, or just an unbuilt folder containing javascript/html/images.

### Suggested Usage

Run

```.\WebResourceHelper.exe /h```

for full instructions. There are many paramaters (17+) for different options. A suggested run might be:

```.\WebResourceHelper.exe /crmconnectionstring:"Url=https://contoso.crm2.dynamics.com/Contoso; Username=john.doe@contoso.com; Password=PASS" /solutionname:"WebResources" /folderpath:"build/webresources/" /removemissing```

This syncs the web resource content of the folder at ./build/webresources to the CRM using the given connection string. It will prompt you to confirm before any actions are taken. Once happy with its behaviour, you can use the `/automate` paramater in a batch / bash file to run the cli without a prompt. It will return an (error) code 1 if it errors so that a CI build can be failed.

It will only sync files with one of the following extensions (as of v1.0.0): .css, .xml, .gif, .htm, .html, .ico, .jpg, .jpeg, .png, .js, .json, .map, .xap, .xsl, .xslt, .eot, .svg, .ttf, .woff, .woff2

As of v1.0.0, the following command line options are supported:

* ShouldShowHelp - `/h` - _Shows the help message._
* InformationLevel - `/loglevel:X` - _How verbose the output should be. Allowed values: trace, debug, info, warn, error. Default: debug_
* ConnectionString (required) - `/crmconnectionstring:X` - _See: https://technet.microsoft.com/en-us/library/gg695810.aspx. Example use - /crmconnectionstring:'Url=http://server/org; Domain=dom; Username=user; Password=pass'_
* RootFolderPath (required) - `/folderpath:X` - _Provide the path (relative or absolute) to the directory to upload files from. Example use - /folderpath:build/release/_
* SolutionName (required) - `/solutionname:X` - _The solution to upload to. Example use - /solutionname:Contoso_
* PathSeparator - `/pathseparator:X` - _The path separator. Defaults to /. Example use - /pathseparator:\\_
* PublisherPrefixOverride - `/publisherprefixoverride:X` - _Including this option means that the solution's publisher prefix (eg new- _) is replaced. Don't include the underscore in this parameter. Example usage - /publisherprefixoverride:pub which will result in file names such as (eg) pub- _/abc.htm_
* Prefix - `/prefix:X` - _The prefix after the publisher prefix, but before the relative paths are used. Defaults to the path separator (which defaults to /). Example use - /prefix:/projecta/ which would result in a file at widgetb/index.htm being uploaded (say) as new- _/projecta/widgetb/index.htm. A blank string may also be used by writing: /prefix:_
* FileSizeLimit - `/filesizelimitkb:X` - _The file size limit in kilobytes at which a file is marked as invalid. By default, this uses 5120 (ie, 5MB), which is the default value on a fresh Dynamics install. Only change this command line option if you have changed the corresponding value on the CRM (the CRM setting is the same setting as the email attachment size limit)._
* StatusOnly - `/status` - _Using this parameter only gives a diff against the CRM solution, but will not run or ask permission to run any actions._
* Force - `/force` - _Using this bypasses the confirmation dialog, and all valid actions will be run. Invalid resources will be ignored. Normally you should prefer /automate_
* Automate - `/automate` - _Using this causes the confirmation dialog to be bypassed. Actions are run only if all resources pass validation checks - otherwise the program exits with an error. This is recommended in CI situations._
* NoNew - `/nonew` - _Using this parameter means that no new files not currently present in the CRM are uploaded to the CRM. This could be useful if you don't have a build process, or your build process outputs web resources you do not wish to be uploaded. In this case, the solution defines the file names you are allowed to upload._
* NoUpdates - `/noupdates` - _Using this parameter means that local web resources are ignored if there is currently a web resource with the same name in the solution on the CRM._
* RemoveMissing - `/removemissing` - _Using this parameter deletes web resources off the CRM where they are present in the solution but not in the specified root folder. This is useful for where a CRM solution and build folder should be in 1:1 correspondance._
* NoPublish - `/nopublish` - _Using this parameter means that a publish all isn't performed after upload._
* PublishAllOnly - `/publishallonly` - _Using this parameter just runs a publish all command. A valid rootpath and solution still have to be given, but these are ignored._

### Known Issues

There are some known issues, see the [github issues listing](https://github.com/Softwire/dynamics-crm-ci-clis/issues) for the full list.

In particular, the helper can error if a web resource with a given name already exists in the CRM, but isn't in the specified solution.

# Planned Helpers

## PluginWorkflowHelper

This is currently under development, as replicating the behaviour of the SDK PluginUploader to create PluginTypes is complex and requires more reverse engineering than previously planned!

It is almost in a state where it can update existing assemblies, but automatically upserting PluginTypes is where it will really become useful.

## SolutionHelper

There is an open source solution CLI on codeplex at [this link](https://crmsolutioncmdhlp.codeplex.com/) - which is already very good.

It has some bugs (particularly concerining its error messages and lack of logs when an error is encountered), but generally works quite well.

It would be nice if we could create our own version of a solution helper as part of this effort.

# Compatability

The command line helpers have been tested on Windows 8, running against Dynamics CRM 2016 and CRM Online (8.x) - but will probably work with other configurations (in particular, due to the API used, it likely works with CRM 2011 onwards).

# Contributing

Any bug reports or pull requests to new/known issues will be appreciated.