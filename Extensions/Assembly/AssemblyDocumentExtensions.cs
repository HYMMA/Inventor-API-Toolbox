using Inventor;
using InventorDna.Extensions.Tools;
using System;
using System.Collections.Generic;

namespace InventorDna.Extensions
{
    public static class AssemblyDocumentExtensions
    {
        #region private fields

        private static List<ComponentOccurrence> _list = new List<ComponentOccurrence>();
        #endregion

        #region private methode/functions

        /// <summary>
        /// recursively processes a document and adds componets to a private filed <see cref="_list"/>
        /// </summary>
        /// <param name="componentOccurrences"></param>
        /// <param name="targetDoc"></param>
        private static void CalculateAllNonPhantomNonReferencedOccurances(ComponentOccurrences componentOccurrences, object targetDoc)
        {
            foreach (ComponentOccurrence occurrence in componentOccurrences)
            {
                if (occurrence.Definition.BOMStructure != BOMStructureEnum.kReferenceBOMStructure
                   &&
                   occurrence.Definition.BOMStructure != BOMStructureEnum.kPhantomBOMStructure)
                {
                    if (occurrence.Definition.Document == targetDoc)
                    {
                        _list.Add(occurrence);
                    }
                    else if (occurrence.DefinitionDocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
                    {
                        CalculateAllNonPhantomNonReferencedOccurances(occurrence.Definition.Occurrences, targetDoc);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// cast this object to <see cref="Document"/>
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Document AsDocument(this AssemblyDocument assembly)
        {
            return assembly as Document;
        }

        /// <summary>
        /// returns BOMQuantity of specified document in the assembly document.
        /// BOMQuantity gives you access to quantity units and such.
        /// </summary>
        /// <param name="targetDoc">Target document to be queried should be non-phantom, non-reference</param>
        /// <param name="assembly">assembly where the document resides</param>
        /// <param name="bomViewType">type of bom view defined in inventor assembly environment</param>
        /// <returns>BOMQuantity</returns>
        public static BOMQuantity GetBomQuantity(this AssemblyDocument assembly, Document targetDoc, BOMViewTypeEnum bomViewType)
        {
            if (targetDoc == null)
                throw new ArgumentNullException(nameof(targetDoc), "Null argument");
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly), "Null argument");

            if (targetDoc.DocumentType == DocumentTypeEnum.kUnknownDocumentObject)
                throw new ArgumentException(nameof(targetDoc), "Unknown document");

            //Get the ActiveLevelOfDetailRepresentation Name
            string MyLOD_Name;
            MyLOD_Name = assembly.ComponentDefinition.RepresentationsManager.ActiveLevelOfDetailRepresentation.Name;

            if (MyLOD_Name != "Master")
                //activate master because only it can do the trick
                assembly.ComponentDefinition.RepresentationsManager.LevelOfDetailRepresentations[1].Activate();

            //Get Bom Object
            var oBOM = assembly.ComponentDefinition.BOM;
            //define a bomView object
            BOMView bomView = assembly.ComponentDefinition.BOM.BOMViews["Structured"];
            switch (bomViewType)
            {
                case BOMViewTypeEnum.kModelDataBOMViewType:
                    break;
                case BOMViewTypeEnum.kStructuredBOMViewType:
                    //Make sure structured view is enabled
                    oBOM.StructuredViewEnabled = true;
                    oBOM.StructuredViewFirstLevelOnly = true;
                    bomView = assembly.ComponentDefinition.BOM.BOMViews["Structured"];
                    break;
                case BOMViewTypeEnum.kPartsOnlyBOMViewType:
                    //Make sure parts only view is enabled
                    oBOM.PartsOnlyViewEnabled = true;
                    bomView = assembly.ComponentDefinition.BOM.BOMViews["Parts Only"];
                    break;
                default:
                    break;
            }
            //look for the targetDoc in assembly
            foreach (BOMRow row in bomView.BOMRows)
            {
                if (row.ComponentDefinitions[1].Document == targetDoc)
                    return row.ComponentDefinitions[1].BOMQuantity;
            }
            //at this stage the targetDoc is not found int the assembly
            return null;
        }

        /// <summary>
        /// get number of a document in a assembly but dont look into sub-assemblies
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="assembly"></param>
        /// <param name="countPhantomAndReference">if document is set to phantom or reference</param>
        /// <returns></returns>
        public static int GetStructuredQuantity(this AssemblyDocument assembly, Document targetDoc, bool countPhantomAndReference = false)
        {
            if (targetDoc == null)
                throw new ArgumentNullException(nameof(targetDoc), "Null argument");
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly), "Null argument");

            if (targetDoc.DocumentType == DocumentTypeEnum.kUnknownDocumentObject)
                return 0;
            int counter = 0;

            if (countPhantomAndReference)
            {
                foreach (ComponentOccurrence occurrence in assembly.ComponentDefinition.Occurrences.AllReferencedOccurrences[targetDoc])
                {
                    counter++;
                }
            }
            else
            {
                foreach (ComponentOccurrence occurrence in assembly.AllNonPhantomNonReferencedOccurances(targetDoc))
                {
                    counter++;
                }
            }
            return counter;
        }

        /// <summary>
        /// get number of a document in a assembly,recursively looks into sub-assemblies
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="assembly"></param>
        /// <returns>int32 number of targetDoc in the assembly</returns>
        public static int GetPartsOnlyQuantity(this AssemblyDocument assembly, Document targetDoc, bool countPhantomAndReference = false)
        {
            if (targetDoc == null)
                throw new ArgumentNullException(nameof(targetDoc), "Null argument");
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly), "Null argument");

