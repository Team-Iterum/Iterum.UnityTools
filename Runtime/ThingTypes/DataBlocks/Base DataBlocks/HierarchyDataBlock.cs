using System.Collections.Generic;
using System.Linq;
using Iterum.ThingTypes;
using UnityEngine;
using static Iterum.BaseSystems.TTManagerAlias;

namespace Iterum.DataBlocks
{
    /// <summary>
    /// Write inner ThingTypeRef as hierarchy maprefs
    /// </summary>
    [AutoRegisterDataBlock("HasHierarchy", nameof(Create))]
    public class HierarchyDataBlock : IDataBlock
    {
        public class HierarchyNode
        {
            public string id;
            public string tag;
            
            public float[] pos;
            public float[] rot;
            
            public List<HierarchyNode> nodes;
        }

        public HierarchyNode root;
        
        public static IDataBlock Create(GameObject root)
        {
            HierarchyDataBlock data = new HierarchyDataBlock();
            
            // Create Root Node
            data.root = new HierarchyNode
            {
                nodes = new List<HierarchyNode>(),
                id = root.GetComponent<ThingTypeRef>().ID.ToString(),
                pos = Math.Vector3.zero,
                rot = Math.Vector3.zero,
            };
            
            
            var thingTypes = TTStore.ThingTypes;
            
            // Find thingtyprefs
            TraverseChild(root, data.root, root.transform, ref thingTypes);

            return data;
        }

        private static void TraverseChild(GameObject rootGo, HierarchyNode rootNode, Transform root,
            ref Dictionary<int, ThingType> thingTypes)
        {
            foreach (Transform childTransform in root)
            {
                if (childTransform.CompareTag("IgnoreHierarchyDataBlock"))
                    continue;

                var ttRef = childTransform.GetComponent<ThingTypeRef>();
                var rootThingTypeRef = rootGo.GetComponent<ThingTypeRef>();
                
                // Skip inner hierarchy blocks
                var parentTTRef = childTransform.GetComponentInParent<ThingTypeRef>();
                if (parentTTRef != null &&
                    // not rootTT
                    parentTTRef != rootThingTypeRef &&
                    // not self TT
                    parentTTRef != ttRef)
                {

                    var tt = thingTypes.Values.FirstOrDefault(e => e.ID == parentTTRef.ID);
                    if (tt.HasFlag("HasHierarchy")) continue;
                }
                
                

                var node = new HierarchyNode
                {
                    id = childTransform.name,
                    pos = (Math.Vector3) childTransform.localPosition,
                    rot = (Math.Vector3) childTransform.localRotation.eulerAngles,
                    
                    nodes = new List<HierarchyNode>(),
                };
                
                var hierarchyTag = childTransform.GetComponent<HierarchyTag>();
                
                if (ttRef != null) 
                    node.id = ttRef.ID.ToString();

                if (hierarchyTag != null)
                    node.tag = hierarchyTag.Tag;
                
                rootNode.nodes.Add(node);
                
                TraverseChild(rootGo, node, childTransform, ref thingTypes);


            }
        }
        
    }
}
