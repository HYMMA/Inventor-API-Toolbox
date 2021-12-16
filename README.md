# Overview
If you use Inventor API you will find yourself dealing with different aspects of CAD and mathematics. Although Autodesk Inventor's API is well documented and structured it is not efficient to solve the same API problem over and over again. That is why we have decided to expand this library and simplify this API even further. This library was originally forked from [InventorToolBox] (https://github.com/H-Ashrafi/InventorToolBox).
## Extensions :information_source:
This library provides useful extension methods for Autodesk Inventor API. The idea is to convert popular macros or complex API subjects (such as transformation matrix and transient geometries) into extension methods.

|These Inventor Interfaces are covered|
|----------------------------------|
|Application|
|AssemblyDocument|
|ComponentOccurances|
|Document|
|PartDocument|

### Extensions Sample
To read or write the iProperties you can use ```GetProperty()``` and ```SetProperty()``` extension methods.
```csharp
using InventorToolBox;
//set up a console app
//get an instance of Inventor
            App.ConnectToInventor();

            //Get partNo of active document
            var partNo = App.ActiveDocument.GetProperty(kDocumnetProperty.PartNumber);
```
To insert a part or subassembly into another assembly you can use ```.AddMember()``` extension methods.
```csharp
//make a new part document or open it if already exists
try
{
  part = app.NewPart(partName);
}
catch (Exception)
{
  part = app.Open(partName, false) as PartDocument;
}

//make a new assembly document or open it if already exists
 try
{
  assembly = app.NewAssembly(assyName);
}
  catch (Exception)
{
   assembly = app.Open(assyName) as AssemblyDocument;
}

//add the part that was created to the assembly
var partOccurance = assembly.AddMemeber(part.AsDocument(), new[] { 0d, 0d, 1d }, new[] { 10d, 10d, 0d });

//insert a content center component into assembly
//in this case the third row of the BS 4848 family is selected. 
//And the length of the angle is set to 658. This family is under Structural Sahpes category
var contentCenterDesc = new ContentCenterItemDescriptor(row: 3, "Structural Shapes", "Angles", "BS 4848") 
   { CustomValue = new KeyValuePair<string, object>("B_L", 658) };
   
var contentCenterOccurance = assembly.AddMemeber(contentCenterDesc, new[] { 0d, 0d, 1d }, new[] { 10d, 10d, 0d });
```