            if (targetDoc.DocumentType == DocumentTypeEnum.kUnknownDocumentObject)
                return 0;
            int counter = 0;

            if (countPhantomAndReference)
            {
                foreach (ComponentOccurrence occurrence in assembly.ComponentDefinition.Occurrences.AllReferencedOccurrences[targetDoc])
                {
                    counter++;
                }
            }
            else
            {
                foreach (ComponentOccurrence occurrence in assembly.AllNonPhantomNonReferencedOccurances(targetDoc))
                {
                    counter++;
                }
            }
            return counter;
        }

        /// <summary>
        /// list of occuraces that are not phantom nor are set as referenced in their document settings.
        /// </summary>
        /// <param name="targetDoc">the document that needs to be searched for</param>
        /// <returns>List<ComponentOccurrence></returns>
        public static List<ComponentOccurrence> AllNonPhantomNonReferencedOccurances(this AssemblyDocument assembly, object targetDoc)
        {
            ComponentOccurrences componentOccurrences = assembly.ComponentDefinition.Occurrences;
            CalculateAllNonPhantomNonReferencedOccurances(componentOccurrences, targetDoc);
            return _list;
        }
        
        /// <summary>
        /// makes a translate matrix from the assymbly origin and main planes.
        /// </summary>
        /// <param name="position">relative position from the origin of the assembly all units are in centimeters</param>
        /// <param name="rotation">angular postion in relation to the main planes in the assembly all units are in degrees</param>
        /// <returns><see cref="Matrix"/> object in the space of the assembly.</returns>
        public static Matrix GetMatrix(this AssemblyDocument assembly, double[] position, double[] rotation)
        {
            Application inventor = assembly.ComponentDefinition.Application;

            // Set a reference to the transient geometry object.
            TransientGeometry oTG = inventor.TransientGeometry;

            // Create a matrix.  A new matrix is initialized with an identity matrix.
            Matrix tempMatrix = oTG.CreateMatrix();
            Matrix transMatrix = oTG.CreateMatrix();

            //for all rotational directions . . .
            for (int i = 0; i < rotation.Length; i++)
            {
                var index = new List<int>(new[] { 0, 0, 0 });
                index[i] = 1;
                var origin = oTG.CreatePoint(0, 0, 0);

                //rotate about an axis that goeas through origin point and is along the rotaional direction
                tempMatrix.SetToRotation(MathHelper.ToRadian(rotation[i]), oTG.CreateVector(index[0], index[1], index[2]), origin);
                transMatrix.TransformBy(tempMatrix);
                tempMatrix.SetToIdentity();
            }

            //move the object to the position 
            transMatrix.SetTranslation(oTG.CreateVector(position[0], position[1], position[2]));
            return transMatrix;
        }

