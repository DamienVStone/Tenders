using System.Collections.Generic;
using System.Xml.Serialization;
using Tenders.Sberbank.Abstractions.Models.Requesting;

namespace Tenders.Sberbank.Models
{
    [XmlRoot(ElementName = "data", Namespace = "", IsNullable = false)]
    [XmlType(AnonymousType = true)]
    public partial class PurchaseRequest : IPurchaseRequest
    {
        public string purchid { get; set; }
        public string publicdate { get; set; }
        public string reqid { get; set; }
        public string purchtype { get; set; }
        public string purchasepreferenceet44 { get; set; }
        public string purchasepreferencesmpsonko { get; set; }
        public string purchcode { get; set; }
        [XmlElement(ElementName = "purchVersion")]
        public string purchVersion { get; set; }
        public string purchname { get; set; }
        public string orgname { get; set; }
        public string purchamount { get; set; }
        public string currency { get; set; }
        public string purchcoveramount { get; set; }
        [XmlElement(ElementName = "notificationFeatures")]
        public NotificationFeatures NotificationFeatures { get; set; }
        public dataPurchdescriptions purchdescriptions { get; set; }
        [XmlIgnore]
        public IBxaccount Account { get { return bxaccount; } set { bxaccount = (dataBxaccount)value; } }
        public dataBxaccount bxaccount { get; set; }
        public string reqagreement { get; set; }
        public string reqagreementanswer { get; set; }
        public string requireddocs1 { get; set; }
        //[XmlElement(ElementName = "maxContractAmount")]
        //public string maxContractAmount { get; set; }
        public dataReqdocspart1 reqdocspart1 { get; set; }
        [XmlIgnore]
        public IdataSupplier Supplier { get { return supplier; } set { supplier = (dataSupplier)value; } }
        public dataSupplier supplier { get; set; }
        public dataRequireddocs22 requireddocs22 { get; set; }
        public dataReqdocspart2 reqdocspart2 { get; set; }
        public string templatecreatedate { get; set; }
        public string clientinfo { get; set; }

        public void AddDocs(IAttachableFile file)
        {
            reqdocspart2 = new dataReqdocspart2
            {
                reqdoc = new dataReqdocspart2Reqdoc
                {
                    file = new dataReqdocspart2ReqdocFile
                    {
                        signtype = "hash",
                        fileid = file.UploadedFileID,
                        filename = file.FileName,
                        signinfo = string.Empty,
                        hash = file.UploadedHash,
                        hash2012 = file.UploadedHash2012,
                        maxsignes = "1",
                        sign = file.UploadedSign,
                        signrequired = "True",
                        sourcefileid = string.Empty
                    }
                }
            };
        }

