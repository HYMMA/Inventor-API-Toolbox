using Inventor;
using InventorDna.Extensions;
using System;
using System.Collections.Generic;

namespace InventorDna.Console
{
    /// <summary>
    /// a console app as a sample applicaiton for InventorDna.Extensions
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            CreateAssemblyOfParts();
        }

        /// <summary>
        /// create an assembly in inventor and add numerious memebers to it
        /// </summary>
        private static void CreateAssemblyOfParts()
        {
            //connect to inventor
            var app = App.Start();

            //generate file name
            var partName = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "PartFileName.ipt");
            var assyName = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "AssemblyFileName.iam");
            PartDocument part;
            AssemblyDocument assembly;
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

            //set the part number proprety of the part
            part.AsDocument().SetProperty(kDocumnetProperty.PartNumber, "S/S6758");

            //add the part that was created to the assembly
            var partOccurance = assembly.AddMemeber(part.AsDocument(), new[] { 0d, 0d, 1d }, new[] { 10d, 10d, 0d });

            var contentCenterDesc = new ContentCenterItemDescriptor(row: 3, "Structural Shapes", "Angles", "BS 4848") 
                { CustomValue = new KeyValuePair<string, object>("B_L", 658) };
            var contentCenterOccurance = assembly.AddMemeber(contentCenterDesc, new[] { 0d, 0d, 1d }, new[] { 10d, 10d, 0d });
        }
    }
}