        /// <summary>
        /// Inserts a member (part/Subassembly or 3D model) in assembly and returns the created occurance
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="inventor">inventor assembly</param>
        /// <param name="member">any of the supported <see cref="Document"/> types to insert into assembly</param>
        /// <param name="position">position of the member relative to assembly's origin</param>
        /// <param name="rotation">rotation about the X and Y and Z axis</param>
        /// <returns><see cref="ComponentOccurrence"/> that is created inside the assembly</returns>
        /// <remarks>remeber that Inventors internal units for length are centimeters</remarks>
        public static ComponentOccurrence AddMemeber(this AssemblyDocument assembly, Document member, double[] position, double[] rotation)
        {
            if (member.DocumentType == DocumentTypeEnum.kDrawingDocumentObject ||
                member.DocumentType == DocumentTypeEnum.kNoDocument ||
                member.DocumentType == DocumentTypeEnum.kPresentationDocumentObject ||
                member.DocumentType == DocumentTypeEnum.kUnknownDocumentObject)
                throw new ArgumentException("documnet type is not supported", nameof(member));

            if (member.FullFileName == "")
                throw new Exception("FullFileName of the part object was null, you need to save the part before passing to this method");

            if (position.Length > 3 || rotation.Length > 3)
                throw new ArgumentOutOfRangeException("position or rotaion array cannot have more than three memebers");

            //get transitional matrix
            var transMatrix=assembly.GetMatrix(position, rotation);
            
            // Add the occurrence.
            return assembly.ComponentDefinition.Occurrences.Add(member.FullFileName, transMatrix);
        }

        /// <summary>
        /// Add a member to this assembly from content center
        /// </summary>
        /// <param name="itemDescriptor">identifier for the content center member that you want to insert into this assembly</param>
        /// <param name="position">relative position (from assembly origin) of the member inside assembly</param>
        /// <param name="rotation">relative angular (from main planes) position fo the membe rinside assembly</param>
        /// <returns></returns>
        public static ComponentOccurrence AddMemeber(this AssemblyDocument assembly, ContentCenterItemDescriptor itemDescriptor, double[] position, double[] rotation)
        {
            //get inventor application from assembly
            var inventor = assembly.ComponentDefinition.Application as Application;

            //get the top node
            ContentTreeViewNode node = inventor.ContentCenter.TreeViewTopNode.ChildNodes[itemDescriptor.NodeHierarchy[0]];

            //traverse through the nodes and get the last one
            for (int i = 1; i <= itemDescriptor.NodeHierarchy.Length - 2; i++)
            {
                node = node.ChildNodes[itemDescriptor.NodeHierarchy[i]];
            }
            
            //get the family displya name from the params
            var familyDisplayName = itemDescriptor.NodeHierarchy[itemDescriptor.NodeHierarchy.Length - 1];

            string memberFullFileName="";
            
            //find the family with the family display name
            foreach (ContentFamily family in node.Families)
            {
                if (family.DisplayName.ToLower() != familyDisplayName.ToLower())
                    continue;
                var nameMap = itemDescriptor.GetNameValueMap(inventor);
                //create the member (part file) from the table.
                memberFullFileName = family.CreateMember(itemDescriptor.Row, out MemberManagerErrorsEnum failReason, out string failMessage, itemDescriptor.RefreshState, itemDescriptor.IsCustom, itemDescriptor.FileName, nameMap);
                
                //update the error messages in itemdescriptor
                itemDescriptor.FailuerReason = failReason;
                itemDescriptor.FailuerMessage = failMessage;
                if (failReason != MemberManagerErrorsEnum.kMemberManagerNoError)
                    return null;
            }
            var transMatrix = assembly.GetMatrix(position, rotation);
            
            // Add the occurrence.
            return assembly.ComponentDefinition.Occurrences.Add(memberFullFileName, transMatrix);
        }
    }
}