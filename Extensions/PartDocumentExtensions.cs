﻿using Inventor;
namespace InventorDna.Extensions
{
    /// <summary>
    /// Extensions for <see cref="PartDocument"/>
    /// </summary>
    public static class PartDocumentExtensions
    {
        /// <summary>
        /// adds an empty sketch to a <see cref="PartDocument"/> and retrieves a pointer to it
        /// </summary>
        /// <param name="part"></param>
        /// <param name="plane"></param>
        /// <param name="ProjectEdges"></param>
        /// <returns></returns>
        public static Sketch AddSketch(this PartDocument part, KMainPlane plane, bool ProjectEdges = false)
        {
            // get the component definition
            PartComponentDefinition partDef = part.ComponentDefinition;

            //get the workplane
            WorkPlane workPlane = partDef.WorkPlanes[plane];

            //create a new sketch
            return partDef.Sketches.Add(workPlane, ProjectEdges) as Sketch;
        }

        /// <summary>
        /// cast this object to <see cref="Document"/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static Document AsDocument(this PartDocument part)
        {
            return part as Document;
        }
    }
}