        public void Init()
        {
            reqdocspart1 = new dataReqdocspart1
            {
                reqdoc = new dataReqdocspart1Reqdoc
                {
                    file = new dataReqdocspart1ReqdocFile
                    {
                        signtype = string.Empty,
                        fileid = string.Empty,
                        filename = string.Empty,
                        signinfo = string.Empty,
                        hash = string.Empty,
                        hash2012 = string.Empty,
                        maxsignes = 1,
                        sign = string.Empty,
                        signrequired = "True",
                        sourcefileid = string.Empty
                    }
                }
            };

            reqid = string.Empty;
            purchasepreferenceet44 = string.Empty;
            purchasepreferencesmpsonko = string.Empty;
            requireddocs1 = string.Empty;
            requireddocs22.reqdeclarationrequirements = string.Empty;
            requireddocs22.reqdeclarationrequirementsanswer = string.Empty;
            requireddocs22.reqdeclarationsmp = string.Empty;
            requireddocs22.reqdeclarationsmpanswer = string.Empty;
            requireddocs22.preferencedocfiles = new dataRequireddocs22Preferencedocfiles
            {
                preferencedocfile = new dataRequireddocs22PreferencedocfilesPreferencedocfile
                {
                    file = new dataRequireddocs22PreferencedocfilesPreferencedocfileFile
                    {
                        signtype = string.Empty,
                        fileid = string.Empty,
                        filename = string.Empty,
                        signinfo = string.Empty,
                        hash = string.Empty,
                        hash2012 = string.Empty,
                        maxsignes = "1",
                        sign = string.Empty,
                        signrequired = "True",
                        sourcefileid = string.Empty
                    }
                }
            };
        }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataPurchdescriptions
    {
        public dataPurchdescriptionsPurchdescription purchdescription { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataPurchdescriptionsPurchdescription
    {
        public string purchdescr { get; set; }
    }

    [XmlRoot(ElementName = "notificationFeature")]
    public class NotificationFeature
    {
        [XmlElement(ElementName = "placementFeature")]
        public PlacementFeature PlacementFeature { get; set; }
    }

    [XmlRoot(ElementName = "notificationFeatures")]
    public class NotificationFeatures
    {
        [XmlElement(ElementName = "notificationFeature")]
        public List<NotificationFeature> NotificationFeature { get; set; }
    }

    [XmlRoot(ElementName = "placementFeature")]
    public class PlacementFeature
    {
        //[XmlElement(ElementName = "featureType")]
        //public string FeatureType { get; set; }
        [XmlElement(ElementName = "featureTypeName")]
        public string FeatureTypeName { get; set; }
        //[XmlElement(ElementName = "shortName")]
        //public string ShortName { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "content")]
        public string Content { get; set; }
    }


    [XmlType(AnonymousType = true)]
    public partial class dataNotificationfeaturePlacementfeature
    {
        public string featuretypename { get; set; }
        public string name { get; set; }
        public string content { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataBxaccount : IBxaccount
    {
        public string account { get; set; }
        public string bankcode { get; set; }
        public string bankname { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataReqdocspart1
    {
        public dataReqdocspart1Reqdoc reqdoc { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataReqdocspart1Reqdoc
    {
        public dataReqdocspart1ReqdocFile file { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataReqdocspart1ReqdocFile
    {
        public object signtype { get; set; }
        public object fileid { get; set; }
        public object filename { get; set; }
        public object signinfo { get; set; }
        public object hash { get; set; }
        public object hash2012 { get; set; }
        public byte maxsignes { get; set; }
        public object sign { get; set; }
        public string signrequired { get; set; }
        public object sourcefileid { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataSupplier : IdataSupplier
    {
        public string suppbuid { get; set; }
        public string reqsuppname { get; set; }
        public string opfid { get; set; }
        public string opfname { get; set; }
        public string personfullname { get; set; }
        public string reqsuppfactaddress { get; set; }
        public string reqsupppostaddress { get; set; }
        public string reqsuppphone { get; set; }
        public string suppinn { get; set; }
        public string suppkpp { get; set; }
        public string supptype { get; set; }
        public string suppmaxcontractamount { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataRequireddocs22
    {
        [XmlArrayItem("parentinn", IsNullable = false)]
        public dataRequireddocs22Parentinn[] parentsinn { get; set; }
        public object reqdeclarationrequirements { get; set; }
        public object reqdeclarationrequirementsanswer { get; set; }
        public object reqdeclarationsmp { get; set; }
        public object reqdeclarationsmpanswer { get; set; }
        public dataRequireddocs22Preferencedocfiles preferencedocfiles { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataRequireddocs22Parentinn
    {
        public string inn { get; set; }
        public string name { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataRequireddocs22Preferencedocfiles
    {
        public dataRequireddocs22PreferencedocfilesPreferencedocfile preferencedocfile { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataRequireddocs22PreferencedocfilesPreferencedocfile
    {
        public dataRequireddocs22PreferencedocfilesPreferencedocfileFile file { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataRequireddocs22PreferencedocfilesPreferencedocfileFile
    {
        public string signtype { get; set; }
        public string fileid { get; set; }
        public string filename { get; set; }
        public string signinfo { get; set; }
        public string hash { get; set; }
        public string hash2012 { get; set; }
        public string maxsignes { get; set; }
        public string sign { get; set; }
        public string signrequired { get; set; }
        public string sourcefileid { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataReqdocspart2
    {
        public dataReqdocspart2Reqdoc reqdoc { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataReqdocspart2Reqdoc
    {
        public dataReqdocspart2ReqdocFile file { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public partial class dataReqdocspart2ReqdocFile
    {
        public string signtype { get; set; }
        public string fileid { get; set; }
        public string filename { get; set; }
        public string signinfo { get; set; }
        public string hash { get; set; }
        public string hash2012 { get; set; }
        public string maxsignes { get; set; }
        public string sign { get; set; }
        public string signrequired { get; set; }
        public string sourcefileid { get; set; }
    }
}
