namespace INTERNAL.DTO
{
    public class CapturedMetersDTO
    {
        public string Region { get; set; }
        public string Feeder33Name { get; set; }
        public string Feeder33Id { get; set; }
        public string DtrName { get; set; }
        public string StaffName { get; set; }
        public string StaffId { get; set; }

        public string CapturedDatetime { get; set; }

        public string AccountNo { get; set; }
        public string MeterNo { get; set; }
        public string IsReplaced { get; set; }
        public string MeterPhase { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public string ContractorName { get; set; }
        public string InstallerName { get; set; }

        public string SealNo1 { get; set; }
        public string SealNo2 { get; set; }

        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }

        public string CIN { get; set; }
        public string BVN { get; set; }
        public string OldMeterNo { get; set; }

        public string Feeder11Name { get; set; }
        public string Band { get; set; }
    }
}
