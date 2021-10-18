using Inventor;
using System.Collections.Generic;

namespace InventorToolBox
{
    /// <summary>
    /// descriptor for a content center item 
    /// </summary>
    public class ContentCenterItemDescriptor
    {
        /// <summary>
        /// allows access to a content center descriptor 
        /// </summary>
        /// <param name="row">Heirarchy of the part inside content center, the last item should be the display name of the family for example ('Fasteners', 'Bolts','Hex Head','DIN EN 24016')</param>
        /// <param name="nodeHierarchy"></param>
        public ContentCenterItemDescriptor(object row, params string[] nodeHierarchy)
        {
            Row = row;
            NodeHierarchy = nodeHierarchy;
            CustomValue = default(KeyValuePair<string, object>);
            RefreshState = ContentMemberRefreshEnum.kUseDefaultRefreshSetting;
            IsCustom = false;
            FileName = "";
            FailuerReason = MemberManagerErrorsEnum.kMemberManagerUnknown;
            FailuerMessage = "Unknown";
        }

        /// <summary>
        /// Output String that contains a message describing the reason for failure, if the creation of the member did fail. If it didn"t fail this will return an empty String.
        /// </summary>
        public string FailuerMessage { get; internal set; }

        /// <summary>
        /// specifies which row to use to create the member. The row index can be specified by a Long (row index), a ContentTableRow object, or the internal name of a ContentTableRow object.
        /// </summary>
        public object Row { get; set; }

        /// <summary>
        /// Output MemberManagerErrorsEnum that indicates the reason for failure, if the creation of the member did fail.
        /// </summary>
        public MemberManagerErrorsEnum FailuerReason { get; internal set; }

        /// <summary>
        /// Optional input NameValueMap that specifies the input to use for the custom input. If the family is custom and this is not supplied, the default values for custom values are used. For each input value you use the NameValueMap to specify the Column ID as the name and the custom value as the new value . If the factory is not a custom factory this argument is ignored.
        /// </summary>
        public KeyValuePair<string, object> CustomValue { get; set; }

        /// <summary>
        /// Optional Input ContentMemberRefreshEnum that specifies the behavior to use if the member already exists locally.<br/>
        /// kUseDefaultRefreshSetting indicates that the method should use whatever the default setting is as set by the user using the "Content Center" tab of the Application options dialog. <br/>
        /// kRefreshOutOfDateParts indicates that if the part already exists in the local cache and is out of date if it should be replaced with an up to date version of the part.<br/>
        /// kDoNotRefreshOutOfDateParts indicates that the existing part will be used and not overridden.
        /// </summary>
        public ContentMemberRefreshEnum RefreshState { get; set; }
        public bool IsCustom { get; set; }

        /// <summary>
        /// Optional Input String that defines the filename of the resulting member. This property is only used in the case of a custom family (<see cref="IsCustom=true"/>) and is ignored for a standard family. In the case of a custom factory the filename must be specified and is not optional.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// heirarchy of the part inside content center, the last item should be the display name of the family for example ('Fasteners', 'Bolts','Hex Head','DIN EN 24016')
        /// </summary>
        public string[] NodeHierarchy { get; set; }

        /// <summary>
        /// get <see cref="NameValueMap"/> based on the <see cref="CustomValue"/>
        /// </summary>
        /// <param name="applicaiton"></param>
        /// <returns></returns>
        public NameValueMap GetNameValueMap(Application applicaiton)
        {
            var nameMap=applicaiton.TransientObjects.CreateNameValueMap();
            nameMap.Add(CustomValue.Key, CustomValue.Value);
            return nameMap;
        }
    }
}
