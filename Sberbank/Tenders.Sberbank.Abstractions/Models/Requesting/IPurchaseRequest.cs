namespace Tenders.Sberbank.Abstractions.Models.Requesting
{
    public interface IPurchaseRequest
    {
        IBxaccount Account { get; set; }
        void AddDocs(IAttachableFile file);
        void Init();
        string clientinfo { get; set; }
        string reqagreement { get; set; }
        string reqagreementanswer { get; set; }
        IdataSupplier Supplier { get; set; }

    }
    public interface IdataReqdocspart2ReqdocFile
    {
        string signtype { get; set; }
        string fileid { get; set; }
        string filename { get; set; }
        object signinfo { get; set; }
        string hash { get; set; }
        string hash2012 { get; set; }
        byte maxsignes { get; set; }
        string sign { get; set; }
        string signrequired { get; set; }
        object sourcefileid { get; set; }
    }
    public interface IdataReqdocspart2Reqdoc
    {
        IdataReqdocspart2ReqdocFile File { get; set; }
    }
    public interface IdataReqdocspart2
    {
        IdataReqdocspart2Reqdoc Doc { get; set; }
    }

    public interface IBxaccount
    {
        string account { get; set; }
        string bankcode { get; set; }
        string bankname { get; set; }
    }

    public interface IdataSupplier
    {
        string opfid { get; set; }
        string opfname { get; set; }
    }
}
