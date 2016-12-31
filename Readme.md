# Overview

Currently released is the WebResourceHelper, the PluginWorkflowHelper is currently under development.

The WebResourceHelper can be used to upload / update / sync web resources from a folder to a Microsoft Dynamics CRM solution, through a command line interface, and is very useful when you wish to integrate continous integration into your Dynamics CRM Web Resource workflow.

Running the helper requires .NET framework 4.5.2.

# Download & Installation

The lastest helpers are published at:

-- TBC

Download and extract the zip file into a folder.

Running the helper requires .NET framework 4.5.2.

# Explanation

## WebResourceHelper
This tool can be used to syncing an entire folder with the webresources in a solution on CRM.

It is a much improved / automatic version of the Web Resource Uploader that bundles with the CRM SDK. It is particularly useful as part of a continuous integration set-up.


### Suggested Usage

Run

    .\WebResourceHelper.exe /h

for full instructions. There are many paramaters (17+) for different options. A suggested run might be:

    .\WebResourceHelper.exe /crmconnectionstring:"Url=https://contoso.crm2.dynamics.com/Contoso; Username=john.doe@contoso.com; Password=PASS" /solutionname:"WebResources" /folderpath:"build/webresources/" /removemissing

This syncs the web resource content of the folder at ./build/webresources to the CRM using the given connection string. It will prompt you to confirm before any actions are taken. Once happy with its behaviour, you can use the `/automate` paramater in a batch file to run the cli without a prompt.

You can, for example, use with a grunt/gulp build, or just an unbuilt folder.

## PluginWorkflowHelper

This is currently under development, as replicating the behaviour of the SDK PluginUploader to create PluginTypes is complex and requires more reverse engineering than previously planned!

It is almost in a state where it can update existing assemblies, but automatically upserting PluginTypes is where it will really become useful.

## SolutionHelper

There is an open source solution CLI at: INSERTLINKHERE - which may be worth using.

It has some bugs (particularly concerining its error messages and lack of logs when an error is encountered), but generally works quite well.

It would be nice if we could create our own version of a solution helper as part of this effort.

# Compatability

The command line helpers have been tested with Dynamics CRM 2016 and CRM Online (8.x) - but will probably work with others.