using System;
using System.Collections.Generic;
using System.Text;

namespace Sberbank.Bidding.Models
{

    // Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class data
    {

        private dataRow rowField;

        /// <remarks/>
        public dataRow row
        {
            get
            {
                return this.rowField;
            }
            set
            {
                this.rowField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class dataRow
    {

        private string reqIDField;

        private string reqStateField;

        private string reqDocIDField;

        private string reqStateStrField;

        private string purchIDField;

        private string purchCodeField;

        private string purchTypeField;

        private string purchNameField;

        private string purchStateStrField;

        private string purchStateField;

        private string publicDateField;

        private string requestDateField;

        private string auctionBeginDateField;

        private string toASField;

        private string aSIDField;

        private string rnField;

        private string mybgguaranteeField;

        /// <remarks/>
        public string reqID
        {
            get
            {
                return this.reqIDField;
            }
            set
            {
                this.reqIDField = value;
            }
        }

        /// <remarks/>
        public string reqState
        {
            get
            {
                return this.reqStateField;
            }
            set
            {
                this.reqStateField = value;
            }
        }

        /// <remarks/>
        public string reqDocID
        {
            get
            {
                return this.reqDocIDField;
            }
            set
            {
                this.reqDocIDField = value;
            }
        }

        /// <remarks/>
        public string reqStateStr
        {
            get
            {
                return this.reqStateStrField;
            }
            set
            {
                this.reqStateStrField = value;
            }
        }

        /// <remarks/>
        public string purchID
        {
            get
            {
                return this.purchIDField;
            }
            set
            {
                this.purchIDField = value;
            }
        }

        /// <remarks/>
        public string purchCode
        {
            get
            {
                return this.purchCodeField;
            }
            set
            {
                this.purchCodeField = value;
            }
        }

        /// <remarks/>
        public string purchType
        {
            get
            {
                return this.purchTypeField;
            }
            set
            {
                this.purchTypeField = value;
            }
        }

        /// <remarks/>
        public string purchName
        {
            get
            {
                return this.purchNameField;
            }
            set
            {
                this.purchNameField = value;
            }
        }

        /// <remarks/>
        public string purchStateStr
        {
            get
            {
                return this.purchStateStrField;
            }
            set
            {
                this.purchStateStrField = value;
            }
        }

        /// <remarks/>
        public string purchState
        {
            get
            {
                return this.purchStateField;
            }
            set
            {
                this.purchStateField = value;
            }
        }

        /// <remarks/>
        public string PublicDate
        {
            get
            {
                return this.publicDateField;
            }
            set
            {
                this.publicDateField = value;
            }
        }

        /// <remarks/>
        public string RequestDate
        {
            get
            {
                return this.requestDateField;
            }
            set
            {
                this.requestDateField = value;
            }
        }

        /// <remarks/>
        public string AuctionBeginDate
        {
            get
            {
                return this.auctionBeginDateField;
            }
            set
            {
                this.auctionBeginDateField = value;
            }
        }

        /// <remarks/>
        public string ToAS
        {
            get
            {
                return this.toASField;
            }
            set
            {
                this.toASField = value;
            }
        }

        /// <remarks/>
        public string ASID
        {
            get
            {
                return this.aSIDField;
            }
            set
            {
                this.aSIDField = value;
            }
        }

        /// <remarks/>
        public string RN
        {
            get
            {
                return this.rnField;
            }
            set
            {
                this.rnField = value;
            }
        }

        /// <remarks/>
        public string mybgguarantee
        {
            get
            {
                return this.mybgguaranteeField;
            }
            set
            {
                this.mybgguaranteeField = value;
            }
        }
    }
}
