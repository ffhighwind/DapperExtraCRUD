# Description:

A Dapper extension for MS SQL Server. Includes the functionality of Dapper-Plus, Dapper.SimpleCRUD, and MUCH more. 
Unique additions include new attributes (pseudo keys: MatchUpdate, MatchDelete) and standard SQL operations: Distinct, 
Top, etc.

# Installation:

This project requires [Visual Studio](https://visualstudio.microsoft.com/) to compile.
Once installed to install the required NuGet packages. Right click on "Utilities" in the
Solution Explorer and select "Manage NuGet Packages...". From here Visual Studio will
prompt you to install the missing packages.

You can add the library to another Visual Studio solution by adding it as a
reference. Right click on the Solution of your other project and
Add > Existing Project. Then right click on "References in the Solution Explorer
and add this as a project reference.

# NuGet Packages:

* [Dapper](https://github.com/StackExchange/Dapper)
   For querying SQL Databases in a type-safe way.
* [FastMember](https://github.com/mgravell/fast-member)
   For Excel (xlsx) file reading and writing via Excel.Spreadsheet and Excel.Worksheet.

# License:

*MIT License*

Copyright (c) 2018 Wesley Hamilton

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.